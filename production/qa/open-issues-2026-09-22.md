# Open issues — consolidated register, 2026-09-22

> **Snapshot after commit `e306c63`** (branch `origin/feature/fix-player-control`, pushed).
> Written because the per-bug files are now long enough that nobody can hold them in their head.
> Each entry below carries the context needed to act on it **without** reading anything else:
> what is wrong, where, why it matters, what fixing it takes, and what it depends on.
>
> This file is a **snapshot, not a source of truth**. The per-bug files under `production/qa/bugs/`
> remain authoritative and carry the full evidence chains and the dated correction history.
> Regenerate or delete this file rather than letting it drift.
>
> **Counts:** 31 bug files — 10 closed/fixed, 1 accepted-deferred, 1 partial, **19 open**.

---

## 0. Read this first — what is currently blocking what

| Blocker | Blocks |
|---|---|
| **BUG-088** — `SetPositionToCheck()` is a silent no-op | nothing structurally, but the enemy knockback-reposition feature does not work and nothing reports it |
| **BUG-084** — no `.asmdef` anywhere, `tests/` outside `Assets/` | TD-014, and **every** sprint story that estimates "write the first EditMode test" at 0.3d |
| **BUG-086** — `ON_PLAYER_DEATH` fires every frame | **BUG-087** — do not add a subscriber to that event until this is fixed |
| **BUG-072** — `layerMask` not set on `Lightning.prefab` | summon abilities dealing damage; code is complete, this is one Inspector field |
| No Abilities v2 GDD | **BUG-089** (the gain-tier feature), and the sustain-vs-purchase question |
| No ADR choosing v1 vs v2 (**TD-040**) | `design/gdd/skill-ability-system.md` being authoritative again |

---

## 1. Abilities v2

### BUG-072 — summon abilities deal no damage · S2 · **code complete, one Inspector field left**

**Context.** Abilities v2 is the player's only ability path. A summon effect
(`SpawnSummonEffect`) spawns a prefab, which fires its payload from a Unity Animation Event calling
`Execute()`. `LightningController.Execute()` had its target query as three comment lines, so it
called `base.Execute()` — which invokes the damage callback — without ever assigning
`Services.NegativeReceiver`. The effect's `if (negativeReceiver != null)` guard then skipped
silently.

**State now.** Implemented in `8b23174`:

```csharp
int n = Physics2D.OverlapCircleNonAlloc(transform.position, radius, _buffer, layerMask);
for (int i = 0; i < n; i++)
{
    if (!_buffer[i].TryGetComponent<INegativeReceiver>(out var r)) continue;
    _context.Services.NegativeReceiver = r;
    base.Execute();          // = _callback.Invoke(_context)
}
```

`_buffer` is allocated in an `Awake()` override sized by `maxTargets` (currently `4`), because a
serialized `Collider2D[]` is given **length 0** by Unity and `OverlapCircleNonAlloc` would have
returned `0` on every call. `base.Awake()` is preserved — `SpawnSummonBase.Awake()` caches the
Animator that `Launch()` uses.

**What is left.** `layerMask` ships as `0` = *Nothing*, matching no layer. It has no safe code
default: a literal would be a hardcoded layer index, which `engine-code.md` and `ai-code.md` both
forbid. **Set it on `Assets/Prefab/Particle Effect/Lightning.prefab`** to the same enemy layer
`MeleeWeapon` uses via `WeaponStats.LayerMask`. Also sanity-check `radius` (defaults to `2f`, a
placeholder) against the Lightning VFX footprint.

**How to verify.** An enemy standing inside a Consecrate strike loses HP. **Not** by a clean Console
— every failure mode in this bug has been silent.

**Note.** Damage lives on the effect asset (`baseDamage: 30` on
`Paladin Spawn Consecrat Effect.asset`) while `radius` and `layerMask` live on the prefab. Tuning
this ability means two files.

---

