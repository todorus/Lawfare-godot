using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Lawfare.scripts.logic.initiative.state;

namespace Lawfare.scripts.logic.initiative;

public sealed class InitiativeTrack : IInitiativeTrack
{
    // Internal representation:
    // - SortedDictionary delay -> row (queue)
    // - _current is a reference to the "current state" entity
    //
    // Invariant (most of the time):
    // - _current is the first element of slot 0.
    //
    // Exception:
    // - Immediately after moving Current (SetDelay(Current, ...)/CommitTurn), we allow negative delays
    //   temporarily, then we REANCHOR by shifting so min delay becomes 0, and set _current to first of slot 0.

    private readonly SortedDictionary<int, List<IHasInitiative>> _slots = new();
    private readonly Dictionary<IHasInitiative, int> _index = new();
    private IHasInitiative? _current;

    private Snapshot? _stagingBase;

    private IReadOnlyList<InitiativeSlot> _cachedSlots = Array.Empty<InitiativeSlot>();
    private IHasInitiative? _cachedCurrent;
    private IHasInitiative? _cachedNext;
    private int _cachedDelayToNext = int.MaxValue;

    public event Action? OnChange;
    public event Action<IReadOnlyList<InitiativeSlot>>? SlotsChanged;
    public event Action<IHasInitiative?>? CurrentChanged;
    public event Action<IHasInitiative?>? NextChanged;
    public event Action<int>? DelayToNextChanged;
    public event Action<int>? Tick;

    public IReadOnlyList<InitiativeSlot> Slots => _cachedSlots;
    public IHasInitiative? Current => _cachedCurrent;
    public IHasInitiative? Next => _cachedNext;
    public int DelayToNext => _cachedDelayToNext;

    public bool IsStaging => _stagingBase != null;

    public InitiativeTrack()
    {
        RecomputeAndEmitIfChanged(forceEmit: true);
    }

    // -----------------------------
    // Public API
    // -----------------------------

    public void Add(IHasInitiative entity, int delay = 0)
    {
        if (_index.ContainsKey(entity))
            return;

        if (_current is null)
        {
            // First entity becomes current at slot 0 front.
            _current = entity;
            EnsureSlotExists(0);
            _slots[0].Insert(0, entity);
            _index[entity] = 0;

            EnsureAnchoredInvariant();
            RecomputeAndEmitIfChanged();
            return;
        }

        // Non-current add cannot overtake Current:
        if (delay < 0)
        {
            InsertAsNextInZero(entity);
        }
        else
        {
            GetOrCreateRow(delay).Add(entity);
            _index[entity] = delay;
        }

        EnsureAnchoredInvariant();
        RecomputeAndEmitIfChanged();
    }

    public bool Remove(IHasInitiative entity)
    {
        if (!_index.TryGetValue(entity, out var d))
            return false;

        _slots[d].Remove(entity);
        if (_slots[d].Count == 0) _slots.Remove(d);
        _index.Remove(entity);

        if (ReferenceEquals(_current, entity))
        {
            _current = null;

            // Choose new current deterministically:
            // earliest slot, first entity in that slot.
            if (_slots.Count > 0)
            {
                var (minDelay, row) = _slots.First();
                if (row.Count > 0)
                {
                    _current = row[0];
                }

                // Reanchor so earliest slot becomes 0 and current becomes first of slot 0.
                ReanchorToMinimumDelay();
            }
        }

        EnsureAnchoredInvariant();
        RecomputeAndEmitIfChanged();
        return true;
    }

    public void Move(int dt)
    {
        if (dt == 0) return;
        if (_current is null) return;

        var dir = dt > 0 ? +1 : -1;
        var steps = Math.Abs(dt);

        for (int i = 0; i < steps; i++)
        {
            ApplyAnchoredSingleTick(dir);
            EnsureAnchoredInvariant();
            RecomputeAndEmitIfChanged();
            Tick?.Invoke(dir);
        }
    }

    public void Stage(int dt)
    {
        if (dt == 0) return;
        if (_current is null) return;

        _stagingBase ??= Snapshot.Capture(_slots, _index, _current);

        var dir = dt > 0 ? +1 : -1;
        var steps = Math.Abs(dt);

        // Apply ticks, but DO NOT emit Tick events.
        for (int i = 0; i < steps; i++)
            ApplyAnchoredSingleTick(dir);

        EnsureAnchoredInvariant();

        // Emit other change events once for the staged end state.
        RecomputeAndEmitIfChanged();
    }

