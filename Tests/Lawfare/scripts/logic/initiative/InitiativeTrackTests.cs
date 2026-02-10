using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.logic.initiative;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeTrackTests
{
    private sealed class TestEntity : IHasInitiative
    {
        public TestEntity(string name) => Name = name;
        public string Name { get; }
        public override string ToString() => Name;
    }

    private static IInitiativeTrack CreateTrack() => new InitiativeTrack();

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

    private static void AssertSlotsInclude(IInitiativeTrack track, params (int delay, IHasInitiative[] row)[] expected)
    {
        foreach (var (delay, row) in expected)
        {
            var slot = track.Slots.Single(s => s.Delay == delay);
            Assert.Equal(row.Length, slot.Row.Count);
            for (int i = 0; i < row.Length; i++)
                Assert.Same(row[i], slot.Row[i]);
        }
    }

    // ---------------------------------------------------------------------------
    // Adding and removing entities
    // ---------------------------------------------------------------------------

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
  And the delay to next is 0
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
        Assert.Equal(0, track.DelayToNext);
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

        Assert.True(track.Remove(B));
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
        Assert.True(track.Remove(A));

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

        Assert.True(track.Remove(A));

        Assert.Same(C, track.Current);
        Assert.Same(C, track.Slots.Single(s => s.Delay == 0).Row[0]);
    }

    // ---------------------------------------------------------------------------
    // Slot representation and collision ordering
    // ---------------------------------------------------------------------------

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
  When I move the anchor by 1
  Then the slot with delay 0 row order is A,B,C
