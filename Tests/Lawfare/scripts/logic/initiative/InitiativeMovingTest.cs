using System;
using System.Linq;
using Xunit;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.logic.effects.initiative;
using Lawfare.scripts.subject;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeMovingTest
{
    private sealed class TestEntity : SubjectStub, IHasInitiative
    {
        public TestEntity(string name) : base(name) { }
        public override string ToString() => Label;
    }

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
        public Lawyer GetSpeaker(Team team) => null;
        public Team GetTeam(ISubject subject) => throw new NotImplementedException();
        public Team GetOpposingTeam(ISubject subject) => throw new NotImplementedException();
    }

    private static TestEntity E(string name) => new TestEntity(name);

    private static InitiativeTrackState State(
        int currentIndex,
        int roundEndIndex,
        params (IHasInitiative? occupant, bool isStaggered)[] slots)
    {
        return new InitiativeTrackState
        {
            CurrentIndex = currentIndex,
            RoundEndIndex = roundEndIndex,
            Slots = slots.Select(s => new InitiativeSlotState
            {
                Occupant = s.occupant,
                IsStaggered = s.isStaggered
            }).ToArray()
        };
    }

    private static void ApplyDiffs(TestContext ctx, InitiativeDiff[] diffs)
    {
        foreach (var d in diffs)
            d.Apply();
    }

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
    Scenario: MoveEntity to an empty destination produces one diff and vacates the source slot
      Given RoundEndIndex is 4, CurrentIndex is 1, TrackLength is 5
      And slots: (empty), B, X, (empty), (empty)
      When I move entity X by dt 2
      Then slots become: (empty), B, (empty), (empty), X
      And exactly 1 diff: X originalIndex=2 updatedIndex=4 becameStaggered=false
    */
    [Fact]
    public void MoveEntity_ToEmptyDestination_ProducesOneDiff()
    {
        var B = E("B");
        var X = E("X");

        var ctx = new TestContext
        {
            InitiativeTrack = State(1, 4,
                (null, false),
                (B,    false),
                (X,    false),
                (null, false),
                (null, false))
        };

        var diffs = Initiative.MoveEntity(ctx, X, 2);
        ApplyDiffs(ctx, diffs);

        AssertSlots(ctx.InitiativeTrack,
            (null, false),
            (B,    false),
            (null, false),
            (null, false),
            (X,    false));

        Assert.Single(diffs);
        Assert.Same(X, diffs[0].Subject);
        Assert.Equal(2, diffs[0].OriginalIndex);
        Assert.Equal(4, diffs[0].UpdatedIndex);
        Assert.False(diffs[0].BecameStaggered);
    }

    /*
    Scenario: Destination at or before CurrentIndex is clamped to CurrentIndex+1
      Given RoundEndIndex is 5, CurrentIndex is 2, TrackLength is 6
      And slots: A, (empty), C, (empty), (empty), X
      When I move entity X by dt -10
      Then slots become: A, (empty), C, X, (empty), (empty)
      And diff: X originalIndex=5 updatedIndex=3 becameStaggered=false
    */
    [Fact]
    public void MoveEntity_DestinationBeforeCurrentIndex_ClampedToCurrentPlusOne()
    {
        var A = E("A");
        var C = E("C");
        var X = E("X");

        var ctx = new TestContext
        {
            InitiativeTrack = State(2, 5,
                (A,    false),
                (null, false),
                (C,    false),
                (null, false),
                (null, false),
                (X,    false))
        };

        var diffs = Initiative.MoveEntity(ctx, X, -10);
        ApplyDiffs(ctx, diffs);

        AssertSlots(ctx.InitiativeTrack,
            (A,    false),
            (null, false),
            (C,    false),
            (X,    false),
            (null, false),
            (null, false));

        Assert.Single(diffs);
        Assert.Equal(5, diffs[0].OriginalIndex);
        Assert.Equal(3, diffs[0].UpdatedIndex);
        Assert.False(diffs[0].BecameStaggered);
    }

    /*
    Scenario: Collision resolves via forward cascade when a vacancy exists between CurrentIndex and destination
      Given RoundEndIndex is 6, CurrentIndex is 1, TrackLength is 7
      And slots: (empty), B, (empty), C, D, (empty), X
      When I move entity X by dt=-2 (destination index 4)
      Then slots become: (empty), B, C, D, X, (empty), (empty)
      And diffs for C, D, X — none becameStaggered
    */
    [Fact]
    public void MoveEntity_ForwardCascade_WhenVacancyExistsBetweenCurrentAndDestination()
    {
        var B = E("B");
        var C = E("C");
        var D = E("D");
        var X = E("X");

        var ctx = new TestContext
        {
            InitiativeTrack = State(1, 6,
                (null, false),
                (B,    false),
                (null, false),
                (C,    false),
                (D,    false),
                (null, false),
                (X,    false))
        };

        var diffs = Initiative.MoveEntity(ctx, X, -2);
        ApplyDiffs(ctx, diffs);

        AssertSlots(ctx.InitiativeTrack,
            (null, false),
            (B,    false),
            (C,    false),
            (D,    false),
            (X,    false),
            (null, false),
            (null, false));

        var subjects = diffs.Select(d => d.Subject).ToArray();
        Assert.Contains(C, subjects);
        Assert.Contains(D, subjects);
        Assert.Contains(X, subjects);
        Assert.All(diffs, d => Assert.False(d.BecameStaggered));
    }

    /*
    Scenario: Forward cascade uses the vacancy left by the moved object itself
      Given RoundEndIndex is 3, CurrentIndex is 0, TrackLength is 4
      And slots: Z, A, B, C
      When I move entity A by dt 1 (destination index 2)
      Then slots become: Z, B, A, C
      And diffs for A (1->2) and B (2->1), none staggered
    */
    [Fact]
    public void MoveEntity_ForwardCascade_UsesVacancyCreatedByMove()
    {
        var Z = E("Z");
        var A = E("A");
        var B = E("B");
        var C = E("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(0, 3,
                (Z, false),
                (A, false),
                (B, false),
                (C, false))
        };

        var diffs = Initiative.MoveEntity(ctx, A, 1);
        ApplyDiffs(ctx, diffs);

        AssertSlots(ctx.InitiativeTrack,
            (Z, false),
            (B, false),
            (A, false),
            (C, false));

        var aDiff = diffs.Single(d => ReferenceEquals(d.Subject, A));
        var bDiff = diffs.Single(d => ReferenceEquals(d.Subject, B));
        Assert.Equal(1, aDiff.OriginalIndex);
        Assert.Equal(2, aDiff.UpdatedIndex);
        Assert.Equal(2, bDiff.OriginalIndex);
        Assert.Equal(1, bDiff.UpdatedIndex);
        Assert.False(aDiff.BecameStaggered);
        Assert.False(bDiff.BecameStaggered);
    }

    /*
    Scenario: Moving entity beyond RoundEndIndex places it in a stagger slot and grows the track
      Given RoundEndIndex is 3, CurrentIndex is 0, TrackLength is 4
      And slots: A, (empty), X, (empty)
      When I move entity X by dt 4 (destination index 6)
      Then TrackLength becomes 7
      And slots: A, (empty), (empty), (empty), (empty), (empty), X(staggered)
      And diff: X originalIndex=2 updatedIndex=6 becameStaggered=true
    */
    [Fact]
    public void MoveEntity_BeyondRoundEndIndex_BecomesStaggered_TrackGrows()
    {
        var A = E("A");
        var X = E("X");

        var ctx = new TestContext
        {
            InitiativeTrack = State(0, 3,
                (A,    false),
                (null, false),
                (X,    false),
                (null, false))
        };

        var diffs = Initiative.MoveEntity(ctx, X, 4);
        ApplyDiffs(ctx, diffs);

        Assert.Equal(7, ctx.InitiativeTrack.TrackLength);

        AssertSlots(ctx.InitiativeTrack,
            (A,    false),
            (null, false),
            (null, false),
            (null, false),
            (null, false),
            (null, false),
            (X,    true));

        Assert.Single(diffs);
        Assert.Equal(2, diffs[0].OriginalIndex);
        Assert.Equal(6, diffs[0].UpdatedIndex);
        Assert.True(diffs[0].BecameStaggered);
    }

    /*
    Scenario: Moving the current occupant with forward cascade available
      Given RoundEndIndex is 6, CurrentIndex is 2, TrackLength is 7
      And slots: A, B, C(current), (empty), D, E, (empty)
      When I move entity C by dt 2 (destination index 4)
      Then CurrentIndex remains 2, slots: A, B, (empty), D, C, E, (empty)
      And diffs for C and D
    */
    [Fact]
    public void MoveEntity_CurrentOccupant_ForwardCascade()
    {
        var A = E("A");
        var B = E("B");
        var C = E("C");
        var D = E("D");
        var E_ = E("E");

        var ctx = new TestContext
        {
            InitiativeTrack = State(2, 6,
                (A,    false),
                (B,    false),
                (C,    false),
                (null, false),
                (D,    false),
                (E_,   false),
                (null, false))
        };

        var diffs = Initiative.MoveEntity(ctx, C, 2);
        ApplyDiffs(ctx, diffs);

        Assert.Equal(2, ctx.InitiativeTrack.CurrentIndex);

        AssertSlots(ctx.InitiativeTrack,
            (A,    false),
            (B,    false),
            (null, false),
            (D,    false),
            (C,    false),
            (E_,   false),
            (null, false));

        var subjects = diffs.Select(d => d.Subject).ToArray();
        Assert.Contains(C, subjects);
        Assert.Contains(D, subjects);
    }

    /*
    Scenario: Moving the current occupant triggers backward cascade and stagger
      Given RoundEndIndex is 3, CurrentIndex is 1, TrackLength is 4
      And slots: A, B(current), C, D
      When I move entity B by dt 1 (destination index 2)
      Then TrackLength becomes 5, CurrentIndex remains 1
      And slots: A, (empty), B, C, D(staggered)
      And diffs for B, C, D — only D becameStaggered
    */
    [Fact]
    public void MoveEntity_CurrentOccupant_BackwardCascade_StaggersTail()
    {
        var A = E("A");
        var B = E("B");
        var C = E("C");
        var D = E("D");

        var ctx = new TestContext
        {
            InitiativeTrack = State(1, 3,
                (A, false),
                (B, false),
                (C, false),
                (D, false))
        };

        var diffs = Initiative.MoveEntity(ctx, B, 1);
        ApplyDiffs(ctx, diffs);

        Assert.Equal(5, ctx.InitiativeTrack.TrackLength);
        Assert.Equal(1, ctx.InitiativeTrack.CurrentIndex);

        AssertSlots(ctx.InitiativeTrack,
            (A,    false),
            (null, false),
            (B,    false),
            (C,    false),
            (D,    true));

        var dDiff = diffs.Single(d => ReferenceEquals(d.Subject, D));
        Assert.True(dDiff.BecameStaggered);
        Assert.All(diffs.Where(d => !ReferenceEquals(d.Subject, D)), d => Assert.False(d.BecameStaggered));
    }

    /*
    Scenario: MoveEntity is a no-op when the entity is not present
      Given CurrentIndex is 0, RoundEndIndex is 0, slots: A
      When I move entity D by dt 3
      Then slots unchanged: A
      And GetCurrent returns A
    */
    [Fact]
    public void MoveEntity_EntityNotPresent_NoOp()
    {
        var A = E("A");
        var D = E("D");

        var ctx = new TestContext
        {
            InitiativeTrack = State(0, 0,
                (A, false))
        };

        var diffs = Initiative.MoveEntity(ctx, D, 3);
        ApplyDiffs(ctx, diffs);

        AssertSlots(ctx.InitiativeTrack, (A, false));
        Assert.Same(A, Initiative.GetCurrent(ctx.InitiativeTrack));
        Assert.Empty(diffs);
    }

    /*
    Scenario: MoveEntity with dt=0 is a no-op
      Given CurrentIndex is 0, RoundEndIndex is 1, slots: A, B
      When I move entity A by dt 0
      Then slots unchanged, no diffs
    */
    [Fact]
    public void MoveEntity_DtZero_NoOp()
    {
        var A = E("A");
        var B = E("B");

        var ctx = new TestContext
        {
            InitiativeTrack = State(0, 1,
                (A, false),
                (B, false))
        };

        var diffs = Initiative.MoveEntity(ctx, A, 0);
        ApplyDiffs(ctx, diffs);

        AssertSlots(ctx.InitiativeTrack,
            (A, false),
            (B, false));

        Assert.Empty(diffs);
    }
}