### BUG-071 — heal-over-time and damage-over-time never run · S2 · open

**Context.** `VitalStatsComponent.RecoveryPerTimeForDuration()` and `ReductionPerTimeForDuration()`
(`VitalComponent.cs:63-75`) build a coroutine iterator and **never call `StartCoroutine`** on it. A
C# iterator method does nothing until enumerated, so **not even the first tick runs** — these are
complete no-ops. The sibling `BuffDebuffForDuration` (`:102-105`) does it correctly and is the
pattern to copy.

**Two more defects hide behind it**, and fixing only the `StartCoroutine` leaves both:

1. `var count = duration / perTime;` is computed, incremented, and then **discarded** — `duration`
   is passed where a tick count is expected.
2. The coroutine recurses back into the public wrapper rather than looping, so once started it would
   leak one coroutine per tick.

**Fix.** Rewrite as a single `while` loop with an explicit tick count, started once with
`StartCoroutine`. Roughly 15 lines. Fix all three defects in one pass.

**Who uses it.** `RecoveryReductionPerTimeForDuration` — a live effect base with a
`[CreateAssetMenu]`, so a designer can author a DoT/HoT ability at any time and it will silently do
nothing.

---

### BUG-074 — stat effects apply the modifier **total**, not the **delta** · S3 · open

**Context.** `StatsEffectBase.Apply()` (`StatEffectBase.cs:23-30`) groups a `StatModifierGroup` by
`StatType`, then calls `Utility.ModifierStatsCalculate(modifiers, currentValue)` and passes the
**result** as the recovery/reduction amount:

```csharp
float currentStatVital = context.Services.Vital.GetCurrentStatValue(currentStatTypeKey);
float impactValue      = Utility.ModifierStatsCalculate(kvp.Value, currentStatVital);
ApplyImpact(context, currentStatTypeKey, impactValue);
```

`ModifierStatsCalculate` returns the **new total**, not the change. So a `PercentAdd 0.10` modifier
on a player at 100 HP heals **110**, not 10.

**Fix belongs at the call site, not in the helper:** `delta = total - currentStatVital`.
`ModifierStatsCalculate` has exactly **one** caller project-wide, so changing either is safe — but
the helper's contract ("compute the modified total") is the correct one and other callers will want
it.

**Affects.** Every `RecoveryReductionStatsEffect` and `RecoveryReductionPerTimeForDuration` asset —
i.e. all the Paladin healing content.

---

### BUG-083 — `HoldTime` and `HoldRatio` are always `0f` · S2 · open

**Context.** Charge-scaling effects read `AbilityContext.HoldTime` / `.HoldRatio`. Both are plain
fields (`AbilityContext.cs:11-12`), snapshotted once by `BuildContext()` — which is called from
`CanStart()` (`AbilityInstance.cs:93`), i.e. **before** `AbilityHolder.StartHold()` zeroes the
counter. Nothing rebuilds the context afterwards, and `Tick()` increments `CurrentHoldTime` on the
instance, not on the snapshot.

```
PlayerInputHandle.cs:254  TryDoAbility()  →  CanStart()  →  BuildContext()   ← snapshot taken
PlayerInputHandle.cs:256  StartHold()     →  CurrentHoldTime = 0f
                          Tick() increments CurrentHoldTime every frame
                          …context never rebuilt…
                          effects read ctx.HoldTime  →  always 0f
```

**Masked today:** all four Paladin abilities ship `MaxHoldTime = 0`, which forces the
`holdRatio = 0f` branch at `:111-115` regardless.

**Fix.** Make `HoldTime` / `HoldRatio` computed properties reading back through
`context.AbilityInstance`, or rebuild the context at each `CastInstant()` dispatch.

**Why it matters beyond itself.** If the BUG-089 gain tiers are meant to be reached by holding
longer, `HoldRatio` is the natural driver — so this gates that feature too.

---

### BUG-089 — the gain-tier feature is unimplemented · S3 · dormant · **spec, not a patch**

