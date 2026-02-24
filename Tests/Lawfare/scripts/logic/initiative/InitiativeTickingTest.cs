using System;
using System.Linq;
using Xunit;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.logic.effects.initiative;
using Lawfare.scripts.board.factions;
using Lawfare.scripts.characters;
using Lawfare.scripts.characters.lawyers;
using Lawfare.scripts.subject;

namespace Tests.Lawfare.scripts.logic.initiative;

public sealed class InitiativeTickingTest
{
    private sealed class TestEntity : SubjectStub, IHasInitiative
    {
        public TestEntity(string name) : base(name) { }
        public override string ToString() => Label;
        public bool HasActed { get; set; }
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

    private static void Tick(TestContext ctx)
    {
        var diffs = Initiative.Tick(ctx);
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
    Scenario: Tick on an empty track yields an empty track
      Given no slots
      When I tick once
      Then no slots and GetCurrent returns null
    */
    [Fact]
    public void Tick_EmptyTrack_RemainsEmpty()
    {
        var ctx = new TestContext
        {
            InitiativeTrack = new InitiativeTrackState
            {
                CurrentIndex = 0,
                RoundEndIndex = 0,
                Slots = Array.Empty<InitiativeSlotState>()
            }
        };

        var diffs = Initiative.Tick(ctx);
        Assert.Empty(diffs);

        foreach (var d in diffs) d.Apply();

        Assert.Empty(Initiative.ReadSlots(ctx.InitiativeTrack));
        Assert.Null(Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: Tick advances CurrentIndex by one without changing slots
      Given RoundEndIndex=4, CurrentIndex=2
      And slots: A, (empty), B, C, (empty)
      When I tick once
      Then CurrentIndex is 3
      And slots unchanged
    */
    [Fact]
    public void Tick_AdvancesCurrentIndex_SlotUnchanged()
    {
        var A = E("A");
        var B = E("B");
        var C = E("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(2, 4,
                (A,    false),
                (null, false),
                (B,    false),
                (C,    false),
                (null, false))
        };

        var diffs = Initiative.Tick(ctx);

        Assert.Single(diffs);
        Assert.IsType<TickDiff>(diffs[0]);

        foreach (var d in diffs) d.Apply();

        Assert.Equal(3, ctx.InitiativeTrack.CurrentIndex);

        AssertSlots(ctx.InitiativeTrack,
            (A,    false),
            (null, false),
            (B,    false),
            (C,    false),
            (null, false));
    }

    /*
    Scenario: GetCurrent returns Slots[CurrentIndex].Occupant after tick
      Given RoundEndIndex=4, CurrentIndex=2
      And slots: A, (empty), B, C, (empty)
      When I tick once (CurrentIndex becomes 3)
      Then GetCurrent returns C
    */
    [Fact]
    public void Tick_GetCurrent_ReturnsOccupantAtNewCurrentIndex()
    {
        var A = E("A");
        var B = E("B");
        var C = E("C");

        var ctx = new TestContext
        {
            InitiativeTrack = State(2, 4,
                (A,    false),
                (null, false),
                (B,    false),
                (C,    false),
                (null, false))
        };

        var diffs = Initiative.Tick(ctx);
        Assert.Single(diffs);
        Assert.IsType<TickDiff>(diffs[0]);

        foreach (var d in diffs) d.Apply();

        Assert.Same(C, Initiative.GetCurrent(ctx.InitiativeTrack));
    }

    /*
    Scenario: Ticking when CurrentIndex == RoundEndIndex rebuilds next round
      Given RoundEndIndex=4, CurrentIndex=4, TrackLength=7
      And slots: A(0), (empty)(1), B(2), C(3), (empty)(4), X(5,staggered), Y(6,staggered)
      When I tick once
      Then new round begins: CurrentIndex=0, TrackLength=5
      And slots: X, Y, A, B, C — all not staggered
    */
    [Fact]
    public void Tick_AtRoundEnd_RebuildsNextRound_StaggeredFirst()
    {
        var A = E("A");
        var B = E("B");
        var C = E("C");
        var X = E("X");
        var Y = E("Y");

        A.HasActed = true;
        B.HasActed = true;
        C.HasActed = true;
        X.HasActed = true;
        Y.HasActed = true;

        var ctx = new TestContext
        {
            InitiativeTrack = State(4, 4,
                (A,    false),
                (null, false),
                (B,    false),
                (C,    false),
                (null, false),
                (X,    true),
                (Y,    true))
        };

        var diffs = Initiative.Tick(ctx);

        Assert.IsType<RoundRebuildDiff>(diffs[0]);

        foreach (var d in diffs) d.Apply();

        Assert.Equal(0, ctx.InitiativeTrack.CurrentIndex);
        Assert.Equal(4, ctx.InitiativeTrack.RoundEndIndex);
        Assert.Equal(5, ctx.InitiativeTrack.TrackLength);

        AssertSlots(ctx.InitiativeTrack,
            (X, false),
            (Y, false),
            (A, false),
            (B, false),
            (C, false));

        Assert.False(A.HasActed);
        Assert.False(B.HasActed);
        Assert.False(C.HasActed);
        Assert.False(X.HasActed);
        Assert.False(Y.HasActed);
    }

    /*
    Scenario: Round rebuild skips empty slots in active range
      Given RoundEndIndex=3, CurrentIndex=3, TrackLength=5
      And slots: A(0), (empty)(1), B(2), (empty)(3), Z(4,staggered)
      When I tick once
      Then new round: CurrentIndex=0, RoundEndIndex=3, TrackLength=4
      And slots: Z, A, B, (empty) — Z not staggered
    */
    [Fact]
    public void Tick_AtRoundEnd_SkipsEmptySlotsInRebuild()
    {
        var A = E("A");
        var B = E("B");
        var Z = E("Z");

        A.HasActed = true;
        B.HasActed = true;
        Z.HasActed = true;

        var ctx = new TestContext
        {
            InitiativeTrack = State(3, 3,
                (A,    false),
                (null, false),
                (B,    false),
                (null, false),
                (Z,    true))
        };

        var diffs = Initiative.Tick(ctx);

        Assert.IsType<RoundRebuildDiff>(diffs[0]);

        foreach (var d in diffs) d.Apply();

        Assert.Equal(0, ctx.InitiativeTrack.CurrentIndex);
        Assert.Equal(3, ctx.InitiativeTrack.RoundEndIndex);
        Assert.Equal(4, ctx.InitiativeTrack.TrackLength);

        AssertSlots(ctx.InitiativeTrack,
            (Z,    false),
            (A,    false),
            (B,    false),
            (null, false));

        Assert.False(A.HasActed);
        Assert.False(B.HasActed);
        Assert.False(Z.HasActed);
    }
}
