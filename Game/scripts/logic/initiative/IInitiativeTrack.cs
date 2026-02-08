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
    /// Whole-track move anchored to Current. dt can be positive or negative.
    /// Emits Tick per step (|dt| times).
    /// </summary>
    void Move(int dt);

    /// <summary>
    /// Whole-track move anchored to Current. dt can be positive or negative.
    /// MUST NOT emit Tick, but MUST emit other change events.
    /// Revertible via ClearStaging().
    /// </summary>
    void Stage(int dt);

    /// <summary>
    /// Reverts to the snapshot captured at the first Stage() since the last ClearStaging().
    /// </summary>
    void ClearStaging();

    /// <summary>
    /// Sets an entity delay.
    ///
    /// - If entity != Current:
    ///     newDelay < 0 => entity becomes NEXT in slot 0 (index 1), Current unchanged.
    ///     newDelay >=0 => entity is appended to that slot (collision rule).
    ///
    /// - If entity == Current:
    ///     Current is moved to newDelay (can be negative), then the track is REANCHORED:
    ///     minimum delay becomes 0 and Current becomes first in the new 0-slot.
    /// </summary>
    bool SetDelay(IHasInitiative entity, int newDelay);

    /// <summary>
    /// Convenience for "commit turn": move Current back by +cost, then reanchor.
    /// (Equivalent to SetDelay(Current, cost) when Current is anchored at 0.)
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
    /// Fired once per single tick during Move(). NOT fired during Stage().
    /// direction: +1 for forward tick, -1 for backward tick
    /// </summary>
    event Action<int>? Tick;
}