**Design, as stated by the owner.** A per-effect `Costs` entry is the price of an **upgrade tier**
(*"cột mốc sức mạnh"*). Cannot afford it → the effect still runs, at its **default** level. Can
afford it → the cost is charged and the effect runs at its **gained** level.

Under that design, `Execute()` calling `Apply()` unconditionally is **correct**, and the earlier
reading of this bug ("a refused effect is applied anyway") is **withdrawn**.

**The implementation does not deliver the design.** Seven gaps; the first is decisive.

| # | Gap | Severity |
|---|---|---|
| 1 | **Paying buys nothing.** `Apply(AbilityContext)` receives no record that a cost was paid, and every concrete effect applies one fixed serialized value — `SpawnProjectileEffect.baseDamage`, `SpawnSummonEffect.baseDamage + PhysicalDamage`, `StatsEffectBase.statModifier`, `BuffDebuffStatsForDuration.statModifierGroup`. There is exactly one power level in the code, so affording the tier is **strictly worse** than not affording it | blocks the feature |
| 2 | A refusal calls `CancelHold()`, clearing the flag `CastInstant():52` tests, which terminates the **whole channel** for **all** effects — instead of dropping that one effect to default | high |
| 3 | `Costs` is a `List<StatCost>` that `CheckPayCostValid()` requires to be affordable **in full** and `PayCost()` charges **in full** — one all-or-nothing bundle, not a tier ladder. A ladder needs `List<AbilityGainTier>` with a cost bundle and a power delta per tier | high, data model |
| 4 | Sustain-vs-purchase is undefined: while held, each animation loop re-enters `Casting()` and charges again. Nothing records that a tier was already bought | medium, decide before authoring |
| 5 | If tiers are charge-driven, `HoldRatio` is the input — and it is always `0f` (**BUG-083**) | medium |
| 6 | `Casting()` walks effects in list order and charges as it goes, so `Effects[0]` gets first claim on mana and Inspector order silently becomes balance | low |
| 7 | `SpawnSummonEffect.TryCast()` spawns the Cast-phase telegraph **after** the gate, so a refused tier removes Consecrate's RuneCircle entirely while the `Do`-phase Lightning still fires | low, visible |

**Dormant.** Every live effect asset has `SubConditions: []` and **no serialized `Costs` key at
all** — the field was added in `73ab8e7`, after those assets were authored, so Unity materialises an
empty list on load. `TryCast()` therefore returns `true` unconditionally and none of the above can
fire today. It becomes live the moment a designer fills either field in.

**Next step when picked up:** write the tier model into the Abilities v2 GDD *first* — ladder shape,
purchase vs sustain, what "gained" changes, and which input selects the tier. Gaps 1, 3 and 4 are
design decisions the code cannot infer.

---

### BUG-079 — `AbilityInstance.Exit()` body is commented out · S3 · open

**Context.** `Exit()` (`:63-66`) is empty; the commented line still names the pre-rename
`SkillState`. An earlier doc claimed this made abilities "castable once per scene load" — **that was
wrong and is corrected**: `AbilityHolder.StartHold()` (`:142`) forces
`ChangeState(AbilityState.Start)` on every fresh press, so abilities do re-cast.

**What remains** is a layering defect: the instance cannot reset itself, and any activation path that
does not go through `StartHold()` gets a stuck instance.

**Why it may become load-bearing.** Any fix that routes a refusal or an abort through
`ChangeState(AbilityState.Exit)` parks the instance there until the next `StartHold()`.

**Fix.** Uncomment the line, correcting `SkillState` → `AbilityState`. Then decide whether
`AbilityHolder.StartHold()` should still force the state or trust the instance.

---

### BUG-068 — `GetAbility()` dereferences a discarded `TryGetValue` result · S3 · dormant

**Context.** `AbilityHolder.cs:93-99`:

```csharp
_equipped.TryGetValue(slot, out var instance);        // bool discarded
currentAbility = instance;
core.Player.Anim.runtimeAnimatorController = currentAbility.Definition.AnimatorOverride;   // :97
```

**Zero callers project-wide** — dead code carrying a live defect. Second half of the same bug:
`CurrentActivationType` (`:22`) dereferences `currentAbility` unguarded while `CurrentAbilityState`
(`:21`) directly beside it uses `?.`; its one caller is safe only by accident of call ordering.

**Fix.** Delete the method, or guard it. Do not leave a zero-caller method holding an unguarded
dereference.

---

### BUG-073 + BUG-090 — missing-script references on the ShootSpirit assets · S2 / S3 · open · **bundle**

**BUG-073.** `Assets/SO/Skill/ShootSpirit/MainEffect/ShootSpirit.asset:12` names script guid
`ac9ac7c011812d042ac992007bc0cf48`, which resolves to **0** `.meta` files under `Assets/`. Still
reachable: `PlayerTest.prefab` → `SpiritBomd.asset` → this asset. Separately,
`SpawnEffectBase.cs:14,30` still log the prefixes of deleted classes
(`[ShootSpiritOrbEffect]`, `SpiritOrbProjectile`).

**BUG-090.** A *second* broken reference in the same pair, created by the correct fix for BUG-077.
`2a83469` deleted `HasEnoughManaCondition.cs` but left
`Assets/SO/Skill/Conditions/Has Enough Mana Condition.asset`, whose `m_Script` guid
`6f896bb258601fc4cb5fc183399618ea` now resolves to 0 files. `ShootSpirit.asset` and
`SpiritBomd.asset` both still reference that asset.

Note the distinction that caused confusion: the **script** was deleted (correct — that is BUG-077);
the **asset instance** of that script was not.

**Fix.** One Editor session, no code. Delete the orphaned `.asset` and its `.meta`, then clear the
references in the two ShootSpirit assets. **Decide there whether the ShootSpirit pair is kept at
all** — if it is deleted, both bugs close together and there is nothing else to do.

---

## 2. Player — damage, death, vitals

### BUG-087 — death is a permanent hard lock · S1 · open · **feature, not a fix**

**Context.** The demo target is "start menu → dungeon run → death/restart". The death third does not
exist:

- `ON_PLAYER_DEATH` has **zero subscribers** — only `PlayerDeathState.cs:18` emits it
- `grep GameManager Assets --include=*.cs` returns **0 hits**; no `GameManager` exists
- `VitalStatsComponent` has **no reset path at all** — `currentStats` is filled once in `Start()`
- `ON_REALOAD_GAME` has 0 emitters and 0 subscribers
- `PlayerData.Reborn()` still has no caller

So the player dies, the event fires into nothing, and the run cannot be restarted.

**The enemy side already has the pattern to copy:** `EntityVitalStats.Reborn()` (`:31-35`), called
from both `Start()` (`:23`) and `OnEnable()` (`:28`).

**Fix.** A `GameManager` as a scene component registered in `GameLifetimeScope` — **not** a
singleton, per `.claude/rules/manager-event-code.md` — plus a `VitalStatsComponent.Reborn()` and a
subscriber for `ON_PLAYER_DEATH`.

⚠️ **Do not add that subscriber until BUG-086 is fixed** — the event currently fires every frame.

---

### BUG-086 — `ON_PLAYER_DEATH` is emitted every frame · S2 · open · **do before BUG-087**

**Context.** `PlayerDeathState.LogicUpdate()` (`:14-20`):

```csharp
public override void LogicUpdate()
{
    if (Status == StatusAnimation.EndRangeTrigger)
    {
        EventManager.Emit(EventID.ON_PLAYER_DEATH);
    }
}
```

It never consumes `Status` and never changes state, so once the animation reaches
`EndRangeTrigger` the event fires on **every subsequent frame**, unbounded.

