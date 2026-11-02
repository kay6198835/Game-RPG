# Archer / Mage Abilities — Task Log (2026-11-02)

> Branch `sprint-21`, HEAD at start `5f70bfd9`. Single log for the two-phase task
> "design (GDD) then create assets for the 4 Archer and 4 Mage abilities".
> **Current state: Phase 1 complete, waiting for owner approval. Phase 2 not started.**
>
> File rules for this task: create only; never modify, delete, rename or move an existing file;
> no new `.cs`; no push. Local commit on `sprint-21` allowed by the owner (chat, 2026-11-02),
> which overrides "no commit" in the task brief.

---

## 1. Summary

**Pending & warnings first.** Ability status (READY / PARTIAL / PENDING) is decided in Phase 2;
the column below is the Phase 1 design status only.

| Ability | Slot | Class | Phase 2 status | One-line reason |
|---|---|---|---|---|
| Hawk Eye | Primary | Archer | not assessed (Phase 2) | — |
| Kick-back Trap | Secondary | Archer | not assessed (Phase 2) | — |
| Volley | Utility | Archer | not assessed (Phase 2) | — |
| Rain Arrows | Ultimate | Archer | not assessed (Phase 2) | — |
| Fireball | Primary | Mage | not assessed (Phase 2) | — |
| Chain Lightning | Secondary | Mage | not assessed (Phase 2) | — |
| Frost Nova | Utility | Mage | not assessed (Phase 2) | — |
| Meteor | Ultimate | Mage | not assessed (Phase 2) | — |

## 2. Pending & Warnings

Phase 2 fills the per-ability pending entries. Warnings already known from Phase 1:

1. **Ability shells already exist.** The owner created named, empty ability assets with 8-direction
   animator overrides, and `Archer.asset` / `Mage.asset` already bind them:
   - Archer: `Hawk Eye` (Primary), `Kick-back Trap` (Secondary), `Volley` (Utility),
     `Rain Arrows` (Ultimate, not empty — it points at the **Paladin** Consecrate effect
     `76d5856358712314aa01cab60fcb824b` and the Paladin Consecrate override controller), plus an
     unbound `Piercing Shot`.
   - Mage: `Fireball`, `Chain Lightning`, `Frost Nova`, `Meteor` — all `Effects: []`,
     `Costs: []`, `Cooldown: 1`, empty `id` / `DisplayName`.
   - `Assets/SO/Skill/Archer/Ability/Rain Arrow/Effect/Archer Spawn Rain Arrows Effect.asset` is a
     copy of the Paladin Consecrate summon effect (RuneCircle + Lightning prefabs), referenced by
     nothing.
   - Because existing files may not be modified, Phase 2 cannot fill these shells. New assets need
     different names (proposal: suffix ` (GDD)`), and the owner rebinds `abilityBindings` by hand.
     **Owner decision needed (Q-L1 below).**
2. **Ability clips are 2.3–5.0 s long** (Hawk Eye, Volley, Piercing Shot 5.0 s; Frost Nova 4.3 s;
   Chain Lightning 4.0 s; Fireball 2.7 s; Meteor 2.27 s; Kick-back Trap 0.93 s), with the
   `AnimationOnAction` event at 1.5–3.2 s into State 1. The GDD targets 0.25–0.45 s to payload.
   Clip timing drives the phase timeline, so every ability will feel several times slower than
   designed until the clips are re-timed. GDD Q7.
3. **Mage has no weapon** (`Mage.asset` `<Weapon>k__BackingField: {fileID: 0}`). The GDD makes
   Fireball the free basic attack (GDD Q2). Note: `WeaponHolderBase.EquipDefaultWeapon()` with a
   null weapon is the BUG-107 item 2 path.
4. **Archer basic arrow ignores `PhysicalDamage` and crit** — `RangeWeapon.cs:75`
   `receiver.TakeDamage(currentStage.attackDamage, hitPos)`. The balance model assumes this
   (GDD Q4).
5. **Class stat profiles are not differentiated.** `ArcherStats.asset` equals `BaseStats.asset`
   (all primaries 10, level 10). `MageStats.asset` differs only by leaked modifiers (STR 12,
   `PhysicalDamage` 56.4 — BUG-115 pattern). `PaladinStats.asset` has INT 100 / Mana 1070
   (test or leaked value). GDD Q8.
6. **Paladin baseline numbers are placeholders**: every Paladin ability has `Cooldown: 1`;
   `Gain For Time Effect` (Avatar of Light) is `MagicDamage +100000 Flat` for 20 s; `Avatar of Light`
   costs 40 HP + 50 Mana. Normalisation therefore uses a Paladin reference (see §3).
7. **Mitigation makes the authored Boss nearly immune to small hits** — flat
   `amount − Defense` (`EntityNegativeReciver.DamageCalculate`), Boss Defense 112.5. GDD Q1.

---

## 3. Phase 1 summary

- **GDD:** [`design/gdd/class-abilities-archer-mage.md`](../../design/gdd/class-abilities-archer-mage.md)
  (all 8 required sections + Summary, Visual/Audio, Game Feel, UI, Cross-References, Open Questions).
