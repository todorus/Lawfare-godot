using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.logic.initiative.state;

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
    public static IReadOnlyList<(int Delay, IReadOnlyList<IHasInitiative> Row)> ReadSlots(InitiativeTrackState state)
    {
        if (state?.Slots == null) return Array.Empty<(int, IReadOnlyList<IHasInitiative>)>();
        return state.Slots
            .OrderBy(s => s.Delay)
            .Select(s => (s.Delay, (IReadOnlyList<IHasInitiative>)s.Row))
            .ToArray();
    }

    /// <summary>
    /// Advances time by exactly 1 tick:
    /// - Delays > 0 decrement by 1.
    /// - Delay 0 remains 0.
    /// - Ordering inside each slot is preserved.
    /// </summary>
    public static InitiativeTrackState Tick(InitiativeTrackState state)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (state.Slots.Length == 0) return state;

        var map = new Dictionary<int, List<IHasInitiative>>();

        foreach (var slot in state.Slots)
        {
            int newDelay = slot.Delay <= 0 ? 0 : slot.Delay - 1;
            if (!map.TryGetValue(newDelay, out var row))
            {
                row = new List<IHasInitiative>();
                map[newDelay] = row;
            }
            row.AddRange(slot.Row);
        }

        var slots = map
            .OrderBy(kv => kv.Key)
            .Select(kv => new InitiativeSlotState
            {
                Delay = kv.Key,
                Row = kv.Value.ToArray()
            })
            .ToArray();

        return new InitiativeTrackState { Slots = slots };
    }

    /// <summary>
    /// Pure move of a single entity by +dt (dt should be >= 0 for action costs).
    /// Special-case: if the entity is the current (slot0[0]) and dt>0,
    /// it is removed from the front of slot 0 and reinserted at delay dt (appended on collision).
    ///
    /// If the entity isn't current, it is relocated by adding dt to its current delay.
    /// (No negative rules needed for your current loop.)
    /// </summary>
    public static InitiativeTrackState MoveEntity(InitiativeTrackState state, IHasInitiative entity, int dt)
    {
        if (state == null) throw new ArgumentNullException(nameof(state));
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (dt == 0) return state;

        // Build a mutable map delay -> row preserving order.
        var map = state.Slots
            .OrderBy(s => s.Delay)
            .ToDictionary(s => s.Delay, s => s.Row.ToList());

        // Find current slot of entity
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

        if (foundDelay == null)
            return state; // not present -> no-op (you can also throw)

        bool isCurrent = (foundDelay.Value == 0 && foundIndex == 0);

        // Remove from old location
        map[foundDelay.Value].RemoveAt(foundIndex);
        if (map[foundDelay.Value].Count == 0)
            map.Remove(foundDelay.Value);

        int newDelay;

        if (isCurrent)
        {
            // Action cost: current actor goes "back" by +dt
            newDelay = dt;
        }
        else
        {
            newDelay = checked(foundDelay.Value + dt);
        }

        if (!map.TryGetValue(newDelay, out var newRow))
        {
            newRow = new List<IHasInitiative>();
            map[newDelay] = newRow;
        }

        // Collision rule: append to back of that slot's row.
        newRow.Add(entity);

        var slots = map
            .OrderBy(kv => kv.Key)
            .Select(kv => new InitiativeSlotState
            {
                Delay = kv.Key,
                Row = kv.Value.ToArray()
            })
            .ToArray();

        return new InitiativeTrackState { Slots = slots };
    }
}
