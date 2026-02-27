Below is a companion **Lawyer Specification Template Document**.
It formalizes how a lawyer kit should be written and structured for design, iteration, or content generation.

This document is system-facing, not flavor-first.

---

# Lawyer Specification Document

## Purpose

This document defines the standardized structure for designing a Lawyer within the Courtroom Hearing System.

Each Lawyer must:

* Integrate cleanly into the initiative system.
* Use only defined entity properties.
* Have a distinct mechanical identity.
* Avoid reliance on hidden global rules.
* Express their archetype through interaction with:

    * Favor
    * Leniency
    * Stress
    * Defensiveness
    * Charge
    * Credibility
    * Initiative
    * Speaker state
    * HasActedThisRound

Each Lawyer contains:

* Name
* Flavor line
* Description
* Optional Triggers
* Three Actions
* One Ult

---

# Lawyer Template Structure

---

## Lawyer Name

**Flavor:**
*"Short archetypal quote that captures identity."*

**Description:**
One paragraph explaining:

* Core fantasy.
* Primary stat axis.
* Intended play pattern.
* Strengths.
* Weaknesses.
* Strategic role (generator, breaker, controller, scaler, sniper, etc.).

---

# Properties Interaction Summary (Optional but Recommended)

Specify which axes this lawyer primarily interacts with:

* Primary Axis:
* Secondary Axis:
* Vulnerabilities:
* Counterplay:

---

# Triggers (0–3)

Triggers are passive effects.

Each Trigger must specify:

* Name
* Flavor (optional)
* Event
* Condition (if any)
* Effect

---

### Trigger Template

**Name:**
**Flavor:**
**Event:** (Action / AfterAction / StartTurn / Tick)
**Condition:** (Optional)
**Effect:** Clear mechanical effect.

---

### Trigger Design Constraints

* Must reference existing stats.
* Must not create new entity properties.
* Must not permanently modify base rules.
* Should reinforce archetype identity.

---

# Actions (Exactly 3)

Each Lawyer has three standard Actions.

Actions must specify:

* Name
* Flavor
* Initiative Cost
* Description
* Conditional effects (if any)

---

### Action Template

**Name:**
**Flavor:**
**Initiative Cost:** (integer)
**Description:**
List mechanical effects in clear order of resolution.

---

### Action Design Constraints

* May modify:

    * Stress
    * Defensiveness
    * Favor
    * Leniency
    * Charge
    * Credibility
    * Initiative position
* May check:

    * Speaker state
    * HasActedThisRound
    * Stat thresholds
* Must not reduce Secrecy.
* Must not create new core mechanics.

---

# Ult (Exactly 1)

Each Lawyer has one Ult.

Ults:

* Require sufficient Charge.
* Have an Initiative Cost.
* Reduce Secrecy.
* May have additional scaling or secondary effects.

---

### Ult Template

**Name:**
**Flavor:**
**Initiative Cost:**
**Charge Requirement:** (implicit full charge unless otherwise specified)
**Description:**
Reduce Secrecy of target Statement by [formula].

Optional secondary effects allowed but must not:

* Bypass Secrecy rule.
* Skip threshold logic.
* Add hidden global modifiers.

---

# Compromised Behavior (If Applicable)

If the Lawyer has a unique Compromised effect, define:

* What happens at Credibility = 0.
* What is disabled or weakened.
* How recovery occurs (if unique).

If no unique behavior is specified:

* Compromised follows default rule.

---

# Speaker Interaction (Optional)

If the Lawyer references Speaker state, specify:

* What changes when they are Speaker.
* What changes when they are not Speaker.
* Whether effects target opposing Speaker.

---

# Design Roles Classification (Optional)

Mark the Lawyer’s systemic role:

* Generator (Charge/Favor/Leniency)
* Breaker (Stress-based Ult spike)
* Eroder (Scaling Ult)
* Controller (Initiative manipulation)
* Sniper (Credibility pressure)
* Support (Resource transfer)
* Scaler (Passive growth)
* Denial (Tempo suppression)

This helps team composition planning.

---

# Validation Checklist

Before finalizing a Lawyer, confirm:

* Does the kit rely on one clear primary stat?
* Is there meaningful counterplay?
* Does Compromised hurt but not delete them?
* Is their Ult scaling readable?
* Can they function in a duo?
* Do they avoid universal stat dominance?
* Do they avoid infinite loops?
* Do their actions do multiple weak effects, or one strong effect?

---

# Example Skeleton (Minimal)

Lawyer Name
Flavor:
Description:

Triggers:

* Name / Event / Effect

Actions:

1. Name / Cost / Effect
2. Name / Cost / Effect
3. Name / Cost / Effect

Ult:

* Name / Cost / Secrecy Reduction Formula

Compromised:

* Effect at Credibility = 0

---

# Design Philosophy Reminder

* Ults reveal truth.
* Actions shape state.
* Triggers reinforce identity.
* Credibility is a soft defense.
* Favor and Leniency are contested.
* Initiative defines tempo.
* Speaker defines narrative focus.

---

This template ensures that every new Lawyer remains:

* Mechanically coherent.
* Archetypically distinct.
* Compatible with the core system.
* Balanced against the meta axes.
