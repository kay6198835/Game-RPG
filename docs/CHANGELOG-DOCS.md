# Documentation Changelog

Every documentation change in this project, with the code change that caused it.

**Why this file exists.** Between 2026-08-22 and 2026-09-09 four structural changes landed in
`Assets/Script/` with no documentation update. By 2026-09-11 that had produced 23 drifted and
9 outdated living documents, including rules files that instructed contributors to write code
that would not compile. This log exists so a documentation change is never again silent about
which code change caused it.

**Conventions used here**
- Every entry names the **cause** (a commit, a file, or a verified source read), not just the effect.
- **Nothing is hard-deleted.** Outdated documents move to `docs/archive/` with a `⚠️ DEPRECATED`
  banner and their original content untouched.
- Dated snapshots (sprint logs, retros, triage reports, playtests) are **historical records** and
  are never rewritten — they were accurate on their own date.

---

## 2026-09-22 (later same day) — Owner review: three findings corrected or retracted

**Cause.** The findings from the re-verification pass earlier the same day were put to the owner.
Three were challenged. Each was re-checked against source and against the live `.asset` files; two
were wrong, one was misattributed. This entry records the corrections and, per the log's own
convention, **nothing from the earlier entry was deleted** — the claim and its rebuttal both stay on
the record.

### What changed

| Finding | Owner's position | Verified outcome |
|---|---|---|
| **BUG-072** — framed as an architectural defect: "`Action<AbilityContext>` cannot carry a hit target, so per-hit state is smuggled through a shared mutable field" | `SummonExecute` is a callback invoked elsewhere, after the caller has set `Services.NegativeReceiver`; the `!= null` guard is deliberate | **Owner right — escalation WITHDRAWN.** Set-then-invoke is the intended contract and is already correctly implemented at `SpawnProjectileBase.cs:31-33` (assign, then `_callback.Invoke` — adjacent and synchronous, so no interleaving window; the earlier "overwritten by whichever object last hit" claim was wrong). The summon invoke wiring exists too: `SpawnSummonBase.cs:12-15` fires the callback from a Unity Animation Event and `LightningController.cs:18` calls `base.Execute()`. **Only the overlap query is missing** — `LightningController.cs:12-19`, still three comment lines. Bug returns to its original scope |
| **BUG-078** — "double spawn from two prefab fields, still open" | Deliberate: the effect controls two objects within one effect | **Owner right — finding RETRACTED, bug CLOSED.** Verified in `Assets/SO/Skill/Paladin/Ability/Consecrate/Effect/Paladin Spawn Consecrat Effect.asset`: `Prefab` → `RuneCircle.prefab` (Cast-phase telegraph; `Execute()` = `//Do nothing`, correct for something that deals no damage), `summonPrefab` → `Lightning.prefab` (Do-phase payload, `SummonExecute`). Telegraph-then-strike. `RuneCircleController` being an empty subclass is consistent. The inverted-return half remains fixed in `73ab8e7`, so the whole bug is closed with no code change |
| **BUG-076 (a)** — "Hold re-charges per-effect cost every dispatch" | Correct flow, intended | **Accepted as by design** (channelled cast). Additionally verified **dormant**: no effect asset in the project carries a serialized `Costs` list — all seven predate the field added in `73ab8e7`, so `TryPayEffectCost()` returns at `statCosts.Count == 0`. Blessed Slash (the only `Hold` ability) pays its 5 Mana once from the ability-scope list |
| **BUG-076 (b)** — "`TryPayEffectCost` never checks affordability" | The check is already in the base class; `Casting()` will be renamed `TryCasting()` to make that legible | **Owner right for the effect-scope list — WITHDRAWN.** `AbilityEffectDefinition.Casting():20` calls `CheckPayCostValid()`, and `AbilityInstance.cs:83-86` pays only inside that gate. **But the ability-scope list has no gate at all**: `TryPayCost()` (`:164-178`) pays unconditionally and `HasEnoughManaCondition` reads `StatType.Mana` only. `Avatar of Light.asset` costs 40 Mana (`statType: 101`) **+ 50 HP** (`statType: 100`); the HP half is validated by nothing, and `Reduction()` clamps at 0. Re-filed as **BUG-076 (b′)**, S2, live |
| **BUG-076 (c)** — "conditions walked 3× per activation" | Not understood | **Stands.** Restated in full in `BUG-076.md` with the complete call chain: `AbilityHolder.cs:109-112` (walk 1), `AbilityInstance.cs:44` (walk 2), `AbilityInstance.cs:166-171` (walk 3 — a verbatim duplicate of walk 2, five lines later in the same call). Each walk writes to a committed `.asset` via BUG-077 ⇒ three asset writes per button press |

### Net effect on the bug list

- **BUG-078: CLOSED** (S1 → non-issue). Half fixed, half retracted.
- **BUG-072: still open, scope reduced** to its original filing — summon target resolution
  unimplemented. The "no v2 effect deals damage" *outcome* is unchanged and still true, but the
  *cause* is now correctly stated as two independent implementation gaps (BUG-075 for projectiles,
  BUG-072 for summons), not one design flaw. Either can be fixed without the other.
- **BUG-076: S1 → S2**, re-scoped a second time. (a) and (b) resolved; (b′) and (c) open.
- **TD-046: WITHDRAWN** — kept in the register as the record of a retracted claim, marked
  *do not action*.
- Two behaviours are now recorded as **confirmed by design** rather than defects: the channelled
  per-effect cost on `Hold` abilities, and the `if (negativeReceiver != null)` guard pattern in the
  spawn effects.

### Documents changed

