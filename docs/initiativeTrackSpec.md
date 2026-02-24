### State model

1. The initiative track is an indexed array of slots `0..TrackLength-1`.
2. Slots have:
    * `Occupant` (nullable)
    * `IsStaggered : bool`
3. There is a `RoundEndIndex` (last active index). `RoundLength = RoundEndIndex + 1`.
4. `CurrentIndex` is a slot index (not an entity). `GetCurrent()` returns `Slots[CurrentIndex].Occupant` (or null).

### Seeding

5. `Seed(entries)` accepts a list of `(entity, initialInitiative)` pairs and produces the initial `InitiativeTrackState`:
    * Sort entities by their `initialInitiative` value ascending (lower value = acts sooner). Entities with equal values preserve input order.
    * Place each entity into consecutive slots starting at index 0, one entity per slot, in sorted order. There are no gaps between entities.
    * Append one additional empty slot after the last entity. `TrackLength = entityCount + 1`.
    * `RoundEndIndex = TrackLength - 1` (i.e. `entityCount`).
    * `CurrentIndex = 0` (the entity with the lowest initiative value acts first).
    * All slots have `IsStaggered = false`.
    * If the entry list is empty, the track has no slots, `CurrentIndex = 0`, `RoundEndIndex = 0`.

### MoveEntity

