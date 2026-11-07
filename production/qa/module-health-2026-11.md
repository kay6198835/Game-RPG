# Module Quality Audit — November 2026

**Run**: 2026-11-03 (autonomous scheduled run — `pm-monthly-module-audit`, Monday 2026-11-02 22:00 slot;
session date 2026-11-03, day-of-month 3 passes the first-Monday guard)
**Window**: `8da09b1a` (2026-10-06, the October delta audit) → `origin/sprint-21` `6c61ccdb` (2026-11-02).
29 non-merge commits touched `Assets/Script/` (sprints 18-21).
**Evidence tier**: STATIC (code + docs read at `6c61ccdb`). Latest compile: `compile-check.sh` COMPILED on
2026-11-02 (daily standup). Latest runtime: the automated QA run `20261101-0221` (RUNTIME, cited where used).

**Autonomous-mode choices made**
- Module set = every standalone GDD that `systems-index.md` lists and does not mark Not Started: the
  8 modules of October plus **`item-system.md`** (Reverse-documented 2026-10-20) — **9** scored modules.
- `class-abilities-archer-mage.md` (new 2026-11-02, Phase 1 design) is not in `systems-index.md`, so it is
  not scored; it is listed under Recommended Actions. Abilities v2 (no GDD) stays an informational row.
- Item status "Reverse-documented" is treated like *In Progress*: drift is informational, not a flag.
- Scored inline, not with one subagent per module, to keep the unattended run cheap (same choice as
  October). Same axes and thresholds as the skill.
- Bug status is taken from the bug files (`production/qa/bugs/`) and the 2026-10-31 triage; inbox notes
  are cited but do not count toward the thresholds (they are not confirmed bugs).
- The owner's folder has uncommitted edits to `StatModifierGroup.cs`, `CharacterData.cs`,
  `EffectReceiverBase.cs` and several stat / effect assets (session git status). They are invisible to this
  audit by design and may already address some findings below.
- No follow-up skill was run. Each flagged row says "Not run — requires owner decision".

## Module Scorecard