This violates the durable-`Status` contract in `.claude/rules/manager-event-code.md`: *"`Status` is
durable state, not a one-frame pulse: the state that acts on a value is responsible for writing a new
one to consume it."*

**Fix.** Consume the status (write a different `StatusAnimation` after emitting), or guard with a
`hasEmitted` flag, or transition out of the state.

---

### BUG-065 — the player keeps sliding while dying · S2 · open

`PlayerDeathState.Enter()` (`:10-13`) only calls `base.Enter()`. It does not stop `PlayerMovement`,
so residual velocity carries the corpse across the room during the death animation. An earlier doc
claimed BUG-044's fix "properly stops PlayerMovement" — that claim was false and this bug is what
was split out of it.

---

### BUG-070 + BUG-066 — unguarded dictionary indexer, two instances · S2 · open · **bundle**

**The same defect on both sides of the game.**

| Bug | File | Sites |
|---|---|---|
| BUG-070 | `VitalComponent.cs` (class `VitalStatsComponent`) | `:28, 39, 41, 46, 52, 54, 58` |
| BUG-066 | `EntityVitalStats.cs` | `:39, 49, 55, 61, 67` |

Both index `currentStats[statType]` with no key-existence check. Any `StatType` absent from that
character's stat profile throws `KeyNotFoundException`.

**BUG-070's `:28` is on the live death path** — `PlayerBasicState.cs:74` reads
`GetCurrentStatValue(HP)` to decide the death transition.

Both `.claude/rules/ai-code.md` and `gameplay-code.md` require the guard.

**Fix.** One shared helper (`TryGetCurrent(StatType, out float)` or a `GetOrDefault`), applied on
both sides. Estimated 0.2d for the pair; do not fix one without the other.

---

### BUG-080 — `ResourceReceiver.vitalStatsComponent` is resolved in the wrong place · S1 · open

**Context.** `ResourceReceiver.cs:5` declares the field, and it is assigned **only** inside
`ReceverModifierGroup()` (`:13`):

```csharp
VitalStatsComponent vitalStatsComponent;            // :5 — never assigned in Awake/Start
...
public void ReceverModifierGroup(StatModifierGroup g)
{
    Core.GetCoreComponent<VitalStatsComponent>(out vitalStatsComponent);   // :13 — only writer
    vitalStatsComponent.ApplyBuffDebuff(g);
}
```

`Recovery()`, `Reduction()` and `BuffDebuffForDuration()` all dereference it without that call
having run. On the live item-pickup path — pick up a healing consumable before any buff item — that
is a `NullReferenceException`.

**Fix.** Resolve it once in `Awake()`/`Start()` like every other core component, and remove the
resolve from `ReceverModifierGroup()`.

**History.** Split out of BUG-067 (which is fixed); it is a separate defect that happened to live in
the same file, and was **not** closed with it.

---

### BUG-081 — two `INegativeReceiver` implementers on the player, both on the prefab · S2 · open

**Context.** `NegativeReciver.cs:5-11` and `ResourceReceiver.cs:29-35` have **byte-identical**
`TakeDamage()` bodies, and **both components sit on `Assets/Prefab/Player/PlayerTest.prefab`**
(verified by script GUID). Which one receives a given hit is decided by collider layout, not by
design.

Identical today, so behaviour is unaffected — **the trap is that the next edit to one silently
creates two different damage behaviours on the same character.** This is the exact shape of BUG-053,
which shipped broken on the enemy side for six sprints.

`.claude/rules/ai-code.md` states the one-implementer rule for enemies. No equivalent rule exists for
the player.

**Secondary, same bug.** `Assets/SO/Database/EnemyPrefab.prefab` carries the **player**
`NegativeReciver` (a `CoreComponent<Core>`) and no `EntityNegativeReciver`. Establish whether that
prefab is live or a dead template.

---

### BUG-088 — `SetPositionToCheck()` is a silent no-op · S3 (was S1) · **partial**

