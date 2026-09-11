# Prototype — Composition-Based Ability Framework ("Skill Enhance")

**Moved here**: 2026-08-22, from `Assets/Skill Enhance/` (owner decision after the documentation audit).
**Original date**: ~2026-05-20, branch `claude/review-skill-architecture-2df7z`.

---

## Hypothesis

Can an ability be assembled from **data** — one `AbilityDefinition` asset composed with a list of
effect assets and a list of condition assets — instead of being written as a **subclass**?

The shipped system (`Assets/Script/Skill_Ability/`) is inheritance-based: every new ability is a new
`ActivateSkill` subclass overriding `Cast()` / `Do()`. That makes each ability a code change. This
prototype asks whether a designer could instead build a new ability in the Inspector by picking
"shoot a projectile" + "requires 20 mana" + "caster must not be dead" and tuning numbers.

## Result

**[VALIDATED — promoted to production 2026-09-09]**

> **Status changed 2026-09-11.** This said `[IN PROGRESS — never finished, never wired]` for three
> weeks after it stopped being true. On **2026-09-09** (`9b8d40f`, `5c7afba`) all 17 `.cs` files
> were moved into **`Assets/Script/System/Abilities/`**, `AbilityHolder` was rewritten as
> `: CoreComponent<Core>, IAbilityOwner` to drive them, and SO assets were authored
> (`Assets/SO/Skill/ShootSpirit/ShootSpirit.asset`, `SpiritBomd.asset`,
> `Assets/SO/Skill/Conditions/New Has Enough Mana Condition.asset`).
>
> **`prototypes/skill-enhance-abilities/Scripts/` is now empty** — two orphan `.meta` files and no
> code. This README is kept as the record of the experiment; see **Promotion (2026-09-09)** at the
> end for what actually happened and what the promotion skipped.
>
> Everything between here and that section describes the state as of **2026-08-22** and is retained
> unedited as the historical record. Do not read it as current.

Structurally the skeleton is complete and coherent: `AbilityDefinition` (data) → `AbilityInstance`
(per-owner runtime state: cooldown, hold time) → `AbilityContext` (origin, forward, hold ratio) →
`AbilityEffectDefinition` / `AbilityConditionDefinition` (the composable pieces), driven by
`AbilitySystem` on the owner and reaching the world through `IAbilityOwner`.

What is **not** finished:

- **No effect actually does anything to the world.** `DamageInFrontEffect.Apply()` has its entire
  body commented out. `PlayDebugLogEffect` only logs.
- **It was written against a different project's conventions.** The commented-out code uses **3D**
  `Physics.OverlapSphere` and a `Damageable` type — this is a 2D game whose damage contract is
  `INegativeReceiver.TakeDamage(int, Vector2)`. `IAbilityOwner` still carries commented-out
  `CharacterStats` / `Health` / `SimpleCharacterMotor` members; none of those three types exist here.
- **It never connected to this project.** Verified in both directions: `Assets/Script/` never
  referenced `AbilitySystem`, `AbilityDefinition`, `AbilitySlot` or `IAbilityOwner`, and these files
  never referenced `Player`, `EventManager`, `StatsSO` or `INegativeReceiver`. No SO assets, no
  prefabs, no scene wiring — nothing could instantiate it.

So the hypothesis was never actually tested. The framework compiled (only because every reference to
a missing type is commented out) and sat inert in `Assets/` for about three months.

## Decision

**Keep, but move out of `Assets/`** — owner decision, 2026-08-22.

Not deleted: the composition design is a genuine alternative worth revisiting if authoring abilities
in code becomes the bottleneck. Not adopted: adopting it would mean rewriting every effect against
this project's 2D `INegativeReceiver` contract and building the SO assets and wiring, which is real
work with no current demand.

Moving it out of `Assets/` has one concrete effect: **Unity no longer compiles these 17 files.**
That is the point — it removes unreachable code from the build and from the mental model of anyone
reading `Assets/Script/`. It also satisfies `.claude/rules/prototype-code.md`, which requires
prototypes to live under `prototypes/` with this README, and forbids production scripts referencing
prototype code.

