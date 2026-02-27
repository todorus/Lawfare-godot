# Firm 4 — Okafor-Strand Legal Collective

## Firm Theme: Cooperative Resource Engine

Radical teamwork. This firm shares stats, transfers resources, and operates as a collective unit rather than individual stars.

**Stress interpretation:** The witness's *social pressure* — the feeling of being outnumbered and outmatched by a coordinated group.
**Defensiveness interpretation:** The witness's *distrust* — they sense the lawyers are working in concert and clam up.

---

## Adaeze Okafor — Co-Founder

**Flavor:**
*"We speak with one voice."*

**Description:**
Adaeze is the collective's battery. She generates Charge for herself and redistributes it to allies. Her actions are individually modest but she enables her teammates' Ults to fire rapidly. She also maintains team Credibility. Weakness: low Favor and Stress generation — she doesn't interact with the witness much personally.

### Properties Interaction Summary

* **Primary Axis:** Charge (shared)
* **Secondary Axis:** Credibility
* **Vulnerabilities:** Low Favor/Stress generation; weak witness interaction
* **Counterplay:** Focus the witness axis (Favor/Stress) since she can't contest it; rush her down before the engine gets going

### Design Roles

* Generator (Charge distribution)
* Support (Credibility maintenance)

---

### Triggers

**Name:** Collective Will
**Event:** AfterAction
**Condition:** Any allied lawyer gained Charge this action.
**Effect:** +1 Charge to Adaeze.

**Name:** Solidarity
**Event:** StartTurn
**Condition:** An allied lawyer is Compromised.
**Effect:** +1 Credibility to that ally, +1 Charge to Adaeze.

---

### Actions

**Name:** Coordinate
**Flavor:** *"We're all on the same page."*
**Initiative Cost:** 0
**Description:**
1. +2 Charge to self.
2. +1 Charge to target allied lawyer.

**Name:** Bolster
**Flavor:** *"Counsel's record is impeccable."*
**Initiative Cost:** 0
**Description:**
1. +2 Credibility to target allied lawyer.
2. +1 Leniency.
3. +1 Charge.

**Name:** Present United Front
**Flavor:** *"We stand together on this matter."*
**Initiative Cost:** 1
**Description:**
1. +1 Favor.
2. +1 Stress to Witness.
3. +1 Charge to each allied lawyer who has not yet acted this round.

---

### Ult

**Name:** Collective Examination
**Flavor:** *"The full weight of this team demands an answer."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + number of allied lawyers with Charge ≥ 3 × 3).
2. −2 Charge to each ally.

---

### Compromised

* *Collective Will* trigger disabled. Charge distribution engine stalls.

---
---

## Henrik Strand — Co-Founder

**Flavor:**
*"Each piece, in its place."*

**Description:**
Henrik is the collective's architect. He precisely arranges initiative order so the team acts in optimal sequence, and his passive rewards that sequencing. He generates Favor as a byproduct of good coordination. Weakness: if opponents disrupt initiative ordering, Henrik's scaling falls apart.

### Properties Interaction Summary

* **Primary Axis:** Initiative
* **Secondary Axis:** Favor
* **Vulnerabilities:** Initiative disruption collapses his scaling
* **Counterplay:** Push his teammates out of sequence; deny the adjacency his trigger requires

### Design Roles

* Controller (Initiative arrangement)
* Scaler (Passive growth from sequencing)

---

### Triggers

**Name:** Clockwork
**Event:** AfterAction
**Condition:** An allied lawyer acted immediately before Henrik in initiative order this round.
**Effect:** +1 Favor, +1 Charge to Henrik, +1 Charge to that ally.

---

### Actions

**Name:** Sequence
**Flavor:** *"If we proceed in order..."*
**Initiative Cost:** 0
**Description:**
1. Push target allied lawyer forward −1 on initiative.
2. Push self back +1 on initiative.
3. +1 Charge.
4. +1 Favor.

**Name:** Stall
**Flavor:** *"The court's time is valuable — let's not waste it on counsel's theatrics."*
**Initiative Cost:** 1
**Description:**
1. Push target opposing lawyer back +2 on initiative.
2. +1 Leniency.
3. +1 Stress to Witness.

**Name:** Tempo Shift
**Flavor:** *"We'd like to adjust our approach, Your Honor."*
**Initiative Cost:** 0
**Description:**
1. Push all allied lawyers forward −1 on initiative.
2. +1 Charge.
3. −1 Leniency.

---

### Ult