**Context.** Added in `723fab1` to reposition an enemy toward its attacker when it takes damage.
Shipped with two identifiers that do not exist, which blocked the whole project from compiling. Those
are fixed (`8b23174`). What remains:

```csharp
var rangeToCheck = Random.Range(10, 100)/100;      // ← both args are int literals
```

`Random.Range(int, int)` returns an `int` in `[10, 99]`. `int / int` is **integer division**, and the
largest possible numerator is 99, so `99 / 100 == 0` — **the result is `0` for every draw**. `var`
infers `int`. `Vector2.Lerp(a, b, 0f)` returns `a`, so:

```
endPosition = transform.position      // the entity's own current position
```

`CheckNearPostion(endPosition)` is then immediately true and nothing moves. The feature does nothing,
throws nothing, and logs nothing.

**Fix — one character.** `Random.Range(10, 100) / 100f`, or clearer: `Random.Range(0.1f, 1f)`.

**Two smaller items in the same five lines:**

- The parameter `endPosition` shadows the field of the same name (`:11`), which is why the body needs
  `this.`. Rename the parameter.
- `EntityNegativeReciver.cs:27` dereferences `entityMovement` with no guard, on the live damage path.
  `Core.GetCoreComponent<T>(out …)` returns silently `null` when nothing matches, so any enemy prefab
  without an `EntityMovement` under its `EntityCore` throws on the first hit it takes.

---

## 3. Data, build and process

### BUG-063 — `Stat.modifiers` is serialized again · S1 · **ACCEPTED (deferred) by owner**

**Decision, 2026-09-22.** The `#if UNITY_EDITOR [SerializeField]` on `Stat.cs:63-66` **stays through
development** and is removed at demo prep, because seeing live modifiers in the Inspector is useful
while the stat system is being built. This is a deliberate trade and supersedes the "one-line fix,
no blocker, carried 29+ cycles" framing that appears in older documents. **Not scheduled.**

**What the acceptance costs, so it can be re-evaluated with the facts:**

- Play Mode in the Editor **is** `UNITY_EDITOR`, so runtime buffs applied during a play session are
  written back into the `.asset` and marked dirty. The guard protects only the player build, where
  `.asset` files are read-only anyway.
- It has already reached git once: two `STR +1 Flat` modifiers were committed into
  `PlayerStats.asset` and `Test.asset`, cleaned on `sprint-10` (TD-038).
- Nothing reports it. No warning, no log — the diff only shows up if someone reads the `.asset` in a
  commit.

**Mitigations while the acceptance stands:**

1. **Check `git status` after any Play Mode session.** A dirty `Assets/SO/Stat/*.asset` you did not
   edit in the Inspector is the leak.
2. If the debug view is the point, a `[SerializeField] private List<StatModifier> debugView`
   populated in `OnValidate`, or a custom Inspector drawer over `modifiers`, gives the same view
   without persisting anything.
3. Put the removal on the demo-prep checklist explicitly (demo-checklist item 19 in `CLAUDE.md`) so
   it is scheduled rather than remembered.

**Re-evaluation triggers:** a stat `.asset` appears dirty in a commit again; the project enters demo
preparation; or a second developer joins.

---

### BUG-084 — no test can be written today · S2 · open · **precondition, not a symptom**

**Context.** Two independent reasons the Unity Test Runner cannot see a test:

1. `find Assets -name "*.asmdef"` returns **0**. With no assembly definition, nothing can reference
   `UnityEngine.TestRunner` / `UnityEditor.TestRunner`.
2. `tests/EditMode` and `tests/PlayMode` are **siblings** of `Assets/`. Unity compiles only what is
   under `Assets/` and `Packages/` — nothing in `tests/` is ever seen by the compiler.

The root `GameRPG.Combat.EditModeTests.csproj` is an untracked, stale IDE artifact pointing at an
`Assets/Tests/` directory that does not exist.

