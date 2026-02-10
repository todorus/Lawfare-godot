// InitiativeTrack.cs
// Concrete implementation intended to satisfy the current IInitiativeTrack contract
// and the regenerated xUnit tests / Gherkin.
//
// Namespaces match your project conventions shown in the prompt. Adjust if needed.

using System;
using System.Collections.Generic;
using System.Linq;
using Lawfare.scripts.logic.initiative.state;

namespace Lawfare.scripts.logic.initiative
{
    public sealed class InitiativeTrack : IInitiativeTrack
    {
        // Internal representation:
        // - All delays are maintained as integers >= 0 after any non-staged or staged operation completes.
        // - Slot 0 always exists when Current != null.
        // - Current is always slot0[0].
        private readonly SortedDictionary<int, List<IHasInitiative>> _slots = new();

        private IHasInitiative? _current;

        // Staging
        private bool _isStaging;
        private Snapshot? _stagingSnapshot;

        // Derived cached
        private IHasInitiative? _next;
        private int _delayToNext = int.MaxValue;
        private IReadOnlyList<InitiativeSlot> _slotsView = Array.Empty<InitiativeSlot>();

        // Events
        public event Action? OnChange;
        public event Action<IReadOnlyList<InitiativeSlot>>? SlotsChanged;
        public event Action<IHasInitiative?>? CurrentChanged;
        public event Action<IHasInitiative?>? NextChanged;
        public event Action<int>? DelayToNextChanged;
        public event Action<int>? Tick;

        public IReadOnlyList<InitiativeSlot> Slots => _slotsView;
        public IHasInitiative? Current => _current;
        public IHasInitiative? Next => _next;
        public int DelayToNext => _delayToNext;
        public bool IsStaging => _isStaging;
        
        public void Seed(IEnumerable<(IHasInitiative entity, int delay)> entries)
        {
            if (entries == null) throw new ArgumentNullException(nameof(entries));

            var list = entries.ToList();
            if (list.Count == 0) return;

            foreach (var (entity, _) in list)
                if (entity == null) throw new ArgumentNullException(nameof(entries), "Seed() contains a null entity.");

            // Replace semantics: seeding defines the full starting state.
            _slots.Clear();
            _current = null;

            // Determine minimum delay and winner (tie -> first in input order)
            int minDelay = list.Min(e => e.delay);
            int winnerIndex = list.FindIndex(e => e.delay == minDelay);
            var winner = list[winnerIndex].entity;

            // Winner is Current at delay 0
            _slots[0] = new List<IHasInitiative> { winner };
            _current = winner;

            // Add everyone else, shifted by -minDelay, preserving input order
            for (int i = 0; i < list.Count; i++)
            {
                if (i == winnerIndex) continue;

                var (entity, delay) = list[i];
                int shifted = delay - minDelay; // >= 0

                if (!_slots.TryGetValue(shifted, out var row))
                {
                    row = new List<IHasInitiative>();
                    _slots[shifted] = row;
                }

                // If shifted == 0, append behind Current (slot0 index >= 1)
                row.Add(entity);
            }

            // Update all derived views
            RecomputeDerived();

            // Unconditional event emission (Seed is an initialization/reset operation)
            SlotsChanged?.Invoke(_slotsView);
            CurrentChanged?.Invoke(_current);
            NextChanged?.Invoke(_next);
            DelayToNextChanged?.Invoke(_delayToNext);
            OnChange?.Invoke();
        }

        // ---------------------------------------------------------------------
        // Mutations
        // ---------------------------------------------------------------------

        public void Add(IHasInitiative entity, int delay = 0)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            // Spec/tests do not cover duplicates; safest is idempotent remove-then-add.
            if (Contains(entity))
                Remove(entity);

            if (_current == null)
            {
                _slots.Clear();
                _slots[0] = new List<IHasInitiative> { entity };
                RecomputeAndEmit();
                return;
            }

            if (delay < 0)
            {
                // Non-current negative => becomes NEXT in slot0 (index 1)
                EnsureSlot0();
                _slots[0].Insert(1, entity);
            }
            else
            {
                EnsureSlot(delay);
                _slots[delay].Add(entity);
            }