| Document | Change |
|---|---|
| `production/qa/bugs/BUG-072.md` | `## Owner review — 2026-09-22` appended: escalation withdrawn, set-then-invoke confirmed correct with the reference implementation cited, scope returned to the missing overlap query, revised fix sketch using `OverlapCircleNonAlloc` per `.claude/rules/engine-code.md` |
| `production/qa/bugs/BUG-078.md` | `## Owner review` appended with the asset evidence; **Status → CLOSED**, severity annotated as downgraded-to-non-issue. Follow-up suggested (not filed): `[Header]`/`[Tooltip]` on `Prefab` vs `summonPrefab`, since the two are indistinguishable in the Inspector and that is what caused the misreading |
| `production/qa/bugs/BUG-076.md` | `## Owner review` appended: (a) accepted by design + the per-asset `Costs` audit proving it dormant; (b) withdrawn with the gate chain quoted; (b′) newly stated with the Avatar of Light asset evidence; (c) restated in full with the three-walk call chain. Severity S1 → S2, Priority 1 → 2, Status rewritten |
| `docs/tech-debt-register.md` | TD-046 rewritten as WITHDRAWN/Void with the reason, marked *do not action this entry* |
| `CLAUDE.md` | BUG-072, BUG-075, BUG-076 and BUG-078 rows rewritten. "v2 does not currently work" block rewritten around the two-independent-gaps table instead of the withdrawn architectural claim, and the closed/by-design items listed. Damage-chain ability branch rewritten to state the contract first, then the two gaps |
| `.claude/rules/weapon-skill-code.md` | Four rules rewritten: the spawn-damage contract is now stated positively as set-then-invoke with `SpawnProjectileBase.cs:31-33` named as the shape to copy; `Casting()` documented as a gate (rename to `TryCasting()` noted) with the two-object case called out as legitimate; the shared-`ScriptableObject` rule split so `_context`/`.dir` is described as load-bearing rather than a violation; the cost rule rewritten around one-off vs channelled cost, with the ability-scope affordability gap (b′) as the live warning |
| `docs/diagrams/ability-system-diagrams.md` | **§6 and §7 left unedited.** New **§8 Owner review** appended: corrections table, a corrected damage-path diagram (contract → two independent gaps), a corrected Consecrate sequence diagram (telegraph → payload), and revisions to §7 — weakness #1 withdrawn, #2 softened, #3 split, #5 re-aimed, a new strength added for the one-off/channelled cost distinction, and a revised fix order that no longer blocks on a design decision |
| `production/sprint-status.yaml` | Owner decision (option C) recorded — see below |

### Owner decision — S14-01 / S14-14 (option C)

Both stories cover bugs that are now fixed in code but have no Play Mode confirmation. The owner
chose **not to close them**.

Implemented conservatively: **no `status:` value was changed and no story was closed.** Only the
free-text `blocker:` field on S14-01 (BUG-067) and S14-14 (BUG-069) was filled in, recording what
landed, what evidence exists, and what is still missing. `needs-verification` was deliberately **not**
introduced as a status value — it is not in this file's vocabulary (`backlog` / `ready-for-dev` /
`done`) and `/story-done` would not understand it. A dated comment block explaining all of this was
added under the file's existing "DO NOT edit manually" header. Both stories should be closed through
`/story-done` after the first Play Mode session.

---

## 2026-09-22 — Bug-documentation re-verification pass

**Cause.** Two ability commits landed after the 2026-09-21 doc-sync and were never reviewed:
`73ab8e7` ("coding update flow ability, update logic cost") and `e2cb75e` ("done"), reaching `main`
through the merge `d17fcc5`. `CLAUDE.md` still declared HEAD `15242e6`. Separately, the 2026-09-21
pass had introduced three claims that source does not support. Every tracked bug and the
tech-debt register were re-read against `Assets/Script/` at `d17fcc5`.

**Scope constraint.** Documentation only. **No `.cs` file was changed** —
`git diff --stat` shows zero source files touched. `production/sprint-status.yaml` was deliberately
not edited: closing a story is a production decision for the owner, made through `/story-done`.
Status changes below therefore live in the bug files and `CLAUDE.md`, and the sprint files still
list the old state — see "Handover" at the end.

### Status changes, with evidence

| Bug | Was | Now | Decisive evidence |
|---|---|---|---|
| BUG-069 | Open ("apparently fixed, unverified") | **FIXED** | Full loop present: `AbilityHolder.cs:107` calls `CanStart()`; `AbilityInstance.cs:192` sets; `:23-30` ticks; driven by `AbilityHolder.cs:51-59` from `PlayerBasicState.cs:32` / `PlayerSkillWeaponState.cs:68`; gated at `PlayerInputHandle.cs:254` |
| BUG-067 | Apparently fixed (unverified) | **FIXED** | `ResourceReceiver.cs:17-24` polarity correct; consumer `RecoveryEffectDefinition.cs:13` confirms. Two *unrelated* defects in the same file split out as BUG-080 / BUG-081 rather than closed with it |
| BUG-078 | Open (two defects in one) | **PARTIAL** | Inverted return fixed in `73ab8e7` (`SpawnSummonEffect.cs:20-25`); double spawn still open — `Casting()` spawns `Prefab`, `Apply()` spawns `summonPrefab`, at states `Cast` and `Do` of the same cast |
| BUG-076 | Open (double-charge) | **RE-SCOPED, open** | Original defect fixed by a design change: two disjoint cost lists now exist (`AbilityDefinition.cs:25`, `AbilityEffectDefinition.cs:9`). Three new defects replace it — see below |
| BUG-072 | Open — PLAUSIBLE | **Open — CONFIRMED, widened** | The only writer of `Services.NegativeReceiver` repo-wide is `SpawnProjectileBase.cs:32`, inside the dead 3D callback; `AbilityContext.cs:37-47` never assigns it ⇒ always null ⇒ **no Abilities v2 effect deals any damage** |
| BUG-073 | Open (suspected) | **Open — CONFIRMED by GUID** | `ShootSpirit.asset:12` guid `ac9ac7c011812d042ac992007bc0cf48` resolves to **0** `.meta` files under `Assets/` |
| BUG-079 | Open, S2 | **Open, S3** | Consequence corrected — see below |
| BUG-066 / BUG-070 | Two separate entries | Open, **recorded as one defect, two instances** | Near-verbatim duplicate classes; neither file contains a single `TryGetValue`/`ContainsKey` |

### Claims corrected — all three originated in the 2026-09-21 pass