    public void ClearStaging()
    {
        if (_stagingBase is null)
            return;

        _slots.Clear();
        _index.Clear();
        _current = null;

        _stagingBase.RestoreInto(_slots, _index, out _current);
        _stagingBase = null;

        EnsureAnchoredInvariant();
        RecomputeAndEmitIfChanged();
    }

    public bool SetDelay(IHasInitiative entity, int newDelay)
    {
        if (!_index.TryGetValue(entity, out var oldDelay))
            return false;

        if (ReferenceEquals(entity, _current))
        {
            // Moving Current triggers REANCHOR to minimum delay (can be negative).
            MoveCurrentAndReanchor(newDelay);
            RecomputeAndEmitIfChanged();
            return true;
        }

        // Non-current: cannot overtake Current.
        // Remove first:
        _slots[oldDelay].Remove(entity);
        if (_slots[oldDelay].Count == 0) _slots.Remove(oldDelay);
        _index.Remove(entity);

        if (newDelay < 0)
        {
            InsertAsNextInZero(entity);
        }
        else
        {
            GetOrCreateRow(newDelay).Add(entity);
            _index[entity] = newDelay;
        }

        EnsureAnchoredInvariant();
        RecomputeAndEmitIfChanged();
        return true;
    }

    public void CommitTurn(int cost)
    {
        if (cost < 0) throw new ArgumentOutOfRangeException(nameof(cost));
        if (_current is null) return;

        // Current is anchored at delay 0 in the anchored view,
        // so committing means placing it at +cost, then reanchoring to earliest.
        MoveCurrentAndReanchor(cost);
        RecomputeAndEmitIfChanged();
    }

    // -----------------------------
    // State + DTO helpers
    // -----------------------------

    public InitiativeTrackState ToState()
    {
        return new InitiativeTrackState
        {
            IsStaging = IsStaging,
            Current = _current,
            Slots = _slots.Select(kv => new InitiativeSlotState
            {
                Delay = kv.Key,
                Row = new List<IHasInitiative>(kv.Value)
            }).ToList()
        };
    }

    // ============================================================
    // Internal mechanics
    // ============================================================

    private void EnsureSlotExists(int delay)
    {
        if (!_slots.ContainsKey(delay))
            _slots[delay] = new List<IHasInitiative>();
    }

    private List<IHasInitiative> GetOrCreateRow(int delay)
    {
        if (!_slots.TryGetValue(delay, out var row))
        {
            row = new List<IHasInitiative>();
            _slots.Add(delay, row);
        }
        return row;
    }

    private void InsertAsNextInZero(IHasInitiative entity)
    {
        EnsureSlotExists(0);
        EnsureAnchoredInvariant();

        var zero = _slots[0];

        // Insert at index 1 (immediate next), clamped to end if only current exists.
        var idx = Math.Min(1, zero.Count);
        zero.Insert(idx, entity);
        _index[entity] = 0;
    }

    /// <summary>
    /// Whole-track single tick while ANCHORED to Current:
    /// - Current stays at (0,0)
    /// - No entity overtakes Current
    /// - forward (+1): delays decrease; anything that would go <0 collapses to 0 behind Current
    /// - backward (-1): delays increase; entities behind Current in slot 0 move out to slot 1
    /// </summary>
    private void ApplyAnchoredSingleTick(int dir)
    {
        if (_current is null) return;

        EnsureSlotExists(0);
        EnsureAnchoredInvariant();

        var newSlots = new SortedDictionary<int, List<IHasInitiative>>();
        var newIndex = new Dictionary<IHasInitiative, int>(_index.Count);

        // Always start slot 0 with current
        newSlots[0] = new List<IHasInitiative> { _current };
        newIndex[_current] = 0;

        foreach (var (delay, row) in _slots)
        {
            foreach (var e in row)
            {
                if (ReferenceEquals(e, _current))
                    continue;

                if (dir > 0)
                {
                    // forward: delay - 1 (but cannot go below 0 relative to current)
                    var nd = delay - 1;
                    if (nd <= 0)
                    {
                        // collapse into 0 behind current (append; preserves traversal + row order)
                        newSlots[0].Add(e);
                        newIndex[e] = 0;
                    }
                    else
                    {
                        GetOrCreateRow(newSlots, nd).Add(e);
                        newIndex[e] = nd;
                    }
                }
                else
                {
                    // backward: delay + 1 (everyone except current moves away)
                    var nd = delay + 1;
                    GetOrCreateRow(newSlots, nd).Add(e);
                    newIndex[e] = nd;
                }
            }
        }

        _slots.Clear();
        foreach (var kv in newSlots) _slots.Add(kv.Key, kv.Value);

        _index.Clear();
        foreach (var kv in newIndex) _index.Add(kv.Key, kv.Value);
    }