*/
    [Fact]
    public void MoveAnchor_Collision_AppendsToBackOfRow()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 1);
        track.Add(C, 1);

        track.MoveAnchor(1);

        AssertSlotRow(track, 0, A, B, C);
    }

    // ---------------------------------------------------------------------------
    // Derived getters
    // ---------------------------------------------------------------------------

    /*
Scenario: Current is always the first entity in slot 0
  Given I add entity A with delay 0
  And I add entity B with delay 0
  Then the slot with delay 0 starts with A
  And the current entity is A
*/
    [Fact]
    public void Derived_Current_IsFirstInZeroSlot()
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
    public void Derived_Next_IsSecondInZero_WhenPresent()
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
    public void Derived_Next_IsLowestPositiveSlot_WhenZeroHasOnlyCurrent()
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
    public void Derived_DelayToNext_IsMaxInt_WhenOnlyCurrent()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");

        track.Add(A, 0);

        Assert.Equal(int.MaxValue, track.DelayToNext);
        Assert.Null(track.Next);
    }

    // ---------------------------------------------------------------------------
    // MoveAnchor(dt): anchored movement
    // ---------------------------------------------------------------------------

    /*
Scenario: MoveAnchor forward keeps Current at the front of slot 0
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I move the anchor by 1
  Then the current entity is A
  And the slot with delay 0 row order is A,B
*/
    [Fact]
    public void MoveAnchor_Forward_KeepsCurrentAtFrontOfZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        track.MoveAnchor(1);

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B);
    }

    /*
Scenario: MoveAnchor forward decreases delays of non-current entities until they catch up
  Given I add entity A with delay 0
  And I add entity B with delay 3
  When I move the anchor by 2
  Then the slots include:
    | delay | row |
    | 0     | A   |
    | 1     | B   |
  And the current entity is A
*/
    [Fact]
    public void MoveAnchor_Forward_DecreasesDelays_UntilCatchUp()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 3);

        track.MoveAnchor(2);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B })
        );
    }

    /*
Scenario: MoveAnchor forward does not allow overtaking Current
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I move the anchor by 2
  Then the current entity is A
  And the slot with delay 0 row order is A,B
  And no slot exists with delay less than 0
*/
    [Fact]
    public void MoveAnchor_Forward_DoesNotAllowOvertakingCurrent()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        track.MoveAnchor(2);

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B);
        Assert.DoesNotContain(track.Slots, s => s.Delay < 0);
    }

    /*
Scenario: MoveAnchor backward increases delays of all non-current entities
  Given I add entity A with delay 0
  And I add entity B with delay 2
  When I move the anchor by -2
  Then the current entity is A
  And the slots include:
    | delay | row |
    | 0     | A   |
    | 4     | B   |
*/
    [Fact]
    public void MoveAnchor_Backward_IncreasesDelays()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        track.MoveAnchor(-2);

        Assert.Same(A, track.Current);
        AssertSlotsInclude(track,
            (0, new IHasInitiative[] { A }),
            (4, new IHasInitiative[] { B })
        );
    }

    /*
Scenario: MoveAnchor backward moves queued slot-0 followers away preserving order
  Given I add entity A with delay 0
  And I add entity B with delay 0
  And I add entity C with delay 0
  When I move the anchor by -1
  Then the current entity is A
  And the slots are:
    | delay | row |
    | 0     | A   |
    | 1     | B,C |
*/
    [Fact]
    public void MoveAnchor_Backward_MovesZeroFollowersToOne_PreservingOrder()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(B, 0);
        track.Add(C, 0);

        track.MoveAnchor(-1);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B, C })
        );
    }

    // ---------------------------------------------------------------------------
    // Tick emission
    // ---------------------------------------------------------------------------

    /*
Scenario: MoveAnchor emits Tick once per step with direction +1
  Given I add entity A with delay 0
  When I move the anchor by 3
  Then Tick is emitted 3 times
  And each Tick has direction +1
*/
    [Fact]
    public void Tick_MoveAnchorForward_EmitsPerStep_PositiveDirection()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        track.Add(A, 0);

        var ticks = new List<int>();
        track.Tick += dir => ticks.Add(dir);

        track.MoveAnchor(3);

        Assert.Equal(3, ticks.Count);
        Assert.All(ticks, d => Assert.Equal(+1, d));
    }

    /*
Scenario: MoveAnchor emits Tick once per step with direction -1
  Given I add entity A with delay 0
  When I move the anchor by -2
  Then Tick is emitted 2 times
  And each Tick has direction -1
*/
    [Fact]
    public void Tick_MoveAnchorBackward_EmitsPerStep_NegativeDirection()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        track.Add(A, 0);

        var ticks = new List<int>();
        track.Tick += dir => ticks.Add(dir);

        track.MoveAnchor(-2);

        Assert.Equal(2, ticks.Count);
        Assert.All(ticks, d => Assert.Equal(-1, d));
    }

    /*
Scenario: Tick is emitted after each single-tick state update during MoveAnchor
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I move the anchor by 1
  Then Tick is emitted once
  And at the time Tick observers run, the slot with delay 0 row order is A,B
*/
    [Fact]
    public void Tick_IsEmittedAfterEachSingleTickStateUpdate_DuringMoveAnchor()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        var observedCorrectState = false;
        track.Tick += _ =>
        {
            var zero = track.Slots.Single(s => s.Delay == 0);
            observedCorrectState = zero.Row.Count == 2
                                   && ReferenceEquals(zero.Row[0], A)
                                   && ReferenceEquals(zero.Row[1], B);
        };

        track.MoveAnchor(1);

        Assert.True(observedCorrectState);
    }

    // ---------------------------------------------------------------------------
    // StageAnchor(dt)
    // ---------------------------------------------------------------------------

    /*
Scenario: StageAnchor does not emit Tick
  Given I add entity A with delay 0
  When I stage the anchor by 5
  Then Tick is not emitted
*/
    [Fact]
    public void StageAnchor_DoesNotEmitTick()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        track.Add(A, 0);

        var tickCount = 0;
        track.Tick += _ => tickCount++;

        track.StageAnchor(5);

        Assert.Equal(0, tickCount);
    }

    /*
Scenario: StageAnchor updates derived getters and slots
  Given I add entity A with delay 0
  And I add entity B with delay 2
  When I stage the anchor by 1
  Then the slots include:
    | delay | row |
    | 0     | A   |
    | 1     | B   |
  And the current entity is A
  And the delay to next is 1
*/
    [Fact]
    public void StageAnchor_UpdatesGettersAndSlots()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        track.StageAnchor(1);

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B })
        );
        Assert.Equal(1, track.DelayToNext);
        Assert.Same(B, track.Next);
    }

    // ---------------------------------------------------------------------------
    // Move(entity, dt)
    // ---------------------------------------------------------------------------

    /*
Scenario: Move a non-current entity by positive dt increases its delay
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I move entity B by 2
  Then the current entity is A
  And the slots are:
    | delay | row |
    | 0     | A   |
    | 3     | B   |
*/
    [Fact]
    public void MoveEntity_NonCurrent_Positive_IncreasesDelay()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.True(track.Move(B, 2));

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (3, new IHasInitiative[] { B })
        );
    }

    /*
Scenario: Move a non-current entity by negative dt decreases its delay
  Given I add entity A with delay 0
  And I add entity B with delay 5
  When I move entity B by -2
  Then the current entity is A
  And the slots include:
    | delay | row |
    | 3     | B   |
*/
    [Fact]
    public void MoveEntity_NonCurrent_Negative_DecreasesDelay()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 5);

        Assert.True(track.Move(B, -2));

        Assert.Same(A, track.Current);
        AssertSlotsInclude(track, (3, new IHasInitiative[] { B }));
    }

    /*
Scenario: Move a non-current entity into negative makes it NEXT in slot 0
  Given I add entity A with delay 0
  And I add entity C with delay 0
  And I add entity B with delay 2
  When I move entity B by -5
  Then the current entity is A
  And the slot with delay 0 row order is A,B,C
  And the next entity is B
*/
    [Fact]
    public void MoveEntity_NonCurrent_IntoNegative_BecomesNextInZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(C, 0);
        track.Add(B, 2);

        Assert.True(track.Move(B, -5));

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 0, A, B, C);
        Assert.Same(B, track.Next);
    }

    /*
Scenario: Move a non-current entity onto an occupied slot appends to that slot's row
  Given I add entity A with delay 0
  And I add entity C with delay 1
  And I add entity B with delay 3
  When I move entity B by -2
  Then the slot with delay 1 row order is C,B
*/
    [Fact]
    public void MoveEntity_NonCurrent_OntoOccupiedSlot_Appends()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Add(A, 0);
        track.Add(C, 1);
        track.Add(B, 3);

        Assert.True(track.Move(B, -2));

        AssertSlotRow(track, 1, C, B);
    }

    /*
Scenario: Move(Current, dt) triggers reanchor to earliest slot then first-in-row
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I move entity A by 5
  Then the current entity is B
  And the slot with delay 0 starts with B
  And entity A is at delay 4
*/
    [Fact]
    public void MoveEntity_Current_TriggersReanchor_EarliestThenFirstInRow()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.True(track.Move(A, 5));

        Assert.Same(B, track.Current);
        Assert.Same(B, track.Slots.Single(s => s.Delay == 0).Row[0]);
        Assert.True(track.Slots.Any(s => s.Delay == 4 && s.Row.Contains(A)));
    }

    // ---------------------------------------------------------------------------
    // Stage(entity, dt)
    // ---------------------------------------------------------------------------

    /*
Scenario: Stage(entity, dt) does not emit Tick but updates getters and slots
  Given I add entity A with delay 0
  And I add entity B with delay 2
  When I stage entity B by -1
  Then Tick is not emitted
  And the slots include:
    | delay | row |
    | 0     | A   |
    | 1     | B   |
*/
    [Fact]
    public void StageEntity_DoesNotEmitTick_ButUpdatesState()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 2);

        var tickCount = 0;
        track.Tick += _ => tickCount++;

        Assert.True(track.Stage(B, -1));

        Assert.Equal(0, tickCount);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (1, new IHasInitiative[] { B })
        );
    }

    /*
Scenario: Stage(Current, dt) triggers reanchor but does not emit Tick
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I stage entity A by 5
  Then Tick is not emitted
  And the current entity is B
  And entity A is at delay 4
*/
    [Fact]
    public void StageEntity_Current_TriggersReanchor_ButNoTick()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        var tickCount = 0;
        track.Tick += _ => tickCount++;

        Assert.True(track.Stage(A, 5));

        Assert.Equal(0, tickCount);
        Assert.Same(B, track.Current);
        Assert.True(track.Slots.Any(s => s.Delay == 4 && s.Row.Contains(A)));
    }

    // ---------------------------------------------------------------------------
    // ClearStaging semantics
    // ---------------------------------------------------------------------------

    /*
Scenario: ClearStaging reverts to the snapshot captured at the first Stage* call
  Given I add entity A with delay 0
  And I add entity B with delay 2
  When I stage the anchor by 1
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

        track.StageAnchor(1);
        track.ClearStaging();

        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (2, new IHasInitiative[] { B })
        );
        Assert.Same(A, track.Current);
        Assert.Equal(2, track.DelayToNext);
        Assert.Same(B, track.Next);
    }

    /*
Scenario: Multiple Stage* calls compound and ClearStaging reverts to the original pre-stage state
  Given I add entity A with delay 0
  And I add entity B with delay 5
  When I stage the anchor by 2
  And I stage entity B by -1
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
    public void ClearStaging_MultipleStageCalls_RevertsToOriginalPreStage()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 5);

        track.StageAnchor(2);
        Assert.True(track.Stage(B, -1));

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
Scenario: Any mutations after staging begins are discarded by ClearStaging
  Given I add entity A with delay 0
  And I add entity B with delay 3
  When I stage the anchor by 1
  And I move the anchor by 1
  And I move entity B by -3
  And I add entity C with delay 2
  And I remove entity A
  And I clear staging
  Then the track state matches exactly the state as it was immediately before the first Stage* call
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

        track.StageAnchor(1);

        track.MoveAnchor(1);
        Assert.True(track.Move(B, -3));
        track.Add(C, 2);
        Assert.True(track.Remove(A));

        track.ClearStaging();

        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (3, new IHasInitiative[] { B })
        );
        Assert.Same(A, track.Current);
    }

    // ---------------------------------------------------------------------------
    // CommitTurn / SetDelay (explicit absolute delay operations)
    // ---------------------------------------------------------------------------

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
    public void CommitTurn_MovesCurrentBack_ThenReanchors()
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
        Assert.True(track.Slots.Any(s => s.Delay == 3 && s.Row.Contains(A)));
    }

    /*
Scenario: SetDelay of non-current to non-negative moves it to that slot (append on collision)
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
Scenario: SetDelay of non-current to negative does not change Current and makes it NEXT in slot 0
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
Scenario: SetDelay of Current to non-negative reanchors to earliest slot then first-in-row
  Given I add entity A with delay 0
  And I add entity B with delay 1
  When I set delay of A to 5
  Then the current entity is B
  And the slot with delay 0 starts with B
  And entity A is at delay 4
*/
    [Fact]
    public void SetDelay_Current_ToNonNegative_Reanchors()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.True(track.SetDelay(A, 5));

        Assert.Same(B, track.Current);
        Assert.Same(B, track.Slots.Single(s => s.Delay == 0).Row[0]);
        Assert.True(track.Slots.Any(s => s.Delay == 4 && s.Row.Contains(A)));
    }

    /*
Scenario: SetDelay of Current to negative makes that negative the new zero after reanchoring
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

    // ---------------------------------------------------------------------------
    // Change events (non-tick)
    // ---------------------------------------------------------------------------

    /*
Scenario: OnChange fires when state changes
  Given I add entity A with delay 0
  When I add entity B with delay 1
  Then OnChange is emitted at least once
*/
    [Fact]
    public void Events_OnChange_FiresWhenStateChanges()
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
Scenario: SlotsChanged fires when slot membership or order changes
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
Scenario: NextChanged fires when Next changes
  Given I add entity A with delay 0
  And I add entity B with delay 1
  Then the next entity is B
  When I move the anchor by 1
  Then NextChanged is emitted
  And the next entity is B
  And the delay to next is 0