6. `MoveEntity(entity, dt)` returns an **array** of `InitiativeDiff`:
    * If no cascade happens: exactly 1 diff (moved subject).
    * If cascade happens: diffs for **all subjects that changed slots** (including the moved subject).
    * Each diff includes `BecameStaggered : bool` for that subject (true if that subject ends in a stagger slot, but wasn't before, due to this move).
7. Destination index is computed from the entity's current slot + `dt`, then clamped:
    * No entity may be placed into `CurrentIndex` or any slot `< CurrentIndex`.
    * If destination would be `<= CurrentIndex`, destination becomes `CurrentIndex + 1`.
8. If destination slot is empty: move succeeds; source becomes empty.
9. If destination slot is occupied: resolve collision **forward-first**:
    * If there exists a vacant slot `V` with `CurrentIndex < V < Destination`, resolve by shifting occupants in `(V..Destination]` one step toward smaller indices, making `Destination` empty, then place moved entity into `Destination`.
    * Otherwise do backward cascade: moved entity takes `Destination`, displaced occupant goes to `Destination+1`, etc.
    * If any pushed occupant crosses `RoundEndIndex`, it becomes staggered (`IsStaggered=true`) in its final slot.
    * If cascade needs indices beyond `TrackLength-1`, the track grows to include them (new slots are empty, with `IsStaggered` set to if they exceed the round).

### Moving the current occupant

10. Moving the occupant currently at `CurrentIndex` is allowed:
    * The moved entity is inserted at its destination using the same collision rules as moving any other entity, with forward cascade first, then backward cascade as a fallback.

### Tick

11. `Tick()` advances `CurrentIndex` by exactly 1 while `CurrentIndex < RoundEndIndex`.
12. When `CurrentIndex == RoundEndIndex`, the **next tick** starts a new round:

* Gather staggered occupants in increasing slot index order.
* Then gather active occupants from slots `0..RoundEndIndex` in increasing index order.
* Build the next round's active slots `0..RoundEndIndex` filled in that combined order (truncate to `RoundLength` if needed).
* Clear `IsStaggered` for all occupants placed into active slots.
* Shrink the track back to exactly `RoundLength` slots (indices `0..RoundEndIndex`).
* Set `CurrentIndex = 0`.

---

## Gherkin suite (full before/after tables for any slot-changing scenario)

```gherkin
Feature: Initiative track with slot-based current, forward-first cascades, staggering, and round rebuild on tick past round end

  Background:
    Given an initiative track with slots having occupant and isStaggered
    And RoundEndIndex defines the round length (RoundLength = RoundEndIndex + 1)
    And CurrentIndex is a slot index

  # ------------------------------------------------------------
  # Seeding
  # ------------------------------------------------------------

  Scenario: Seed with no entries produces an empty track
    When I seed the track with no entries
    Then TrackLength is 0
    And CurrentIndex is 0
    And RoundEndIndex is 0
    And ReadSlots returns an empty list
    And GetCurrent returns null

  Scenario: Seed with entries places entities in ascending initiative order with one trailing empty slot
    When I seed the track with:
      | entity | initialInitiative |
      | C      | 3                 |
      | A      | 1                 |
      | D      | 4                 |
      | B      | 2                 |
    Then TrackLength is 5
    And RoundEndIndex is 4
    And CurrentIndex is 0
    And the track slots are:
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | B        | false       |
      | 2     | C        | false       |
      | 3     | D        | false       |
      | 4     | (empty)  | false       |
    And GetCurrent returns A

  Scenario: Seed preserves input order for entities with equal initiative values
    When I seed the track with:
      | entity | initialInitiative |
      | A      | 2                 |
      | B      | 2                 |
      | C      | 2                 |
    Then TrackLength is 4
    And RoundEndIndex is 3
    And CurrentIndex is 0
    And the track slots are:
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | B        | false       |
      | 2     | C        | false       |
      | 3     | (empty)  | false       |
    And GetCurrent returns A

  Scenario: Seed with a single entity produces two slots
    When I seed the track with:
      | entity | initialInitiative |
      | A      | 5                 |
    Then TrackLength is 2
    And RoundEndIndex is 1
    And CurrentIndex is 0
    And the track slots are:
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
    And GetCurrent returns A

  # ------------------------------------------------------------
  # Movement: no cascade (single diff)
  # ------------------------------------------------------------

  Scenario: MoveEntity to an empty destination produces one diff and vacates the source slot
    Given RoundEndIndex is 4
    And CurrentIndex is 1
    And TrackLength is 5
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | (empty)  | false       |
      | 1     | B        | false       |
      | 2     | X        | false       |
      | 3     | (empty)  | false       |
      | 4     | (empty)  | false       |
    When I move entity X by dt 2
    Then the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | (empty)  | false       |
      | 1     | B        | false       |
      | 2     | (empty)  | false       |
      | 3     | (empty)  | false       |
      | 4     | X        | false       |
    And the result contains exactly 1 initiative diff
    And the diffs are:
      | subject | originalIndex | updatedIndex | becameStaggered |
      | X       | 2             | 4           | false           |

  # ------------------------------------------------------------
  # Movement: clamping (<= CurrentIndex)
  # ------------------------------------------------------------

  Scenario: Destination at or before CurrentIndex is clamped to CurrentIndex+1
    Given RoundEndIndex is 5
    And CurrentIndex is 2
    And TrackLength is 6
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | C        | false       |
      | 3     | (empty)  | false       |
      | 4     | (empty)  | false       |
      | 5     | X        | false       |
    When I move entity X by dt -10
    Then the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | C        | false       |
      | 3     | X        | false       |
      | 4     | (empty)  | false       |
      | 5     | (empty)  | false       |
    And the diffs are:
      | subject | originalIndex | updatedIndex | becameStaggered |
      | X       | 5             | 3           | false           |

  # ------------------------------------------------------------
  # Movement: forward cascade (vacancy exists between CurrentIndex and destination)
  # ------------------------------------------------------------

  Scenario: Collision resolves via forward cascade when a vacancy exists between CurrentIndex and destination
    Given RoundEndIndex is 6
    And CurrentIndex is 1
    And TrackLength is 7
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | (empty)  | false       |
      | 1     | B        | false       |
      | 2     | (empty)  | false       |  # vacancy usable for forward cascade
      | 3     | C        | false       |
      | 4     | D        | false       |  # destination occupied
      | 5     | (empty)  | false       |
      | 6     | X        | false       |
    When I move entity X to destination index 4
    Then the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | (empty)  | false       |
      | 1     | B        | false       |
      | 2     | C        | false       |
      | 3     | D        | false       |
      | 4     | X        | false       |
      | 5     | (empty)  | false       |
      | 6     | (empty)  | false       |
    And the result contains diffs for subjects: C, D, X
    And no diff has becameStaggered true

  # ------------------------------------------------------------
  # Movement: Forward cascade into previously occupied slot that becomes vacant due to the moved object
  # ------------------------------------------------------------

  Scenario: Forward cascade is used when there is a vacancy left by the moved object, even if that vacancy is created by the move itself
    Given RoundEndIndex is 3
    And CurrentIndex is 0
    And TrackLength is 4
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | Z        | false       |
      | 1     | A        | false       |
      | 2     | B        | false       |  # destination occupied
      | 3     | C        | false       |
    When I move entity A to destination index 2
    And the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | Z        | false       |
      | 1     | B        | false       |
      | 2     | A        | false       |
      | 3     | C        | false       |
    And the result contains diffs for subjects: A, B
    And the diffs are:
      | subject | originalIndex | updatedIndex | becameStaggered |
      | A       | 1             | 2           | false           |
      | B       | 2             | 1           | false           |

  # ------------------------------------------------------------
  # Movement: moved subject becomes staggered by overshooting RoundEndIndex
  # ------------------------------------------------------------

  Scenario: Moving an entity directly beyond RoundEndIndex places it into a stagger slot and grows the track
    Given RoundEndIndex is 3
    And CurrentIndex is 0
    And TrackLength is 4
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | X        | false       |
      | 3     | (empty)  | false       |
    When I move entity X to destination index 6
    Then TrackLength becomes 7
    And the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | (empty)  | false       |
      | 3     | (empty)  | false       |
      | 4     | (empty)  | false       |
      | 5     | (empty)  | false       |
      | 6     | X        | true        |
    And the diffs are:
      | subject | originalIndex | updatedIndex | becameStaggered |
      | X       | 2             | 6           | true            |

  # ------------------------------------------------------------
  # Moving the current occupant (pre-shift + destination resolution)
  # ------------------------------------------------------------

  Scenario: Moving the current occupant resolves destination with forward cascade
    Given RoundEndIndex is 6
    And CurrentIndex is 2
    And TrackLength is 7
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | B        | false       |
      | 2     | C        | false       |  # current occupant
      | 3     | (empty)  | false       |  # vacancy between current and destination
      | 4     | D        | false       |  # destination occupied
      | 5     | E        | false       |
      | 6     | (empty)  | false       |
    When I move entity C to destination index 4
    Then CurrentIndex remains 2
    And the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | B        | false       |
      | 2     | (empty)  | false       |
      | 3     | D        | false       |
      | 4     | C        | false       |
      | 5     | E        | false       |
      | 6     | (empty)  | false       |
    And the result contains diffs for subjects: C, D

  Scenario: Moving the current occupant can trigger backward cascade and stagger the tail
    Given RoundEndIndex is 3
    And CurrentIndex is 1
    And TrackLength is 4
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | B        | false       |  # current occupant
      | 2     | C        | false       |  # destination occupied
      | 3     | D        | false       |
    When I move entity B to destination index 2
    Then TrackLength becomes 5
    And CurrentIndex remains 1
    And the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       | # current is now empty
      | 2     | B        | false       |
      | 3     | C        | false       |
      | 4     | D        | true        |
    And the result contains diffs for subjects: B, C, D
    And only subject D has becameStaggered true

  # ------------------------------------------------------------
  # Tick semantics (one-step) and round rebuild (shrink to round length)
  # ------------------------------------------------------------

  Scenario: Tick advances CurrentIndex by one (no skipping), without changing slots
    Given RoundEndIndex is 4
    And CurrentIndex is 2
    And TrackLength is 5
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | B        | false       |
      | 3     | C        | false       |
      | 4     | (empty)  | false       |
    When I tick once
    Then CurrentIndex is 3
    And the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | B        | false       |
      | 3     | C        | false       |
      | 4     | (empty)  | false       |

  Scenario: Ticking once when CurrentIndex == RoundEndIndex rebuilds next round order and shrinks the track to RoundLength
    Given RoundEndIndex is 4
    And CurrentIndex is 4
    And TrackLength is 7
    And the track slots are (before):
      | index | occupant | isStaggered |
      | 0     | A        | false       |
      | 1     | (empty)  | false       |
      | 2     | B        | false       |
      | 3     | C        | false       |
      | 4     | (empty)  | false       |
      | 5     | X        | true        |
      | 6     | Y        | true        |
    When I tick once
    Then a new round begins
    And TrackLength becomes 5
    And CurrentIndex becomes 0
    And the track slots are (after):
      | index | occupant | isStaggered |
      | 0     | X        | false       |
      | 1     | Y        | false       |
      | 2     | A        | false       |
      | 3     | B        | false       |
      | 4     | C        | false       |
```