    private static List<IHasInitiative> GetOrCreateRow(
        SortedDictionary<int, List<IHasInitiative>> slots, int delay)
    {
        if (!slots.TryGetValue(delay, out var row))
        {
            row = new List<IHasInitiative>();
            slots.Add(delay, row);
        }
        return row;
    }

    /// <summary>
    /// Moves Current to newDelay (can be negative), then REANCHORS:
    /// - find minimum delay among all entities
    /// - shift so min becomes 0
    /// - set Current to first in slot 0
    /// </summary>
    private void MoveCurrentAndReanchor(int newDelay)
    {
        if (_current is null) return;

        // Remove current from wherever it is (it should be at 0, index 0 in anchored view, but be robust).
        if (_index.TryGetValue(_current, out var oldDelay))
        {
            _slots[oldDelay].Remove(_current);
            if (_slots[oldDelay].Count == 0) _slots.Remove(oldDelay);
            _index.Remove(_current);
        }

        // Place current at the requested delay (raw, can be negative).
        GetOrCreateRow(newDelay).Insert(0, _current); // insert front for determinism
        _index[_current] = newDelay;

        // Reanchor to the earliest time (min delay becomes 0).
        ReanchorToMinimumDelay();

        // After reanchor, Current must be first of slot 0.
        EnsureAnchoredInvariant();
    }

    private void ReanchorToMinimumDelay()
    {
        if (_slots.Count == 0)
        {
            _current = null;
            return;
        }

        // Minimum raw delay
        var minDelay = _slots.First().Key;

        if (minDelay != 0)
        {
            var shiftedSlots = new SortedDictionary<int, List<IHasInitiative>>();
            var shiftedIndex = new Dictionary<IHasInitiative, int>(_index.Count);

            foreach (var (d, row) in _slots)
            {
                var nd = d - minDelay;
                shiftedSlots[nd] = row;
                foreach (var e in row)
                    shiftedIndex[e] = nd;
            }

            _slots.Clear();
            foreach (var kv in shiftedSlots) _slots.Add(kv.Key, kv.Value);

            _index.Clear();
            foreach (var kv in shiftedIndex) _index.Add(kv.Key, kv.Value);
        }

        // Now earliest slot is 0. New current is first entity in slot 0.
        EnsureSlotExists(0);
        var zero = _slots[0];
        _current = zero.Count > 0 ? zero[0] : null;

        // If slot 0 is empty (shouldn't happen unless track is corrupted), pick next earliest.
        if (_current is null && _slots.Count > 0)
        {
            var firstNonEmpty = _slots.FirstOrDefault(kv => kv.Value.Count > 0);
            if (firstNonEmpty.Value != null && firstNonEmpty.Value.Count > 0)
            {
                _current = firstNonEmpty.Value[0];
                // reanchor again so that slot becomes 0
                if (firstNonEmpty.Key != 0)
                {
                    // move that delay to 0 by shifting keys
                    var min2 = firstNonEmpty.Key;
                    var tmp = new SortedDictionary<int, List<IHasInitiative>>();
                    var tmpIndex = new Dictionary<IHasInitiative, int>(_index.Count);

                    foreach (var (d, row) in _slots)
                    {
                        var nd = d - min2;
                        tmp[nd] = row;
                        foreach (var e in row)
                            tmpIndex[e] = nd;
                    }

                    _slots.Clear();
                    foreach (var kv in tmp) _slots.Add(kv.Key, kv.Value);

                    _index.Clear();
                    foreach (var kv in tmpIndex) _index.Add(kv.Key, kv.Value);
                }
            }
        }
    }

    /// <summary>
    /// Ensures:
    /// - if _current exists, it is the first element of slot 0
    /// - _index reflects that (_current -> 0)
    /// - removes duplicates of _current from other slots if present
    /// </summary>
    private void EnsureAnchoredInvariant()
    {
        if (_current is null) return;

        // Remove _current from any slot it might appear in (defensive).
        foreach (var (d, row) in _slots.ToList())
        {
            for (int i = row.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(row[i], _current))
                    row.RemoveAt(i);
            }
            if (row.Count == 0) _slots.Remove(d);
        }

        EnsureSlotExists(0);
        _slots[0].Insert(0, _current);

        _index[_current] = 0;

