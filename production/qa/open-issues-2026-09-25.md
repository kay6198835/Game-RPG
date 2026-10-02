# Open issues — consolidated register, 2026-09-25

**Branch**: `origin/feature/fix-player-control` · **HEAD**: `c0067f4` ("fixing")
**Method**: every claim below was read from source or from a `.prefab` / `.asset` / `.meta` file at
this HEAD. Nothing is carried over on trust from the previous snapshot.

**Supersedes** `production/qa/open-issues-2026-09-22.md`. That file is kept as the record of what was
true three days ago; do not read it for current status.

---

## What changed since 2026-09-22

Three merges and two fix commits landed. The largest is **ADR-0005 Amendments 1-3**, which rebuilt
the character layer around shared bases and changed the Abilities v2 damage contract. That refactor
closed five bugs outright as a side effect and merged two more into one.

| Commit range | What it did |
|---|---|
| `2aa225e` … `ece3257` | ADR-0005: `ICharacter` identity, `CharacterInputBase`, `NegativeReceiverBase`, `StatHandlerBase`, `VitalStatsBase`, `MovementBase`/`IMovement`, `WeaponHolderBase`/`IWeaponHolder`, `AbilityHolderBase`/`EntityAbilityHolder`, `CharacterData`. `EntityAttack.cs` deleted |
| `379c267` | merge of the above into this branch |
| `b0b1337` "fix bug" | BUG-074 (delta not total), BUG-071 part 1 (`StartCoroutine`), `RangeWeapon` field attribute, `DamageReceiverBase` renamed `NegativeReceiverBase` |
| `c0067f4` "fixing" | BUG-071 part 2 (dead `count` removed, parameter renamed), BUG-082 residual (`= new()` on three lists) — **and introduced BUG-092** |

### Closed since the last snapshot

| Bug | Closed by | Evidence |
|---|---|---|
| **BUG-043** | `f7d98b1` | `EntityAttack.cs` deleted; the hardcoded `TakeDamage(10, …)` is gone |
| **BUG-068** (main defect) | ADR-0005 | `AbilityHolderBase.cs:121` null-guards before the dereference |
| **BUG-074** | `b0b1337` | `Utility.cs:304` returns `addedValue - baseValue` |
| **BUG-080** | ADR-0005 | `ResourceReceiver` is now `: Interact` with an empty body |
| **BUG-081** | ADR-0005 | one `INegativeReceiver` implementer; enemy prefabs carry 0 player receivers |
| **BUG-082** (residual) | `c0067f4` | all three `AbilityEffectDefinition` lists carry `= new()` |
| **BUG-088** | `379c267` + `b0b1337` | `Random.Range(30, 75)/100f` |
| **BUG-091** | `379c267` | committed; no residual |

### Merged

**BUG-066 + BUG-070** are no longer two instances of one defect — the unguarded dictionary moved into
the shared `VitalStatsBase.cs` and they are literally the same seven lines. One fix closes both.

---

## 0. Read this first — what is currently blocking what

| Rank | Item | Cost | Why it is first |
|---|---|---|---|
| **1** | **BUG-092** — the project does not compile | minutes | `perTime` deleted but still used. Nothing below can be verified in the Editor until this is fixed |
| **2** | **BUG-072** — set `layerMask` on `Lightning.prefab` | minutes | Code has been complete since 2026-09-22. One Inspector field. Cheapest demo-blocking fix in the register |
| **3** | Play Mode smoke on the four Paladin abilities | ~0.2d | Confirms BUG-075 and BUG-072 together. Everything about Abilities v2 damage is static analysis until this runs |
| **4** | **BUG-086** then **BUG-065** then **BUG-087** | ~0.5d | Death recovery. Strict order: stop the per-frame emit, stop the slide, then add the first subscriber |
| **5** | **BUG-066/070** — one dictionary guard | ~0.2d | Was two sites, now one. `:28` is on the live death path |

**Two compile breaks in four days** (`723fab1` → BUG-088, `c0067f4` → BUG-092) with zero detection.
That is **TD-048** (pre-push compile check) and it is now the highest-value process item in the
project. It does not depend on BUG-084 and does not need a test framework — a compiler is enough.

---

## 1. Blocker

### BUG-092 — `RecoveryReductionPerTimeForDuration` does not compile · S1 · **new, open**

`c0067f4` deleted `public float perTime;` and left both uses:

```csharp
vital.RecoveryPerTimeForDuration(statType, impactValue, perTime, timeCount);   // :14
vital.ReductionPerTimeForDuration(statType, impactValue, perTime, timeCount);  // :17
```

`grep -rn "perTime" Assets --include=*.cs` returns 13 lines, none of them a declaration visible to
this class — every other hit is a *parameter* name in `VitalStatsBase` or `IVitalComponent`. Expect
`error CS0103` twice.

**Second defect in the same file, do not close without it**: `timeCount` (`:8`) is `private` with no
`[SerializeField]`, so Unity never serializes it and `[Range(1, 10)]` draws nothing. It is `0` on
every asset, and `VitalStatsBase.RecoveryPerTime` decrements before testing, so **every HoT/DoT
degenerates to exactly one instant tick**, silently. `duration` (`:6`) is now read by nothing.

**Fix**: serialize both fields, delete `duration`, then set them on the assets before judging the
effect.

---

## 2. Abilities v2

### BUG-072 — summon abilities deal no damage · S2 · **code complete, one Inspector step**

`LightningController.Execute():22-32` does the overlap query, assigns `_context.Target` and invokes
once per target — the correct set-then-invoke shape, matching `SpawnProjectileBase.cs:32-33`.

`Assets/Prefab/Particle Effect/Lightning.prefab` serializes **only** `randomIndex: 0` (`:122`). No
`layerMask` key, and a `LayerMask` field has no initialiser, so it is `0` = *Nothing* and
`OverlapCircleNonAlloc` returns 0 hits on every call, silently. `radius` and `maxTargets` are fine —
they have field initialisers (`= 2f`, `= 4`) that survive a missing key.

No safe code default exists: a literal layer index is forbidden by `.claude/rules/engine-code.md`.

**Verify with an enemy losing HP, not with a clean Console.**

### BUG-071 — HoT/DoT cannot be stopped · S3 (was S2) · **partial**

Fixed: the missing `StartCoroutine` (`b0b1337`), the dead `count` and the seconds-vs-ticks parameter
(`c0067f4`).

Still open: `VitalStatsBase.cs:81` and `:92` recurse through the **public wrapper**, so each tick
starts a fresh coroutine and no handle is kept. Consequences — nothing can `StopCoroutine` it (a DoT
survives death, room transitions and a pooled enemy respawn), and `StartCoroutine` on a component
disabled between two ticks throws. Correcting the original report: this is **not** a leak; exactly
one coroutine is alive at a time.

Also: `IVitalComponent.cs:13-14` still names that parameter `duration` and now disagrees with every
implementation. The comment at `VitalStatsBase.cs:63` ("never started") is stale.

**Not observable until BUG-092 is fixed** — the only caller passes `timeCount = 0`.

### BUG-083 — `HoldTime` and `HoldRatio` are always `0f` · S2 · open

`CanStart():93` calls `BuildContext()`; `StartHold():101` zeroes `CurrentHoldTime` afterwards;
nothing rebuilds the context. Masked by content — all four Paladin abilities have `MaxHoldTime = 0`.

New option for the fix: the context now carries `AbilityInstance` (`AbilityContext.cs:14`), so a
charge-scaling effect could read `CurrentHoldTime` live instead of the context being made rebuildable.

### BUG-079 — `AbilityInstance.Exit()` is commented out · S3 · open, dormant

`AbilityInstance.cs:63-66`, unchanged, comment still names the pre-rename `SkillState`.

**Checked this pass**: the enemy path does **not** widen this. `EntityAbilityState.Enter():27` calls
`abilityHolder.StartHold()`, which forces `ChangeState(AbilityState.Start)`, exactly as the player's
input path does; and `LogicUpdate():61-63` leaves `Exit` by changing to `IdleState`.

### BUG-068 — one property still dereferences unguarded · S3 · open (scope reduced)

`GetAbility()` is fixed. `AbilityHolderBase.cs:28`:

```csharp
public AbilityState CurrentAbilityState => currentAbility?.State ?? AbilityState.Start;        // :27
public AbilityActivationType CurrentActivationType => currentAbility.Definition.ActivationType; // :28
public AbilityDefinition CurrentDefinition => currentAbility?.Definition;                       // :29
```

Widened exposure: enemies now run this property through `EntityAbilityHolder`, on pooled objects
whose `currentAbility` is null until the first `TryDoAbility()`.

