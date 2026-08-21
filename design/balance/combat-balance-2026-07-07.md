# Combat Balance — Player / Enemy Ranks / Boss (Levels 1–20)

**Date:** 2026-07-07
**Domain:** Combat + Progression
**Source data:** `stat_system.xlsx` (Per-Entity + Base Formula + Multiplier sheets),
`design/gdd/character-system.md`, `design/gdd/game-concept.md`,
`Assets/Script/StatSystem/DerivedStatFormula.cs`, `StatsSO.cs`

---

## 1. Goals

- Player progresses from **level 1 to a cap of level 20** within one dungeon run.
- Enemies come in **3 ranks**: `creep` → `elite` → `champion`, plus **1 boss**.
- The 5 archetypes from the spreadsheet stay: **Trash Melee, Ranged Caster, Tank, Fast Swarm, Assassin**.
- Time-to-kill (TTK) stays inside a fixed band at every level, so combat *feels* the
  same at L1 and L20 even though the raw numbers inflate.

### TTK design band (the definition of "balanced" here)

| Fight | Target (hits) | Rationale |
|-------|---------------|-----------|
| Player kills creep | 1–2 | trash, dies fast, comes in numbers |
| Player kills elite | 3–5 | a real obstacle |
| Player kills champion | 7–11 | mini-boss of the room |
| Player kills boss | ~30–35 (≈30 s) | multi-phase set-piece |
| Creep kills player | ~10–14 hits | low per-hit threat; danger = swarm |
| Assassin/Ranged kills player | ~9–12 hits | duelists punish mistakes |
| Boss kills player | ~5–7 hits | must respect it |

---

## 2. The three axes of the model

Every enemy value is the product of three independent axes. Keep them separate so each
can be tuned without touching the others.

```
finalStat = archetypeBase(floorLevel) × rankMultiplier
```

- **Archetype (shape)** — which stat is high/low. Identity of the species.
- **Rank (creep/elite/champion)** — a flat power multiplier tier on top of the archetype.
- **Floor level (vertical growth)** — the `perLevel` term. Enemy `StatsSO.Level = current floor`,
  so deeper enemies are tuned to a higher-level player.

The player uses the same `DerivedStatFormula` shape:

```
DerivedStat = baseConstant + level × perLevel + Σ(primaryStat × coefficient)
```

- `perLevel` supplies **guaranteed vertical growth** (resource/flat stats only).
- Primary-stat allocation supplies **horizontal build identity** (player picks +3 pts/level).

---

## 3. Armor must be applied (blocking issue)

The spreadsheet defines `Defense` / `MagicDefense`, and every archetype has values for them,
but `character-system.md` confirms damage is currently `finalDamage = rawDamage` — **armor is
ignored**. That makes the whole Defense column dead weight and is the single biggest reason
Tank feels like a passive damage sponge rather than a durable threat.

**Adopt multiplicative armor** (standard ARPG, no negative-damage edge cases):

```
finalDamage = rawDamage × 100 / (100 + Defense)
```

All TTK numbers in this document assume this formula is live.

---

## 4. Player reference curve (levels 1–20)

Reference build spreads the +3 points/level evenly across STR/DEX/VIT (a "combat" build).
`perLevel`: MaxHP +8/level, AttackDmg +1.2/level, Defense +0.6/level.

| L | MaxHP | AtkDmg | AS | DPS | DEF |
|---|------|--------|-----|-----|-----|
| 1 | 120 | 24.0 | 1.00 | 24.0 | 10 |
| 5 | 176 | 36.0 | 1.04 | 37.4 | 16 |
| 10 | 246 | 51.0 | 1.09 | 55.6 | 23 |
| 15 | 316 | 66.0 | 1.14 | 75.2 | 30 |
| 20 | 386 | 81.0 | 1.19 | 96.4 | 37 |

Formulas (feed straight into `DerivedStatFormula`):