| Module | Status (index) | Verdict | Bugs (open, traced) | Tech Debt (open, traced) | Test Evidence | Top Finding |
|--------|----------------|---------|---------------------|--------------------------|---------------|-------------|
| animation-system.md | Designed | **HEALTHY** (watch) | 0 | TD-016 | None (EditMode); QA-auto RUNTIME touches it indirectly | No change under `Character/Player/Animation/` in the window. Watch: NOTE-20261101-1 (RUNTIME, 4 of 4 runs) — all 8 Paladin `Die` clips fire `AnimationOnAction` only, so `PlayerDeathState` never sees `EndRangeTrigger`. That is an animation-event authoring contract the GDD owns; scored under character until triage decides |
| character-system.md | Designed | **CRITICAL** | S1: BUG-087 (partial), BUG-108 (half fixed in code, NOTE-20261031-7), BUG-116. S2: BUG-065, BUG-066+070, BUG-086, BUG-100, BUG-106, BUG-107 (partial), BUG-114, BUG-117. S3: BUG-111, BUG-118, BUG-119 | TD-018, TD-019, TD-037, TD-041 | None (EditMode); QA-auto RUNTIME: 2 FAIL (QA-VIT-07, QA-ENT-04) | Three open S1 bugs plus two RUNTIME S1 notes (NOTE-20261101-1 player death never emits `ON_PLAYER_DEATH`; NOTE-20261101-2 enemy at HP 0 never dies). The window rebuilt vitals into `EffectReceiverBase` / `RegenComponent` / `WalletComponent` (40+ files, mostly "coding" commits) and the GDD still has **0** mentions of `VitalStatsBase` / `EffectReceiverBase` / `ICharacter` |
| weapons-system.md | Designed | **AT RISK** | S2: BUG-095 (partial), BUG-105, BUG-110; BUG-093/094 code-fixed, Play Mode pending. S3: BUG-103 | TD-006 | None | BUG-105 (sword stage damage loads 0) still open after 3 weeks and is now joined by BUG-121 and NOTE-20261103-2: `SnS_Stat.asset`'s two weapon modifiers are lost on load *and*, once the list rename is fixed, would bind to STR because `targetStat` was renamed too |
| skill-ability-system.md (v1) | Designed | **HEALTHY** | 0 | TD-040 | None | Only change: `WeaponSO` `[CreateAssetMenu]` path (`cae75317`) — no behaviour change. v1 remains enemy-only |
| map-system.md | In Progress | **CRITICAL** | S1: BUG-096. S2: BUG-097. S3: BUG-099, Bug #14, #15, #17 | TD-020, TD-022, TD-025, TD-026, TD-027, TD-028, TD-032 | None | BUG-096 unchanged for a month: `LoadRoom()` still has no zero-spawn branch, so Boss / Buff / Rest / Shop rooms never open (the Start room now opens through the Champion, `221d54be`). Window changes were the in-scope `ON_CLEAR_ENEMY` → `ON_OPEN_DOOR` rename and `IsCleared` moved into `OpenDoors()` |
| stat-system.md | Designed | **AT RISK** ⬇ | S2: BUG-113, BUG-114, BUG-115, BUG-121. S3: BUG-119. BUG-063 (S1) accepted/deferred | — | None | Regressed a tier. Two serialized renames in one window with no `[FormerlySerializedAs]`: `StatModifierGroup.authoredModifiers` → `statsModifiers` (BUG-121) and `StatModifier.targetStat` → `targetStatType` (`6e484f0c`, **NOTE-20261103-2**, new). GDD still names `authoredModifiers` and has no `VitalModifier` / `StatPointSession` (**NOTE-20261103-3**). ADR-0001 still `Proposed` (4 months) |
| enemy-spawn-system.md | **Approved** | **AT RISK** | S4: BUG-098 (GDD now has 1 `RarityTierEnemy` mention; ADR-0003 0). BUG-ES-2 | TD-030, TD-031, TD-039 (accepted) | None | Only mechanical changes (`IObjecPoolService` → `IObjectPoolService`, one log line removed in `MapModel`). Recommendation 3 from October (`/consistency-check`) not done; `retry > 4` fallback and unread `overflowPercent` still open; ADR-0002 still `Proposed` |
| attack-speed-system.md | Designed | **HEALTHY** | 0 | — | None | Still not implemented: `StatType.AttackSpeed` is read only by `GameConstants.cs:173` (display name). Nothing to drift |
| item-system.md | Reverse-documented | **AT RISK** | S2: BUG-120, BUG-121 (item effect assets load empty). Note: NOTE-20261031-10 (S3, coin effect cannot be authored) | TD-054 | None | Picked-up stat items do nothing (BUG-120: `StatModifierEffect.Apply()` only resolves the receiver; menus on abstract classes), and their authored modifiers are lost on load (BUG-121 + NOTE-20261103-2). First month in the audit, so no trend |
| *Abilities v2 (no GDD — informational)* | *Implemented, undesigned* | *(AT RISK)* | *S2: BUG-072, BUG-073, BUG-083, BUG-109, BUG-117. S3: BUG-068, BUG-071 (partial), BUG-079, BUG-090, BUG-092 (partial), BUG-118. RUNTIME note NOTE-20261101-3 (Avatar of Light never reaches Do)* | *TD-040* | *QA-auto RUNTIME: 1 FAIL* | *Still the player's only ability path with no GDD; Archer / Mage abilities (`class-abilities-archer-mage.md`) now design onto it* |

**Tally**: 2 CRITICAL (character, map), 4 AT RISK (weapons, stat, enemy-spawn, item), 3 HEALTHY
(animation, skill-ability v1, attack-speed).

**Systemic, every module**: still zero EditMode / PlayMode test files and zero `.asmdef` under `Assets/`
(BUG-084 / TD-044). New since October: the automated Play Mode QA run (`qa/weekly-automation`,
`FastTest/QAProbe/`, from 2026-11-01) gives the first RUNTIME evidence — 53 PASS / 3 FAIL / 12 KNOWN /
16 SKIP on 2026-11-01. It is a smoke harness, not unit tests, so test-evidence columns stay "None" for
Logic stories.

## Change-Impact: Approved/Designed Modules Touched This Cycle