### BUG-073 + BUG-090 — missing-script references · S3 · open · bundle them

| GUID | Referenced from | `.meta` files resolving it |
|---|---|---|
| `ac9ac7c011812d042ac992007bc0cf48` | `ShootSpirit.asset:12` | **0** |
| `6f896bb258601fc4cb5fc183399618ea` | `Has Enough Mana Condition.asset` | **0** |

Both assets still in the tree, unchanged by any commit this week. `SpiritBomd.asset` carries
`ActivationType: 1` and is the only Hold ability outside the Paladin set — note that before deleting
it, since it is the only asset-level coverage of the Hold path.

### BUG-089 — gain-tier scaffolding · **closed by design** · reopen at demo/release

Unchanged. `production/qa/bugs/BUG-089.md` holds the five-step specification and the seven-point
re-check list. Do not author a per-effect `Costs` list until the tier fields exist.

---

## 3. Player — damage, death, vitals

### BUG-087 — death is a permanent hard lock · S1 · open · **scope reduced**

Re-verified all four claims at this HEAD:

| Claim | Result |
|---|---|
| `ON_PLAYER_DEATH` subscribers | 2 grep hits total — the emitter and the enum member. **0 subscribers** |
| `class GameManager` | **0 files** |
| `ON_REALOAD_GAME` | 1 hit — the enum member. 0 emitters, 0 subscribers |
| player has no reset path | **CHANGED** — it now has one |

`VitalStatsBase.Reborn():20-24` re-seeds `currentStats` from `statHandler.GetFullStat()`, and
`VitalStatsComponent` inherits it. The mechanism exists; it has no caller and no owner. The enemy
side calls it from `OnEnable()` (`EntityVitalStats.cs:7-10`) because pooled enemies are re-enabled —
the player is not pooled, so a revive must call it explicitly.

Shape of the fix is unchanged: a `GameManager` as a scene component registered in
`GameLifetimeScope`, **not** a singleton.

### BUG-086 — `ON_PLAYER_DEATH` fires every frame · S2 · **do before BUG-087**

`PlayerDeathState.LogicUpdate():14-20` never consumes `Status`, never changes state. Violates the
durable-`Status` contract in `.claude/rules/manager-event-code.md`. Attaching a scene reload to an
event firing 60×/second is worse than the current silence.

### BUG-065 — the player keeps sliding while dying · S2 · open · **now a one-liner**

`PlayerDeathState.Enter():10-13` is still `base.Enter();` alone. What changed is the tooling:
`IMovement` (ADR-0005 Amendment 2) exposes `Lock(object source)` / `Unlock(object source)`.

**Use `Lock(source)`, not `Stop()`** — `Stop()` zeroes velocity for one call and the next
`PhysicsUpdate` writes it again. Every `Lock` needs a paired `Unlock`; for death that pairing belongs
in the revive, which does not exist yet (BUG-087). Note the debt when adding it.

### BUG-066 + BUG-070 — unguarded dictionary indexer · S2 · open · **merged, one site**

`VitalStatsBase.cs:28,39,41,45,52,54,58`. `:28` is read every frame by `PlayerBasicState` to decide
the death transition.

A naive `TryGetValue ? v : 0f` on `GetCurrentStatValue` would make a missing HP key trigger death.
Audit the seeding in `Reborn()` at the same time — this is a two-part fix, not a one-liner.

---

## 4. Weapons, data, build, process

### BUG-064 sub-item 7 — ranged weapons silently never fire · S1 · partial

`b0b1337` removed the inert `[SerializeField]` from the interface field. Nothing assigns it:

| `RangeWeapon.cs` | |
|---|---|
| `:7` | declaration |
| `:16` | `&& poolManager != null` inside `CanAttack()` |
| `:45` | `poolManager.Spawn(...)` |

No `[Inject]`, no Inspector path. `CanAttack()` is permanently false, so the weapon does nothing —
no exception, no log. Fix with method injection in the shape of `AbilityHolderBase.Construct():51-55`;
note a weapon is pooled, so it must be injected at spawn time, not registered in the container.

### BUG-063 — `Stat.modifiers` serialized again · S1 · **ACCEPTED (deferred)**

