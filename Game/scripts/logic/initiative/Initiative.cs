using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.context;
using Lawfare.scripts.logic.effects.initiative;
using Lawfare.scripts.logic.initiative.state;
using Lawfare.scripts.subject;

namespace Lawfare.scripts.logic.initiative;

public static class Initiative
{
    /// <summary>
    /// Seed initial state. Normalizes so the minimum delay becomes 0.
    /// Preserves input order within the same delay (tiebreak rule).
    /// </summary>
    public static InitiativeTrackState Seed(IEnumerable<(IHasInitiative entity, int delay)> entries)
    {
        if (entries == null) throw new ArgumentNullException(nameof(entries));
        var list = entries.ToList();
        if (list.Count == 0) return new InitiativeTrackState();

        foreach (var (e, _) in list)
            if (e == null) throw new ArgumentNullException(nameof(entries), "Seed contains null entity.");

        int min = list.Min(x => x.delay);

        var groups = list
            .Select(x => (entity: x.entity, shifted: x.delay - min))
            .GroupBy(x => x.shifted)
            .OrderBy(g => g.Key)
            .Select(g => new InitiativeSlotState
            {
                Delay = g.Key,
                Row = g.Select(x => x.entity).ToArray()
            })
            .ToArray();

        return new InitiativeTrackState { Slots = groups };
    }

    /// <summary>
    /// Current exists iff there is a slot with Delay==0 and it has at least one entity.
    /// Tiebreak is row order: first in Row goes first.
    /// </summary>
    public static IHasInitiative? GetCurrent(InitiativeTrackState state)
    {
        if (state?.Slots == null) return null;
        var slot0 = state.Slots.FirstOrDefault(s => s.Delay == 0);
        if (slot0?.Row == null || slot0.Row.Length == 0) return null;
        return slot0.Row[0];
    }

    /// <summary>
    /// UI reader: returns slots as (delay, row) in ascending delay.
    /// No mutation; safe for ControlNodes.
    /// </summary>
    public static IReadOnlyList<InitiativeSlotState> ReadSlots(InitiativeTrackState state)
    {
        if (state?.Slots == null) return Array.Empty<InitiativeSlotState>();

        return state.Slots
            .OrderBy(s => s.Delay)
            .Select(s => new InitiativeSlotState {
                Delay = s.Delay,
                Row = s.Row
            })
            .ToArray();
    }


    /// <summary>
    /// Advances time by exactly 1 tick by staging per-entity delay diffs:
    /// - Delays > 0 decrement by 1.
    /// - Delay 0 remains 0.
    /// - Ordering inside each slot is preserved (enforced by SetDelay collision rule + no-op optimization).
    ///
    /// Returns one diff per entity (including no-op diffs for delay 0).
    /// </summary>
    public static InitiativeDiff[] Tick(IContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        var state = context.InitiativeTrack;
        if (state?.Slots == null || state.Slots.Length == 0) return [];

        var diffs = new List<InitiativeDiff>();

        foreach (var slot in state.Slots)
        {
            var oldDelay = slot.Delay;
            var newDelay = oldDelay <= 0 ? 0 : oldDelay - 1;

            foreach (var entity in slot.Row)
            {
                // Your design: Subject extends IHasInitiative.
                // If some entries are not ISubject, you can either skip or throw.
                if (entity is not ISubject subject)
                    continue;

                diffs.Add(new InitiativeDiff(context, subject, oldDelay, newDelay));
            }
        }

        return diffs.ToArray();
    }

    /// <summary>
    /// Stages a single per-entity delay diff for moving an entity by +dt.
    /// Apply() will update the track using SetDelay(...), so this composes with other diffs.
    ///
    /// Rules:
    /// - If entity is Current (slot 0, index 0): newDelay = dt
    /// - Else: newDelay = oldDelay + dt
    ///
    /// Returns null if dt==0 or entity isn't present.
    /// </summary>
    public static InitiativeDiff? MoveEntity(IContext context, IHasInitiative entity, int dt)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        if (dt == 0) return null;

        var state = context.InitiativeTrack;
        if (state?.Slots == null || state.Slots.Length == 0) return null;

        // Find entity delay & whether it is current (slot0[0]).
        int? foundDelay = null;
        bool isCurrent = false;

        foreach (var slot in state.Slots)
        {
            if (slot.Row == null || slot.Row.Length == 0) continue;

            // current check
            if (slot.Delay == 0 && ReferenceEquals(slot.Row[0], entity))
                isCurrent = true;

            for (int i = 0; i < slot.Row.Length; i++)
            {
                if (ReferenceEquals(slot.Row[i], entity))
                {
                    foundDelay = slot.Delay;
                    break;
                }
            }

            if (foundDelay != null) break;
        }

        if (foundDelay == null)
            return null; // entity not present => no-op (matches your previous behavior)

        var oldDelay = foundDelay.Value;
        var newDelay = isCurrent ? dt : checked(oldDelay + dt);

        // If no actual change, avoid producing diffs (prevents reordering via SetDelay).
        if (newDelay == oldDelay)
            return null;

        if (entity is not ISubject subject)
            throw new InvalidOperationException("MoveEntity requires entity to implement ISubject (which extends IHasInitiative).");

        return new InitiativeDiff(context, subject, oldDelay, newDelay);
    }
    
    public static int? GetDelay(InitiativeTrackState state, IHasInitiative entity)
    {
        if (state?.Slots == null) return null;

        foreach (var slot in state.Slots)
        {
            var idx = Array.IndexOf(slot.Row, entity);
            if (idx >= 0) return slot.Delay;
        }
        return null;
    }

    /// <summary>
    /// Sets entity's delay to newDelay (>=0). Preserves relative row ordering:
    /// - removing entity from old slot preserves old slot row order
    /// - inserting into destination slot appends to the back (collision rule)
    /// No-op if entity not present or delay unchanged.
    /// </summary>
    public static InitiativeTrackState SetDelay(InitiativeTrackState state, IHasInitiative entity, int newDelay)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (newDelay < 0) throw new ArgumentOutOfRangeException(nameof(newDelay));

        // Build delay->row map (preserve order)
        var map = state.Slots
            .OrderBy(s => s.Delay)
            .ToDictionary(s => s.Delay, s => s.Row.ToList());

        int? foundDelay = null;
        int foundIndex = -1;

        foreach (var kv in map.OrderBy(k => k.Key))
        {
            var idx = kv.Value.IndexOf(entity);
            if (idx >= 0)
            {
                foundDelay = kv.Key;
                foundIndex = idx;
                break;
            }
        }

        if (foundDelay == null) return state;

        if (foundDelay.Value == newDelay) return state; // important: avoid reorder

        // remove
        map[foundDelay.Value].RemoveAt(foundIndex);
        if (map[foundDelay.Value].Count == 0)
            map.Remove(foundDelay.Value);

        // insert (append)
        if (!map.TryGetValue(newDelay, out var dest))
        {
            dest = new List<IHasInitiative>();
            map[newDelay] = dest;
        }
        dest.Add(entity);

        var slots = map
            .OrderBy(kv => kv.Key)
            .Select(kv => new InitiativeSlotState { Delay = kv.Key, Row = kv.Value.ToArray() })
            .ToArray();

        return new InitiativeTrackState { Slots = slots };
    }
}