**Name:** Synchronized Cross
**Flavor:** *"Now — together."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (3 + team Favor).
2. Push all allied lawyers forward −1 on initiative.

---

### Compromised

* Cannot manipulate allied initiative positions. Can only slow opponents.

---
---

## Nkechi Okafor — Associate

**Flavor:**
*"You were saying?"*

**Description:**
Nkechi is the collective's warm face. She builds enormous Favor through genuine rapport, then converts it into devastating Ult damage. She also converts Favor into Stress at key moments — the witness trusted her, which makes betrayal cut deeper. Weakness: she needs time and Favor buildup; she's weak in the early rounds and vulnerable to Credibility attacks.

### Properties Interaction Summary

* **Primary Axis:** Favor
* **Secondary Axis:** Stress
* **Vulnerabilities:** Slow buildup; weak early rounds; Credibility attacks
* **Counterplay:** Rush her before Favor accumulates; attack Credibility to weaken her warmth

### Design Roles

* Breaker (Favor-to-Stress conversion)
* Eroder (Favor-scaling Ult)

---

### Triggers

**Name:** Trust Betrayed
**Event:** AfterAction
**Condition:** Nkechi's action reduced team Favor by 2 or more.
**Effect:** +2 Stress to Witness, +1 Charge.

---

### Actions

**Name:** Connect
**Flavor:** *"I'm listening."*
**Initiative Cost:** 0
**Description:**
1. +2 Favor.
2. +1 Charge.
3. If Witness Defensiveness ≤ 2: −1 additional Defensiveness.

**Name:** Press Gently
**Flavor:** *"I know this is hard. But it matters."*
**Initiative Cost:** 1
**Description:**
1. +1 Stress to Witness.
2. +1 Favor.
3. +1 Charge.
4. −1 Defensiveness on Witness.

**Name:** Cash In
**Flavor:** *"You said you trusted me. Prove it."*
**Initiative Cost:** 0
**Description:**
1. −3 Favor.
2. +3 Stress to Witness.
3. +2 Charge.

---

### Ult

**Name:** Honest Answer
**Flavor:** *"Just tell me the truth."*
**Initiative Cost:** 1
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + team Favor + Witness Stress ÷ 2, rounded down).
2. −1 Stress on Witness.

---

### Compromised

* Favor gained by Nkechi's actions is halved (round down).

---
---

## Yuki Tanabe — Of Counsel

**Flavor:**
*"Allow me to handle that objection."*

**Description:**
Yuki is the collective's shield. She absorbs Credibility attacks aimed at teammates, denies opposing tempo plays, and maintains Leniency so the Judge doesn't punish the collective's coordination tactics. She generates Charge very slowly but she's nearly impossible to Compromise. Weakness: zero offensive output — she can't close out a hearing.

### Properties Interaction Summary

* **Primary Axis:** Credibility
* **Secondary Axis:** Leniency
* **Vulnerabilities:** Zero offensive output; cannot close hearings
* **Counterplay:** Ignore her entirely and race to Secrecy; she can't stop direct Ult pressure

### Design Roles

* Denial (Credibility absorption)
* Support (Leniency maintenance)

---

### Triggers

**Name:** Objection Shield
**Event:** AfterAction
**Condition:** An allied lawyer lost Credibility this action.
**Effect:** Yuki loses −1 Credibility instead (transferred). That ally regains +1 Credibility.

**Name:** Procedural Cover
**Event:** StartTurn
**Condition:** Team Leniency ≤ 1.
**Effect:** +2 Leniency.

---

### Actions

**Name:** Deflect
**Flavor:** *"I believe that remark was directed at me."*
**Initiative Cost:** 0
**Description:**
1. +1 Credibility to target allied lawyer.
2. +1 Credibility to self.
3. +1 Charge.

**Name:** Procedural Shelter
**Flavor:** *"Your Honor, a point of procedure."*
**Initiative Cost:** 1
**Description:**
1. +2 Leniency.
2. Push target opposing lawyer back +1 on initiative.
3. +1 Charge.

**Name:** Absorb Blow
**Flavor:** *"I can take it."*
**Initiative Cost:** 0
**Description:**
1. +3 Credibility to self.
2. +1 Charge.

---

### Ult

**Name:** Protective Summation
**Flavor:** *"This team's integrity is beyond question."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + team Leniency + Yuki's Credibility ÷ 2, rounded down).
2. +1 Credibility to all allies.

---

### Compromised

* *Objection Shield* trigger disabled.