`Stat.cs:63-66` unchanged. Owner decision stands: keep through development, remove at demo prep.
Standing mitigation: check `git status` for a dirty `Assets/SO/Stat/*.asset` after any Play Mode
session. This is now the only known open instance of the "runtime state on a shared asset" family —
BUG-077 was closed by deleting the class, BUG-082's residual closed on 2026-09-25.

### BUG-084 — no test can be written today · S2 · open · precondition, not a symptom

`find Assets -name "*.asmdef"` = **0**. `tests/EditMode` and `tests/PlayMode` are siblings of
`Assets/`. Unchanged.

A test pipeline would not have caught either compile break this week — a compiler would. Keep this
separate from **TD-048**, which is hours of work and is what actually hurts right now.

### BUG-052 — live subsystems with no ADR · S3 · open, narrowed

ADR-0005 and its three amendments now cover the character layer, which was the largest gap. Still
uncovered: the Item system, Abilities v2 as a whole (its GDD), and the UI layer.

### TD-050 — orphan `.meta` from the `NegativeReceiverBase` rename · S4 · new

`DamageReceiverBase.cs.meta` has no `.cs`; `NegativeReceiverBase.cs` has no `.meta`. Orphan GUID
`8c1a7c078b91417ba4bfdf0c016f48ae` is referenced by no prefab, asset or scene, and the class is
abstract, so nothing breaks — but the same slip on a concrete MonoBehaviour is exactly BUG-073 and
BUG-090. Delete the orphan `.meta`.

---

## 5. Legacy CLAUDE.md bugs — #12 to #17

**Not re-verified in this pass.** They were last read against source on 2026-09-22 and no commit
since has touched `Assets/Script/Map/`, `Assets/Script/LevelEdit/` or the room JSON loader. Treat the
2026-09-22 entries as current but unconfirmed; re-read before scheduling any of them.

| # | Summary | Location |
|---|---|---|
| 12 | `LevelManager` singleton reach-through (TD-023) | `LevelManager.cs:10` |
| 13 | start-room teleport commented out | `RoomGridController.cs:82` |
| 14 | `MazeController.Awake()` missing `return` after `Destroy` | `MazeController.cs:17` |
| 15 | room JSON via `File.ReadAllText(Application.dataPath…)` — Editor-only | `RoomGeneraterController.cs:69` |
| 16 | `RoomType` never read; start/end forced by list position | `RoomGeneraterController.cs:47` |
| 17 | dead door-gating code that reads as live | `DoorController.cs:29` |

---

## 6. Counts

| | |
|---|---|
| Bug files in `production/qa/bugs/` | **32** (31 + BUG-092) |
| Closed / fixed | **15** — 053, 067, 069, 074, 075, 076, 077, 078, 080, 081, 082, 085, 088, 089, 091 |
| Accepted (deferred) | **1** — BUG-063 |
| Partial | **2** — BUG-064, BUG-071 |
| Open | **14** — 052, 065, 066, 068, 070, 072, 073, 079, 083, 084, 086, 087, 090, 092 |

The six legacy `#12`–`#17` items are tracked in CLAUDE.md's table, not as bug files, and are not
included in the counts above.

⚠️ The `Open bugs: NN` line printed by `.claude/hooks/session-start.sh` is **not a count of open
bugs**. `:29-34` iterates over both `production/qa/bugs` and `production` recursively and adds the
results, so every file is counted twice, closed or not. It reported 62 for 31 files. Ignore it, or
fix the hook.

---

## 7. Suggested order

1. **BUG-092** — restore `perTime`, serialize `timeCount`, delete `duration`. Unblocks everything.
2. **BUG-072** — one Inspector field on `Lightning.prefab`.
3. **Play Mode smoke** on all four Paladin abilities. Closes S14-01 / S14-14 and converts every
   "static analysis only" caveat in this document into a verified statement.
4. **BUG-086 → BUG-065 → BUG-087**, strictly in that order.
5. **BUG-066/070** — one guard, plus a seeding audit.
6. **BUG-064-7** — `[Inject]` on `RangeWeapon`.
7. **BUG-073 + BUG-090 + TD-050** — one Editor session, all three are dangling references.
8. **BUG-071** defect 3, **BUG-083**, **BUG-079**, **BUG-068** `:28`, and the hardcoded
   `AbilitySlot.Utility` at `PlayerInputHandle.cs:246`.
9. **TD-048** — pre-push compile check. Schedule it before the third compile break.
