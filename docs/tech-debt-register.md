## Technical Debt Register
Last updated: 2026-05-31
Total items: 20 | Estimated total effort: 16S + 2M + 2XL

| ID | Category | Description | Files | Effort | Impact | Priority | Added | Sprint |
|----|----------|-------------|-------|--------|--------|----------|-------|--------|
| TD-001 | Architecture | `PlayerUserItemState`, `EntityDeathState` extend `MonoBehaviour` instead of base state class — not wired into state machine | `PlayerUserItemState.cs`, `EntityDeathState.cs` | S | Critical | 1 | 2026-05-31 | Sprint 1 |
| TD-004 | Code Quality | `using UnityEditor.*` in 7 runtime scripts — breaks Player builds | `AnimationEventManager.cs`, `DualAbility.cs`, `EnemySO.cs`, `StatsCharacter.cs`, `AbilityHolder.cs`, `EntityData.cs`, `WeaponMeleeStats.cs` | S | Critical | 1 | 2026-05-31 | Sprint 1 |
| TD-008 | Code Quality | `WeaponMelee.Attack()` foreach body empty — player melee applies no damage | `Assets/Script/Weapons/MeleeWeapon/WeaponMelee.cs:29` | S | Critical | 1 | 2026-05-31 | Sprint 1 |
| TD-009 | Code Quality | `AnimationPlayerController.OnEnable()` line 21 registers `StartAnimation` twice; `EndAnimation` never fires — all combat states permanently stuck | `Assets/Script/Character/Player/Animation/AnimationPlayerController.cs:21` | S | Critical | 1 | 2026-05-31 | Sprint 1 |
| TD-011 | Code Quality | `EntityStatsSO.ModifiersAmor` property getter calls itself → StackOverflowException | `EntityStatsSO.cs` | S | Critical | 1 | 2026-05-31 | Sprint 1 |
| TD-012 | Code Quality | `EntityMoveState` dereferences `Target.transform.position` (line 30) before null check (line 34) — NullReferenceException when target lost | `Assets/Script/Character/Entity/States/EntityMoveState.cs:30` | S | Critical | 1 | 2026-05-31 | Sprint 1 |
| TD-005 | Code Quality | `Physics2D.OverlapCircle` (allocating) in hot paths — runs every frame in `EntityWeaponMelee` and `EntityFindTarget` | `EntityWeaponMelee.cs:33`, `EntityFindTarget.cs:21` | S | Medium | 2 | 2026-05-31 | Backlog |
| TD-006 | Code Quality | `Instantiate()` in gameplay paths without pooling — `SlashAbility`, `Shooting`, `EntityWeaponHolder` | `SlashAbility.cs`, `Shooting.cs`, `EntityWeaponHolder.cs` | M | Medium | 2 | 2026-05-31 | Backlog |
| TD-007 | Code Quality | `GetComponent<>()` called in method bodies (not Awake/Start) — hot path allocation | `SlashAbility.cs:33`, `Shooting.cs:30`, `WeaponsController.cs:30,37,48,53` | S | Medium | 2 | 2026-05-31 | Backlog |
| TD-013 | Code Quality | `Shooting.Update()` reads `Input.GetMouseButton(0)` directly — bypasses state machine | `Assets/Script/Weapons/RangeWeapon/Shooting.cs` | S | High | 2 | 2026-05-31 | Sprint 1 |
| TD-018 | Architecture | `TalentManager.cs` hardcodes Str/Dex/Int/Cha stats in `Awake()` — not SO-driven, violates data-driven convention | `Assets/Script/Character/Player/CoreComponent/TalentManager.cs` | S | Medium | 2 | 2026-05-31 | Backlog |
| TD-003 | Architecture | 0 ADRs exist — all architectural decisions are implicit in code, not documented or traceable | `docs/architecture/` | XL | High | 3 | 2026-05-31 | Ongoing |
| TD-014 | Test Debt | 0 test files (EditMode or PlayMode) — no regression safety net for any system | All | XL | High | 3 | 2026-05-31 | Ongoing |
| TD-016 | Documentation | `AnimationName.cs` is empty — string constants missing, magic strings used throughout | `Assets/Script/Character/Player/Animation/AnimationName.cs` | S | Medium | 3 | 2026-05-31 | Backlog |
| TD-017 | Documentation | `UIManager.cs` is an empty stub — HUD unimplemented | `Assets/Script/Manager/UI/UIManager.cs` | M | Medium | 3 | 2026-05-31 | Backlog |
| TD-019 | Architecture | `EntityData.rangeCheckChase` field missing — chase range hardcoded as `10f` in `EntityMoveState` | `EntityMoveState.cs`, `EntityData.cs` | S | Low | 4 | 2026-05-31 | Backlog |
| TD-002 | Architecture | `PlayerCombat.cs` is 129 lines of commented-out legacy code — dead file never referenced | `Assets/Script/Weapons/MeleeWeapon/PlayerCombat.cs` | S | Low | 4 | 2026-05-31 | Backlog |
| TD-020 | Architecture | Legacy files not removed: `Map/Legacy/Door.cs`, `Map/Legacy/Room.cs` — superseded, still in project | `Assets/Script/Map/Legacy/` | S | Low | 4 | 2026-05-31 | Backlog |
| TD-015 | Documentation | See TD-003 — no ADR files | `docs/architecture/` | XL | High | 3 | 2026-05-31 | Ongoing |
| TD-010 | Code Quality | Intentional typos preserved for serialization compatibility: `attackDamege`, `currrentSA`, `deplayTime`, `CaculateIndex`, `uppgradeData`, `Resgister` | Multiple | S | Low | 5 | 2026-05-31 | Accepted |
