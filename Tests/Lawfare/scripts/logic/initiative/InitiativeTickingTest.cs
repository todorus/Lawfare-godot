using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Xunit;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeTickingTest
{
    // Minimal test subject: must be both ISubject and IHasInitiative for initiative diffs.
    private sealed class TestEntity : ISubject, IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public override string ToString() => Name;

        // ---- ISubject members ----
        // Adjust these stubs to match your actual ISubject interface requirements.
        // If your real ISubject has many members, prefer creating a dedicated TestSubject in Tests project.
        public Quantities Quantities => throw new NotImplementedException();
        public Relations Relations { get; }
        public KeywordBase[] Keywords { get; }
        public Allegiances Allegiances => throw new NotImplementedException();
        public bool CanHaveFaction { get; }
        public IEnumerable<SkillPool> Pools { get; }
        public bool IsExpired { get; set; }
        public HostedTrigger[] Triggers => Array.Empty<HostedTrigger>();
        public int Minimum(Property property) => 0;
        public Vector3 DamagePosition { get; }
    }

    // Minimal context that only needs to carry the initiative track for diff Apply().
    private sealed class TestContext : IContext
    {
        public InitiativeTrackState InitiativeTrack { get; set; } = new();

        // ---- IContext members (not needed for these tests) ----
        public Faction[] Factions => Array.Empty<Faction>();
        public Lawyer[] Lawyers => Array.Empty<Lawyer>();
        public Witness[] Witnesses => Array.Empty<Witness>();
        public Judge[] Judges => Array.Empty<Judge>();
        public event Action<InitiativeTrackState>? InitiativeTrackChanged;
        public Team[] Teams => Array.Empty<Team>();
        public ISubject[] AllSubjects => Array.Empty<ISubject>();
        public Lawyer GetSpeaker(Team team) => null;

        public Team GetTeam(ISubject subject) => throw new NotImplementedException();
        public Team GetOpposingTeam(ISubject subject) => throw new NotImplementedException();
    }

    private static ISubject S(string name) => new TestEntity(name);

    private static InitiativeTrackState State(params (int delay, ISubject[] row)[] slots)
    {
        return new InitiativeTrackState
        {
            Slots = slots.Select(s => new InitiativeSlotState { Delay = s.delay, Row = s.row.Cast<IHasInitiative>().ToArray() }).ToArray()
        };
    }

    private static InitiativeTrackState TickN(InitiativeTrackState state, int n)
    {
        var ctx = new TestContext { InitiativeTrack = state };

        for (int i = 0; i < n; i++)
        {
            var diffs = Initiative.Tick(ctx);      // returns InitiativeDelayDiff[]
            foreach (var d in diffs)
                d.Apply();                         // mutates ctx.InitiativeTrack via SetDelay(...)
        }

        return ctx.InitiativeTrack;
    }

    private static void AssertSlots(
        InitiativeTrackState state,
        params (int delay, ISubject[] row)[] expected)
    {
        var actual = Initiative.ReadSlots(state); // IReadOnlyList<InitiativeSlotDTO>

        Assert.Equal(expected.Length, actual.Count);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].delay, actual[i].Delay);
            Assert.Equal(expected[i].row.Length, actual[i].Row.Length);

            for (int j = 0; j < expected[i].row.Length; j++)
                Assert.Same(expected[i].row[j], actual[i].Row[j]);
        }
    }

    /*
    Scenario: Tick on an empty track yields an empty track
      Given an initiative state with no slots
      When I tick the track once
      Then the track has no slots
      And GetCurrent returns null
    */
    [Fact]
    public void Tick_EmptyTrack_RemainsEmpty()
    {
        var ctx = new TestContext
        {
            InitiativeTrack = new InitiativeTrackState { Slots = Array.Empty<InitiativeSlotState>() }
        };

        var diffs = Initiative.Tick(ctx);
        foreach (var d in diffs)
            d.Apply();

        Assert.Empty(Initiative.ReadSlots(ctx.InitiativeTrack));
        Assert.Null(Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: Tick decrements all positive delays by 1 and merges collisions
      Given an initiative state with slots:
        | delay | row |
        | 0     | A   |
        | 2     | B   |
        | 1     | C   |
      When I tick the track once
      Then ReadSlots returns:
        | delay | row |
        | 0     | A,C |
        | 1     | B   |
      And GetCurrent returns A
    */
    [Fact]
    public void Tick_DecrementsAndMergesCollisions()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A }),
                (2, new[] { B }),
                (1, new[] { C })
            )
        };

        var diffs = Initiative.Tick(ctx);
        foreach (var d in diffs)
            d.Apply();

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { A, C }),
            (1, new[] { B })
        );

        Assert.Same(A, Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: Tick preserves order when multiple slots merge into the same delay
      Given an initiative state with slots:
        | delay | row |
        | 2     | B   |
        | 1     | C   |
      When I tick the track once
      Then ReadSlots returns:
        | delay | row |
        | 0     | C   |
        | 1     | B   |
    */
    [Fact]
    public void Tick_PreservesOrder_WhenSlotsShift()
    {
        var B = S("B");
        var C = S("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (2, new[] { B }),
                (1, new[] { C })
            )
        };

        var diffs = Initiative.Tick(ctx);
        foreach (var d in diffs)
            d.Apply();

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { C }),
            (1, new[] { B })
        );

        Assert.Same(C, Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: Tick does not move entities already at delay 0
      Given an initiative state with slots:
        | delay | row |
        | 0     | A,B |
        | 1     | C   |
      When I tick the track once
      Then ReadSlots returns:
        | delay | row   |
        | 0     | A,B,C |
    */
    [Fact]
    public void Tick_DoesNotMoveDelay0_OnlyAddsArrivals()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A, B }),
                (1, new[] { C })
            )
        };

        var diffs = Initiative.Tick(ctx);
        foreach (var d in diffs)
            d.Apply();

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { A, B, C })
        );

        Assert.Same(A, Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: Repeated ticks eventually produce a current when an entity reaches delay 0
      Given an initiative state with slots:
        | delay | row |
        | 2     | A   |
      When I tick the track twice
      Then GetCurrent returns A
      And ReadSlots returns:
        | delay | row |
        | 0     | A   |
    */
    [Fact]
    public void Tick_Repeated_EntityReachesZero_BecomesCurrent()
    {
        var A = S("A");

        var state = State(
            (2, new[] { A })
        );

        var ticked = TickN(state, 2);

        AssertSlots(ticked,
            (0, new[] { A })
        );

        Assert.Same(A, Initiative.GetCurrent(ticked));
    }
}
