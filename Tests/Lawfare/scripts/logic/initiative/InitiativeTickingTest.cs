using System;
using System.Linq;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeTickingTest
{
    private sealed class TestEntity : IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public override string ToString() => Name;
    }

    private static IHasInitiative S(string name) => new TestEntity(name);

    private static InitiativeTrackState State(params (int delay, IHasInitiative[] row)[] slots)
    {
        return new InitiativeTrackState
        {
            Slots = slots.Select(s => new InitiativeSlotState { Delay = s.delay, Row = s.row }).ToArray()
        };
    }

    private static InitiativeTrackState TickN(InitiativeTrackState state, int n)
    {
        for (int i = 0; i < n; i++)
            state = Initiative.Tick(state);
        return state;
    }

    private static void AssertSlots(
        InitiativeTrackState state,
        params (int delay, IHasInitiative[] row)[] expected)
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
        var state = new InitiativeTrackState { Slots = Array.Empty<InitiativeSlotState>() };

        var ticked = Initiative.Tick(state);

        Assert.Empty(Initiative.ReadSlots(ticked));
        Assert.Null(Initiative.GetCurrent(ticked));
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

        var state = State(
            (0, new[] { A }),
            (2, new[] { B }),
            (1, new[] { C })
        );

        var ticked = Initiative.Tick(state);

        AssertSlots(ticked,
            (0, new[] { A, C }),
            (1, new[] { B })
        );

        Assert.Same(A, Initiative.GetCurrent(ticked));
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

        var state = State(
            (2, new[] { B }),
            (1, new[] { C })
        );

        var ticked = Initiative.Tick(state);

        AssertSlots(ticked,
            (0, new[] { C }),
            (1, new[] { B })
        );

        Assert.Same(C, Initiative.GetCurrent(ticked));
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

        var state = State(
            (0, new[] { A, B }),
            (1, new[] { C })
        );

        var ticked = Initiative.Tick(state);

        AssertSlots(ticked,
            (0, new[] { A, B, C })
        );

        Assert.Same(A, Initiative.GetCurrent(ticked));
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