| Module | Status | Files Touched | Commit(s) | In-Scope? | Recommended Follow-Up | Run This Cycle? |
|--------|--------|---------------|-----------|-----------|-----------------------|-----------------|
| stat-system.md | Designed | `StatModifier.cs`, `StatModifierGroup.cs`, `Stat.cs`, `BaseStatsSO.cs`, `VitalModifier.cs` (new); `StatPointAllocator.cs`, `StatPointSession.cs` (new) | `786c031b` "add effec component", `e156bdf4` "fix conflict", `6e484f0c` "update logic effect", `343a3830` "coding", `7f632021` "coding start room"; `1bc5d2ad` (stats-panel UI refactor) | **No** — effect-receiver (character / abilities) work and UI work. Produced BUG-121 and NOTE-20261103-2. `1bc5d2ad` is an additive extraction, low risk | `/consistency-check` on `stat-system.md` (code changed, GDD not updated; two serialized-field renames) | Not run — requires owner decision |
| character-system.md | Designed | 47 files under `Character/**` + `Interface/IEffectReceiver.cs`, `IVitalComponent.cs`, `IRegenResource.cs`, `IWallet.cs` | `786c031b`, `6e484f0c`, `343a3830`, `3d768b3d` "add regen system", `041292ce` "done flow pick champion", `51f8d2d4` "polish", `221d54be`, `ca90fbac` / `6c11bf6d` (`ICharacter` fixes), `fc98fa2a` (BUG-108 stats key) | **Mixed** — the effect refactor was the planned S20-F1 / S21-F1 feature, but the GDD was not touched; regen, wallet and champion-flow commits are off-plan with no story ID | `/architecture-review` (ADR-0005 still `Proposed` while two new architecture plans — `core-component-access-plan.md`, `effect-source-plan.md` — build on it), then `/consistency-check` on `character-system.md` | Not run — requires owner decision |
| weapons-system.md | Designed | `Weapons/Weapon.cs`, `Weapons/RangeWeapon/RangeWeapon.cs` | `7f632021` "coding start room", `3d768b3d` "add regen system", `343a3830` "coding" (`Weapon.Equid/UnEquid` stat-modifier path); `cae75317` (pool interface rename); `05dcf630` "add animation event" | **No** — start-room, regen and effect work changed how a weapon applies its stat modifiers and dropped the `Equid_UnEquid` bookkeeping (BUG-110) | `/consistency-check` on `weapons-system.md` | Not run — requires owner decision |
| enemy-spawn-system.md | **Approved** | `System/Enemy/EnemySpawner.cs`, `Database-SO/Modal/MapModel.cs` | `cae75317` "add asset" (interface rename), `ddb1d702` "coding" (removed the empty-pool `LogWarning` in `GetRandomRoom()`) | **No**, but mechanical. The removed warning makes an empty `fullRoomList` fail silently (`return null`) | `/consistency-check` on `enemy-spawn-system.md` — still owed from October (BUG-098) | Not run — requires owner decision |

**Not flagged:** `animation-system.md` (no code change in its paths), `skill-ability-system.md` (menu path
only), `attack-speed-system.md` (not implemented), `map-system.md` (In Progress; changes in scope),
`item-system.md` (Reverse-documented; treated as In Progress).

**GDD / ADR files changed in the window:** `item-system.md` (new, reverse-documented `38516a2d`),
`class-abilities-archer-mage.md` (new design, `ae0d6e82` / `6c61ccdb`), and doc-truth passes on
`character-system.md`, `map-system.md`, `enemy-spawn-system.md`, `attack-speed-system.md` (`f466745f`,
`29916f0b`) — none is a design revision of an existing GDD, so `/propagate-design-change` is not
indicated. ADR changes were doc-truth banners only (ADR-0002, ADR-0005); two new non-ADR architecture
plans were added (`26c5b4e1`, `ef0d9c6f`).

**Count: 4 cross-module impact flags** (1 Approved, 3 Designed).

## Trend vs. Previous Audit (2026-10)