1. **BUG-063 — the `#if UNITY_EDITOR` guard is not a mitigation.** `CLAUDE.md` framed it as
   narrowing the blast radius. Play Mode in the Editor *is* `UNITY_EDITOR`, which is precisely the
   leak path the capitalised comment at `Stat.cs:49-62` warns about; the guard only protects the
   player build, where `.asset` files are read-only anyway. The framing is why a three-line deletion
   has been carried 29+ cycles. Corrected in `CLAUDE.md` (Known Bugs row, tree annotation) and
   `production/qa/bugs/BUG-063.md`.
2. **BUG-079 — "castable once per scene load" is false.** `AbilityHolder.StartHold()` (`:143-148`)
   forces `ChangeState(AbilityState.Start)` on every fresh press via `PlayerInputHandle.cs:254-256`,
   so abilities do re-cast. What remains is a layering defect: the instance cannot reset itself.
   Severity lowered S2 → S3. Also propagated: `docs/diagrams/ability-system-diagrams.md` lines 226
   and 248 still carry the wrong claim and are flagged in the handover below.
3. **BUG-074 — there is no work-in-progress fix.** An earlier review recorded an uncommitted,
   ineffective edit in `Assets/Script/Utility/Utility.cs` (a dead `finalValue` variable). Verified
   2026-09-22: `git status --porcelain` is empty and `ModifierStatsCalculate` (`Utility.cs:274-304`)
   contains no such variable. Anyone picking BUG-074 up starts from the untouched version.

### Eight defects filed for the first time

| ID | Severity | Summary |
|---|---|---|
| BUG-080 | S1 | `ResourceReceiver.vitalStatsComponent` (`:5`) is assigned only inside `ReceverModifierGroup()` (`:13`); `Recovery()`/`Reduction()`/`BuffDebuffForDuration()` dereference it unresolved. Live on the item-pickup path via `RecoveryEffectDefinition.cs:13`. `TakeDamage()` escapes only via a shadowing local |
| BUG-081 | S2 | Two `INegativeReceiver` implementers with byte-identical bodies, **both on `PlayerTest.prefab`** (verified by GUID). Player-side twin of the closed BUG-053. Secondary: `EnemyPrefab.prefab` carries the *player* `NegativeReciver` and no `EntityNegativeReciver` |
| BUG-082 | S3 | `if (Costs.Count == 0 \|\| Costs == null)` — `\|\|` short-circuits, so the null test is unreachable. Latent only because Unity's serializer materialises empty lists; becomes live the moment code-constructed instances appear (i.e. in tests) |
| BUG-083 | S2 | `AbilityContext.HoldTime` / `.HoldRatio` are plain fields snapshotted by `BuildContext()` *before* `StartHold()` zeroes the counter, and never rebuilt — always `0f`. Masked because all four Paladin abilities have `MaxHoldTime = 0` |
| BUG-084 | S2 | **No `.asmdef` anywhere under `Assets/`**, and `tests/` sits outside the Unity asset tree. Test Runner can neither compile nor discover a test. Precondition of TD-014, not a symptom |
| BUG-085 | S3 | `NotDeadCondition.IsMet()` body commented out, always `true`. Dormant (no asset references it) but carries `[CreateAssetMenu]` |
| BUG-086 | S2 | `PlayerDeathState.LogicUpdate()` never consumes `Status` and never changes state ⇒ `ON_PLAYER_DEATH` emitted every frame. Violates the durable-`Status` contract in `manager-event-code.md`. Was only a parenthetical inside BUG-065 |
| BUG-087 | S1 | `ON_PLAYER_DEATH` has zero subscribers; no `GameManager` exists; player `VitalStatsComponent` has no reset path; `ON_REALOAD_GAME` has 0 emitters and 0 subscribers. **Death is a permanent hard lock.** Closes the open half of Bug #6 / S10-08 |

BUG-076's three replacement defects are recorded inside `BUG-076.md` rather than as new IDs, since
they occupy the same code and the same open design question: (a) Hold abilities re-charge
per-effect cost every dispatch (`AbilityInstance.cs:57-65`, live on Blessed Slash); (b)
`TryPayEffectCost()` (`:155-162`) never checks affordability and `Reduction` clamps at 0, so an
unaffordable cast drains to zero instead of being refused; (c) conditions are evaluated 3× per
activation (`AbilityHolder.cs:109-112`, `AbilityInstance.cs:44`, `:166-171`), each evaluation
writing to a committed asset via BUG-077.

### Documents changed

| Document | Change |
|---|---|
| `CLAUDE.md` | New header entry (HEAD corrected `15242e6` → `d17fcc5`). Known Bugs preamble rewritten with this pass's status changes and the three corrections. All fifteen BUG-063…BUG-079 rows rewritten from source; eight rows added (BUG-080…BUG-087). "v2 is not healthy" block rewritten around BUG-072 as the subsuming defect. Damage-chain section: player branch rewritten (death is a dead end), new "Player ability hits anything — ❌ DOES NOT WORK AT ALL" branch added. Repo-layout annotations updated on 12 lines. Demo checklist: items 6, 20 and 21 rewritten; items 23 (test pipeline) and 24 (death recovery path) added |
| `production/qa/bugs/BUG-063…079.md` | Dated `## Re-verification — 2026-09-22` section appended to each, with file:line evidence. Status lines updated on 067, 069, 072, 074, 076, 078, 079, 066, 070. `**Priority**` field added to BUG-075…079, which had none and appeared in no sprint or carry-over list. BUG-079 severity S2 → S3 |
| `production/qa/bugs/BUG-080…087.md` | New, in the existing BUG-*.md format |
| `docs/tech-debt-register.md` | Header date and counts updated (43 → 47 items). TD-014 re-scoped as blocked by TD-044. TD-044 (asmdef/test pipeline), TD-045 (death recovery path), TD-046 (`NegativeReceiver` shared mutable state / callback signature), TD-047 (duplicate player `INegativeReceiver`) added |
| `.claude/rules/weapon-skill-code.md` | "Rules that exist because of open bugs" block rewritten. Corrected a wrong bug ID (`SpawnEffectBase._context`/`.dir` is BUG-078, not BUG-079). Four rules added: no v2 effect deals damage today; `Casting()` and `Apply()` run at different phases of one cast; the 3D trigger form compiles clean with no warning; `HoldTime`/`HoldRatio` are always `0f`; cost now comes from two disjoint lists |
| `.claude/rules/test-standards.md` | Warning added under "Unity Test Naming": the `tests/EditMode/` / `tests/PlayMode/` paths this file prescribes do not compile, and no test can be written to them until BUG-084 / TD-044 is resolved. This file has prescribed non-working paths since it was written |
| `docs/diagrams/ability-system-diagrams.md` | **Additive only — §1–§5 untouched.** Pointer block added under the existing defect list, then **§6 Current Version** (five stale §1–§3 annotations tabulated, plus three new source-drawn diagrams: the two-list cost model, the damage dead end, the `SpawnSummonEffect` double spawn) and **§7 System assessment** (7 strengths / 11 weaknesses, ranked, with a recommended fix order that puts the two undefined contracts first) appended |
| `docs/CHANGELOG-DOCS.md` | This entry |

