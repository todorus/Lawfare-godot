Below is a compact **Game Design Overview** written as a reusable prompt-style document. It summarizes the current mechanical model of a hearing.

---

# Game Design Overview — Courtroom Hearing System

## High Concept

A tactical, initiative-based courtroom RPG.

Two teams of lawyers compete in a hearing before a Judge, attempting either to:

* **Elicit testimony** from a Witness by reducing the Secrecy of statements to 0, or
* **Shut down the opposing legal team** by attacking their Credibility and forcing tempo loss.

The system is turn-order based, with strong emphasis on resource management, narrative momentum, and control of the “Speaker” role.

---

# Hearing Context

A hearing consists of:

* **Two Teams** (e.g., Prosecution and Defense)
    * Each fields 1 to 3 Lawyers.
* **One Judge**
* **One Witness**

The hearing is resolved through a round-bounded initiative system, and is limited to five rounds.

---

# Entities and Properties

## 1. Team

Each team has shared properties:

### Favor (of Witness)

* Represents rapport and trust with the Witness.
* Used as input for certain actions and Ults.
* Can be increased or reduced by actions.
* Does not reset automatically between turns.

### Leniency (of Judge)

* Represents how tolerant the Judge is toward the team.
* Used as input for procedural or authority-based actions.
* Can affect action efficiency or scaling.
* Is contestable between teams.

---

## 2. Witness

The Witness has psychological properties:

### Stress

* Increased by aggressive actions.
* May amplify certain Ults.
* May be consumed by certain effects.
* Can be reduced by calming actions.

### Defensiveness

* Represents resistance to being cornered.
* Reduced by subtle or reassuring tactics.
* High Defensiveness may reduce effectiveness of some effects.

The Witness holds multiple **Statements**, each with:

### Secrecy

* A numeric resistance value.
* Reduced only by Ults.
* When Secrecy reaches 0 → the Statement is elicited.

---

## 3. Lawyer

Each Lawyer has individual properties:

### Initiative

* Determines initial position on the timeline.
* Lawyers act once per round.
* Can be pushed backward to delay their turn.
* If pushed past the round boundary, they lose their action this round.

### Charge

* Resource used to perform Ult.
* Generated via actions or triggers.
* Lawyer-specific scaling.

### Credibility

* Represents courtroom authority and reputation.
* If Credibility reaches 0 → Lawyer becomes **Compromised**.
* Compromised state is lawyer-specific and must be recovered from.
* Recovery typically requires an action or allied support.

### Speaker State (Derived)

* The lawyer who most recently acted for their team becomes the **Speaker**.
* Remains Speaker until a teammate acts.
* Certain effects conditionally check whether a lawyer is the current Speaker.

---

# Initiative System

The system is round-bounded:

* Each Lawyer acts once per round.
* All Lawyers are placed on an initiative timeline.
* Actions progress the timeline.
* Some actions push targets backward.
* If a lawyer is pushed beyond the round boundary:

    * They lose their action this round.
    * They re-enter next round normally.

Conditional effects may check:

* Whether a target has already acted this round.
* Whether a target is the current Speaker.

---

# Actions

An Action is a lawyer ability that:

* Has an Initiative Cost.
* May modify:

    * Witness stats (Stress, Defensiveness)
    * Team stats (Favor, Leniency)
    * Lawyer stats (Charge, Credibility)
    * Initiative positions
* May conditionally check:

    * Speaker state
    * HasActedThisRound
    * Stat thresholds

Actions do **not** directly reduce Secrecy.

---

# Ults

Each Lawyer has one Ult:

* Requires a full Charge.
* Has an Initiative Cost.
* Reduces Secrecy of a target Statement.
* If Secrecy reaches 0 → Statement is elicited.

Ults may scale based on:

* Team Favor
* Witness Stress
* Leniency
* Lawyer Credibility
* Other kit-specific inputs

---

# Triggers

Triggers are passive effects that respond to specific game events.

Triggers have:

* A Name
* A Trigger Event
* A Conditional Clause (optional)
* An Effect

---

## Trigger Events

The system currently supports:

### AfterAction

* Fires after an Action fully resolves.

### StartTurn

* Fires when a Lawyer’s turn begins.

### Tick

* Fires when the timeline advances. Even when that slot in the timeline is unoccupied.

Triggers can modify:

* Charge
* Favor
* Leniency
* Credibility
* Initiative
* Witness stats

---

# Compromised State

When a Lawyer’s Credibility reaches 0:

* They enter a **Compromised** state.
* Effects are lawyer-specific.
* They do not automatically recover next turn.
* Recovery requires:

    * A recovery action, or
    * Allied intervention.

Compromised effects typically:

* Prevent Charge generation,
* Increase initiative costs,
* Disable specific triggers.

---

# Win Conditions (Hearing-Level)

A team can win by:

* Successfully eliciting key Statements.
* Preventing the opposing team from functioning effectively.
* Forcing tempo collapse.
* Achieving narrative or procedural advantage.

---

# Design Pillars

* All testimony is revealed through Ults.
* Initiative defines tempo and control.
* Speaker defines narrative focus.
* Credibility is a soft defensive stat.
* Favor and Leniency are contested social resources.
* Archetypes scale off specific stat axes.
* No stat is universally dominant.

---
