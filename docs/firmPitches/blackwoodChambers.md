# Firm 3 — Blackwood Chambers

## Firm Theme: Narrative Dominance & Speaker Control

Controlling *who* is talking and *when*. This firm weaponizes the Speaker mechanic and initiative ordering.

**Stress interpretation:** The witness's *narrative confusion* — losing track of their own story as different lawyers seize and release the floor.
**Defensiveness interpretation:** The witness's *ability to maintain a coherent narrative* against interruption.

---

## James Blackwood — Head of Chambers

**Flavor:**
*"The floor is mine."*

**Description:**
James is strongest when he is Speaker and weakest when he isn't. His entire kit rewards holding the floor — he generates Charge, Favor, and Leniency passively while speaking. His actions let him reclaim Speaker status or punish opponents who try to take the narrative. Weakness: if the opponent forces rapid Speaker rotation within James's own team, his passives never fire.

### Properties Interaction Summary

* **Primary Axis:** Speaker state
* **Secondary Axis:** Initiative
* **Vulnerabilities:** Forced Speaker rotation within own team disrupts passives
* **Counterplay:** Force James's teammates to act frequently; attack Credibility to disable his passive triggers

### Design Roles

* Scaler (Passive growth while Speaker)
* Controller (Narrative control)

---

### Triggers

**Name:** Commanding Presence
**Event:** Tick
**Condition:** James is Speaker.
**Effect:** +1 Charge, +1 Favor.

**Name:** Narrative Gravity
**Event:** AfterAction
**Condition:** James was Speaker before this action and is still Speaker after.
**Effect:** +1 Leniency.

**Name:** Interrupted
**Event:** AfterAction
**Condition:** James lost Speaker status this action (a teammate acted).
**Effect:** −1 Charge.

---

### Actions

**Name:** Hold the Floor
**Flavor:** *"I'm not finished."*
**Initiative Cost:** 0
**Description:**
1. +1 Favor.
2. +1 Stress to Witness.
3. Push self back +1 on initiative.
4. +1 Charge.

**Name:** Yield and Reclaim
**Flavor:** *"My colleague will elaborate — briefly."*
**Initiative Cost:** 1
**Description:**
1. Push target allied lawyer forward −2 on initiative.
2. James becomes Speaker.
3. +1 Leniency.

**Name:** Object to Narrative
**Flavor:** *"That is not what the witness said."*
**Initiative Cost:** 0
**Description:**
1. −1 Credibility to opposing Speaker.
2. +1 Stress to Witness.
3. +1 Charge.

---

### Ult

**Name:** Closing Argument
**Flavor:** *"Members of the court — the truth has been before you all along."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (3 + team Favor).
2. James becomes Speaker.

---

### Compromised

* *Commanding Presence* and *Narrative Gravity* triggers disabled. James loses all passive generation.

### Speaker Interaction

* *Commanding Presence* and *Narrative Gravity* both require James to be Speaker.
* *Interrupted* fires when James *loses* Speaker status.
* **Object to Narrative** targets opposing Speaker.
* **Closing Argument** makes James Speaker on resolution.

---
---

## Seren Alder — Deputy Head

**Flavor:**
*"I believe my colleague was making a point."*

**Description:**
Seren is the firm's tempo engine. She manipulates initiative positions for the entire team, ensuring James holds the floor at the right moments and that opponents can never establish their own narrative rhythm. She passes Speaker back to James efficiently. Her own Ult is modest but fast. Weakness: low personal stat generation — she's a pure enabler.

### Properties Interaction Summary

* **Primary Axis:** Initiative
* **Secondary Axis:** Speaker state (allied)
* **Vulnerabilities:** Low personal stat generation; weak in isolation
* **Counterplay:** Target her Credibility to shut down the enabler; disrupt initiative ordering

### Design Roles

* Controller (Initiative manipulation)
* Support (Tempo enabler)

---

### Triggers

**Name:** Smooth Handoff
**Event:** AfterAction
**Condition:** Seren is Speaker and an allied lawyer acts next in initiative order.
**Effect:** That ally gains +1 Charge.

---

### Actions

**Name:** Cede the Floor
**Flavor:** *"I defer to my colleague."*
**Initiative Cost:** 0
**Description:**
1. Push target allied lawyer forward −2 on initiative.
2. +1 Charge to that ally.
3. +1 Favor.

**Name:** Interject
**Flavor:** *"If I may — counsel was not recognized."*
**Initiative Cost:** 0
**Description:**
1. Push target opposing lawyer back +2 on initiative.
2. +1 Stress to Witness.
3. +1 Charge.

**Name:** Sustain
**Flavor:** *"The court's patience will be rewarded."*
**Initiative Cost:** 1
**Description:**
1. +2 Leniency.
2. +1 Credibility to self.
3. −1 Defensiveness on Witness.

---

### Ult

**Name:** Orchestrated Revelation
**Flavor:** *"Every voice has led to this moment."*
**Initiative Cost:** 1
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (3 + number of allied lawyers who have acted this round × 2).

---

### Compromised

* Cannot push allied lawyers forward on initiative.

---
---

## Rowan Falk — Silk

**Flavor:**
*"Strike that. I'll rephrase."*

**Description:**
Rowan is the narrative saboteur. He deliberately fragments the witness's story by rephrasing, contradicting, and interrupting — driving up Stress (confusion) and reducing Defensiveness (narrative coherence). He generates Charge by keeping the witness off-balance. He's the firm's closer when James is shut down. Weakness: heavy Leniency cost — the Judge dislikes his style.

### Properties Interaction Summary

* **Primary Axis:** Stress
* **Secondary Axis:** Defensiveness
* **Vulnerabilities:** Leniency drain; the Judge punishes his approach
* **Counterplay:** Deny Leniency to make his style unsustainable; keep Stress low via calming actions

### Design Roles

* Eroder (Dual-axis Stress + inverse Defensiveness scaling)
* Breaker (High-ceiling Ult)

---

### Triggers

**Name:** Contradiction
**Event:** AfterAction
**Condition:** Witness Stress increased and Witness Defensiveness decreased in the same action.
**Effect:** +1 Charge.

---

### Actions

**Name:** Rephrase
**Flavor:** *"What the witness meant to say..."*
**Initiative Cost:** 0
**Description:**
1. +2 Stress to Witness.
2. −1 Defensiveness on Witness.
3. −1 Leniency to own team.

**Name:** Strike from Record
**Flavor:** *"Your Honor, I move to strike."*
**Initiative Cost:** 1
**Description:**
1. −2 Defensiveness on Witness.
2. +1 Stress to Witness.
3. +1 Charge.
4. If Witness Defensiveness was already ≤ 2 before this effect: +1 additional Charge.

**Name:** Redirect
**Flavor:** *"Let's get back on track — my track."*
**Initiative Cost:** 0
**Description:**
1. +1 Favor.
2. +1 Charge.
3. Push opposing Speaker back +1 on initiative.

---

### Ult

**Name:** Fractured Testimony
**Flavor:** *"The witness can no longer keep the story straight."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + Witness Stress + (4 − Witness Defensiveness)).
2. −2 Stress on Witness.

---

### Compromised

* All actions cost +1 additional Leniency.

### Speaker Interaction

* **Redirect** targets opposing Speaker.