### Handover — deliberately left for the owner

- **`production/sprint-status.yaml` not touched.** Its own header requires `/story-done`. BUG-067
  and BUG-069 are now FIXED and their stories should be closed there by the owner.
- **`production/sprints/sprint-15.md` line 39** still reads "Open bugs: 13 (BUG-052, 063-066,
  070-074 + verify-only 067/069)". BUG-075…079 were never added to any sprint or carry-over list,
  and BUG-080…087 are new. Sprint files are dated records and were not rewritten.
- **`docs/diagrams/ability-system-diagrams.md`** — **addressed 2026-09-22 by addition, not rewrite**,
  per owner instruction. Lines 200, 207, 226 and 248 still carry the pre-`73ab8e7` cost model and the
  wrong BUG-079 consequence; they are deliberately left in place and are instead tabulated as stale
  in the new §6.1, so the drift remains auditable. §6 supplies the current diagrams and §7 the
  strengths/weaknesses assessment. Regenerating §1–§3 in place is still open, and is a larger job.
- **Estimates now known to be wrong**: sprint-14 S14-13 and the sprint-15 test story estimate
  "write the first EditMode test" at 0.3d; BUG-084 must land first, and it entails splitting
  `Assembly-CSharp`.

---

## 2026-09-21 — Abilities v2 re-synchronisation + Paladin direction renumber

**Cause.** Branch `origin/feature/fix-player-control`, HEAD `15242e6`. Between `8295539`
("update spawn effect", 2026-09-17) and `7cceda2` ("done paladin consecrate ability", 2026-09-19)
the Abilities v2 **effect layer and runtime layer were replaced wholesale**, and the Paladin sprite
set was imported. No documentation entry accompanied either. Verified by direct source read of all
23 files under `Assets/Script/System/Abilities/`, plus image inspection of the Paladin sprite sheets
against `paladin_8dir_controller` state names.

### What the code did that no document recorded

| Change | Evidence |
|---|---|
| `SkillState` enum renamed **`AbilityState`**, `None` member removed | `AbilityDefinition.cs:74-80` |
| `DamageInFrontEffect`, `LungeForwardEffect`, `ShootObjectEffect`, `PlayDebugLogEffect` deleted | `Effects/` now holds seven different files |
| `AbilityRuntimeHelpers`, `SpiritOrbProjectile`, `SpiritDoTBehaviour` deleted | `Runtime/` restructured into `BaseController/` + `SpawnMono/` |
| New `SpawnEffectBase` → `SpawnProjectileEffect` / `SpawnSummonEffect` | `Effects/SpawnEffectBase.cs` |
| New `StatsEffectBase` → `RecoveryReductionStatsEffect` / `RecoveryReductionPerTimeForDuration`; new `BuffDebuffStatsForDuration` | `Effects/StatEffectBase.cs` (⚠️ file/class name mismatch) |
| New runtime layer `SpawnMono` → `SpawnProjectileBase` / `SpawnSummonBase` → `SlashProjectile`, `LightningController`, `RuneCircleController` | `Runtime/` |
| **New and never documented anywhere**: `IAbilityServices` / `AbilityServices` — the service bundle carried on every `AbilityContext` | `AbilityContext.cs:20-47` |
| `AbilityEffectDefinition` gained `SubConditions`, `SubEffects` and the `Casting()` hook | `AbilityEffectDefinition.cs` |
| v2's live content became the **Paladin ability set** (Consecrate, Blessed Slash, Blessing, Avatar of Light) | `Assets/SO/Skill/Paladin/Ability/` |
| Paladin sprite sets imported numbered `dir0 = N`, counter-clockwise — against `DirectionResolver` | `Assets/Sprite/Paladin/Sprites/` |

### Documents changed

| Document | Change |
|---|---|
| `CLAUDE.md` | Header entry for this pass. `System/Abilities/` subtree re-listed file-by-file against source. `AbilityHolder` row corrected (`AbilityState`, `AbilityServices`, `abilityBindings` overwritten in `Start()`). "Skill / Ability — TWO frameworks" section rewritten: lifecycle box, effect-hierarchy table, runtime layer, live asset paths, open-defect list. `SO/Skill/` inventory corrected. Thirteen rows added to Known Bugs (BUG-067…BUG-079). Demo checklist items 21 and 22 added. |
| `docs/diagrams/ability-system-diagrams.md` | **§1–§3 rewritten from source.** The 2026-09-11 pass corrected this file's status banner but left the diagrams describing the deleted Spirit Orb prototype. New architecture flowchart, activation sequence and lifecycle state diagram, each annotated with the open bug that breaks that step. §4–§5 kept and explicitly marked HISTORICAL. Deletion table added at the top. |
| `.claude/rules/weapon-skill-code.md` | Abilities v2 section: `SkillState` → `AbilityState`; effect-hierarchy table added; `abilityBindings` authoring location corrected; `IAbilityServices` documented; three new prohibitions added (no per-cast state on an effect/condition SO, no new `Casting()` override until its contract is defined, 2D trigger callbacks are `OnTriggerEnter2D`). |
| `design/gdd/skill-ability-system.md` | v1/v2 comparison table: lifecycle corrected to `AbilityState: Start → Cast → Do → Exit`; live v2 assets updated to the Paladin set. Cross-system table: the "`PlayerInputHandle` provides a `SkillState` enum" row was wrong and is corrected. |
| `design/gdd/animation-system.md` | **New section "8-direction index convention"** under Formulas — the `DirectionResolver` index table (0 = down-left, clockwise), the asset classes it binds, and the Paladin renumber precedent. This convention had never been written down, which is how the Paladin set was imported against a different one. Header re-verification banner updated. |
| `production/qa/bugs/BUG-075.md` … `BUG-079.md` | New. See below. |