| Module | Oct 2026 | Nov 2026 | Change |
|--------|----------|----------|--------|
| animation-system.md | HEALTHY | HEALTHY | ↔ — watch NOTE-20261101-1 (Die clip events) |
| character-system.md | CRITICAL | CRITICAL | ↔ — more S1 than before (BUG-108, BUG-116 new; BUG-087 still partial) plus 2 RUNTIME S1 notes |
| weapons-system.md | AT RISK | AT RISK | ↔ — BUG-110 new; BUG-105 open since 2026-10-10 |
| skill-ability-system.md | HEALTHY | HEALTHY | ↔ |
| map-system.md | CRITICAL | CRITICAL | ↔ — BUG-096 / BUG-097 not fixed (October recommendation 1 not done) |
| stat-system.md | HEALTHY | AT RISK | ⬇ regressed — BUG-113/114/115/121 (S2) and a second serialized rename |
| enemy-spawn-system.md | AT RISK | AT RISK | ↔ — October recommendation 3 not done |
| attack-speed-system.md | HEALTHY | HEALTHY | ↔ |
| item-system.md | — | AT RISK | new module this month |

Net: 0 improved, 1 regressed (stat), 7 unchanged, 1 new. CRITICAL count 2 → 2. October's
recommendations checked this run: 1 (BUG-096/097), 2 (BUG-087), 3 (enemy-spawn `/consistency-check`),
4 (ADR-0001/0002/0005 still `Proposed`) and 5 (`character-system.md` for ADR-0005) are not done; 6
(BUG-064) and 7 (GDD status headers) were not re-checked; 8 is done — this run landed on schedule.

## New Bugs / Debt Found This Cycle

Recorded as notes in `production/qa/bug-inbox.md` (no bug IDs — `/weekly-wrapup` triages on Saturday):

- **NOTE-20261103-2** (S2 guess, STATIC) — `StatModifier.targetStat` renamed `targetStatType` in `6e484f0c`
  with no `[FormerlySerializedAs]`. `SnS_Stat.asset`, the Blessed Slash recovery effect and both item effect
  assets still store `targetStat:`. Relates to BUG-121: restoring the list name alone would bring the
  modifiers back pointing at `StatType` 0 (STR).
- **NOTE-20261103-3** (S4 guess, STATIC) — `stat-system.md` (`:103`, `:112`) names
  `StatModifierGroup.authoredModifiers`; code is `statsModifiers` (`343a3830`), and the GDD has no
  `VitalModifier` or `StatPointSession`.

Existing items re-seen, not re-filed: BUG-105 / BUG-108 / BUG-121 (the serialized-rename pattern, now four
occurrences counting NOTE-20261103-2), NOTE-20261101-1/2/3 (RUNTIME), NOTE-20261031-7…10, and
NOTE-20261103-1 (written by today's standup while this audit ran: 12 `.meta` files of the new Archer / Mage
ability assets in `6c61ccdb` fail to parse in batchmode — relevant to Recommendation 8 below).

## Recommended Actions Next Cycle

1. **Add a guard against serialized-field renames** (pre-commit check for a `[SerializeField]` field that
   disappears without a `[FormerlySerializedAs]`). Four occurrences in nine weeks: BUG-105, BUG-108,
   BUG-121, NOTE-20261103-2. Fix BUG-121 and NOTE-20261103-2 together, in one asset re-save.
2. **Fix BUG-096 + BUG-097** — carried from October recommendation 1, still blocks every run past the
   start room.
3. **Player and enemy death (character, CRITICAL):** triage NOTE-20261101-1/2 on Saturday (both RUNTIME,
   4 of 4 runs), then BUG-087 / BUG-086 / BUG-065. The Die-clip event question belongs to the animation GDD.
4. **Fix BUG-116 before any demo build** (`CharacterData` imports `UnityEditor.Animations`) — only the
   Editor compiles it.
5. **Run `/architecture-review` for ADR-0005** and move it out of `Proposed`; two architecture plans now
   depend on it. Resolve ADR-0001 / ADR-0002 in the same session (carried from October recommendation 4).
6. **Update `character-system.md`** for ADR-0005 and the effect-receiver split (still 0 mentions), and
   `stat-system.md` for `statsModifiers` / `VitalModifier` / `StatPointSession` (NOTE-20261103-3).
7. **Run `/consistency-check` on `enemy-spawn-system.md`** — the only Approved module; owed since October.
8. **Add `class-abilities-archer-mage.md` to `systems-index.md`** with a status, so the next audit can
   score it; consider whether it is the start of the missing Abilities v2 GDD.