- **Process notes / deviations from the skills:**
  - `/design-system` was run non-interactively: the owner asked to review the whole GDD at the
    Phase 1 stop, so the per-section approval widgets were waived. `review_mode: lean`, so
    `creative-director`, `art-director` and `qa-lead` were not consulted (noted in the GDD).
  - Skill steps that write other files were **skipped because of the create-only file rule**:
    `production/session-state/active.md`, `design/registry/entities.yaml` (registry candidates
    listed below), `design/gdd/systems-index.md` (new row needed), and the dependency back-links in
    `stat-system.md`, `character-system.md`, `animation-system.md`.
  - `/balance-check`: result recorded here instead of `design/balance/…` (no extra files allowed).
  - `/design-review`: run by an independent read-only agent (the skill forbids reviewing in the
    authoring context). Result below.
- **Inputs read:** Paladin ability + effect assets (11), `Assets/SO/Player/{Paladin,Archer,Mage}/*.asset`,
  `Assets/SO/Stat/Player/*.asset`, `Assets/SO/Stat/Enemy/*.asset` + `Boss/BossStats.asset`,
  `StatType.cs`, `StatModifier.cs`, `WeaponHolderBase.CalculateCurrentDamage()`,
  `EntityNegativeReciver.cs`, `NegativeReceiverBase.cs`, `SpawnProjectileEffect.cs`,
  `SpawnSummonEffect.cs`, `RangeWeapon.cs`, weapon stage assets, ability clip timings,
  `design/gdd/{game-concept,systems-index,stat-system,skill-ability-system,attack-speed-system}.md`,
  `design/balance/combat-balance-2026-07-07.md`, `design/registry/entities.yaml` (template only — no
  live entries).

### Paladin baseline (as authored)

