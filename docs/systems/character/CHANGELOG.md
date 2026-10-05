# Character Core — Changelog

Newest first. Entries before 2026-10-05 reconstructed from `CLAUDE.md` history, `docs/CHANGELOG-DOCS.md` and `git log`.

## 2026-09-28 … 10-02 — Player spawned at runtime; ability input split
- **Commits:** `5b035b73` (runtime spawn, weapon injection), `9154763f` (state log), `3a395fe9` (input split), `0bc36406`
- **Changed:** player instantiation, input map, debug fields
- **From → To:**
  - Player prefab placed in the scene and registered in `GameLifetimeScope` → spawned by `PlayerManager.SpawnPlayer()` via `resolver.Instantiate`; nothing on the player is registered
  - Input `SkillWeapon` (`E`) → Utility slot → four actions `PrimaryAbility`/`SecondaryAbility`/`UtilityAbility`/`UltimateAbility` on keys `1`-`4`, routed through one `OnAbility(context, slot)`
  - `Block` (RMB) handler subscribed → subscription commented out
  - `StateMachine<T>` concrete → `abstract` + `[Serializable]`; `PlayerStateMachine` / `EntityStateMachine` `[Serializable]` (Inspector visibility)
  - `VitalStatsBase` → adds serialized `_currentHP` mirror updated after every change
  - `PlayerState.Enter()` → now `Debug.Log("Enter State: …")`
  - `WeaponHolder` → equips a `Weapon` found on its own GameObject at setup
- **Why:** player had to exist before the room loader could place it (start-room teleport, Bug #13); four ability slots needed four keys. Logging/debug fields: not recorded.
- **Bugs:** closes Bug #13 (with the map change); opens BUG-094.

## 2026-09-30 / 10-01 — Combo attack animation polish
- **Commits:** `9154763f`, `e8d117e3`
- **Changed:** attack animation assets and related input buffering (`PlayerInputHandler.bufferIsAttack` assignment fix)
- **From → To:** `this.BufferIsAttack = …` (property) → `this.bufferIsAttack = …` (field)
- **Why:** polish of the melee combo feel (commit message).
- **Bugs:** none.

## 2026-09-23 … 09-25 — ADR-0005 unified character contract (Amendments 1-3)
- **Commits:** `2aa225e4`, `b489f94f`, `8c3c350a`, `5007d3f6`, `38c4b620`, `e6d9603a`, `4e05da46`, `59d871a9`, `83954bc6`, `05e88133`, `4faf6233`, `f7d98b19`
- **Changed:** player and enemy components rebuilt on shared generic bases
- **From → To:**
  - Duplicate player/enemy implementations → `StatHandlerBase`, `VitalStatsBase`, `NegativeReceiverBase` (first named `DamageReceiverBase`), `WeaponHolderBase`, `AbilityHolderBase`, `MovementBase`, `CharacterInputBase`, `CharacterBase`, `CharacterData`
  - `ICharacter` empty, zero implementers → character-root identity marker carrying `Transform`
  - `ResourceReceiver : Interact, INegativeReceiver, IResourceReceiver` → `: Interact`, empty body; `IResourceReceiver.cs` deleted
  - `VitalComponent.cs` full class → two-line shim over `VitalStatsBase<Core>`
  - Abilities required an equipped weapon → independent of the weapon
- **Why:** one `AbilityDefinition` / weapon must run for player or enemy; remove the two-implementer ambiguity (BUG-081).
- **Bugs:** closes BUG-080, BUG-081; BUG-066 and BUG-070 become one site.

## 2026-09-08 — Player stat / vitals split
- **Commit:** `9b8d40f`
- **Changed:** `StatHandler` (max) + `VitalStatsComponent` (current) + `ResourceReceiver`
- **From → To:** `NegativeReciver` owning a private `currentHealth` → damage routed into `VitalStatsComponent.ReceiveReduction(StatType.HP, …)`
- **Why:** Sprint 12 stat refactor — single source for current values.
- **Bugs:** narrows Bug #6.

## 2026-09-03 — Folder reorganisation
- **Commit:** `1c0742e`
- **Changed:** seven system folders moved under `Assets/Script/System/`
- **From → To:** `Assets/Script/<System>/` → `Assets/Script/System/<System>/`
- **Why:** grouping (commit message "update folder").
- **Bugs:** none.
