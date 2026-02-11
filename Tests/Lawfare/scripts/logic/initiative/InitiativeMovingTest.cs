using System;
using System.Linq;
using Xunit;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeMovingTest
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

        var state = State(
            (0, new[] { A }),
            (2, new[] { B })
        );

        var moved = Initiative.MoveEntity(state, A, 0);

        AssertSlots(moved,
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

        var state = State(
            (0, new[] { A }),
            (2, new[] { B }),
            (3, new[] { C })
        );

        var moved = Initiative.MoveEntity(state, A, 3);

        AssertSlots(moved,
            (2, new[] { B }),
            (3, new[] { C, A })
        );

        Assert.Null(Initiative.GetCurrent(moved));
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

        var state = State(
            (0, new[] { A, B, C }),
            (2, new[] { D })
        );

        var moved = Initiative.MoveEntity(state, A, 2);

        AssertSlots(moved,
            (0, new[] { B, C }),
            (2, new[] { D, A })
        );

        Assert.Same(B, Initiative.GetCurrent(moved));
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

        var state = State(
            (0, new[] { A }),
            (1, new[] { B })
        );

        var moved = Initiative.MoveEntity(state, B, 2);

        AssertSlots(moved,
            (0, new[] { A }),
            (3, new[] { B })
        );

        Assert.Same(A, Initiative.GetCurrent(moved));
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

        var state = State(
            (0, new[] { A }),
            (2, new[] { C }),
            (4, new[] { B }),
            (6, new[] { D })
        );

        var moved = Initiative.MoveEntity(state, B, 2);

        AssertSlots(moved,
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

        var state = State(
            (0, new[] { A })
        );

        var moved = Initiative.MoveEntity(state, D, 3);

        AssertSlots(moved,
            (0, new[] { A })
        );
        Assert.Same(A, Initiative.GetCurrent(moved));
    }
}
