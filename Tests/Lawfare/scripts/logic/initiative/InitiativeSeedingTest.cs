using System;
using Xunit;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeSeedingTest
{
    private sealed class TestEntity : IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public bool HasActed { get; set; }
        public override string ToString() => Name;
    }

    private static IHasInitiative S(string name) => new TestEntity(name);

    private static void AssertSlots(
        InitiativeTrackState state,
        params (IHasInitiative? occupant, bool isStaggered)[] expected)
    {
        var actual = Initiative.ReadSlots(state);
        Assert.Equal(expected.Length, actual.Count);
        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Same(expected[i].occupant, actual[i].Occupant);
            Assert.Equal(expected[i].isStaggered, actual[i].IsStaggered);
        }
    }

    /*
    Scenario: Seed with empty entries yields an empty track
      When I seed initiative with no entries
      Then TrackLength is 0
      And CurrentIndex is 0
      And RoundEndIndex is 0
      And GetCurrent returns null
      And ReadSlots returns an empty list
    */
    [Fact]
    public void Seed_EmptyEntries_YieldsEmptyTrack()
    {
        var state = Initiative.Seed(Array.Empty<(IHasInitiative entity, int initiative)>());

        Assert.Empty(Initiative.ReadSlots(state));
        Assert.Null(Initiative.GetCurrent(state));
        Assert.Equal(0, state.CurrentIndex);
        Assert.Equal(0, state.RoundEndIndex);
    }

    /*
    Scenario: Seed with a single entity produces two slots
      When I seed initiative with: A=5
      Then TrackLength is 2
      And RoundEndIndex is 1
      And CurrentIndex is 0
      And slots: A, (empty) — all not staggered
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_SingleEntity_ProducesTwoSlots()
    {
        var A = S("A");

        var state = Initiative.Seed(new[] { (A, 5) });

        Assert.Equal(2, state.TrackLength);
        Assert.Equal(1, state.RoundEndIndex);
        Assert.Equal(0, state.CurrentIndex);

        AssertSlots(state,
            (A,    false),
            (null, false));

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed places entities in ascending initiative order with one trailing empty slot
      When I seed initiative with: C=3, A=1, D=4, B=2
      Then TrackLength is 5
      And RoundEndIndex is 4
      And CurrentIndex is 0
      And slots: A, B, C, D, (empty) — all not staggered
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_SortsEntitiesByInitiative_ConsecutiveNoGaps()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");
        var D = S("D");

        var state = Initiative.Seed(new[]
        {
            (C, 3),
            (A, 1),
            (D, 4),
            (B, 2),
        });

        Assert.Equal(5, state.TrackLength);
        Assert.Equal(4, state.RoundEndIndex);
        Assert.Equal(0, state.CurrentIndex);

        AssertSlots(state,
            (A,    false),
            (B,    false),
            (C,    false),
            (D,    false),
            (null, false));

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed preserves input order for entities with equal initiative values
      When I seed initiative with: A=2, B=2, C=2
      Then TrackLength is 4
      And RoundEndIndex is 3
      And slots: A, B, C, (empty) — all not staggered
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_PreservesInputOrder_WithinEqualInitiativeValues()
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

        Assert.Equal(4, state.TrackLength);
        Assert.Equal(3, state.RoundEndIndex);
        Assert.Equal(0, state.CurrentIndex);

        AssertSlots(state,
            (A,    false),
            (B,    false),
            (C,    false),
            (null, false));

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed tiebreak for current when multiple entities share the minimum initiative value
      When I seed initiative with: B=1, A=1, C=2
      Then GetCurrent returns B (first in input order among tied minimum)
      And slots: B, A, C, (empty) — all not staggered
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
            (B,    false),
            (A,    false),
            (C,    false),
            (null, false));
    }

    /*
    Scenario: Seed places entities consecutively regardless of gaps in initiative values
      When I seed initiative with: A=1, B=10, C=100
      Then TrackLength is 4 (no gaps — consecutive slots)
      And slots: A, B, C, (empty) — all not staggered
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_ConsecutiveSlots_EvenWithLargeInitiativeGaps()
    {
        var A = S("A");
        var B = S("B");
        var C = S("C");

        var state = Initiative.Seed(new[]
        {
            (A, 1),
            (B, 10),
            (C, 100),
        });

        Assert.Equal(4, state.TrackLength);
        Assert.Equal(3, state.RoundEndIndex);

        AssertSlots(state,
            (A,    false),
            (B,    false),
            (C,    false),
            (null, false));

        Assert.Same(A, Initiative.GetCurrent(state));
    }

    /*
    Scenario: Seed with negative initiative values still sorts correctly
      When I seed initiative with: A=-3, B=0, C=-1
      Then slots: A, C, B, (empty) — sorted ascending, no gaps
      And GetCurrent returns A
    */
    [Fact]
    public void Seed_NegativeInitiativeValues_SortCorrectly()
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

        Assert.Equal(4, state.TrackLength);
        Assert.Equal(3, state.RoundEndIndex);

        AssertSlots(state,
            (A,    false),
            (C,    false),
            (B,    false),
            (null, false));

        Assert.Same(A, Initiative.GetCurrent(state));
    }
}