**Consequence.** Every sprint story estimating "write the first EditMode test" at 0.3d is
mis-scoped — including S14-13 and the sprint-15 carry-over. TD-014 has been carried since
2026-05-31 and cannot start.

**What it takes.** A decision on where tests live; a **runtime** `.asmdef` for gameplay code; a test
`.asmdef`; and a correction to `.claude/rules/test-standards.md`, which currently points at the
non-compiling paths.

⚠️ The runtime `.asmdef` is a breaking change in its own right: it splits `Assembly-CSharp` and
surfaces every implicit cross-directory dependency at once. Budget for that, not for 0.3d.

---

### BUG-052 — live subsystems with no ADR · S3 · open (widened)

Originally `Character/Base/`, `Pathfinding/` and `Poolable/`. Now also the Item system, Abilities v2,
and the UI layer. VContainer was in this set until ADR-0004 landed on 2026-09-11.

---

### BUG-064 — sub-item 7 only · S1 · partial

Six of seven sub-items are fixed. **Sub-item 7: `RangeWeapon` DI wiring** is still open. Pattern to
copy: `ItemSpawner.cs:8-10`.

---

## 4. Legacy CLAUDE.md bugs — #12 to #17, all open

| # | Summary | Location |
|---|---|---|
| 12 | `LevelManager` uses a bare `public static Instance` field — violates "no new singletons"; `RoomGeneraterController.Setting()` reaches through it (TD-023) | `LevelManager.cs:10` |
| 13 | The player is never teleported into the start room — the teleport line is commented out, and `RoomGeneraterController.OnDoneLoadRoomGrid()` has no caller | `RoomGridController.cs:82` |
| 14 | `MazeController.Awake()` is missing `return` after `Destroy(gameObject)`, so a duplicate still overwrites `Instance` and re-runs the generator. `EnemyManager.Awake()` has the correct shape to copy | `MazeController.cs:17` |
| 15 | Room JSON loads via `File.ReadAllText(Application.dataPath + filePath)` — Editor-only; `Assets/Data/Json/` is not packaged into a Player build | `RoomGeneraterController.cs:69` |
| 16 | `RoomType` is never read at runtime; start and end rooms are forced by list position `room[0]` / `room[last]`, so reordering `Maze_Storage.asset` breaks selection silently | `RoomGeneraterController.cs:47` |
| 17 | Dead code that reads as live gating: `DoorController.OpenDoor()`, `CheckCanBeOpened()` and `RoomCell.UpdateStatusDoor()` are all no-ops. The real mechanism is `OpenDoors()` / `CloseDoor()` | `DoorController.cs:29` |

Also still open: **BUG-043 partial** — `EntityAttack.Attack()` survives as a second enemy attack path
alongside `EntityWeapon` and hardcodes `TakeDamage(10, …)`, which `gameplay-code.md` forbids outright
(`EntityAttack.cs:33`, = TD-042).

---

## 5. Tech debt — Priority 1 and 2