*/
    [Fact]
    public void Events_NextChanged_FiresWhenNextChanges()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        var count = 0;
        track.NextChanged += _ => count++;

        track.Add(A, 0);
        track.Add(B, 1);

        Assert.Same(B, track.Next);

        track.MoveAnchor(1);

        Assert.True(count >= 1);
        Assert.Same(B, track.Next);
        Assert.Equal(0, track.DelayToNext);
    }

    /*
Scenario: DelayToNextChanged fires when DelayToNext changes
  Given I add entity A with delay 0
  And I add entity B with delay 2
  Then the delay to next is 2
  When I move the anchor by 1
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

        track.MoveAnchor(1);

        Assert.True(count >= 1);
        Assert.Equal(1, track.DelayToNext);
    }

    /*
Scenario: Stage* emits non-tick change events but not Tick
  Given I add entity A with delay 0
  And I add entity B with delay 2
  When I stage the anchor by 1
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

        track.StageAnchor(1);

        Assert.True(slotsChanged >= 1);
        Assert.True(delayChanged >= 1);
        Assert.Equal(0, ticked);
    }

    // ---------------------------------------------------------------------------
    // Edge cases and invariants
    // ---------------------------------------------------------------------------

    /*
Scenario: Moving an entity not on the track returns false and does not change state
  Given I add entity A with delay 0
  When I move entity D by 1
  Then the result is false
  And the current entity is A
  And the slots are:
    | delay | row |
    | 0     | A   |
*/
    [Fact]
    public void MoveEntity_NotOnTrack_ReturnsFalse_NoStateChange()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var D = new TestEntity("D");

        track.Add(A, 0);

        var ok = track.Move(D, 1);

        Assert.False(ok);
        Assert.Same(A, track.Current);
        AssertSlots(track, (0, new IHasInitiative[] { A }));
    }

    /*
Scenario: Staging an entity not on the track returns false and does not change state
  Given I add entity A with delay 0
  When I stage entity D by 1
  Then the result is false
  And the current entity is A
  And the slots are:
    | delay | row |
    | 0     | A   |
*/
    [Fact]
    public void StageEntity_NotOnTrack_ReturnsFalse_NoStateChange()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var D = new TestEntity("D");

        track.Add(A, 0);

        var ok = track.Stage(D, 1);

        Assert.False(ok);
        Assert.Same(A, track.Current);
        AssertSlots(track, (0, new IHasInitiative[] { A }));
    }

    /*
Scenario: MoveAnchor on an empty track does not emit Tick and does not emit OnChange
  When I move the anchor by 3
  Then the slots are empty
  And Tick is not emitted
  And OnChange is not emitted
*/
    [Fact]
    public void MoveAnchor_EmptyTrack_NoTick_NoOnChange()
    {
        var track = CreateTrack();

        var ticked = 0;
        var changed = 0;

        track.Tick += _ => ticked++;
        track.OnChange += () => changed++;

        track.MoveAnchor(3);

        Assert.Empty(track.Slots);
        Assert.Equal(0, ticked);
        Assert.Equal(0, changed);
    }

    /*
Scenario: StageAnchor on an empty track does not emit Tick and does not emit OnChange
  When I stage the anchor by 3
  Then the slots are empty
  And Tick is not emitted
  And OnChange is not emitted
*/
    [Fact]
    public void StageAnchor_EmptyTrack_NoTick_NoOnChange()
    {
        var track = CreateTrack();

        var ticked = 0;
        var changed = 0;

        track.Tick += _ => ticked++;
        track.OnChange += () => changed++;

        track.StageAnchor(3);

        Assert.Empty(track.Slots);
        Assert.Equal(0, ticked);
        Assert.Equal(0, changed);
    }
    
    // ---------------------------------------------------------------------------
    // Seed(entries)
    // ---------------------------------------------------------------------------

    /*
    Scenario: Seeding an empty list does nothing
      Given an empty initiative track
      When I seed with no entries
      Then the track remains empty
      And the current entity is null
      And the next entity is null
      And the delay to next is MAX_INT
    */
    [Fact]
    public void Seed_EmptyList_DoesNothing()
    {
        var track = CreateTrack();

        track.Seed(Array.Empty<(IHasInitiative entity, int delay)>());

        Assert.Empty(track.Slots);
        Assert.Null(track.Current);
        Assert.Null(track.Next);
        Assert.Equal(int.MaxValue, track.DelayToNext);
    }

    /*
    Scenario: Seeding determines Current from the lowest initial delay and reanchors to 0
      Given an empty initiative track
      When I seed:
        | entity | delay |
        | A      | 2     |
        | B      | 5     |
        | C      | 5     |
      Then the current entity is A
      And the slots are:
        | delay | row |
        | 0     | A   |
        | 3     | B,C |
      And the next entity is B
      And the delay to next is 3
    */
    [Fact]
    public void Seed_ReanchorsToMinimumDelay_Example_2_5_5()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Seed(new (IHasInitiative entity, int delay)[]
        {
            (A, 2),
            (B, 5),
            (C, 5),
        });

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (3, new IHasInitiative[] { B, C })
        );
        Assert.Same(B, track.Next);
        Assert.Equal(3, track.DelayToNext);
    }

    /*
    Scenario: Seeding with a tie for lowest delay picks Current by input order
      Given an empty initiative track
      When I seed:
        | entity | delay |
        | B      | 1     |
        | A      | 1     |
        | C      | 2     |
      Then the current entity is B
      And the slot with delay 0 row order is B,A
      And the next entity is A
      And the delay to next is 0
      And the slot with delay 1 row order is C
    */
    [Fact]
    public void Seed_TiedMinimum_PicksCurrentByInputOrder()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Seed(new (IHasInitiative entity, int delay)[]
        {
            (B, 1),
            (A, 1),
            (C, 2),
        });

        Assert.Same(B, track.Current);
        AssertSlotRow(track, 0, B, A);
        Assert.Same(A, track.Next);
        Assert.Equal(0, track.DelayToNext);
        AssertSlotRow(track, 1, C);
    }

    /*
    Scenario: Seeding preserves row order within the same resulting delay by input order
      Given an empty initiative track
      When I seed:
        | entity | delay |
        | A      | 10    |
        | B      | 12    |
        | C      | 12    |
        | D      | 12    |
      Then the current entity is A
      And the slot with delay 2 row order is B,C,D
    */
    [Fact]
    public void Seed_PreservesRowOrder_OnCollision_ByInputOrder()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");
        var D = new TestEntity("D");

        track.Seed(new (IHasInitiative entity, int delay)[]
        {
            (A, 10),
            (B, 12),
            (C, 12),
            (D, 12),
        });

        Assert.Same(A, track.Current);
        AssertSlotRow(track, 2, B, C, D);
    }

    /*
    Scenario: Seeding supports negative delays and reanchors so minimum becomes 0
      Given an empty initiative track
      When I seed:
        | entity | delay |
        | A      | -3    |
        | B      | 0     |
        | C      | -1    |
      Then the current entity is A
      And the slots are:
        | delay | row |
        | 0     | A   |
        | 2     | C   |
        | 3     | B   |
      And the next entity is C
      And the delay to next is 2
    */
    [Fact]
    public void Seed_AllowsNegativeDelays_ReanchorsSoMinimumIsZero()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");
        var C = new TestEntity("C");

        track.Seed(new (IHasInitiative entity, int delay)[]
        {
            (A, -3),
            (B, 0),
            (C, -1),
        });

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (2, new IHasInitiative[] { C }),
            (3, new IHasInitiative[] { B })
        );
        Assert.Same(C, track.Next);
        Assert.Equal(2, track.DelayToNext);
    }

    /*
    Scenario: Seeding replaces existing track state
      Given an initiative track with entity X at delay 0
      When I seed:
        | entity | delay |
        | A      | 1     |
        | B      | 4     |
      Then the current entity is A
      And the slots are:
        | delay | row |
        | 0     | A   |
        | 3     | B   |
      And entity X is not on the track
    */
    [Fact]
    public void Seed_ReplacesExistingState()
    {
        var track = CreateTrack();
        var X = new TestEntity("X");
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        track.Add(X, 0);

        track.Seed(new (IHasInitiative entity, int delay)[]
        {
            (A, 1),
            (B, 4),
        });

        Assert.Same(A, track.Current);
        AssertSlots(track,
            (0, new IHasInitiative[] { A }),
            (3, new IHasInitiative[] { B })
        );

        Assert.False(track.Remove(X)); // if X isn't present, Remove should return false
    }

    /*
    Scenario: Seeding emits non-tick change events but not Tick
      Given an empty initiative track
      When I seed:
        | entity | delay |
        | A      | 2     |
        | B      | 5     |
      Then Tick is not emitted
      And OnChange is emitted at least once
      And SlotsChanged is emitted at least once
      And CurrentChanged is emitted at least once
    */
    [Fact]
    public void Seed_EmitsChangeEvents_ButNotTick()
    {
        var track = CreateTrack();
        var A = new TestEntity("A");
        var B = new TestEntity("B");

        var ticked = 0;
        var changed = 0;
        var slotsChanged = 0;
        var currentChanged = 0;

        track.Tick += _ => ticked++;
        track.OnChange += () => changed++;
        track.SlotsChanged += _ => slotsChanged++;
        track.CurrentChanged += _ => currentChanged++;

        track.Seed(new (IHasInitiative entity, int delay)[]
        {
            (A, 2),
            (B, 5),
        });

        Assert.Equal(0, ticked);
        Assert.True(changed >= 1);
        Assert.True(slotsChanged >= 1);
        Assert.True(currentChanged >= 1);
    }
}