```
MaxHP          = 60  + level×8   + VIT×6
AttackDamage   = 6   + level×1.2 + STR×1.4 + DEX×0.4
Defense        = 2   + level×0.6 + VIT×0.8
AttackSpeed    = clamp(0.9 + DEX×0.01, 0.3, 2.0)
```

> Percentage stats (CritChance, Evasion, LifeSteal, AttackSpeed, CritDamage) keep
> `perLevel = 0` — never grant flat % per level or builds trivialise by leveling.

---

## 5. Enemy archetype base (creep rank, at floor level L)

```
HP  = (hp0  + hpPerLevel  × (L−1)) × rankHP
DMG = (dmg0 + dmgPerLevel × (L−1)) × rankDMG
```

| Archetype | hp0 | hp/lvl | dmg0 | dmg/lvl | AS | Role |
|-----------|-----|--------|------|---------|-----|------|
| Trash Melee | 22 | 6.0 | 10 | 1.6 | 0.9 | Baseline bruiser |
| Ranged Caster | 14 | 3.5 | 11 | 1.7 | 0.7 | Poke from distance |
| Tank | 45 | 12.0 | 8 | 1.2 | 0.6 | Wall, low threat, high DEF |
| Fast Swarm | 10 | 2.5 | 7 | 1.1 | 1.4 | Cheap, many, fast |
| Assassin | 14 | 3.5 | 14 | 2.0 | 1.3 | Burst duelist |

Tank additionally gets **+50% Defense**. Enemy base Defense `= (1 + 0.3×(L−1)) × archetypeDefBonus`.

---

## 6. Rank multipliers (creep / elite / champion)

Applied on top of the archetype base. One row = one rank; reused by all 5 archetypes.

| Rank | HP × | DMG × | DEF × | XP reward × | Notes |
|------|------|-------|-------|-------------|-------|
| **creep** | 1.0 | 1.0 | 1.0 | 1.0 | fills rooms |
| **elite** | 2.4 | 1.5 | 1.0 | 3.5 | 1–2 per room |
| **champion** | 5.5 | 2.1 | 1.5 | 9.0 | room centrepiece; add 1 mechanic (enrage / summon / shield) |

Verified player-kills-enemy TTK (Trash Melee archetype, hits):

| L | creep | elite | champion |
|---|-------|-------|----------|
| 1 | 0.9 | 2.2 | 5.1 |
| 10 | 1.5 | 3.7 | 8.7 |
| 20 | 1.8 | 4.3 | 10.2 |

All inside band across the whole 1–20 range.

---

## 7. Boss

Own formula, not a rank multiplier (it needs an independent tuning handle):

```
Boss MaxHP   = 700 + level×70
Boss AtkDmg  = 26  + level×2.6
Boss AS      = 0.8
Boss Defense = 8   + level×1.0
```

| L | Boss HP | Player kills in | Boss kills player in |
|---|---------|-----------------|----------------------|
| 1 | 700 | 32 hits / 32 s | 5.1 hits |
| 10 | 1330 | 31 hits / 28 s | 6.1 hits |
| 20 | 2030 | 32 hits / 27 s | 7.0 hits |

Boss is intended at the end of a floor tier; split the HP across 2–3 phases so the ~30 s
fight has beats. Bump `70/level` up if you want a longer set-piece.

---

## 8. Progression / XP curve

```
xpToNext(L) = round(40 + 12 × L^1.6)     # L = current level, 1..19
```

| L→L+1 | XP | Cumulative |
|-------|-----|-----------|
| 1→2 | 52 | 52 |
| 5→6 | 198 | ~660 |
| 10→11 | 518 | ~3,050 |
| 15→16 | 954 | ~6,600 |
| 19→20 | 1,374 | **11,186 total** |

**Reward side (tune to run length).** To reach L20 near the end of a full run, rewards must
cover ~11,186 XP over the run. Suggested rewards, `× floor level`:

```
xpReward = baseReward × rankXpMult × floorLevel
  creep base 4 · elite ×3.5 · champion ×9 · boss flat 400×floor
  room-clear bonus: 25 × floorLevel
```