The `.meta` files were moved along with the sources deliberately. They are inert outside `Assets/`,
but if this is ever moved back, Unity will restore the original GUIDs instead of generating new ones.

### If you pick this back up

1. Move the folder back under `Assets/` (the `.meta` files will keep the old GUIDs).
2. Rewrite the effects against `INegativeReceiver.TakeDamage(int, Vector2)` and `Physics2D`
   NonAlloc queries — see `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs` `OnActivate()`, which
   `.claude/rules/weapon-skill-code.md` names the reference implementation.
3. Decide what `IAbilityOwner` exposes in **this** project's vocabulary (`StatsSO`, not
   `CharacterStats`).
4. Per `.claude/rules/prototype-code.md`, promotion to `Assets/Script/` requires a rewrite to
   production standards — values in SOs, null checks, no `Find()`.

## Related documents

- `docs/diagrams/ability-system-diagrams.md` — five diagrams of this framework. They are accurate
  about the code, with one overstatement: the class diagram shows `IAbilityOwner` exposing
  `CharacterStats Stats`, `Health Health` and `SimpleCharacterMotor Motor`, but all three are
  commented out in the source; the interface really only exposes `Transform`.
- `Assets/Script/Skill_Ability/` — the inheritance-based system the game actually runs.
- `.claude/rules/prototype-code.md` — the rules this directory now falls under.


---

## Promotion (2026-09-09) — what actually happened

Recorded 2026-09-11 during the full documentation/code re-synchronisation. The promotion itself
carried no doc update, which is why three separate documents (this README,
`docs/diagrams/ability-system-diagrams.md` and `design/gdd/skill-ability-system.md`) went on
describing this framework as parked and unwired while it was running in the game.

### What was done

| "If you pick this back up" step | Done? |
|---|---|
| 1. Move the folder back under `Assets/` | ✅ — to `Assets/Script/System/Abilities/`, keeping the `.meta` GUIDs as intended |
| 2. Rewrite the effects against `INegativeReceiver` + `Physics2D` NonAlloc | ✅ — `DamageInFrontEffect.Apply()` is no longer commented out. Note the contract is now `TakeDamage(**float**, Vector2)`, not `int` |
| 3. Decide what `IAbilityOwner` exposes in this project's vocabulary | ✅ — `GetCurrentStatValue(StatType)`, `PayCost(StatType, float)`, `Transform`, `IsHolding`. Services arrive via `IObjecPoolService` / `IPlayerStatService` / `IVitalComponent` / `IResourceReceiver` (VContainer, ADR-0004). Note the target type is `BaseStatsSO`, not `StatsSO` — that class was deleted in `b0512f4` |
| 4. Rewrite to production standards (values in SOs, null checks, no `Find()`) | ⚠️ **PARTIAL** |

### What step 4 skipped

- `AbilityHolder.HandleInput()` still carries an unresolved TODO comment stating that the state
  routing is in place but the per-state logic is not finished.
- `AbilityHolder.GetAbility(slot)` dereferences `currentAbility.Definition.AnimatorOverride`
  without a null guard, immediately after a `TryGetValue` that is allowed to fail.
- **No ADR exists** deciding the relationship between this framework and the v1 `ActivateSkill`
  system it was meant to replace. Both are now live: v2 drives the player, v1 still drives
  `WeaponStats.AbilityWeapon` / `.SkillWeapon`, `AttackSO.ability`, `Weapon` and `EntityWeapon`.

That outstanding cleanup is tracked as **demo-checklist item 18** in `CLAUDE.md`, and the
promotion is logged in `.claude/rules/prototype-code.md` under **Promotion Log**.

### Why `Scripts/` is not deleted

The two remaining `.meta` files (`Scripts.meta`, `Scripts/Abilities.meta`) are inert. They are left
in place rather than hard-deleted so the move stays legible in `git log --follow`, consistent with
this project's archive-don't-delete convention.
