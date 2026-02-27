# Firm 2 — Valdez, Kaine & Sun

## Firm Theme: Psychological Pressure Specialists

Reading and exploiting the witness psychologically. This firm wants high Stress and low Defensiveness simultaneously.

**Stress interpretation:** The witness's *emotional overwhelm* — confusion, guilt, anxiety.
**Defensiveness interpretation:** The witness's *psychological guardedness* — how aware they are of being manipulated.

---

## Elena Valdez — Named Partner

**Flavor:**
*"Take your time. There's no wrong answer."*

**Description:**
Elena is disarmingly warm. She builds Favor while quietly ratcheting up Stress — the witness trusts her but feels increasingly pressured. Her Ult is a massive Stress-scaling spike. The catch: she needs low Defensiveness to land her best actions, so she needs teammates to soften the witness first. Vulnerable to Leniency denial — the Judge sees through her eventually.

### Properties Interaction Summary

* **Primary Axis:** Stress
* **Secondary Axis:** Favor
* **Vulnerabilities:** High Defensiveness blocks her best action; Leniency denial
* **Counterplay:** Keep Defensiveness high; attack her Credibility to disable her warmth trigger

### Design Roles

* Breaker (Stress-based Ult spike)
* Eroder (Gradual Stress buildup)

---

### Triggers

**Name:** False Comfort
**Event:** AfterAction
**Condition:** This action increased team Favor.
**Effect:** +1 Stress to Witness.

**Name:** Reading the Room
**Event:** StartTurn
**Condition:** Witness Stress ≥ 4.
**Effect:** +2 Charge.

---

### Actions

**Name:** Empathize
**Flavor:** *"I understand. Truly."*
**Initiative Cost:** 0
**Description:**
1. +2 Favor.
2. If Witness Defensiveness ≤ 2: +1 Charge.

**Name:** Leading Question
**Flavor:** *"So what you're really saying is..."*
**Initiative Cost:** 1
**Description:**
1. +2 Stress to Witness.
2. −1 Defensiveness on Witness.
3. Requires: Witness Defensiveness ≤ 3.

**Name:** Pivot
**Flavor:** *"Actually — let's go back to what you said earlier."*
**Initiative Cost:** 0
**Description:**
1. −1 Favor.
2. +3 Stress to Witness.
3. +1 Charge.

---

### Ult

**Name:** Moment of Truth
**Flavor:** *"You know what happened. Just say it."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (2 + Witness Stress).
2. −2 Stress on Witness.
3. −1 Favor.

---

### Compromised

* *False Comfort* trigger is disabled. Favor gains no longer generate Stress.

---
---

## Marcus Kaine — Named Partner

**Flavor:**
*"Interesting. That's not what you said on the 14th."*

**Description:**
Marcus is the firm's knife. He targets opposing lawyers' Credibility to remove their ability to function, and converts their crumbling authority into Witness Stress — the witness sees the courtroom unraveling and panics. His Ult doesn't scale as high as Elena's but he doesn't need setup. Vulnerable to initiative manipulation — he's slow and gets pushed around.

### Properties Interaction Summary

* **Primary Axis:** Credibility (opponent's)
* **Secondary Axis:** Stress
* **Vulnerabilities:** Slow; easily pushed on initiative
* **Counterplay:** Initiative manipulation; keep distance so he can't act before recovery

### Design Roles

* Sniper (Credibility pressure)
* Breaker (Stress via courtroom chaos)

---

### Triggers

**Name:** Exposed Weakness
**Event:** AfterAction
**Condition:** An opposing lawyer's Credibility dropped to ≤ 1 this action.
**Effect:** +2 Stress to Witness, +1 Charge to Marcus.

---

### Actions

**Name:** Impeach
**Flavor:** *"Your Honor, counsel has misrepresented..."*
**Initiative Cost:** 1
**Description:**
1. −2 Credibility to target opposing lawyer.
2. +1 Charge.

**Name:** Discredit
**Flavor:** *"Is that really the best they could send?"*
**Initiative Cost:** 0
**Description:**
1. −1 Credibility to target opposing lawyer.
2. +1 Stress to Witness.
3. If target is Speaker: −1 additional Credibility.

**Name:** Undermine
**Flavor:** *"I question opposing counsel's standing."*
**Initiative Cost:** 0
**Description:**
1. −1 Leniency to opposing team.
2. +1 Charge.
3. +1 Stress to Witness.

---

### Ult

**Name:** Collapse of Testimony
**Flavor:** *"The witness can see — no one is coming to save them."*
**Initiative Cost:** 2
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (4 + number of Compromised opposing lawyers × 3).
2. +1 Credibility to self.

---

### Compromised

* Cannot target opposing lawyers with Credibility-reducing effects.

### Speaker Interaction

* **Discredit** has bonus effect when target is opposing Speaker.

---
---

## Iris Sun — Junior Partner

**Flavor:**
*"Let me rephrase that for you."*

**Description:**
Iris is the setup specialist. She systematically lowers the witness's Defensiveness so Elena and Marcus can operate at full power. She reframes testimony, builds rapport, and ensures the witness's psychological guard is down. Her personal Charge generation is slow but her Ult is efficient. Weakness: almost no Credibility pressure — she can't fight back directly against snipers.

### Properties Interaction Summary

* **Primary Axis:** Defensiveness
* **Secondary Axis:** Favor
* **Vulnerabilities:** No Credibility pressure; cannot fight snipers
* **Counterplay:** Attack her Credibility; ignore Defensiveness and race to Secrecy via Leniency-based Ults

### Design Roles

* Controller (Defensiveness erosion)
* Support (Credibility for allies)

---

### Triggers

**Name:** Reframe
**Event:** AfterAction
**Condition:** Witness Defensiveness decreased this action.
**Effect:** +1 Favor to own team.

**Name:** Soft Landing
**Event:** StartTurn
**Condition:** Iris is Speaker.
**Effect:** −1 Defensiveness on Witness.

---

### Actions

**Name:** Build Rapport
**Flavor:** *"We're all just trying to get to the truth."*
**Initiative Cost:** 0
**Description:**
1. +1 Favor.
2. −1 Defensiveness on Witness.
3. +1 Charge.

**Name:** Clarify
**Flavor:** *"Could you walk us through that one more time?"*
**Initiative Cost:** 0
**Description:**
1. −2 Defensiveness on Witness.
2. If Witness Defensiveness was ≥ 4 before this effect: +1 Charge.

**Name:** Shield Counsel
**Flavor:** *"I'd like to address the court on a procedural matter."*
**Initiative Cost:** 1
**Description:**
1. +2 Credibility to target allied lawyer.
2. +1 Leniency.

---

### Ult

**Name:** Gentle Unraveling
**Flavor:** *"You don't have to protect anyone anymore."*
**Initiative Cost:** 1
**Charge Requirement:** Full
**Description:**
1. Reduce Secrecy of target Statement by (3 + team Favor − Witness Defensiveness).

---

### Compromised

* *Soft Landing* trigger disabled.
* Defensiveness reduction from actions is halved (round down).

### Speaker Interaction

* *Soft Landing* requires Iris to be Speaker.