        // Rebuild index for others in slot 0 if needed (cheap, consistent).
        // (Optional, but keeps index coherent if external code mutated lists.)
        foreach (var (d, row) in _slots)
            foreach (var e in row)
                _index[e] = d;
    }

    // ============================================================
    // Derived values + change events
    // ============================================================

    private void RecomputeAndEmitIfChanged(bool forceEmit = false)
    {
        var snapshot = BuildSlotsSnapshot();
        var newCurrent = _current;
        var newNext = ComputeNext(snapshot, newCurrent);
        var newDelayToNext = ComputeDelayToNext(snapshot, newCurrent);

        var any = forceEmit;

        if (forceEmit || !SlotsEqual(_cachedSlots, snapshot))
        {
            _cachedSlots = snapshot;
            SlotsChanged?.Invoke(_cachedSlots);
            any = true;
        }

        if (forceEmit || !ReferenceEquals(_cachedCurrent, newCurrent))
        {
            _cachedCurrent = newCurrent;
            CurrentChanged?.Invoke(_cachedCurrent);
            any = true;
        }

        if (forceEmit || !ReferenceEquals(_cachedNext, newNext))
        {
            _cachedNext = newNext;
            NextChanged?.Invoke(_cachedNext);
            any = true;
        }

        if (forceEmit || _cachedDelayToNext != newDelayToNext)
        {
            _cachedDelayToNext = newDelayToNext;
            DelayToNextChanged?.Invoke(_cachedDelayToNext);
            any = true;
        }

        if (any)
            OnChange?.Invoke();
    }

    private IReadOnlyList<InitiativeSlot> BuildSlotsSnapshot()
    {
        if (_slots.Count == 0)
            return Array.Empty<InitiativeSlot>();

        return _slots
            .Where(kv => kv.Value.Count > 0)
            .Select(kv => new InitiativeSlot(kv.Key, kv.Value.ToList()))
            .ToList();
    }

    private static IHasInitiative? ComputeNext(IReadOnlyList<InitiativeSlot> slots, IHasInitiative? current)
    {
        if (slots.Count == 0) return null;
        if (current is null) return slots[0].Row.FirstOrDefault();

        var zero = slots.FirstOrDefault(s => s.Delay == 0);
        if (zero != null)
        {
            // invariant: current at index 0
            if (zero.Row.Count >= 2) return zero.Row[1];
        }

        foreach (var slot in slots)
            if (slot.Delay > 0 && slot.Row.Count > 0)
                return slot.Row[0];

        return null;
    }

    private static int ComputeDelayToNext(IReadOnlyList<InitiativeSlot> slots, IHasInitiative? current)
    {
        if (slots.Count == 0) return int.MaxValue;
        if (current is null) return slots[0].Delay;

        var zero = slots.FirstOrDefault(s => s.Delay == 0);
        if (zero != null && zero.Row.Count >= 2) return 0;

        var nextSlot = slots.FirstOrDefault(s => s.Delay > 0 && s.Row.Count > 0);
        return nextSlot?.Delay ?? int.MaxValue;
    }

    private static bool SlotsEqual(IReadOnlyList<InitiativeSlot> a, IReadOnlyList<InitiativeSlot> b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a.Count != b.Count) return false;

        for (int i = 0; i < a.Count; i++)
        {
            if (a[i].Delay != b[i].Delay) return false;
            if (a[i].Row.Count != b[i].Row.Count) return false;

            for (int j = 0; j < a[i].Row.Count; j++)
                if (!ReferenceEquals(a[i].Row[j], b[i].Row[j]))
                    return false;
        }
        return true;
    }

    // ============================================================
    // Staging snapshot
    // ============================================================

    private sealed class Snapshot
    {
        private readonly List<(int delay, List<IHasInitiative> row)> _slots;
        private readonly Dictionary<IHasInitiative, int> _index;
        private readonly IHasInitiative? _current;

        private Snapshot(
            List<(int delay, List<IHasInitiative> row)> slots,
            Dictionary<IHasInitiative, int> index,
            IHasInitiative? current)
        {
            _slots = slots;
            _index = index;
            _current = current;
        }

        public static Snapshot Capture(
            SortedDictionary<int, List<IHasInitiative>> slots,
            Dictionary<IHasInitiative, int> index,
            IHasInitiative? current)
        {
            var slotsCopy = slots
                .Select(kv => (kv.Key, new List<IHasInitiative>(kv.Value)))
                .ToList();

            var indexCopy = new Dictionary<IHasInitiative, int>(index);
            return new Snapshot(slotsCopy, indexCopy, current);
        }

        public void RestoreInto(
            SortedDictionary<int, List<IHasInitiative>> slots,
            Dictionary<IHasInitiative, int> index,
            out IHasInitiative? current)
        {
            foreach (var (delay, row) in _slots)
                slots.Add(delay, new List<IHasInitiative>(row));

            foreach (var kv in _index)
                index.Add(kv.Key, kv.Value);

            current = _current;
        }
    }
}