            RecomputeAndEmit();
        }

        public bool Remove(IHasInitiative entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!Contains(entity)) return false;

            if (ReferenceEquals(entity, _current))
            {
                // Remove current from slot0
                EnsureSlot0();
                _slots[0].RemoveAt(0);

                if (_slots[0].Count > 0)
                {
                    // Promote next-in-row to current (still at slot 0)
                    _current = _slots[0][0];
                    RecomputeAndEmit();
                    return true;
                }

                // Slot0 now empty: remove it and choose new current by earliest delay then first-in-row.
                _slots.Remove(0);

                if (_slots.Count == 0)
                {
                    _current = null;
                    RecomputeAndEmit();
                    return true;
                }

                // Reanchor to earliest delay
                ReanchorToMinimumDelay();
                RecomputeAndEmit();
                return true;
            }

            // Removing non-current: remove from whichever slot it is in.
            foreach (var kv in _slots.ToList())
            {
                if (kv.Value.Remove(entity))
                {
                    if (kv.Value.Count == 0)
                        _slots.Remove(kv.Key);
                    break;
                }
            }

            RecomputeAndEmit();
            return true;
        }

        public void MoveAnchor(int dt)
        {
            if (_current == null) return;
            if (dt == 0) return;

            int dir = Math.Sign(dt);
            int steps = Math.Abs(dt);

            for (int i = 0; i < steps; i++)
            {
                if (dir > 0)
                    ApplyAnchorForwardOneTick();
                else
                    ApplyAnchorBackwardOneTick();

                // Observers should see post-step state
                RecomputeAndEmit();
                Tick?.Invoke(dir);
            }
        }

        public void StageAnchor(int dt)
        {
            if (_current == null) return;
            if (dt == 0) return;

            EnsureStagingSnapshot();

            int dir = Math.Sign(dt);
            int steps = Math.Abs(dt);

            for (int i = 0; i < steps; i++)
            {
                if (dir > 0)
                    ApplyAnchorForwardOneTick();
                else
                    ApplyAnchorBackwardOneTick();

                // No Tick during staging
                RecomputeAndEmit();
            }
        }

        public bool Move(IHasInitiative entity, int dt)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!Contains(entity)) return false;
            if (dt == 0) return true;

            if (ReferenceEquals(entity, _current))
            {
                // Current is always at 0 => Move(Current, dt) == SetDelay(Current, dt)
                return SetDelay(entity, dt);
            }

            int oldDelay = GetDelayOf(entity);
            int newDelay = oldDelay + dt;

            RemoveFromCurrentSlot(entity);

            if (newDelay < 0)
            {
                // Non-current cannot overtake: becomes NEXT in slot0
                EnsureSlot0();
                _slots[0].Insert(1, entity);
            }
            else
            {
                EnsureSlot(newDelay);
                if (newDelay == 0)
                {
                    // Append to slot0 (back of row) when exactly 0
                    _slots[0].Add(entity);
                }
                else
                {
                    _slots[newDelay].Add(entity);
                }
            }

            RecomputeAndEmit();
            return true;
        }

        public bool Stage(IHasInitiative entity, int dt)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!Contains(entity)) return false;
            if (dt == 0) return true;

            EnsureStagingSnapshot();

            // Must not emit Tick (and Move(entity, dt) doesn't emit Tick anyway),
            // but must emit other change events => we perform the mutation and recompute.
            if (ReferenceEquals(entity, _current))
                return SetDelay(entity, dt); // SetDelay emits change events; no Tick involved.

            int oldDelay = GetDelayOf(entity);
            int newDelay = oldDelay + dt;

            RemoveFromCurrentSlot(entity);

            if (newDelay < 0)
            {
                EnsureSlot0();
                _slots[0].Insert(1, entity);
            }
            else
            {
                EnsureSlot(newDelay);
                if (newDelay == 0) _slots[0].Add(entity);
                else _slots[newDelay].Add(entity);
            }

            RecomputeAndEmit();
            return true;
        }

        public void ClearStaging()
        {
            if (!_isStaging) return;

            if (_stagingSnapshot != null)
            {
                _slots.Clear();
                foreach (var kv in _stagingSnapshot.Slots)
                    _slots[kv.Key] = new List<IHasInitiative>(kv.Value);

                _current = _stagingSnapshot.Current;
            }

            _isStaging = false;
            _stagingSnapshot = null;

            RecomputeAndEmit();
        }

        public bool SetDelay(IHasInitiative entity, int newDelay)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (!Contains(entity)) return false;

            if (!ReferenceEquals(entity, _current))
            {
                // Non-current rules
                RemoveFromCurrentSlot(entity);

                if (newDelay < 0)
                {
                    // Becomes NEXT in slot0 (index 1), Current unchanged
                    EnsureSlot0();
                    _slots[0].Insert(1, entity);
                }
                else
                {
                    if (newDelay == 0)
                    {
                        EnsureSlot0();
                        _slots[0].Add(entity);
                    }
                    else
                    {
                        EnsureSlot(newDelay);
                        _slots[newDelay].Add(entity);
                    }
                }

                RecomputeAndEmit();
                return true;
            }

            // Current rules: move current to newDelay (can be negative), then REANCHOR
            // such that minimum delay becomes 0, and Current becomes the first entity
            // in the new 0-slot (earliest delay then first-in-row).
            // (Tests require Current can change when it no longer is earliest.)
            EnsureSlot0();
            _slots[0].RemoveAt(0);
            
            // Clean up slot 0 if it's now empty
            if (_slots[0].Count == 0)
                _slots.Remove(0);

            // Place old current into its new delay row, appended on collision
            AddToDelayRow(entity, newDelay);

            // Ensure there is still at least one entity overall
            if (TotalEntityCount() == 0)
            {
                _current = null;
                _slots.Clear();
                RecomputeAndEmit();
                return true;
            }

            // Reanchor to minimum delay (which might be negative in temp representation)
            ReanchorToMinimumDelayAllowingNegative();

            RecomputeAndEmit();
            return true;
        }

        public void CommitTurn(int cost)
        {
            if (_current == null) return;
            // Equivalent to SetDelay(Current, cost) when Current anchored at 0
            SetDelay(_current, cost);
        }

        // ---------------------------------------------------------------------
        // Internal tick mechanics for MoveAnchor / StageAnchor
        // ---------------------------------------------------------------------

        private void ApplyAnchorForwardOneTick()
        {
            // Forward: non-current delays decrease by 1, entities at delay 1 reach delay 0 and append to slot0.
            EnsureSlot0();
            var slot0 = _slots[0];

            // Move delay 1 to delay 0 (append in row order)
            if (_slots.TryGetValue(1, out var slot1))
            {
                foreach (var e in slot1)
                    slot0.Add(e);
                _slots.Remove(1);
            }

            // Shift other delays down by 1
            var keys = _slots.Keys.Where(k => k >= 2).OrderBy(k => k).ToList();
            foreach (var k in keys)
            {
                var row = _slots[k];
                _slots.Remove(k);
                EnsureSlot(k - 1);
                _slots[k - 1].AddRange(row);
            }
        }

        private void ApplyAnchorBackwardOneTick()
        {
            // Backward: non-current delays increase by 1.
            // Any followers queued at slot0 (index > 0) move away to delay 1 preserving order.
            EnsureSlot0();
            var slot0 = _slots[0];
            if (slot0.Count == 0)
                return;

            var current = slot0[0];
            var followers = slot0.Skip(1).ToList();

            // Reset slot0 to just current
            _slots[0] = new List<IHasInitiative> { current };

            // Shift all positive delays up by 1
            var keys = _slots.Keys.Where(k => k >= 1).OrderByDescending(k => k).ToList();
            foreach (var k in keys)
            {
                var row = _slots[k];
                _slots.Remove(k);
                EnsureSlot(k + 1);
                _slots[k + 1].InsertRange(0, row); // preserve row order
            }

            // Put prior followers into delay 1 (they must become delay 1 exactly)
            if (followers.Count > 0)
            {
                EnsureSlot(1);
                // Followers should be the row for delay 1, and any prior delay1 entities were shifted to 2,
                // so no collision in 1. Still, append defensively.
                _slots[1].AddRange(followers);
            }

            _current = current;
        }

        // ---------------------------------------------------------------------
        // Reanchoring
        // ---------------------------------------------------------------------

        private void ReanchorToMinimumDelay()
        {
            // Minimum delay is among existing keys (>=0)
            int min = _slots.Keys.Min();
            if (min == 0)
            {
                // Ensure Current aligns with earliest then first-in-row
                var first = _slots[0][0];
                _current = first;
                return;
            }

            // Shift all keys down by min
            var newSlots = new SortedDictionary<int, List<IHasInitiative>>();
            foreach (var kv in _slots)
                newSlots[kv.Key - min] = kv.Value;

            _slots.Clear();
            foreach (var kv in newSlots)
                _slots[kv.Key] = kv.Value;

            // Choose new current: earliest delay (0) then first-in-row
            EnsureSlot0();
            _current = _slots[0][0];
        }

        private void ReanchorToMinimumDelayAllowingNegative()
        {
            // At this moment we may have "temporary" negative delays stored in _slots
            // because SetDelay(Current, negative) placed an entry under a negative key.
            int min = _slots.Keys.Min();
            if (min != 0)
            {
                var newSlots = new SortedDictionary<int, List<IHasInitiative>>();
                foreach (var kv in _slots)
                    newSlots[kv.Key - min] = kv.Value;

                _slots.Clear();
                foreach (var kv in newSlots)
                    _slots[kv.Key] = kv.Value;
            }

            // Now delays are all >= 0 and min is 0.
            EnsureSlot0();

            // Current becomes earliest (0) then first-in-row (tie-break)
            // After reanchoring, slot 0 should have at least one entity
            if (_slots[0].Count > 0)
                _current = _slots[0][0];
            else
                _current = null; // Shouldn't happen if we have entities, but be defensive
        }

        // ---------------------------------------------------------------------
        // Helpers: slot / entity management
        // ---------------------------------------------------------------------

        private bool Contains(IHasInitiative entity)
        {
            foreach (var row in _slots.Values)
                if (row.Contains(entity)) return true;
            return false;
        }

        private int GetDelayOf(IHasInitiative entity)
        {
            foreach (var kv in _slots)
            {
                if (kv.Value.Contains(entity))
                    return kv.Key;
            }
            throw new InvalidOperationException("Entity not on track.");
        }

        private void RemoveFromCurrentSlot(IHasInitiative entity)
        {
            foreach (var kv in _slots.ToList())
            {
                if (kv.Value.Remove(entity))
                {
                    if (kv.Value.Count == 0)
                        _slots.Remove(kv.Key);
                    return;
                }
            }
        }

        private void AddToDelayRow(IHasInitiative entity, int delay)
        {
            if (!_slots.TryGetValue(delay, out var row))
            {
                row = new List<IHasInitiative>();
                _slots[delay] = row;
            }
            row.Add(entity);
        }

        private void EnsureSlot0()
        {
            if (!_slots.TryGetValue(0, out _))
                _slots[0] = new List<IHasInitiative>();
        }

        private void EnsureSlot(int delay)
        {
            if (!_slots.TryGetValue(delay, out _))
                _slots[delay] = new List<IHasInitiative>();
        }

        private int TotalEntityCount() => _slots.Values.Sum(r => r.Count);

        // ---------------------------------------------------------------------
        // Staging snapshot
        // ---------------------------------------------------------------------

        private void EnsureStagingSnapshot()
        {
            if (_isStaging) return;

            // Capture snapshot at first Stage* call
            _isStaging = true;
            _stagingSnapshot = Snapshot.Capture(_current, _slots);
        }

        private sealed class Snapshot
        {
            public required IHasInitiative? Current { get; init; }
            public required SortedDictionary<int, List<IHasInitiative>> Slots { get; init; }

            public static Snapshot Capture(IHasInitiative? current, SortedDictionary<int, List<IHasInitiative>> slots)
            {
                var copy = new SortedDictionary<int, List<IHasInitiative>>();
                foreach (var kv in slots)
                    copy[kv.Key] = new List<IHasInitiative>(kv.Value);

                return new Snapshot
                {
                    Current = current,
                    Slots = copy
                };
            }
        }

        // ---------------------------------------------------------------------
        // Derived recompute + change detection + event emission
        // ---------------------------------------------------------------------

        private void RecomputeAndEmit()
        {
            var prevCurrent = _current;
            var prevNext = _next;
            var prevDelayToNext = _delayToNext;
            var prevSlotsSig = BuildSlotsSignature(_slotsView);

            // Recompute views
            RecomputeDerived();

            var newSlotsSig = BuildSlotsSignature(_slotsView);

            bool anyChange = false;

            if (!EqualsSig(prevSlotsSig, newSlotsSig))
            {
                anyChange = true;
                SlotsChanged?.Invoke(_slotsView);
            }

            if (!ReferenceEquals(prevCurrent, _current))
            {
                anyChange = true;
                CurrentChanged?.Invoke(_current);
            }

            if (!ReferenceEquals(prevNext, _next))
            {
                anyChange = true;
                NextChanged?.Invoke(_next);
            }

            if (prevDelayToNext != _delayToNext)
            {
                anyChange = true;
                DelayToNextChanged?.Invoke(_delayToNext);
            }

            if (anyChange)
                OnChange?.Invoke();
        }

        private void RecomputeDerived()
        {
            // If empty
            if (_slots.Count == 0)
            {
                _current = null;
                _next = null;
                _delayToNext = int.MaxValue;
                _slotsView = Array.Empty<InitiativeSlot>();
                return;
            }

            // Ensure current is consistent with slot0[0] when possible
            if (_slots.TryGetValue(0, out var slot0) && slot0.Count > 0)
            {
                _current = slot0[0];
            }
            else
            {
                // Find the first entity in the earliest slot
                var earliestSlot = _slots.Where(kv => kv.Value.Count > 0).OrderBy(kv => kv.Key).FirstOrDefault();
                _current = earliestSlot.Value?.FirstOrDefault();
            }

            // Build Slots view (ordered), filtering out empty slots
            _slotsView = _slots
                .Where(kv => kv.Value.Count > 0)
                .OrderBy(kv => kv.Key)
                .Select(kv => new InitiativeSlot
                (
                    delay: kv.Key,
                    row: kv.Value.AsReadOnly()
                ))
                .ToList();

            // Next + DelayToNext
            // Check if slot 0 exists and has entities
            if (_slots.TryGetValue(0, out var slot0Check) && slot0Check.Count >= 2)
            {
                _next = slot0Check[1];
                _delayToNext = 0;
                return;
            }

            // Find smallest positive delay with entities
            var nextSlot = _slots.Where(kv => kv.Key > 0 && kv.Value.Count > 0).OrderBy(kv => kv.Key).FirstOrDefault();
            if (nextSlot.Value != null)
            {
                _next = nextSlot.Value[0];
                _delayToNext = nextSlot.Key;
            }
            else
            {
                _next = null;
                _delayToNext = int.MaxValue;
            }
        }

        private static List<(int delay, IHasInitiative[] row)> BuildSlotsSignature(IReadOnlyList<InitiativeSlot> slots)
        {
            var sig = new List<(int, IHasInitiative[])>(slots.Count);
            foreach (var s in slots)
                sig.Add((s.Delay, s.Row.ToArray()));
            return sig;
        }

        private static bool EqualsSig(List<(int delay, IHasInitiative[] row)> a, List<(int delay, IHasInitiative[] row)> b)
        {
            if (a.Count != b.Count) return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].delay != b[i].delay) return false;
                var ar = a[i].row;
                var br = b[i].row;
                if (ar.Length != br.Length) return false;
                for (int j = 0; j < ar.Length; j++)
                    if (!ReferenceEquals(ar[j], br[j])) return false;
            }
            return true;
        }
    }
}