### Bugs filed this pass

| ID | Severity | Summary |
|---|---|---|
| BUG-075 | S1 | `SpawnProjectileBase` uses the 3D `OnTriggerEnter(Collider)` signature in a 2D project — Unity never dispatches it, so every projectile ability deals zero damage |
| BUG-076 | S1 | Ability cost paid in `TryActivateInstant()` **and** again per effect inside `Casting()` — a 3-effect ability charges its full `Costs` four times per cast |
| BUG-077 | S1 | `HasEnoughManaCondition.currentMana` / `.costMana` are public serialized fields written on every `IsMet()` — runtime state committed into a `.asset`, same class as BUG-063 |
| BUG-078 | S1 | `SpawnSummonEffect.Casting()` returns its gate inverted (charges cost on the failure path) and spawns the summon in both `Casting()` and `Apply()` |
| BUG-079 | S2 | `AbilityInstance.Exit()` body is commented out — `State` never returns to `Start`, so an `Active` ability is castable once per scene load. Also collects three minor findings not separately filed |

Per the owner's decision on 2026-09-21, **no ability code was changed in this pass** — BUG-076 and
BUG-078 in particular cannot be fixed without first defining what `AbilityEffectDefinition.Casting()`
is for, and v2 still has no GDD (BUG-052, demo-checklist item 18).

### Asset change: Paladin direction renumber

`DirectionResolver.Calculate()` yields index 0 = **down-left (SW)**, running clockwise. The Knight
sprite sets already follow it (verified by rendering `KnightSnS/Block/Direction_0..7.png`). The
imported Paladin `cast` and `idle` sets were numbered `dir0 = N`, counter-clockwise — confirmed both
by rendering the sheets and by the compass suffixes on `paladin_8dir_controller`'s own state names
(`a1_cast_dir0_n`, `a1_cast_dir1_nw`, …).

Renumbered with the involution `0↔3, 1↔2, 4↔7, 5↔6`, applied to:

- 32 PNGs (`paladin_{cast,idle}_dirN.png` + `_NormalMap.png`) and their `.meta`
- the sprite names and `nameFileIdTable` keys inside every `.png.meta`
- 16 generated `.anim` and 16 `.mat` (files, and `m_Name` inside)
- `paladin_8dir_controller`: 16 state names **and** the 16 `direction` parameter thresholds that
  select them, so state and condition moved together

Unity references are GUID-based, so content followed each file through the rename and nothing broke.
Confirmed afterwards: `paladin_cast_dir0.png` is the SW sheet, `a1_cast_dir0_sw` is reached by
`direction == 0`, and the three pre-existing `Knight_Consecrate_State*_dir 0` clips (which already
pointed at the SW sheet by GUID) still resolve to it.

### Asset change: Consecrate directional clips

`Paladin Consecrate.overrideController` previously overrode only three clips — `dir 0` of
`Knight_SpinAttack_State1/2/3`. Generated the remaining 21 clips (`dir 1`…`dir 7` × 3 states) by
remapping each frame of the `dir 0` clips onto the corresponding sheet by **sprite ordinal**, so the
frame ranges, sample rate and Animation Events are identical across all eight directions:

| State | Sheet frames | Animation events |
|---|---|---|
| State1 (wind-up) | 0–30 | `AnimationStart` @0, `AnimationOnAction` @0.4167, `AnimationFinishTrigger` @0.625 |
| State2 (hold loop) | 29–30 | inherited from `dir 0` |
| State3 (release) | 31–88 | inherited from `dir 0` |

All 24 entries are now wired in the override controller.

---

## 2026-09-11 — Full documentation/code re-synchronisation

**Scope:** all living documentation. **Verified against:** HEAD `6d6a8e4`, branch
`origin/feature/synce-doc-and-code`. **Code changed:** none — this was a documentation-only pass.

### Root causes

Four structural code changes drove almost every edit below.

| ID | Change | Commit(s) | Date |
|---|---|---|---|
| **R1** | Seven directories moved under `Assets/Script/System/` — `Enemy/`, `Pathfinding/`, `Poolable/`→`PoolableService/`, `StatSystem/`, `Skill_Ability/`, `Item/`, `LifetimeScope/`. 77 renames | `1c0742e` (also `853fe39`, `f7acffc`, `0686619`) | 2026-09-03 |
| **R2** | **VContainer 1.19.0** dependency injection adopted; `GameLifetimeScope` composition root | `aa4e620` | 2026-08-22 |
| **R3** | `StatsSO.cs` deleted → `BaseStatsSO` + `EnemyStatSO`; `StatPointAllocator` added | `b0512f4`, `1c0742e` | 2026-08-28 / 09-03 |
| **R4** | `prototypes/skill-enhance-abilities/Scripts/` (17 files) promoted into `Assets/Script/System/Abilities/`; `AbilityHolder` rewritten to drive it | `9b8d40f`, `5c7afba` | 2026-09-09 |

Secondary causes: `INegativeReceiver.TakeDamage` changed `int` → `float` (`f3f5f08`); `EventID`
grew 20 → 23; a boss system was added and reverted (`ffe1976` → `4421fdc`, `ff67f4d`).

---

### Updated

#### `CLAUDE.md` — rewritten (563 → 743 lines)
**Cause:** R1, R2, R3, R4 + three false bug statuses.
**Changed:**
- Repository Layout rebuilt against the real tree (R1), with a Was→Is migration table so older
  documents remain readable.
- New sections: **Dependency Injection** (R2), **Player stat / vitals split**, **Skill / Ability —
  TWO frameworks now coexist** (R4), **Orphan directories**.
- `EventID` corrected 20 → **23** (`ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`),
  with the count history extended rather than replaced.
