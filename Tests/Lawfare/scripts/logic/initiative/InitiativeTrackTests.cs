using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.logic.initiative;

namespace Tests.Lawfare.scripts.logic.initiative;

// InitiativeTrackTests.cs
// Drop into your test project (xUnit).
//
// Assumptions (from your accepted spec):
// - Tie-breaking for Current selection: earliest slot (minimum delay), then first-in-row.
// - ClearStaging semantics: reverts to snapshot captured at first Stage() and discards ALL changes since then.
// - MAX_INT is represented by int.MaxValue.
// - Public API types exist in your solution:
//     IHasInitiative, IInitiativeTrack, InitiativeTrack, InitiativeSlot
//
// If your test project doesn't already reference the assembly that contains InitiativeTrack,
// add that reference and the appropriate using statements / namespace.

public sealed class InitiativeTrackTests
{
    private sealed class TestEntity : IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public override string ToString() => Name;
    }

    // -----------------------------
    // Helpers
    // -----------------------------

    private static InitiativeTrack CreateTrack() => new InitiativeTrack();

    private static void AssertSlots(IInitiativeTrack track, params (int delay, IHasInitiative[] row)[] expected)
    {
        var slots = track.Slots;

        Assert.Equal(expected.Length, slots.Count);

        for (int i = 0; i < expected.Length; i++)
        {
            Assert.Equal(expected[i].delay, slots[i].Delay);
            Assert.Equal(expected[i].row.Length, slots[i].Row.Count);

            for (int j = 0; j < expected[i].row.Length; j++)
                Assert.Same(expected[i].row[j], slots[i].Row[j]);
        }
    }

    private static void AssertSlotRow(IInitiativeTrack track, int delay, params IHasInitiative[] expectedRow)
    {
        var slot = track.Slots.Single(s => s.Delay == delay);
        Assert.Equal(expectedRow.Length, slot.Row.Count);
        for (int i = 0; i < expectedRow.Length; i++)
            Assert.Same(expectedRow[i], slot.Row[i]);
    }

    // ============================================================
    // Adding and removing entities
    // ============================================================

    /*
    Scenario: Adding the first entity establishes Current at slot 0
      When I add entity A with delay 0
      Then the current entity is A
      And the slots are:
        | delay | row |
        | 0     | A   |
      And the next entity is null
      And the delay to next is MAX_INT
    */
    [Fact]
    public void Add_FirstEntity_EstablishesCurrentAtZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");

        track.Add(A, 0);

        Assert.Same(A, track.Current);
        AssertSlots(track, (0, new IHasInitiative[] { A }));
        Assert.Null(track.Next);
        Assert.Equal(int.MaxValue, track.DelayToNext);
    }

    /*
    Scenario: Adding a non-current entity with non-negative delay appends to that slot
      Given I add entity A with delay 0
      When I add entity B with delay 3
      And I add entity C with delay 3
      Then the slots are:
        | delay | row |
        | 0     | A   |
        | 3     | B,C |
      And the current entity is A
    */
    [Fact]
    public void Add_NonCurrent_NonNegativeDelay_AppendsToSlot()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 3);
        track.Add(C, 3);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (3, new IHasInitiative[] { B, C })
        );
    }

    /*
    Scenario: Adding a non-current entity with negative delay makes it NEXT in slot 0
      Given I add entity A with delay 0
      When I add entity B with delay -1
      Then the current entity is A
      And the slot with delay 0 row order is A,B
      And the next entity is B
    */
    [Fact]
    public void Add_NonCurrent_NegativeDelay_BecomesNextInZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, -1);

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B);
        Assert.Same(B, track.Next);
    }

    /*
    Scenario: Removing a non-current entity removes it from its slot
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I remove entity B
      Then the slots are:
        | delay | row |
        | 0     | A   |
      And the current entity is A
    */
    [Fact]
    public void Remove_NonCurrent_RemovesFromSlot()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        var removed = track.Remove(B);

        Assert.True(removed);
        Assert.Same(A, track.Current);
        AssertSlots(track, (0, new IHasInitiative[] { A }));
    }

    /*
    Scenario: Removing the only entity leaves the track empty
      Given I add entity A with delay 0
      When I remove entity A
      Then the current entity is null
      And the next entity is null
      And the slots are empty
      And the delay to next is MAX_INT
    */
    [Fact]
    public void Remove_OnlyEntity_EmptiesTrack()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");

        track.Add(A, 0);
        var removed = track.Remove(A);

        Assert.True(removed);
        Assert.Null(track.Current);
        Assert.Null(track.Next);
        Assert.Empty(track.Slots);
        Assert.Equal(int.MaxValue, track.DelayToNext);
    }

    /*
    Scenario: Removing Current selects a new Current by earliest slot then first-in-row
      Given I add entity A with delay 0
      And I add entity B with delay 1
      And I add entity C with delay 0
      When I remove entity A
      Then the current entity is C
      And the slot with delay 0 starts with C
    */
    [Fact]
    public void Remove_Current_SelectsNewCurrent_ByEarliestThenFirstInRow()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 1);
        track.Add(C, 0);

        var removed = track.Remove(A);

        Assert.True(removed);
        Assert.Same(C, track.Current);
        Assert.Same(C, track.Slots.Single(s => s.Delay == 0).Row[0]);
    }

    // ============================================================
    // Slot representation and collision ordering
    // ============================================================

    /*
    Scenario: Slot snapshot groups entities by equal delay preserving insertion order
      Given I add entity A with delay 0
      And I add entity B with delay 2
      And I add entity C with delay 2
      Then the slot with delay 2 row order is B,C
    */
    [Fact]
    public void Slots_GroupByDelay_PreserveInsertionOrder()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 2);
        track.Add(C, 2);

        AssertSlotRow(track, 2, B, C);
    }

    /*
    Scenario: Landing on an occupied slot appends to the back of that slot's row
      Given I add entity A with delay 0
      And I add entity B with delay 1
      And I add entity C with delay 1
      When I move the track by 1
      Then the slot with delay 0 row order is A,B,C
    */
    [Fact]
    public void Move_Collision_AppendsToBackOfRow()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 1);
        track.Add(C, 1);

        track.Move(1);

        AssertSlotRow(track, 0, A, B, C);
    }

    // ============================================================
    // Whole-track Move (anchored to Current)
    // ============================================================

    /*
    Scenario: Whole-track move forward keeps Current at the front of slot 0
      Given I add entity A with delay 0
      And I add entity B with delay 1
      When I move the track by 1
      Then the current entity is A
      And the slot with delay 0 starts with A
      And the slot with delay 0 row order is A,B
    */
    [Fact]
    public void Move_Forward_KeepsCurrentAtFrontOfZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        track.Move(1);

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B);
    }

    /*
    Scenario: Whole-track move forward decreases delays until catch-up
      Given I add entity A with delay 0
      And I add entity B with delay 3
      When I move the track by 2
      Then entity B is at delay 1
      And the current entity is A
    */
    [Fact]
    public void Move_Forward_DecreasesDelays_UntilCatchUp()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 3);

        track.Move(2);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B })
        );
    }

    /*
    Scenario: Whole-track move forward does not allow overtaking Current
      Given I add entity A with delay 0
      And I add entity B with delay 1
      When I move the track by 2
      Then the current entity is A
      And the slot with delay 0 row order is A,B
      And no slot exists with delay less than 0
    */
    [Fact]
    public void Move_Forward_DoesNotAllowOvertakingCurrent()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        track.Move(2);

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B);
        Assert.DoesNotContain(track.Slots, s => s.Delay < 0);
    }

    /*
    Scenario: Whole-track move backward increases delays of non-current entities
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I move the track by -2
      Then the current entity is A
      And entity B is at delay 4
    */
    [Fact]
    public void Move_Backward_IncreasesDelaysOfNonCurrent()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        track.Move(-2);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (4, new IHasInitiative[] { B })
        );
    }

    /*
    Scenario: Whole-track move backward moves queued slot-0 followers away preserving order
      Given I add entity A with delay 0
      And I add entity B with delay 0
      And I add entity C with delay 0
      When I move the track by -1
      Then the current entity is A
      And the slots are:
        | delay | row |
        | 0     | A   |
        | 1     | B,C |
    */
    [Fact]
    public void Move_Backward_MovesZeroFollowersToOne_PreservingOrder()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 0);
        track.Add(C, 0);

        track.Move(-1);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B, C })
        );
    }

    // ============================================================
    // Tick emission
    // ============================================================

    /*
    Scenario: Move emits Tick once per step with direction +1
      Given I add entity A with delay 0
      When I move the track by 3
      Then Tick is emitted 3 times
      And each Tick has direction +1
    */
    [Fact]
    public void Tick_MoveForward_EmittedPerStep_WithPositiveDirection()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        track.Add(A, 0);

        var ticks = new List<int>();
        track.Tick += dir => ticks.Add(dir);

        track.Move(3);

        Assert.Equal(3, ticks.Count);
        Assert.All(ticks, d => Assert.Equal(+1, d));
    }

    /*
    Scenario: Move emits Tick once per step with direction -1
      Given I add entity A with delay 0
      When I move the track by -2
      Then Tick is emitted 2 times
      And each Tick has direction -1
    */
    [Fact]
    public void Tick_MoveBackward_EmittedPerStep_WithNegativeDirection()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        track.Add(A, 0);

        var ticks = new List<int>();
        track.Tick += dir => ticks.Add(dir);

        track.Move(-2);

        Assert.Equal(2, ticks.Count);
        Assert.All(ticks, d => Assert.Equal(-1, d));
    }

    /*
    Scenario: Tick is emitted after each single-tick state update
      Given I add entity A with delay 0
      And I add entity B with delay 1
      When I move the track by 1
      Then Tick is emitted once
      And at the time Tick observers run, the slot with delay 0 row order is A,B
    */
    [Fact]
    public void Tick_IsEmittedAfterSingleTickStateUpdate()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        var observedCorrectState = false;
        track.Tick += _ =>
        {
            // Observers see post-step state.
            observedCorrectState = track.Slots.Any(s => s.Delay == 0)
                                   && track.Slots.Single(s => s.Delay == 0).Row.Count == 2
                                   && ReferenceEquals(track.Slots.Single(s => s.Delay == 0).Row[0], A)
                                   && ReferenceEquals(track.Slots.Single(s => s.Delay == 0).Row[1], B);
        };

        track.Move(1);

        Assert.True(observedCorrectState);
    }

    /*
    Scenario: Stage does not emit Tick
      Given I add entity A with delay 0
      When I stage the track by 5
      Then Tick is not emitted
    */
    [Fact]
    public void Stage_DoesNotEmitTick()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        track.Add(A, 0);

        var tickCount = 0;
        track.Tick += _ => tickCount++;

        track.Stage(5);

        Assert.Equal(0, tickCount);
    }

    // ============================================================
    // Stage and ClearStaging
    // ============================================================

    /*
    Scenario: Stage updates derived getters and slots
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I stage the track by 1
      Then the slots include:
        | delay | row |
        | 0     | A   |
        | 1     | B   |
      And the current entity is A
      And the delay to next is 1
    */
    [Fact]
    public void Stage_UpdatesSlotsAndDerivedGetters()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        track.Stage(1);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B })
        );
        Assert.Equal(1, track.DelayToNext);
        Assert.Same(B, track.Next);
    }

    /*
    Scenario: ClearStaging reverts to the snapshot captured at the first Stage
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I stage the track by 1
      And I clear staging
      Then the slots are:
        | delay | row |
        | 0     | A   |
        | 2     | B   |
      And the current entity is A
      And the delay to next is 2
    */
    [Fact]
    public void ClearStaging_RevertsToFirstStageSnapshot()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        track.Stage(1);
        track.ClearStaging();

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (2, new IHasInitiative[] { B })
        );
        Assert.Equal(2, track.DelayToNext);
        Assert.Same(B, track.Next);
    }

    /*
    Scenario: Multiple Stage calls compound and ClearStaging reverts to original pre-stage state
      Given I add entity A with delay 0
      And I add entity B with delay 5
      When I stage the track by 2
      And I stage the track by 1
      Then the slots include:
        | delay | row |
        | 0     | A   |
        | 2     | B   |
      When I clear staging
      Then the slots include:
        | delay | row |
        | 0     | A   |
        | 5     | B   |
    */
    [Fact]
    public void Stage_MultipleCallsCompound_ClearStagingRevertsToOriginal()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 5);

        track.Stage(2);
        track.Stage(1);

        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (2, new IHasInitiative[] { B })
        );

        track.ClearStaging();

        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (5, new IHasInitiative[] { B })
        );
    }

    /*
    Scenario: Any mutations after Stage are discarded by ClearStaging
      Given I add entity A with delay 0
      And I add entity B with delay 3
      When I stage the track by 1
      And I move the track by 1
      And I set delay of B to 0
      And I add entity C with delay 2
      And I remove entity A
      And I clear staging
      Then the track state matches exactly the state as it was immediately before the first Stage
      And the slots are:
        | delay | row |
        | 0     | A   |
        | 3     | B   |
      And the current entity is A
    */
    [Fact]
    public void ClearStaging_DiscardsAllMutationsSinceFirstStage()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 3);

        // Begin staging baseline
        track.Stage(1);

        // Mutations after staging begins
        track.Move(1);
        Assert.True(track.SetDelay(B, 0));
        track.Add(C, 2);
        Assert.True(track.Remove(A));

        // Revert
        track.ClearStaging();

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (3, new IHasInitiative[] { B })
        );
    }

    // ============================================================
    // Derived getters Current, Next, DelayToNext
    // ============================================================

    /*
    Scenario: Current is always the first entity in slot 0
      Given I add entity A with delay 0
      And I add entity B with delay 0
      Then the slot with delay 0 starts with A
      And the current entity is A
    */
    [Fact]
    public void Current_IsAlwaysFirstOfZeroSlot()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 0);

        Assert.Same(A, track.Current);
        Assert.Same(A, track.Slots.Single(s => s.Delay == 0).Row[0]);
    }

    /*
    Scenario: Next is the second entity in slot 0 if present
      Given I add entity A with delay 0
      And I add entity B with delay 0
      Then the next entity is B
      And the delay to next is 0
    */
    [Fact]
    public void Next_IsSecondInZeroSlot_AndDelayToNextIsZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 0);

        Assert.Same(B, track.Next);
        Assert.Equal(0, track.DelayToNext);
    }

    /*
    Scenario: Next is first entity of the lowest positive-delay slot if slot 0 has only Current
      Given I add entity A with delay 0
      And I add entity B with delay 2
      And I add entity C with delay 1
      Then the next entity is C
      And the delay to next is 1
    */
    [Fact]
    public void Next_IsFirstOfLowestPositiveSlot_WhenZeroHasOnlyCurrent()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 2);
        track.Add(C, 1);

        Assert.Same(C, track.Next);
        Assert.Equal(1, track.DelayToNext);
    }

    /*
    Scenario: DelayToNext is MAX_INT when track has only Current
      Given I add entity A with delay 0
      Then the delay to next is MAX_INT
      And the next entity is null
    */
    [Fact]
    public void DelayToNext_IsMaxInt_WhenOnlyCurrent()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");

        track.Add(A, 0);

        Assert.Equal(int.MaxValue, track.DelayToNext);
        Assert.Null(track.Next);
    }

    // ============================================================
    // Single-entity delay changes (non-current cannot overtake)
    // ============================================================

    /*
    Scenario: Setting delay of non-current to non-negative moves it to that slot (append on collision)
      Given I add entity A with delay 0
      And I add entity B with delay 3
      And I add entity C with delay 1
      When I set delay of B to 1
      Then the slot with delay 1 row order is C,B
      And the current entity is A
    */
    [Fact]
    public void SetDelay_NonCurrent_ToNonNegative_AppendsOnCollision()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 3);
        track.Add(C, 1);

        Assert.True(track.SetDelay(B, 1));

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 1, C, B);
    }

    /*
    Scenario: Setting delay of non-current to negative does not change Current and makes it NEXT in slot 0
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I set delay of B to -1
      Then the current entity is A
      And the slot with delay 0 row order is A,B
      And the next entity is B
    */
    [Fact]
    public void SetDelay_NonCurrent_ToNegative_BecomesNextInZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        Assert.True(track.SetDelay(B, -1));

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B);
        Assert.Same(B, track.Next);
    }

    /*
    Scenario: Setting delay of non-current to negative inserts immediately after Current even if others queued at 0
      Given I add entity A with delay 0
      And I add entity C with delay 0
      And I add entity B with delay 2
      When I set delay of B to -5
      Then the slot with delay 0 row order is A,B,C
      And the next entity is B
    */
    [Fact]
    public void SetDelay_NonCurrent_ToNegative_InsertsAfterCurrent_EvenIfZeroHasFollowers()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(C, 0);
        track.Add(B, 2);

        Assert.True(track.SetDelay(B, -5));

        AssertSlotRow(track, 0, A, B, C);
        Assert.Same(B, track.Next);
    }

    // ============================================================
    // Commit turn / moving Current reanchors to earliest
    // ============================================================

    /*
    Scenario: CommitTurn moves Current back and reanchors to earliest slot then first-in-row
      Given I add entity A with delay 0
      And I add entity B with delay 0
      And I add entity C with delay 2
      When I commit turn with cost 3
      Then the current entity is B
      And the slot with delay 0 starts with B
      And entity A is at delay 3
    */
    [Fact]
    public void CommitTurn_MovesCurrentBack_ThenReanchorsToEarliest_FirstInRow()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 0);
        track.Add(C, 2);

        track.CommitTurn(3);

        Assert.Same(B, track.Current);
        Assert.Same(B, track.Slots.Single(s => s.Delay == 0).Row[0]);

        // A moved back by 3 relative to the new anchor.
        Assert.True(track.Slots.Any(s => s.Delay == 3 && s.Row.Contains(A)));
    }

    /*
    Scenario: Setting delay on Current to non-negative reanchors to earliest slot then first-in-row
      Given I add entity A with delay 0
      And I add entity B with delay 1
      When I set delay of A to 5
      Then the current entity is B
      And the slot with delay 0 starts with B
      And entity A is at delay 4
    */
    [Fact]
    public void SetDelay_Current_ToNonNegative_Reanchors_EarliestThenFirstInRow()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.True(track.SetDelay(A, 5));

        Assert.Same(B, track.Current);
        Assert.Same(B, track.Slots.Single(s => s.Delay == 0).Row[0]);

        // After reanchor (min becomes 0), A ends up 4 ticks away.
        Assert.True(track.Slots.Any(s => s.Delay == 4 && s.Row.Contains(A)));
    }

    /*
    Scenario: Setting delay on Current to negative makes that negative the new zero after reanchoring
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I set delay of A to -3
      Then the current entity is A
      And A is first in slot 0
      And entity B is at delay 5
      And no slot exists with delay less than 0
    */
    [Fact]
    public void SetDelay_Current_ToNegative_ReanchorsSoNegativeBecomesZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        Assert.True(track.SetDelay(A, -3));

        Assert.Same(A, track.Current);
        Assert.Same(A, track.Slots.Single(s => s.Delay == 0).Row[0]);

        Assert.True(track.Slots.Any(s => s.Delay == 5 && s.Row.Contains(B)));
        Assert.DoesNotContain(track.Slots, s => s.Delay < 0);
    }

    // ============================================================
    // Change events (non-tick)
    // ============================================================

    /*
    Scenario: OnChange fires when state changes
      Given I add entity A with delay 0
      When I add entity B with delay 1
      Then OnChange is emitted at least once
    */
    [Fact]
    public void Events_OnChange_FiresOnStateChange()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        var count = 0;
        track.OnChange += () => count++;

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.True(count >= 1);
    }

    /*
    Scenario: SlotsChanged fires when slot membership/order changes
      Given I add entity A with delay 0
      When I add entity B with delay 1
      Then SlotsChanged is emitted
    */
    [Fact]
    public void Events_SlotsChanged_FiresWhenSlotsChange()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        var count = 0;
        track.SlotsChanged += _ => count++;

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.True(count >= 1);
    }

    /*
    Scenario: CurrentChanged fires when Current changes
      Given I add entity A with delay 0
      And I add entity B with delay 1
      When I set delay of A to 5
      Then CurrentChanged is emitted
      And the current entity is B
    */
    [Fact]
    public void Events_CurrentChanged_FiresWhenCurrentChanges()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        var count = 0;
        track.CurrentChanged += _ => count++;

        track.Add(A, 0);
        track.Add(B, 1);

        track.SetDelay(A, 5);

        Assert.True(count >= 1);
        Assert.Same(B, track.Current);
    }

    /*
    Scenario: DelayToNextChanged fires when DelayToNext changes
      Given I add entity A with delay 0
      And I add entity B with delay 2
      Then the delay to next is 2
      When I move the track by 1
      Then DelayToNextChanged is emitted
      And the delay to next is 1
    */
    [Fact]
    public void Events_DelayToNextChanged_FiresWhenDelayChanges()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        Assert.Equal(2, track.DelayToNext);

        var count = 0;
        track.DelayToNextChanged += _ => count++;

        track.Move(1);

        Assert.True(count >= 1);
        Assert.Equal(1, track.DelayToNext);
    }

    /*
    Scenario: Stage emits non-tick change events but not Tick
      Given I add entity A with delay 0
      And I add entity B with delay 2
      When I stage the track by 1
      Then SlotsChanged is emitted
      And DelayToNextChanged is emitted
      And Tick is not emitted
    */
    [Fact]
    public void Events_Stage_EmitsNonTickEvents_ButNotTick()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        var slotsChanged = 0;
        var delayChanged = 0;
        var ticked = 0;

        track.SlotsChanged += _ => slotsChanged++;
        track.DelayToNextChanged += _ => delayChanged++;
        track.Tick += _ => ticked++;

        track.Stage(1);

        Assert.True(slotsChanged >= 1);
        Assert.True(delayChanged >= 1);
        Assert.Equal(0, ticked);
    }

    // ============================================================
    // Edge cases and invariants
    // ============================================================

    /*
    Scenario: Setting delay of an entity not on the track returns false and does not change state
      Given I add entity A with delay 0
      When I set delay of D to 1
      Then the result is false
      And the current entity is A
      And the slots are:
        | delay | row |
        | 0     | A   |
    */
    [Fact]
    public void SetDelay_EntityNotOnTrack_ReturnsFalse_AndNoStateChange()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var D = new TestEntity("D");

        track.Add(A, 0);

        var result = track.SetDelay(D, 1);

        Assert.False(result);
        Assert.Same(A, track.Current);
        AssertSlots(track, (0, new IHasInitiative[] { A }));
    }

    /*
    Scenario: Move on an empty track does not emit Tick and does not emit OnChange
      When I move the track by 3
      Then the slots are empty
      And Tick is not emitted
      And OnChange is not emitted
    */
    [Fact]
    public void Move_EmptyTrack_NoTick_NoOnChange()
    {
        var track = CreateTrack();

        var ticked = 0;
        var changed = 0;

        track.Tick += _ => ticked++;
        track.OnChange += () => changed++;

        track.Move(3);

        Assert.Empty(track.Slots);
        Assert.Equal(0, ticked);
        Assert.Equal(0, changed);
    }

    /*
    Scenario: Stage on an empty track does not emit Tick and does not emit OnChange
      When I stage the track by 3
      Then the slots are empty
      And Tick is not emitted
      And OnChange is not emitted
    */
    [Fact]
    public void Stage_EmptyTrack_NoTick_NoOnChange()
    {
        var track = CreateTrack();

        var ticked = 0;
        var changed = 0;

        track.Tick += _ => ticked++;
        track.OnChange += () => changed++;

        track.Stage(3);

        Assert.Empty(track.Slots);
        Assert.Equal(0, ticked);
        Assert.Equal(0, changed);
    }
}
