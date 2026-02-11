using System;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeSeedingTest
{
    private sealed class TestEntity : IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public override string ToString() => Name;
    }
    
    private static IHasInitiative S(string name) => new TestEntity(name);

    private static void AssertSlots(
        InitiativeTrackState state,
        params (int delay, IHasInitiative[] row)[] expected)
    {
        var actual = Initiative.ReadSlots(state);

        Assert.Equal(expected.Length, actual.Count);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].delay, actual[i].Delay);
            Assert.Equal(expected[i].row.Length, actual[i].Row.Count);

            for (int j = 0; j < expected[i].row.Length; j++)
                Assert.Same(expected[i].row[j], actual[i].Row[j]);
        }
    }

    /*
    Scenario: Seed with empty entries yields an empty track
      When I seed initiative with no entries
      Then the track has no slots
      And GetCurrent returns null
      And ReadSlots returns an empty list
    */
    [Fact]
    public void Seed_EmptyEntries_YieldsEmptyTrack()
    {
        var state = Initiative.Seed(Array.Empty<(IHasInitiative entity, int delay)>());

        Assert.Empty(Initiative.ReadSlots(state));
        Assert.Null(Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed normalizes delays so the minimum becomes 0
      When I seed initiative with:
        | entity | delay |
        | A      | 2     |
        | B      | 5     |
        | C      | 5     |
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
        | 3     | B,C |
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_NormalizesMinimumToZero_Example_2_5_5()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var state = Initiative.Seed(new[]
        {
            (A, 2),
            (B, 5),
            (C, 5),
        });

        AssertSlots(state,
            (0, new[] { A }),
            (3, new[] { B, C })
        );

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed preserves input order within equal delays
      When I seed initiative with:
        | entity | delay |
        | A      | 2     |
        | B      | 2     |
        | C      | 2     |
      Then ReadSlots returns:
        | delay | row   |
        | 0     | A,B,C |
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_PreservesInputOrder_WithinEqualDelays()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var state = Initiative.Seed(new[]
        {
            (A, 2),
            (B, 2),
            (C, 2),
        });

        AssertSlots(state,
            (0, new[] { A, B, C })
        );

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed tiebreak for current when multiple entities share the minimum delay
      When I seed initiative with:
        | entity | delay |
        | B      | 1     |
        | A      | 1     |
        | C      | 2     |
      Then GetCurrent returns B
      And ReadSlots returns:
        | delay | row |
        | 0     | B,A |
        | 1     | C   |
    */
    [Fact]
    public void Seed_TiedMinimum_PicksCurrentByInputOrder()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var state = Initiative.Seed(new[]
        {
            (B, 1),
            (A, 1),
            (C, 2),
        });

        Assert.Same(B, Initiative.GetCurrent(state));
        AssertSlots(state,
            (0, new[] { B, A }),
            (1, new[] { C })
        );
    }

    /*
    Scenario: Seed supports negative delays and normalizes them
      When I seed initiative with:
        | entity | delay |
        | A      | -3    |
        | B      | 0     |
        | C      | -1    |
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
        | 2     | C   |
        | 3     | B   |
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_AllowsNegativeDelays_AndNormalizes()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var state = Initiative.Seed(new[]
        {
            (A, -3),
            (B, 0),
            (C, -1),
        });

        AssertSlots(state,
            (0, new[] { A }),
            (2, new[] { C }),
            (3, new[] { B })
        );

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed groups entities by resulting normalized delay
      When I seed initiative with:
        | entity | delay |
        | A      | 10    |
        | B      | 12    |
        | C      | 12    |
        | D      | 11    |
      Then ReadSlots returns:
        | delay | row |
        | 0     | A   |
        | 1     | D   |
        | 2     | B,C |
    */
    [Fact]
    public void Seed_GroupsByShiftedDelay()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");
        var D = S("D");

        var state = Initiative.Seed(new[]
        {
            (A, 10),
            (B, 12),
            (C, 12),
            (D, 11),
        });

        AssertSlots(state,
            (0, new[] { A }),
            (1, new[] { D }),
            (2, new[] { B, C })
        );
    }
}
