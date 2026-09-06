# Lead Programmer Memory — Game-RPG

> Rewritten 2026-08-20 by the documentation audit. The previous version listed seven
> "active" bugs, five of which were fixed or superseded, and pointed at classes deleted in
> June. Do not trust bug lists here over `CLAUDE.md` — that file is re-verified against
> source; this one is a shortcut.

## Project Context

Unity 2022.3.62f3 LTS action roguelike, URP 2D. Combat inspired by Cult of the Lamb.
Main dev scene: `Assets/Scenes/Main/Test/LoadRandomMap.unity`.

## Blockers (verified against source 2026-08-20)

1. **Enemies can never die.** Damage lands on `EntityNegativeReciver.currentHealth`, but
   `EntityBasicState.LogicUpdate()` reads `entity.Data.StatsSO.Health` for the death check —
   two disconnected numbers. `EntityCore.TakeDamage()` still throws `NotImplementedException`.
   `EntityNegativeReciver` also resolves `PlayerInputHandler` off an `EntityCore` (→ NRE) and
   emits `ON_PLAYER_DEATH` when an *enemy* dies. Fix per story S10-01: implement
   `EntityCore.TakeDamage()`, route health through `EntityStatsSO`, delete
   `EntityNegativeReciver.cs`. (BUG-042 + BUG-053 / TD-036)
2. **Enemies never see the player.** `EntityInput.Update()` line 67 has
   `//GetTargetInRange();` commented out — the only writer of `targetTransform`. Restore it
   with a null guard *and* NonAlloc queries. (TD-037)
3. **Player death chain incomplete.** `NegativeReciver` emits `ON_PLAYER_DEATH` but writes its
   own `currentHealth`, never `PlayerData.currentHealth`. `PlayerDeathState.LogicUpdate()` is
   commented out and the state is never constructed in `Player.Awake()`. No `GameManager`
   exists, so `PlayerData.Reborn()` has no caller. (Bug #6 + BUG-044)
4. **StatSystem silent failures.** `EntityStatsSO.ModifiersAmor` recurses into itself →
   `StackOverflowException` (kills the Editor, uncatchable). `StatsSO.RecalculateDerived()`
   uses `||` where the skip-guard needs `&&`, so derived stats stop updating unless
   `isDevMode` is on. `Stat.modifiers` is `[SerializeField]` against its own comment and
   ADR-0001 — a leaked `STR +1 Flat` is already committed in `PlayerStats.asset` and
   `Test.asset`. (TD-011, TD-038)

## What is actually working

- Player melee and ranged damage — `MeleeWeapon.OnActivate()` / `RangeWeapon.OnActivate()`,
  both reached through `WeaponHolder.MakeDamage()` from `PlayerAttackState`.
- Combo staging — `Weapon.OnAttackEnter()` advances `CurrentStageIndex` modulo `StageCount`.
- Room clear — `RoomCell.EnemyCount` counts spawns and deaths, emits `ON_CLEAR_ENEMY` at zero,
  `RoomGridController` opens the doors.
- Dungeon generation, room JSON loading, door tile swapping, minimap tweening.
- Pathfinding (A*), object pooling, the enemy spawn pipeline up to the point where enemies
  would need to die.

## Architecture Invariants

- State machine for all characters — no inline if/else in `Update`.
- ScriptableObject-first: all gameplay values in SO assets.
- `INegativeReceiver.TakeDamage(int amount, Vector2 attackPosition)` for all damage.
- `EventManager` static bus for cross-system events. The typo `Resgister` is intentional.
- Singletons: `MazeController` and `EnemyManager` only. `LevelManager` is a standing
  unratified violation (TD-023).
- **Component hubs:** `Core.cs` / `EntityCore.cs` remain the per-character hubs, but both now
  sit on top of `Character/Base/CoreBase.cs`. Registration is **pull-based** —
  `CoreBase.Setup()` runs `GetComponentsInChildren<ICoreComponent>(true)` in `Awake()`;
  components do **not** self-register. This layer has no ADR (BUG-052).
- **Animation handoff is the `StatusAnimation` enum**, not the old `isAnimationTrigger` /
  `isAnimationFinished` booleans. States branch on `Status` in `LogicUpdate()`.

## Traps

- `CoreBase.GetCoreComponent<T>(out T)` returns silently with `null` on a miss — a mis-wired
  prefab NullRefs a frame later somewhere unrelated. This is how blocker 1 produces a
  confusing crash instead of a clear error.
- `EntityMovement.Start()` does `grid = EnemyManager.Instance.Grid` — any scene without an
  `EnemyManager` NullRefs on every enemy.
- Room JSON loads via `File.ReadAllText(Application.dataPath + …)` — Editor-only, breaks
  Player builds (Bug #15).
- Preserved typos are contracts in serialized assets: `Resgister`, `INegativeReciver.cs`,
  `attackDamege`, `Modal`, `ENEBLE`, `CaculateIndex`, `currrentSA`, `deplayTime`.

## Coding Standards Shortcuts

- `[SerializeField] private` not `public` for inspector fields.
- Cache `GetComponent` in `Awake`, never in `Update`.
- `OverlapCircleNonAlloc` with a pre-allocated array — never `OverlapCircle` in hot paths.
  `MeleeWeapon` is the reference implementation; `EntityWeaponMelee` is **not** (it still
  allocates — BUG-046).
- Zero tests exist project-wide (TD-014), so nothing catches a regression for you.
