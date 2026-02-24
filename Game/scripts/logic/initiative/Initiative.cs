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
    public static InitiativeTrackState Seed(IEnumerable<(IHasInitiative entity, int initiative)> entries)
    {
        var sorted = entries
            .Select((e, i) => (e.entity, e.initiative, i))
            .OrderBy(e => e.initiative)
            .ThenBy(e => e.i)
            .Select(e => e.entity)
            .ToArray();

        if (sorted.Length == 0)
            return new InitiativeTrackState
            {
                CurrentIndex = 0,
                RoundEndIndex = 0,
                Slots = Array.Empty<InitiativeSlotState>()
            };

        var slots = sorted
            .Select(e => new InitiativeSlotState { Occupant = e, IsStaggered = false })
            .Append(new InitiativeSlotState { Occupant = null, IsStaggered = false })
            .ToArray();

        // TrackLength = sorted.Length + 1, RoundEndIndex = sorted.Length (the trailing empty slot)
        return new InitiativeTrackState
        {
            CurrentIndex = 0,
            RoundEndIndex = sorted.Length,
            Slots = slots
        };
    }

    public static IHasInitiative? GetCurrent(InitiativeTrackState state)
    {
        if (state.Slots.Length == 0) return null;
        if (state.CurrentIndex >= state.Slots.Length) return null;
        return state.Slots[state.CurrentIndex].Occupant;
    }

    public static IReadOnlyList<InitiativeSlotState> ReadSlots(InitiativeTrackState state) =>
        state.Slots;

    public static int? GetIndex(InitiativeTrackState state, IHasInitiative entity)
    {
        for (int i = 0; i < state.Slots.Length; i++)
            if (ReferenceEquals(state.Slots[i].Occupant, entity))
                return i;
        return null;
    }

    public static InitiativeDiff[] MoveEntity(IContext context, IHasInitiative entity, int dt)
    {
        var state = context.InitiativeTrack;

        var sourceIndex = GetIndex(state, entity);
        if (sourceIndex is null || dt == 0)
            return Array.Empty<InitiativeDiff>();

        int src = sourceIndex.Value;
        int destination = src + dt;

        if (destination <= state.CurrentIndex)
            destination = state.CurrentIndex + 1;

        var slots = GrowIfNeeded(state.Slots, destination + 1);

        slots[src] = new InitiativeSlotState { Occupant = null, IsStaggered = false };

        var diffs = new List<InitiativeDiff>();

        if (slots[destination].Occupant is null)
        {
            bool becameStaggered = destination > state.RoundEndIndex;
            slots[destination] = new InitiativeSlotState { Occupant = entity, IsStaggered = becameStaggered };
            diffs.Add(new InitiativeDiff(context, entity as ISubject, src, destination, becameStaggered));
        }
        else
        {
            int forwardVacancy = -1;
            for (int v = state.CurrentIndex + 1; v < destination; v++)
            {
                if (slots[v].Occupant is null)
                {
                    forwardVacancy = v;
                    break;
                }
            }

            if (forwardVacancy >= 0)
            {
                for (int i = forwardVacancy; i < destination; i++)
                {
                    var displaced = slots[i + 1];
                    if (displaced.Occupant is not null)
                        diffs.Add(new InitiativeDiff(context, displaced.Occupant as ISubject, i + 1, i, false));
                    slots[i] = displaced;
                }
                bool becameStaggered = destination > state.RoundEndIndex;
                slots[destination] = new InitiativeSlotState { Occupant = entity, IsStaggered = becameStaggered };
                diffs.Add(new InitiativeDiff(context, entity as ISubject, src, destination, becameStaggered));
            }
            else
            {
                // Backward cascade: collect the chain of occupied slots starting at destination
                // before modifying anything, to avoid re-visiting placed entities
                var chain = new System.Collections.Generic.List<(IHasInitiative occupant, int from)>();
                int scan = destination;
                while (scan < slots.Length && slots[scan].Occupant is not null)
                {
                    chain.Add((slots[scan].Occupant!, scan));
                    slots[scan] = new InitiativeSlotState { Occupant = null, IsStaggered = false };
                    scan++;
                }

                // Place moved entity at destination
                bool destStaggered = destination > state.RoundEndIndex;
                slots = GrowIfNeeded(slots, destination + 1);
                slots[destination] = new InitiativeSlotState { Occupant = entity, IsStaggered = destStaggered };
                diffs.Add(new InitiativeDiff(context, entity as ISubject, src, destination, destStaggered));

                // Push chain one step forward
                foreach (var (pushed, from) in chain)
                {
                    int pushTo = from + 1;
                    slots = GrowIfNeeded(slots, pushTo + 1);
                    bool becameStaggered = pushTo > state.RoundEndIndex && from <= state.RoundEndIndex;
                    bool isStaggered = pushTo > state.RoundEndIndex;
                    slots[pushTo] = new InitiativeSlotState { Occupant = pushed, IsStaggered = isStaggered };
                    diffs.Add(new InitiativeDiff(context, pushed as ISubject, from, pushTo, becameStaggered));
                }
            }
        }

        context.InitiativeTrack.Slots = slots;
        return diffs.ToArray();
    }

    public static InitiativeDiff[] Tick(IContext context)
    {
        var state = context.InitiativeTrack;

        if (state.Slots.Length == 0)
            return Array.Empty<InitiativeDiff>();

        if (state.CurrentIndex < state.RoundEndIndex)
        {
            state.CurrentIndex++;
            return Array.Empty<InitiativeDiff>();
        }

        int roundLength = state.RoundEndIndex + 1;

        var staggered = state.Slots
            .Skip(roundLength)
            .Where(s => s.Occupant is not null)
            .Select(s => s.Occupant!);

        var active = state.Slots
            .Take(roundLength)
            .Where(s => s.Occupant is not null)
            .Select(s => s.Occupant!);

        var ordered = staggered.Concat(active).ToArray();

        var newSlots = new InitiativeSlotState[roundLength];
        for (int i = 0; i < roundLength; i++)
        {
            newSlots[i] = i < ordered.Length
                ? new InitiativeSlotState { Occupant = ordered[i], IsStaggered = false }
                : new InitiativeSlotState { Occupant = null, IsStaggered = false };
        }

        state.Slots = newSlots;
        state.CurrentIndex = 0;

        return Array.Empty<InitiativeDiff>();
    }

    public static InitiativeTrackState SetSlot(InitiativeTrackState state, int index, IHasInitiative? occupant, bool isStaggered = false)
    {
        var slots = GrowIfNeeded(state.Slots, index + 1);
        slots[index] = new InitiativeSlotState { Occupant = occupant, IsStaggered = isStaggered };
        state.Slots = slots;
        return state;
    }

    private static InitiativeSlotState[] GrowIfNeeded(InitiativeSlotState[] slots, int requiredLength)
    {
        if (slots.Length >= requiredLength) return slots;

        var grown = new InitiativeSlotState[requiredLength];
        Array.Copy(slots, grown, slots.Length);
        for (int i = slots.Length; i < requiredLength; i++)
            grown[i] = new InitiativeSlotState { Occupant = null, IsStaggered = false };
        return grown;
    }
}