| Ability | ActivationType | Cooldown | Costs | MaxHoldTime | Effect chain | Numbers |
|---|---|---|---|---|---|---|
| Blessed Slash | Hold (1) | 1 | 5 Mana | 0 | Spawn Slash projectile | `baseDamage` 30, SpawnOffset 0.8, Lifetime 8; (recovery effect HP +30 Flat exists, not in the ability's list) |
| Consecrate | Active (0) | 1 | 15 Mana | 0 | Spawn Summon (RuneCircle → Lightning) | `baseDamage` 30 + `MagicDamage` |
| Blessing | Active (0) | 1 | 10 Mana | 0 | Buff/Debuff for duration + Recovery/Reduction (empty) | `PhysicalDamage` +2 PercentAdd, STR +20 Flat, 3 s |
| Avatar of Light | Active (0) | 1 | 40 HP + 50 Mana | 0 | Gain For Time + Recovery (empty) | `MagicDamage` +100000 Flat, 20 s (test value) |

### Ability table (design values)

| Ability | Slot | Fantasy | Activation | Cooldown | Cost |
|---|---|---|---|---|---|
| Hawk Eye | Primary | Charged sniper arrow, stronger the longer you draw and the farther it flies | Hold (max 1.2 s) | 2.0 s | 8 Mana |
| Kick-back Trap | Secondary | Hop back 3.5 u, leave a root-and-knockback trap | Active | 8 s | 15 Mana |
| Volley | Utility | 5-arrow 40° fan; shotgun up close, slow net at range | Active | 6 s | 20 Mana |
| Rain Arrows | Ultimate | 3 s arrow storm on a marked zone, slows | Active, ground target | 35 s | 50 Mana |
| Fireball | Primary | Free exploding fireball with Burn — the Mage's basic attack | Active | 0.6 s | 0 |
| Chain Lightning | Secondary | Auto-target beam jumping to 4 enemies, micro-stun | Active, auto target | 4 s | 22 Mana |
| Frost Nova | Utility | Self-centred freeze ring; Shatter +25% | Active | 10 s | 30 Mana |
| Meteor | Ultimate | Telegraphed impact, stun, burning ground | Active, ground target | 40 s | 80 Mana |

### Balance-check result

**Health summary: CRITICAL ISSUES** under the live flat mitigation (an authored-Boss fight the
Archer cannot realistically finish); **CONCERNS** if the 20% damage floor (GDD Q1) is adopted.

Data sources: the GDD, `Assets/SO/Stat/**`, Paladin ability assets, weapon stage assets,
`design/balance/combat-balance-2026-07-07.md`. Registry: template only, no targets. Method: 60 s
single-target rotation simulation (scratchpad `sim2.ps1`, not committed), assumptions in GDD › Formulas.

| Class | L1 DPS | Δ vs Paladin mid 61.5 | L10 DPS | Δ vs Paladin mid 83.1 | Burst 5 s L1 / L10 | Dmg/Mana L1 / L10 |
|---|---|---|---|---|---|---|
| PaladinBasic | 64.3 | — | 90.6 | — | 351 / 494 | — |
| PaladinRef | 58.8 | — | 75.6 | — | 229 / 298 | 25.2 / 26.7 |
| Archer | 57.6 | −6% | 85.0 | +2% | 392 / 569 | 25.2 / 30.4 |
| Mage | 54.9 | −11% | 92.2 | +11% | 442 / 691 | 24.9 / 13.2 |

Buff uptime: not applicable — neither kit has a self-buff (Paladin Blessing as authored: 3 s per
1 s cooldown, i.e. permanent if mana allows). Cooldown budget: Archer 8.8 Mana/s and Mage 9.7 Mana/s
demand vs 1.88–4.76 regen — both mana-gated by design.

| Priority | Issue | Suggested fix | Impact |
|---|---|---|---|
| P1 | Flat Defense zeroes small hits: Archer vs Boss TTK 2384 s (L1) / 730 s (L10); Archer vs Tank 1.8–2.5× Paladin | Adopt the 20% floor or multiplicative armour (GDD Q1) | All classes; Paladin too |
| P2 | Mage scaling steepest (`MagicDamage` ×3.7 L1→L10); −11% → +11% | Keep Mage `Scale` ≤ 0.5; re-check at L15/L20 | Mage |
| P3 | Ultimate opener burst (Mage +26–40%, Archer +12–15% over PaladinBasic) | Accept as identity; watch room-opening packs | Pack rooms |
| P4 | Archer kiting dominance (MoveSpeed 3.2 > TrashMelee 2.79, Tank 1.84) | Room composition (FastSwarm, RangedCaster) as counter | Archer |
| P5 | Paladin abilities are DPS-negative as authored | Paladin pass (out of scope) | Paladin |
| P6 | Frost Nova control uptime 35%; risk with future CDR | Cap CDR on Frost Nova at 30% | Mage |

### Design-review result

Independent read-only reviewer (lean mode). **Verdict: NEEDS REVISION** — 7 blocking issues.
Revised in the GDD the same day; status after revision:

| # | Blocking issue | Resolution |
|---|---|---|
| 1 | Frost Nova → Meteor combo had zero margin (Freeze 1.5 s ≈ impact time) | **Fixed:** Freeze 2.0 s (Tank 1.0 s), ≈ 0.45 s margin; safe range 1.6–2.5 s; AC added |
| 2 | Burn ticks would trigger Shatter and break Freeze | **Fixed:** Shatter = direct hits only; DoT never shatters; AC added |
| 3 | Cost / cooldown timing contradicted itself (Rule 5 vs state table vs Fireball cycle) | **Fixed:** one model — cooldown starts when Exit ends (what the simulation used); Hold exceptions for Hawk Eye in Rules 4 and 6 and in the state table |
| 4 | Defense rule unresolved (Q1); also Q2, Q3, Q5 | **Owner decision** — recommendations added to the GDD (20% floor, free Fireball, merge Piercing Shot into Hawk Eye, no ability crit) |
| 5 | Kick-back Trap never triggered on an enemy already standing on it | **Fixed:** triggers on any enemy inside at arm time or later; zero-distance knockback along the hop direction |
| 6 | Hawk Eye distance undefined; AC 95.2 and max 152.5 wrong | **Fixed:** distance from the spawn point; AC 95.0 at 7.8 u from caster; max 132.9 |
| 7 | Balance AC not testable by hand | **Fixed:** re-scoped to a simulation acceptance with a pinned Paladin baseline (61.5 / 83.1 DPS); per-hit formula smoke in Play Mode |

Non-blocking fixed: Output Range now covers direct hits (21.4–217.5), Kick-back Trap cooldown
rationale corrected, Archer mana demand 8.6 (was 8.8), Chain Lightning target chosen at press,
status table now carries the Tank/Boss CC rules, blanket `[PLANNED]` tag. Not changed: dependency
back-links (create-only rule), burning-ground knockback interaction (accepted as intended),
input-buffer wording. Burn in the simulation refreshes and does not stack, as the reviewer asked.
A re-review was not run; recommended after the owner answers Q1–Q8.

### Decisions confirmed by the owner

_None yet — Phase 1 stop._

### Registry candidates (not written — create-only rule)

`ability_raw_damage`, `hawk_eye_raw`, `chain_hit`, `final_damage` (flat + floor variant),
constants: boss CC stagger 0.3 s, Tank CC multiplier 0.5, ability input buffer 0.15 s.

---

## 4. Capability matrix + bug re-check

_Phase 2._

## 5. Behavior → system mapping

_Phase 2._

## 6. Created files

| Path | GUID | Phase |
|---|---|---|
| `design/gdd/class-abilities-archer-mage.md` | n/a (outside `Assets/`) | 1 |
| `production/qa/archer-mage-abilities-log-2026-11-02.md` | n/a | 1 |

## 7. GDD deviations

_Phase 2._

## 8. Manual steps for owner

_Phase 2._ Known already: rebinding `abilityBindings` in `Archer.asset` / `Mage.asset`;
re-timing the ability clips (GDD Q7).

## 9. Open questions

GDD Q1–Q8 (see the GDD › Open Questions), plus:

| # | Question |
|---|---|
| Q-L1 | Phase 2 must not modify the existing empty shells. OK to create new ability assets with a ` (GDD)` suffix next to them and rebind by hand — or will you allow filling the existing shells? |