| ID | Summary |
|---|---|
| **TD-048** 🆕 | **Nothing in the pipeline compiles this project except a human opening Unity.** No CI, no compiling pre-push hook (TD-009's hook is a placeholder, carried 25+ cycles), no `.asmdef`. This is why a two-typo commit (BUG-088) reached a branch tip and sat there. Minimum fix: a pre-push hook running Unity in batch mode and failing on any CS error. Depends on nothing |
| **TD-044** | = BUG-084. The `.asmdef` precondition |
| **TD-045** | = BUG-087. Death recovery |
| **TD-049** 🆕 | = BUG-089 + the residual BUG-091 tidy-up. One commit's worth of loose ends |
| TD-022 | Room JSON via `Application.dataPath` (= Bug #15) |
| TD-037 | ⚠️ **needs re-verification** — `EntityInput.Update()` had `//GetTargetInRange();` commented out, the only writer of `targetTransform`. If still true, no enemy detects the player |
| TD-040 | Two ability frameworks, no ADR choosing between them. Blocks `design/gdd/skill-ability-system.md` |
| TD-047 | = BUG-081 |
| TD-005 / 006 / 018 / 023 / 024 / 025 / 026 / 029 / 030 / 031 / 041 / 042 | Priority 2, see the register |
| TD-046 | **WITHDRAWN** — the record of a retracted claim. *Do not action.* Action BUG-072 instead |

---

## 6. Non-bug issues and decisions outstanding

| # | Issue |
|---|---|
| I-1 | **No GDD for Abilities v2.** The narrow remaining questions: may a `TryCast()` override have side effects (Consecrate says yes and is correct to)? Is a per-effect cost bought once or sustained per animation loop? What does a gain tier change? — all three are BUG-089 prerequisites. Does **not** block the fixes in §1 |
| I-2 | **No ADR choosing Abilities v1 or v2** (TD-040). Both compile, both ship assets, they share no types |
| I-3 | `production/sprints/sprint-15.md:39` says "Open bugs: 13". The real figure is **19** |
| I-4 | BUG-075 … BUG-091 are in no sprint or carry-over list |
| I-5 | Every "first EditMode test — 0.3d" estimate is invalid until BUG-084 is done |
| I-6 | **Only `AbilitySlot.Utility` can be cast.** `PlayerInputHandle.cs:254` hardcodes it, so three of four ability slots are unreachable |
| I-7 | `EnemyPrefab.prefab` carries the *player* `NegativeReciver` — live, or a dead template? |
| I-8 | `docs/diagrams/ability-system-diagrams.md` §1–§5 carry stale annotations, deliberately preserved. §6.1 and §8.1 list them. **Read §9 before acting on §6, §7 or §8** |
| I-9 | S14-01 (BUG-067) and S14-14 (BUG-069) are fixed in code with no Play Mode confirmation. `blocker:` fields are filled in; close via `/story-done` after the first Play Mode session |

---

## 7. Suggested order for the rest of this sprint

Ordered by working-state gained per unit of effort.

| # | Item | Effort | What it buys |
|---|---|---|---|
| 1 | **BUG-072** — set `layerMask` (and check `radius`) on `Lightning.prefab` | minutes | **Summon abilities deal damage.** Code is already in |
| 2 | Play Mode smoke: cast all four Paladin abilities | ~0.2d | Confirms BUG-075 and BUG-072 at once, and closes S14-01 / S14-14 |
| 3 | **BUG-088** — `/100f`, rename the shadowing parameter, guard `EntityNegativeReciver.cs:27` | minutes | Enemy reposition-on-damage actually works |
| 4 | **BUG-080** — resolve `vitalStatsComponent` in `Awake()` | minutes | Removes an NRE from the live item-pickup path |
| 5 | **BUG-066 + BUG-070** — one shared key guard, both sides | ~0.2d | Removes `KeyNotFoundException` from the live death path |
| 6 | **BUG-086**, then **BUG-087** | feature | Death → restart. **Strict order** — do not subscribe to an event firing every frame |
| 7 | **BUG-073 + BUG-090** — one Editor session | ~0.2d | Clears both missing-script references; decide whether ShootSpirit stays |
| 8 | **BUG-071** — rewrite the HoT/DoT loop, all three defects at once | ~0.3d | DoT/HoT effects run at all |
| 9 | **BUG-074**, **BUG-083**, **BUG-079**, **BUG-068**, un-hardcode `AbilitySlot.Utility` | — | Remaining correctness |
| 10 | **TD-048** — a pre-push compile hook | ~0.3d | Stops the next BUG-088 from ever reaching a branch tip |

Deferred by decision or dependency: BUG-063 (owner decision), BUG-089 (needs the GDD),
BUG-084 / TD-044 (needs the `.asmdef` decision), BUG-052 / TD-040 (need ADRs).