- Damage Chain rewritten: `float` signature, BUG-053 path now working, Defense mitigation.
- Package list: added VContainer 1.19.0, Timeline 1.7.7; noted DOTween is an Asset Store import.
- **Three bug-status corrections** — each was actively misleading:
  - `BUG-053` listed OPEN → it is **FIXED** (`f3f5f08`; `production/qa/bugs/BUG-053.md` had said so
    since 2026-09-06 — this table never followed the bug file).
  - `BUG-044` claimed the fix "properly stops `PlayerMovement`" → **false**;
    `PlayerDeathState.Enter()` only calls `base.Enter()`. Split out as `BUG-065`.
  - `NEW-4` claimed `Stat.modifiers` is no longer serialized → **regressed**; `Stat.cs:63-65`
    re-serializes it behind `#if UNITY_EDITOR`. Now `BUG-063`.
- Resolved a self-contradiction: the header claimed `EntityFindTarget` was deleted while the
  wiring section required it. The file exists — the header was wrong.
- Demo checklist gained items 18–20 (ability-framework decision, BUG-063, zero tests).
**Evidence:** `EventManager.cs`; `INegativeReciver.cs:7`; `Stat.cs:63-65`; `PlayerDeathState.cs:10-13`;
`EntityNegativeReciver.cs:18-25`; `Player.cs:58`; `GameLifetimeScope.cs`.

#### `README.md` — links repaired
**Cause:** R1.
**Changed:** 7 deep links to `Assets/Script/{Pathfinding,StatSystem}/…` were **404 on GitHub** after
the move — on the repository's public-facing page. All repointed and each verified to resolve on
disk. Project-structure block rebuilt; size corrected `~10k lines / 174 files` → `~11k / 194`.
**Note:** the commit that broke these links was `f7acffc`, *"chore(portfolio): prep repo for clean
public release"*.

#### `memory/project_state.md` — rewritten
**Cause:** R1–R4; three sprints stale (last updated 2026-08-21).
**Changed:** every row of the open-bug table was wrong — 7 bugs listed as open are fixed, 2 pointed
at deleted files (`EntityStatsSO.cs`, `EntityWeaponMelee.cs`), and it stated `prototypes/` "does not
exist". Rewritten with a structural-changes table, a key-API-changes table, and a re-prioritised
fix list.

#### `.claude/rules/gameplay-code.md`
**Cause:** `f3f5f08` (signature), R2.
**Changed:** `TakeDamage(int …)` → **`float`** — this rule would have produced non-compiling code
for three sprints. Added a **Dependency Resolution** section distinguishing the three mechanisms
(Core hub / DI / Inspector), and a max-vs-current stat ownership rule.

#### `.claude/rules/weapon-skill-code.md`
**Cause:** `f3f5f08`, BUG-043/046 closure, R4.
**Changed:** `int` → `float`; removed guidance referencing the deleted `EntityWeaponMelee`;
**"Skill Lifecycle Contract (ActivateSkill)" rewritten as a two-framework section** — the player
no longer uses the lifecycle this rule described.

#### `.claude/rules/engine-code.md`
**Cause:** R2.
**Changed:** "Components discover siblings via `Core.GetCoreComponent<T>()`, **never direct field
references**" was contradicted by shipped DI. Amended to give DI and the Core hub disjoint scopes,
and to constrain `GameLifetimeScope` from becoming a hub.

#### `.claude/rules/ai-code.md`
**Cause:** Bug #7 closed long ago; BUG-053/066; R3.
**Changed:** removed the stale instruction *"`EntityDeathState` must extend `EntityState` — fix the
base class bug before shipping"*. Added a **Damage Reception** section (one `INegativeReceiver` per
enemy, Defense in `DamageCalculate()`, the BUG-066 key-guard warning) and corrected the stat source
to `EnemyStatSO` / `EntityStatsHandler`.

#### `.claude/rules/scriptableobject-data.md`
**Cause:** R3 + the Sprint 8–10 rename.
**Changed:** `WeaponMeleeStats` → `MeleeWeaponStats`; `StatsCharacter` base → **`BaseStatsSO`**.
Added a **Runtime vs Authored Serialization** table — the distinction behind the project's only
committed data-corruption incident, currently regressed as BUG-063.

#### `.claude/rules/manager-event-code.md`
**Cause:** `EventID` 20 → 23.
**Changed:** added an **EventID Count** section. Flagged explicitly that
`ON_PLAYER_TAKE_DAMAGE` — which this rule instructs the health bar to bind to — **has never
existed**, and pointed to `EntityUIController` as the working push-based pattern.

