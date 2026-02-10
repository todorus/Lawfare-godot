using System;
using System.Collections.Generic;
using Lawfare.scripts.logic.initiative.state;

namespace Lawfare.scripts.logic.initiative;

public interface IInitiativeTrack
{
    // ---- Mutations ----
    void Add(IHasInitiative entity, int delay = 0);
    bool Remove(IHasInitiative entity);

    /// <summary>
    /// Moves the anchor (Current) along the timeline by dt.
    /// Equivalent to "time passes relative to the current anchor".
    /// dt can be positive or negative.
    /// Emits Tick per step (|dt| times).
    /// </summary>
    void MoveAnchor(int dt);

    /// <summary>
    /// Staged version of MoveAnchor(dt).
    /// MUST NOT emit Tick, but MUST emit other change events.
    /// Revertible via ClearStaging().
    /// </summary>
    void StageAnchor(int dt);

    /// <summary>
    /// Moves a single entity relative to its current delay by dt.
    /// dt can be positive or negative.
    /// Rules:
    /// - If entity != Current: cannot overtake Current; if it would become negative, it becomes NEXT in slot 0.
    /// - If entity == Current: treated as moving Current; then reanchor to minimum delay.
    /// </summary>
    bool Move(IHasInitiative entity, int dt);

    /// <summary>
    /// Staged version of Move(entity, dt).
    /// MUST NOT emit Tick, but MUST emit other change events.
    /// Revertible via ClearStaging().
    /// </summary>
    bool Stage(IHasInitiative entity, int dt);

    /// <summary>
    /// Reverts to the snapshot captured at the first Stage*() since the last ClearStaging().
    /// Discards ALL mutations since staging began.
    /// </summary>
    void ClearStaging();

    /// <summary>
    /// Sets an entity absolute delay (relative to anchor semantics).
    /// See previous spec re: non-current negative => next in slot 0,
    /// current move => reanchor to minimum delay (even negative).
    /// </summary>
    bool SetDelay(IHasInitiative entity, int newDelay);

    /// <summary>
    /// Convenience for "commit turn": move Current back by +cost, then reanchor.
    /// </summary>
    void CommitTurn(int cost);

    // ---- Derived getters ----
    IReadOnlyList<InitiativeSlot> Slots { get; }
    IHasInitiative? Current { get; }
    IHasInitiative? Next { get; }
    int DelayToNext { get; }

    bool IsStaging { get; }

    // ---- Events ----
    event Action? OnChange;

    event Action<IReadOnlyList<InitiativeSlot>>? SlotsChanged;
    event Action<IHasInitiative?>? CurrentChanged;
    event Action<IHasInitiative?>? NextChanged;
    event Action<int>? DelayToNextChanged;

    /// <summary>
    /// Fired once per single tick during MoveAnchor(dt). NOT fired during any Stage*().
    /// direction: +1 for forward tick, -1 for backward tick
    /// </summary>
    event Action<int>? Tick;
}