> **Pacing knob:** cap is level 20, but *when* the player hits it depends on rooms-per-run.
> Set `baseReward` so the reference player lands ~L18–20 at the boss. If runs are short,
> raise `baseReward`; if L20 arrives too early (trivialises late rooms), lower it.

---

## 9. Tuning knobs summary

| Knob | Where | Effect |
|------|-------|--------|
| `perLevel` (HP/Dmg/Def) | player `DerivedStatFormula` | vertical power ramp |
| primary points/level | level-up logic | build identity spread |
| `hp0 / hpPerLevel` per archetype | enemy `StatsSO` formulas | species durability |
| rank multipliers | rank table (shared) | creep/elite/champion gap |
| boss HP/dmg per level | boss `StatsSO` | boss fight length & threat |
| `xpToNext` exponent | progression config | levels-per-run pacing |
| armor constant `100` | damage formula | how much DEF matters |

---

## 10. Acceptance criteria

- [ ] Damage applies multiplicative armor (`raw × 100/(100+DEF)`); DEF is no longer dead.
- [ ] Player L1→L20 curve matches §4 within ±5%.
- [ ] creep/elite/champion for every archetype produce §6 TTK band at L1, L10, L20.
- [ ] Boss dies in 28–35 s and kills a standing player in 5–7 hits at matching level.
- [ ] A reference run reaches L18–20 by the boss room.
- [ ] Percentage stats have `perLevel = 0`.

---

## Appendix — implementation status (appended 2026-08-21 by documentation audit)

This appendix records what the code does today. **It does not change any target above** — the
design intent in §1–§10 is unmodified and still owns these numbers.

| Claim in this doc | Verified against source | Status |
|---|---|---|
| §3 "damage is currently `finalDamage = rawDamage` — armor is ignored" | `NegativeReciver.cs:9` is `currentHealth -= amoutDamage`; no `Defense` term anywhere in either receiver | ✅ **Still true.** The blocking issue this doc opens with is still open, 6 weeks on |
| §5 five archetypes | `Assets/SO/Stat/Enemy/` holds `TrashMelee`, `RangedCaster`, `Tank`, `FastSwarm`, `Assasin` (+ `Boss/BossStats`) | ✅ Assets match |
| §2/§4 `DerivedStat = baseConstant + level×perLevel + Σ(primary×coefficient)` | `DerivedStatFormula.Evaluate()` implements exactly this shape | ✅ Formula shape matches |
| §6 rank tiers `creep` / `elite` / `champion` | **Zero occurrences in `Assets/Script/`.** No rank field, no multiplier table, no enum | ⚠️ **Unbuilt.** The whole of §6 is aspirational |
| §2 "Enemy `StatsSO.Level` = current floor" | Nothing anywhere assigns `.Level =` at runtime | ⚠️ **Unbuilt.** Enemy level never tracks floor depth |
| Source cited as `stat_system.xlsx` | `design/gdd/stat-system.md` names **`stat_system_formula_reference.xlsx`** as the single source of truth. Both files exist in `ToolExcel/` | ⚠️ **Doc-vs-doc conflict** — unclear which the coefficients should be read from |

> ⚠️ **Naming collision worth knowing about.** This doc's three-tier *power* axis
> (creep / elite / champion) is unrelated to `RarityTier` (Common / Rare / Epic / Legendary) in
> `RoomModel.cs:110`, which is a *spawn-chance* axis governing how often an enemy is picked.
> Neither is derived from the other, and only `RarityTier` exists in code. Anyone implementing
> §6 must add a genuinely separate axis rather than reusing `RarityTier`.

**Not verified:** every concrete coefficient, `perLevel` value and TTK figure traces to
`ToolExcel/*.xlsx`, which is binary and unreadable in the audit environment. All such numbers
remain `[UNVERIFIED]` — they were not checked, and are not asserted to be either right or wrong.
