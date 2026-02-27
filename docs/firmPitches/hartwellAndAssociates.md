# Firm 1 — Hartwell & Associates

## Firm Theme: Old-Guard Proceduralists

Courtroom formality as a weapon. They win by accumulating Leniency, controlling initiative tempo, and grinding the opponent down through procedure.

**Stress interpretation:** The witness's *fatigue from endless procedural back-and-forth*.
**Defensiveness interpretation:** The witness's *wariness of being trapped by legalese*.

---

## Margaret Hartwell — Managing Partner

**Flavor:**
*"Objection sustained — as always."*

**Description:**
The firm's anchor. Margaret builds massive Leniency leads and converts them into devastating initiative delays for opponents. Her Ult scales off Leniency — the more the Judge trusts her, the more truth she extracts. She's slow (high initiative value) but inevitable. Vulnerable to Credibility pressure because her authority *is* her identity.

### Properties Interaction Summary

* **Primary Axis:** Leniency
* **Secondary Axis:** Initiative
* **Vulnerabilities:** Credibility pressure
* **Counterplay:** Snipe her Credibility to collapse her authority; deny Leniency generation

### Design Roles

* Generator (Leniency)
* Controller (Initiative manipulation)

---

### Triggers

**Name:** Bench Privilege
**Event:** StartTurn
**Condition:** Team Leniency ≥ 3
**Effect:** +1 Charge to self.

**Name:** Sustained Objection
**Event:** AfterAction
**Condition:** This action increased team Leniency.
**Effect:** Push opposing Speaker back +1 on initiative.

---

### Actions

**Name:** File Motion
**Flavor:** *"Your Honor, I move to..."*
**Initiative Cost:** 0
**Description:**
1. +2 Leniency to own team.
2. If self is Speaker: +1 Charge.

**Name:** Point of Order
**Flavor:** *"Counsel is out of order."*
**Initiative Cost:** 1
**Description:**
1. Push target opposing lawyer back +2 on initiative.
2. If target has not acted this round: +1 Leniency.

**Name:** Admonish
**Flavor:** *"The court does not appreciate theatrics."*
**Initiative Cost:** 0
**Description:**
1. −1 Credibility to target opposing lawyer.
2. +1 Stress to Witness.

---

### Ult

**Name:** Judicial Mandate
**Flavor:** *"The court compels an answer."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (3 + team Leniency).
2. −1 Leniency to own team.

---

### Compromised

* Cannot gain Leniency from any source while Compromised.
* Triggers still fire but Leniency effects are nullified.

### Speaker Interaction

* **File Motion** gains bonus Charge when Margaret is Speaker.
* **Sustained Objection** targets opposing Speaker.

---
---

## Arthur Pryce — Senior Associate

**Flavor:**
*"Let the record reflect..."*

**Description:**
Arthur is the firm's tempo weapon. He's fast (low initiative value) and his kit revolves around acting early to set up procedural walls. He taxes opponents who act after him and converts tempo advantage into Charge. His weakness is that he generates almost no Favor — he doesn't connect with the witness at all.

### Properties Interaction Summary

* **Primary Axis:** Initiative
* **Secondary Axis:** Leniency
* **Vulnerabilities:** No Favor generation; irrelevant to Witness rapport
* **Counterplay:** Ignore him and focus on Favor/Stress; push him back on initiative to negate his early-action triggers

### Design Roles

* Controller (Initiative manipulation)
* Denial (Tempo suppression)

---

### Triggers

**Name:** Early Filing
**Event:** StartTurn
**Condition:** No other lawyer on either team has acted this round.
**Effect:** +1 Charge, +1 Leniency.

---

### Actions

**Name:** Preemptive Objection
**Flavor:** *"Objection — before counsel even begins."*
**Initiative Cost:** 0
**Description:**
1. Choose target opposing lawyer who has not acted this round.
2. Push target back +1 on initiative.
3. +1 Charge.

**Name:** Cross-Reference
**Flavor:** *"Exhibit 14-B, paragraph three, subsection..."*
**Initiative Cost:** 1
**Description:**
1. +2 Leniency to own team.
2. −1 Defensiveness on Witness.

**Name:** Redact
**Flavor:** *"Move to strike."*
**Initiative Cost:** 0
**Description:**
1. −2 Credibility to target opposing lawyer.
2. −1 Leniency to own team.

---

### Ult

**Name:** Procedural Override
**Flavor:** *"The record speaks for itself."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + team Leniency).
2. Push all opposing lawyers back +1 on initiative.

---

### Compromised

* Initiative cost of all actions increases by +1.

---
---

## Diana Lowe — Of Counsel

**Flavor:**
*"I believe the court will find this... instructive."*

**Description:**
Diana is the firm's insurance policy. She restores Credibility to allies, tops off Leniency, and punishes opponents who target her teammates. She generates Charge slowly but is almost impossible to shut down because her triggers keep the team's stats topped up. Her weakness is low personal damage output — she can't close out a hearing alone.

### Properties Interaction Summary

* **Primary Axis:** Leniency
* **Secondary Axis:** Credibility
* **Vulnerabilities:** Low offensive output; cannot close hearings solo
* **Counterplay:** Ignore her and target her teammates faster than she can heal; rush Secrecy reduction before she stabilizes the board

### Design Roles

* Support (Resource transfer)
* Generator (Leniency / Credibility)

---

### Triggers

**Name:** Friend of the Court
**Event:** AfterAction
**Condition:** An allied lawyer lost Credibility this action.
**Effect:** +1 Leniency, +1 Credibility to that ally.

**Name:** Quiet Authority
**Event:** Tick
**Condition:** Diana is Speaker and has not acted this round.
**Effect:** +1 Charge.

---

### Actions

**Name:** Rehabilitate
**Flavor:** *"My colleague's record speaks volumes."*
**Initiative Cost:** 0
**Description:**
1. +2 Credibility to target allied lawyer.
2. +1 Charge to self.

**Name:** Sidebar
**Flavor:** *"If I may, Your Honor — a brief aside."*
**Initiative Cost:** 1
**Description:**
1. +2 Leniency to own team.
2. +1 Favor.

**Name:** Caution the Witness
**Flavor:** *"Take a moment. There's no rush."*
**Initiative Cost:** 0
**Description:**
1. −2 Stress on Witness.
2. +1 Defensiveness on Witness.
3. +1 Charge.

---

### Ult

**Name:** Amicus Brief
**Flavor:** *"The court's own interest demands an answer."*
**Initiative Cost:** 1
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + team Leniency + highest allied Credibility).
2. +1 Credibility to all allies.

---

### Compromised

* *Friend of the Court* trigger is disabled.

### Speaker Interaction

* *Quiet Authority* requires Diana to be Speaker.

