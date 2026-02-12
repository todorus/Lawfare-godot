using System;
using System.Linq;
using Xunit;
using Godot;
using Lawfare.scripts.board.dice;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.logic.keywords;
using Lawfare.scripts.logic.triggers;
using Lawfare.scripts.logic.modifiers;
using Lawfare.scripts.subject;
using Lawfare.scripts.subject.quantities;
using Lawfare.scripts.subject.relations;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeMovingTest
{
    // Minimal test subject: must be both ISubject and IHasInitiative for initiative diffs.
    private sealed class TestEntity : ISubject, IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public override string ToString() => Name;

        public Quantities Quantities => throw new NotImplementedException();
        public Relations Relations { get; }
        public KeywordBase[] Keywords { get; }
        public Allegiances Allegiances => throw new NotImplementedException();
        public bool CanHaveFaction { get; }
        public System.Collections.Generic.IEnumerable<SkillPool> Pools { get; }
        public bool IsExpired { get; set; }
        public HostedTrigger[] Triggers => Array.Empty<HostedTrigger>();
        public int Minimum(Property property) => 0;
        public Vector3 DamagePosition { get; }
    }

    // Minimal context that only needs to carry the initiative track for diff Apply().
    private sealed class TestContext : IContext
    {
        public InitiativeTrackState InitiativeTrack { get; set; } = new();

        public Faction[] Factions => Array.Empty<Faction>();
        public Lawyer[] Lawyers => Array.Empty<Lawyer>();
        public Witness[] Witnesses => Array.Empty<Witness>();
        public Judge[] Judges => Array.Empty<Judge>();
        public event Action<InitiativeTrackState>? InitiativeTrackChanged;
        public Team[] Teams => Array.Empty<Team>();
        public ISubject[] AllSubjects => Array.Empty<ISubject>();
        public Team GetTeam(ISubject subject) => throw new NotImplementedException();
        public Team GetOpposingTeam(ISubject subject) => throw new NotImplementedException();
    }

    private static TestEntity S(string name) => new TestEntity(name);

    private static InitiativeTrackState State(params (int delay, ISubject[] row)[] slots)
    {
        return new InitiativeTrackState
        {
            Slots = slots
                .Select(s => new InitiativeSlotState
                {
                    Delay = s.delay,
                    Row = s.row.Cast<IHasInitiative>().ToArray()
                })
                .ToArray()
        };
    }

    private static void ApplyMove(TestContext ctx, TestEntity entity, int dt)
    {
        var diff = Initiative.MoveEntity(ctx, entity, dt);
        if (diff != null)
            diff.Value.Apply(); // mutates ctx.InitiativeTrack via SetDelay(...)
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
    Scenario: MoveEntity with dt=0 leaves the state unchanged
      Given an initiative state with slots:
        | delay | row |
        | 0     | A   |
        | 2     | B   |
      When I move entity A by dt 0
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
        | 2     | B   |
    */
    [Fact]
    public void MoveEntity_DtZero_NoChange()
    {
        var A = S("A");
        var B = S("B");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A }),
                (2, new[] { B })
            )
        };

        ApplyMove(ctx, A, 0);

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { A }),
            (2, new[] { B })
        );
    }

    /*
    Scenario: MoveEntity relocates the current entity to delay dt and appends on collision
      Given an initiative state with slots:
        | delay | row |
        | 0     | A   |
        | 2     | B   |
        | 3     | C   |
      When I move entity A by dt 3
      Then ReadSlots returns:
        | delay | row |
        | 2     | B   |
        | 3     | C,A |
      And GetCurrent returns null
    */
    [Fact]
    public void MoveEntity_Current_ToDelayDt_AppendsOnCollision()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A }),
                (2, new[] { B }),
                (3, new[] { C })
            )
        };

        ApplyMove(ctx, A, 3);

        AssertSlots(ctx.InitiativeTrack,
            (2, new[] { B }),
            (3, new[] { C, A })
        );

        Assert.Null(Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: MoveEntity relocates the current entity but preserves remaining slot 0 queue
      Given an initiative state with slots:
        | delay | row   |
        | 0     | A,B,C |
        | 2     | D     |
      When I move entity A by dt 2
      Then ReadSlots returns:
        | delay | row |
        | 0     | B,C |
        | 2     | D,A |
      And GetCurrent returns B
    */
    [Fact]
    public void MoveEntity_Current_PreservesSlot0Queue()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");
        var D = S("D");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A, B, C }),
                (2, new[] { D })
            )
        };

        ApplyMove(ctx, A, 2);

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { B, C }),
            (2, new[] { D, A })
        );

        Assert.Same(B, Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: MoveEntity relocates a non-current entity by increasing its delay by dt
      Given an initiative state with slots:
        | delay | row |
        | 0     | A   |
        | 1     | B   |
      When I move entity B by dt 2
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
        | 3     | B   |
      And GetCurrent returns A
    */
    [Fact]
    public void MoveEntity_NonCurrent_IncreasesDelayByDt()
    {
        var A = S("A");
        var B = S("B");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A }),
                (1, new[] { B })
            )
        };

        ApplyMove(ctx, B, 2);

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { A }),
            (3, new[] { B })
        );

        Assert.Same(A, Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: MoveEntity appends a moved non-current entity to the back of the destination slot row
      Given an initiative state with slots:
        | delay | row |
        | 0     | A   |
        | 2     | C   |
        | 4     | B   |
        | 6     | D   |
      When I move entity B by dt 2
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
        | 2     | C   |
        | 6     | D,B |
    */
    [Fact]
    public void MoveEntity_NonCurrent_AppendsOnCollision()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");
        var D = S("D");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A }),
                (2, new[] { C }),
                (4, new[] { B }),
                (6, new[] { D })
            )
        };

        ApplyMove(ctx, B, 2);

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { A }),
            (2, new[] { C }),
            (6, new[] { D, B })
        );
    }

    /*
    Scenario: MoveEntity is a no-op when the entity is not present in the state
      Given an initiative state with slots:
        | delay | row |
        | 0     | A   |
      When I move entity D by dt 3
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
      And GetCurrent returns A
    */
    [Fact]
    public void MoveEntity_EntityNotPresent_NoOp()
    {
        var A = S("A");
        var D = S("D");

        var ctx = new TestContext
        {
            InitiativeTrack = State(
                (0, new[] { A })
            )
        };

        ApplyMove(ctx, D, 3);

        AssertSlots(ctx.InitiativeTrack,
            (0, new[] { A })
        );

        Assert.Same(A, Initiative.GetCurrent(ctx.InitiativeTrack));
    }
}