#### `.claude/rules/ui-code.md`
**Cause:** R2, R3, the stat/vitals split.
**Changed:** added a table of where player values actually come from (current → `VitalStatsComponent`,
max → `StatHandler`), noted `PlayerData.currentHealth` is not a valid source (Bug #6), documented
the two deliberately-unregistered DI components, and recorded that `StatsUIController` still writes
gameplay state — a standing `ui-code.md` violation that survived the DI refactor.

#### `.claude/rules/prototype-code.md`
**Cause:** R4.
**Changed:** added a **Promotion Log**. The 2026-09-09 promotion bypassed this file's own Promotion
Rules (rewrite to production standards), and the residual prototype-grade code is named.

#### `.claude/rules/map-code.md`
**Cause:** verification only.
**Changed:** a re-verification stamp. **All five bugs it lists were checked in source and are still
open exactly as described** — this file needed no correction.

#### `docs/engine-reference/unity/VERSION.md`
**Cause:** R2.
**Changed:** package table corrected from `manifest.json` — **VContainer 1.19.0 was missing
entirely**, plus Timeline, Test Framework, uGUI and a note that DOTween is vendored and invisible to
the manifest. The **"UI Toolkit … not recommended for runtime UI"** guidance was **reversed**: runtime
UI Toolkit had already shipped (`UIController.cs`), so the file was contradicting the codebase every
time it was read. Added a VContainer API-notes section and a git-URL supply-chain risk note.

#### `docs/architecture/adr-0001-statsystem-dual-data-structure.md`
**Cause:** R3.
**Changed:** header warning + a **2026-09-11 Amendment**. The class the ADR is written about
(`StatsSO`) no longer exists; `BaseStatsSO` keeps the same API surface, so **the decision itself
stands unmodified** — this was a rename, not a redesign. The amendment also records that the
data-corruption hazard the ADR warns about has **regressed** (BUG-063), and that the source comment
saying "NEVER add `[SerializeField]`" did not prevent it.

#### `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
**Cause:** R1, R2.
**Changed:** second amendment — path move, and a note that **ADR-0004 supersedes this ADR's
premise**. A singleton is no longer *necessary* for cross-system access; the exception is retained
only because `EntityMovement` reads it on runtime-spawned enemies, the case DI handles worst.

#### `design/gdd/stat-system.md`
**Cause:** R3.
**Changed:** all `StatsSO` references → `BaseStatsSO` + a scope banner. **Rules, formulas and
acceptance criteria are unchanged and still valid** — only the type name and path moved. Banner also
flags what this GDD does *not* yet cover: `StatPointAllocator`, the max/current split, and BUG-063.

#### `design/gdd/skill-ability-system.md`
**Cause:** R4.
**Changed:** scope banner. This GDD is **not obsolete but now partial** — it documents v1
(`ActivateSkill`), which still drives weapons and enemies, while the **player** moved to v2. A
comparison table names which is which. v2 is deliberately *not* retro-designed here: one system =
one GDD (`.claude/rules/design-docs.md`), and no ADR has decided whether the two converge.

#### `design/gdd/character-system.md`
**Cause:** the Sprint 12 stat/vitals split; `f3f5f08`.
**Changed:** banner mapping the three-component split for both player and enemy, the `float`
signature, Defense mitigation, the ninth player state, and confirmation that death/restart remains
`[PLANNED]`.

#### `design/gdd/enemy-spawn-system.md`
**Cause:** BUG-033/BUG-053 closure, R1, R2.
**Changed:** status banner. **The algorithm did not drift** — `GetSpawnSet()` still matches. Four
status corrections (BUG-033 fixed, epic unblocked, path moved, DI-resolved); the three genuinely
open items are restated.

#### `design/gdd/weapons-system.md`, `attack-speed-system.md`, `map-system.md`, `animation-system.md`
**Cause:** R1, R3, `f3f5f08`; verification.
**Changed:** verification banners. `map-system.md` and `animation-system.md` had **no drift** — all
five map bugs re-confirmed open, and the `StatusAnimation` handoff unchanged. `weapons-system.md`
and `attack-speed-system.md` took `float` / `BaseStatsSO` / path corrections only.

#### `design/gdd/systems-index.md`
**Cause:** R2, R4, the Item system.
**Changed:** **three systems added** (24 DI, 25 Abilities v2, 26 Item) — all shipped without ever
entering the index. "Built without a GDD" corrected 4 → **7**. "Next Systems to Design" re-ordered:
the ability-framework decision is now the top blocker.

#### `docs/diagrams/ability-system-diagrams.md` — banner **inverted**
**Cause:** R4.
**Changed:** the banner said *"These diagrams do NOT describe the ability system the game runs"* and
*"ships no SO assets … nothing can instantiate it"*. **Both became false on 2026-09-09.** This is now
the most accurate description of the player's live ability system. Rewritten with the three deltas
that occurred during promotion (`AbilitySystem` absorbed into `AbilityHolder`;
`ShootSpiritOrbEffect` → `ShootObjectEffect`; `DamageInFrontEffect.Apply()` uncommented).
**Evidence that it is wired:** `Assets/SO/Skill/ShootSpirit/{ShootSpirit,SpiritBomd}.asset`,
`Assets/SO/Skill/Conditions/New Has Enough Mana Condition.asset`.

#### `prototypes/skill-enhance-abilities/README.md`
**Cause:** R4.
**Changed:** Result `[IN PROGRESS — never finished, never wired]` → **`[VALIDATED — promoted 2026-09-09]`**.
Original body retained unedited as the historical record; a **Promotion (2026-09-09)** section audits
the promotion against this README's own four "if you pick this back up" steps — steps 1–3 done,
step 4 (rewrite to production standards) **partial**.

#### `docs/tech-debt-register.md`
**Cause:** R1–R4 + source re-verification of every entry.
**Changed:** 39 → 43 items.
- **TD-011 → VOID** — carried at Critical/Priority-1 for 14 weeks against `EntityStatsSO.cs`,
  a file deleted in `b0512f4`.
- **TD-036 → CLOSED** — every sub-item verified fixed; two residuals re-filed as TD-041/TD-042
  rather than left buried.
- **TD-038 → REOPENED (regression)** — now BUG-063.
- **TD-005 re-scoped**, **TD-003 widened**, **TD-031** corrected (`prototypes/` does exist).
- **New:** TD-040 (two ability frameworks, no ADR), TD-041 (BUG-066 key guard),
  TD-042 (BUG-043 residual), TD-043 (orphan `.meta`-only directories).

#### `docs/registry/architecture.yaml`
**Cause:** R2 + a pre-existing syntax defect.
**Changed:** added the `vcontainer_dependency_injection` decision (ADR-0004). Corrected the
forbidden-patterns note: the allocating-`OverlapCircle` entry pointed at the **deleted**
`EntityWeaponMelee.cs`; `GameObject.Find` re-confirmed as the only remaining instance
(`FollowPlayer.cs:24`); the UI-mutates-state violation re-confirmed — it survived the DI refactor
and now writes through `IPlayerStatService` (`StatsUIController.cs:110,158,172`).
**Also fixed a pre-existing defect:** this file **did not parse as YAML** — an unquoted `:` inside a
`scope:` value, present at HEAD before this pass. A machine-readable registry that no parser can
read is worse than none. Converted to a block scalar; the file now parses.

#### `production/epics/index.md`
**Cause:** TD-036 closure; R2, R4, Item system.
**Changed:** the enemy-spawn epic's end-to-end blocker is recorded as gone. "Systems with code but
no epic" grew 4 → **7**; all three new entries shipped through un-storied commits.

#### `production/qa/bugs/BUG-052.md`
**Cause:** R2 (partial closure), R4, Item system.
**Changed:** one subsystem left the list (DI, now ADR-0004); three joined (Item, Abilities v2, UI).
The gap **grew rather than closed**.

#### `.claude/agent-memory/lead-programmer/MEMORY.md`
**Cause:** R1–R4.
**Changed:** re-synced. It had gone stale within three sprints of its last rewrite — noted in the
header as a recurring property of shortcut files, with a seven-line delta summary.

#### `docs/skill-reference.md`
**Cause:** file count drift.
**Changed:** 79 → **80** skills; **3 → 4 ADRs**, with ADR-0004's 20-day retroactive gap named.

#### `.claude/skills/module-quality-audit/SKILL.md`
**Cause:** R1.
**Changed:** path note. This skill's path-based scans would **silently return nothing** for moved
modules, reporting them as unchanged rather than as missing.

#### `Assets/Animation/DUPLICATE_NOTE.md`
**Cause:** `.claude/rules/language-reporting.md` (stored `.md` must be English) + re-verification.
**Changed:** rewritten in English with the original Vietnamese preserved verbatim below. Counts
**re-verified: 155 `.anim` in each folder**, still unresolved after 13 sprints. Added why this cannot
be settled from the repository alone (`.anim` files do not record their referencing Animator).

---

### Created

#### `docs/architecture/adr-0004-vcontainer-dependency-injection.md` — **new**
**Cause:** R2 — the largest architectural change in the project shipped with no ADR.
**Contents:** context (three competing resolution mechanisms, and the singleton pressure that
produced `LevelManager`/TD-023), the decision as implemented, five alternatives with reasons for
rejection — including `ScriptableObject`-as-service-locator, rejected because this project has
already been burned by SO persistence (TD-038/BUG-063) — consequences, the git-URL supply-chain
risk, a source-verified checklist, and three open questions.
**Status:** Accepted (retroactively documented). The 20-day gap between `aa4e620` and this ADR is
recorded in the ADR itself as the reason it exists.

#### `docs/CHANGELOG-DOCS.md` — this file.

---

### Archived (moved, not deleted)

| From | To | Reason |
|---|---|---|
| `docs/reviews/melee-combat-review.md` | `docs/archive/reviews/melee-combat-review.md` | Every class it reviews is deleted or rewritten. `WeaponMelee.Attack()` (section A) became `MeleeWeapon.OnActivate()` and is now the project's *reference* implementation — the opposite of the review's finding. `EntityWeaponMelee.cs` (section B) was **deleted**. Its header verdict ("both melee directions deal no reliable damage", "0 of 16 issues fixed") was true at HEAD `7995066` on 2026-07-30 and is false today. Content untouched; banner names each superseding document. |
| `docs/adoption-plan-2026-05-19.md` | `docs/archive/adoption-plan-2026-05-19.md` | A one-time `/adopt` onboarding checklist from project start, 13 sprints ago. Living in `docs/` invited reading it as an outstanding to-do list. Content untouched; banner points to the four documents that now carry its concerns. |

`docs/reviews/` is now empty and was removed. `docs/archive/` was created by this pass.

---

### Historical snapshots — banner added, content untouched

These are accurate records of their own date. They were **not rewritten**; each received a
`📅 HISTORICAL SNAPSHOT` banner because each was being read as a current assessment.

| Document | Why the banner |
|---|---|
| `docs/architecture/architecture-review-2026-07-13.md` | Its CONCERNS verdict and "8 GDDs · 3 ADRs" counts are from 2026-07-13; there are now 4 ADRs, ADR-0001's subject class is deleted, and three further systems shipped undocumented. |
| `production/qa/module-health-2026-08.md` | **Every** CRITICAL verdict in its scorecard has since been resolved; its "skill-ability-system HEALTHY" verdict is also stale after R4. |
| `design/balance/combat-balance-2026-07-07.md` | Two of its four cited source files no longer exist at those paths (`StatsSO.cs` deleted; `DerivedStatFormula.cs` moved). The **numbers were not re-derived** in this pass and may still be valid — the banner says so explicitly and names what to re-validate against. |

---

### Deliberately NOT changed

| Set | Count | Reason |
|---|---|---|
| `production/sprints/`, `production/retros/`, `production/qa/bug-triage-*`, `production/qa/playtests/`, `production/session-state/`, `design/gdd/gdd-cross-review-*` | ~59 | **Point-in-time records.** Their stale paths and bug statuses were correct on their own date. Rewriting them would destroy the project's history — the opposite of this pass's purpose. |
| `.claude/docs/*.md` (9 files), `.claude/docs/templates/` (38), `.claude/docs/hooks-reference/` (6) | 53 | Checked for drift (stale paths, `StatsSO`, `EntityWeaponMelee`, `TakeDamage(int`): **zero hits**. Generic tooling and blank templates, not project documentation. |
| `.claude/skills/` (79 of 80), `.claude/agents/` (26) | 105 | Engine- and project-agnostic. Only `module-quality-audit/SKILL.md` hardcoded project paths. |
| All source code | 194 `.cs` | **This pass was documentation-only by instruction.** Every defect found in code — BUG-063, BUG-065, BUG-066, BUG-064 sub-item 7, the orphan directories, the `AbilityHolder` null dereference — was **documented, not fixed**, and is tracked in `CLAUDE.md`, `docs/tech-debt-register.md` and `production/qa/bugs/`. |

---

### Findings that need a decision (documentation cannot resolve these)

1. **BUG-063** — `Stat.modifiers` re-serialized (`Stat.cs:63-65`). A one-line fix, no technical
   blocker, carried 24+ triage cycles, and it can corrupt committed `.asset` files. It has already
   happened once.
2. **Two ability frameworks, no ADR** (TD-040) — v1 drives weapons/enemies, v2 drives the player.
   Until this is decided, `design/gdd/skill-ability-system.md` cannot be made authoritative and
   Abilities v2 cannot be designed at all.
3. **The documentation gap is systemic, not incidental.** Four structural changes shipped across
   three sprints with zero doc updates, in a project that runs weekly wrap-ups and monthly module
   audits. Both rituals read documents rather than diffing the tree, so neither could see R1–R4.
   Consider a pre-commit or weekly check that diffs `Assets/Script/`'s directory list against
   `CLAUDE.md`'s Repository Layout.
