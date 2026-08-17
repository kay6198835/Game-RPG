# UI Architecture — Survey & Implementation Plan

> Status: **Proposed** — awaiting phase-by-phase approval
> Date: 2026-08-17
> Scope: Unity 2022.3.62f3 LTS, uGUI + TextMeshPro 3.0.7
> Related: ADR-0001 (StatSystem dual data structure), ADR-0002 (singleton exception),
> `docs/architecture/architecture-review-2026-07-13.md`, `.claude/docs/technical-preferences.md`

---

## 0.0 Relationship to existing architecture artifacts

**There is no master architecture document.** `docs/architecture/architecture.md` does not
exist — the architecture review of 2026-07-13 states this explicitly and recommends
running `/create-architecture` only *after* the two HIGH-priority Foundation ADRs are
written. What exists instead:

| Artifact | What it covers | Bearing on this plan |
|---|---|---|
| `.claude/rules/*.md` (12 files) | Per-domain coding standards, informally architectural | Cited throughout this document |
| `.claude/docs/technical-preferences.md` | Engine, platform, budgets, forbidden patterns, Architecture Decisions Log | **Conflict found — see below** |
| `docs/architecture/adr-0001..0003` | StatSystem dual data, EnemyManager singleton, spawn selection | All three still `Proposed`, none `Accepted` |
| `docs/architecture/architecture-review-2026-07-13.md` | Traceability: 59 technical requirements, 50 gaps. Verdict **CONCERNS** | Two HIGH-priority gaps are exactly this plan's Phase 0 |

### This plan's Phase 0 collides with the two HIGH-priority ADR gaps

The architecture review's "write ADRs first" list has exactly two entries, and Phase 0
changes both systems:

- **TR-fnd-EVENT (Event Bus)** — "the static `EventManager` pub/sub bus has **no ADR**, yet
  it is the highest-risk system in the index (**12 of 20 systems route through it**)".
  Phase 0 adds `Clear()` to it.
- **TR-char-003 (Damage & Health)** — "the question of **who owns the health value** ... has
  no ADR. This is a shared Foundation contract (Combat, Enemy AI, **HUD**, Death)".
  Phase 0 answers precisely that question by putting health in `StatsSO`.

**Consequence for this plan:** the ADR is not an optional follow-up. Phase 0 makes a
Foundation-level decision that an existing review already flagged as needing an ADR
*before* more systems build on it. The two ADRs are therefore Phase 0 deliverables, not
Section 4 nice-to-haves.

**Also note:** the Architecture Decisions Log in `technical-preferences.md` has five
entries, all gameplay-level (state machines, SO-first, EventManager, `INegativeReceiver`,
no new singletons). **None concern UI.** UI appears exactly once in the entire architecture
review, incidentally. This plan is the project's first UI architecture decision.

### ⚠ Conflict: recorded platform scope contradicts this plan's mobile constraint

`.claude/docs/technical-preferences.md` records:

```
Target Platforms: PC (Windows)
Gamepad Support: None (demo target)
Touch Support:   None
```

This plan was written to a "PC first, **mobile later**" constraint, and Phase 6 exists to
serve it. Those cannot both stand. **This needs an owner decision:**

- if mobile is genuinely planned → `technical-preferences.md` must be updated, and Phase 6
  stays;
- if mobile is aspirational → drop Phase 6, and the "don't hardcode pixel sizes, don't
  assume hover" constraint on Phases 1-5 becomes optional rather than binding.

Nothing else in this plan changes either way — the constraint only affects Phase 6 and two
authoring guidelines. It is called out rather than silently resolved because it is a scope
decision, not a technical one.

*(Minor drift, noted not fixed: `technical-preferences.md` says "New singletons beyond
`MazeController`", omitting `EnemyManager`, which CLAUDE.md and ADR-0002 both permit.)*

---

## 0. Purpose and constraints

Gameplay systems (dungeon generation, player/enemy state machines, stat system) are well
advanced. The UI layer is not: it is four files, two of which are empty. Before a full RPG
UI surface is built on top of it, the architecture (kiến trúc) needs deciding, so the UI
does not accumulate the same tight coupling (phụ thuộc chặt) the rest of the codebase is
currently paying down.

| Constraint | Value |
|---|---|
| Platform (nền tảng) | PC first, mobile later — must not block touch / responsive layout |
| UI scope (quy mô) | Full basic RPG set: HUD, menu flow, inventory/equipment, shop/talent, popups |
| Priority (ưu tiên) | **Testability (khả năng test)** — UI logic must be unit-testable in EditMode |
| Team | Solo dev — minimize layers and file count; no over-engineering |
| Health source of truth (nguồn sự thật) | **`StatsSO`** — decided |
| Event bus (bus sự kiện) scope | **Patch in place + separate typed channel for UI** — decided |

---

## 1. Survey — what exists today

### 1.1 The entire UI surface

| File | Lines | State |
|---|---|---|
| `Assets/Script/Manager/UI/UIManager.cs` | 18 | **Empty stub (rỗng)** — `Start()` and `Update()` are both blank |
| `Assets/Script/MainMenu/MainMenu.cs` | 8 | **Empty stub** — empty class body |
| `Assets/Script/UI/StatsUI.cs` | 59 | The only real screen (màn hình) |
| `Assets/Script/UI/StatSlot.cs` | 21 | The only real widget |

Prefabs that exist but have **no script driving them**:
`Prefab/UI/HealthBar.prefab`, `ManaBar.prefab`, `Content.prefab`, `ChoiceText.prefab`,
`Canvas.prefab`, `StatsSystem/PrimaryStats/Stat_Slot.prefab`,
`StatsSystem/Derived Stats/Stat_Slot.prefab`.

There are **zero `.uxml` / `.uss` files** in the project — no UI Toolkit usage anywhere.
The project is 100% uGUI + TextMeshPro. Only `StatSlot.cs` imports `TMPro`.

Adjacent UI-ish code living outside `Script/UI/`: the minimap
(`Map/Cell/MapGridController.cs`, `Map/Cell/MapCell.cs`) does canvas and visibility work
and subscribes to map events directly.

### 1.2 Problems, with evidence

Each problem below carries a `file:line` citation and can be verified by opening the file.

---

**P1 — Health (máu) has no single source of truth.** It lives in three disconnected places:

| Location | Declaration | Written by |
|---|---|---|
| `Assets/Script/Character/Player/PlayerData.cs:10` | `[SerializeField] public float currentHealth` | only `Reborn()` (`:23`) |
| `Assets/Script/Character/Player/CoreComponent/NegativeReciver.cs:5` | `public int currentHealth` | **damage, at `:9`** |
| `Assets/Script/StatSystem/StatsSO.cs` | derived stat `MaxHP` | stat formulas |

Damage only ever writes the middle one. `PlayerData.currentHealth` is never decremented by
combat, and `StatsSO` does not model current HP at all. **A health bar has nothing to bind
to.** Also note `PlayerData.currentHealth` is a public field on a ScriptableObject — any
script in the project can write it with no notification.

**P2 — No change notification (thông báo thay đổi) on the damage path.**
`NegativeReciver.TakeDamage()` (`:6-14`) mutates health silently and emits only on death
(`:13`). The `EventID` enum (`Manager/EventManager.cs:39-55`) has no
`ON_PLAYER_TAKE_DAMAGE` and no health-changed event. A health bar built today would have
to poll in `Update()` — which `.claude/rules/ui-code.md` explicitly forbids
("Health bar ... update via `EventManager` subscriptions, not polling in Update").

**P3 — The one real data-binding hook in the project is unused.**
`StatsSO.OnStatChanged` (`StatSystem/StatsSO.cs:18`) is a proper
`event Action<StatType>` with a doc comment saying "UI subscribe để cập nhật". It has
**zero subscribers**. `StatsUI.cs:17` subscribes only to `ON_OPEN_STATS_PLAYER_UI`. Stat
changes therefore never reach the UI.

**P4 — `StatsViewDTO` is stale by construction.** The DTO is built once in
`EnsureInitialized()` (`StatsSO.cs:172`). In `RecalculateDerived()` the DTO is fetched at
`:209`:

```csharp
StatsViewDTO statViewDTO = GetOrCreateStatsViewDTO(target.Type);   // :209
target.BaseValue = newBase;                                        // :210
lookup[target.Type] = target;                                      // :211
```

`statViewDTO` is retrieved and then **never written to** — the local variable is discarded.
`StatsUI.cs:41` iterates `stats.statViewDTOs`, so the panel displays first-load values
forever, no matter how stats change.

**P5 — `StatsUI` leaks slots on every open.** `GetStatsUI()` is called in `Awake()`
(`StatsUI.cs:25`) **and again** on every open (`:31`), and it `Instantiate`s a fresh slot
per stat each time (`:46`, `:52`) with no clear and no pooling:

```csharp
statSlot = Instantiate(PrimaryStatSlotPrefab, primaryStatSlotContainer.transform);  // :46
```

Open the panel five times and the container holds five copies of every stat row.
`Assets/Script/Poolable/ObjectPoolManager.cs` exists (with `Get`/`Spawn`/`Register`) but UI
does not use it.

**P6 — `new` on a MonoBehaviour.** `StatsUI.cs:43`:

```csharp
StatSlot statSlot = new StatSlot();
```

Unity logs an error for constructing a MonoBehaviour with `new`, and the value is discarded
three lines later by the `Instantiate` at `:46`/`:52`. Dead and illegal.

**P7 — Unsafe unboxing (ép kiểu không an toàn).** `OnOffUI(object obj = null)` casts
`(bool)obj` at `StatsUI.cs:29` and again at `:37`. The parameter defaults to `null`, so any
`Emit(ON_OPEN_STATS_PLAYER_UI)` sent without a payload throws `NullReferenceException`.
This is the direct consequence of `EventManager` being typed `Action<object>`
(`EventManager.cs:10`) — the compiler cannot help at any call site.

**P8 — Dead events (sự kiện chết).** Grepping all of `Assets/Script/`:

- `ON_OPEN_STATS_PLAYER_UI` / `ON_CLOSE_STATS_PLAYER_UI` — declared
  (`EventManager.cs:53-54`), subscribed once (`StatsUI.cs:17`), **emitted by nobody**.
  The stats panel cannot be opened in game.
- `ON_PLAYER_DEATH` — emitted (`NegativeReciver.cs:13`), **subscribed by nobody**. Player
  death currently does nothing.
- `ON_REALOAD_GAME` — declared (`:43`), neither emitted nor subscribed; the only reference
  is commented out at `Character/Player/States/PlayerDeathState.cs:23`.

**P9 — `EventManager` leaks listeners across scene reloads.** `_events` is a
`public static Dictionary<EventID, Action<object>>` (`EventManager.cs:8`) that is **never
cleared**. Static state survives scene reload in the Editor and in a build, so:

- delegates pointing at destroyed objects stay registered → `MissingReferenceException`
  when `Emit` invokes them (`:34`);
- every reload adds another subscription → handlers run N times after N restarts.

`UnResgister` (`:22-28`) only helps for objects that get a clean `OnDisable`.
**This is the single biggest blocker for a menu → game → death → restart loop.**

**P10 — Public fields throughout the UI.** `StatsUI.cs:7-13` (eight public fields) and
`StatSlot.cs:7-11` (five) are all `public`, violating `.claude/rules/ui-code.md` and
`.claude/rules/gameplay-code.md` ("no `public` fields on MonoBehaviours — use
`[SerializeField] private` + properties").

**P11 — Stray `using UnityEngine.UIElements;` in four gameplay files** that never use it:

| File | Line |
|---|---|
| `Assets/Script/Map/Cell/MapCell.cs` | 2 |
| `Assets/Script/Map/BaseGrid.cs` | 4 |
| `Assets/Script/Skill_Ability/ActivateSkill.cs` | 5 |
| `Assets/Script/Character/Player/CoreComponent/AbilityHolder.cs` | 5 |

These are IDE auto-imports. They are a landmine (mìn): `UnityEngine.UIElements` and
`UnityEngine.UI` both define `Image` and `Button`, so adding `using UnityEngine.UI` to any
of these files produces a CS0104 ambiguous-reference compile error.

**P12 — No game-state owner.** `grep -rln "class GameManager" Assets/` returns nothing.
Nothing owns the menu / playing / paused / dead states, so screen flow has no driver and
nothing can pause the game or gate input.

**P13 — UI logic living in map code.** `Map/Cell/MapGridController.cs` and `MapCell.cs`
handle minimap canvas and visibility and subscribe to `ON_PLAYER_ON_DOOR` /
`ON_LOAD_MAZE_DONE` directly (`MapGridController.cs:20-21`). This works and is not urgent
to change — but it is the precedent **not** to repeat for new UI.

**P14 — Cross-wired death event (already tracked).**
`Character/Entity/CoreComponent/EntityNegativeReciver.cs:13` emits **`ON_PLAYER_DEATH`**
when an *enemy* dies, and `:10` pulls a `PlayerInputHandler` off the enemy's own Core.
This is already sprint-10 story `S10-01` (BUG-042/053/054), which deletes the file.
Consequence for this plan: **any death screen built before `S10-01` lands will fire on
every mob kill.**

### 1.3 Summary of the coupling problem

The UI cannot currently be built well because the layer beneath it is not observable:
health has no owner (P1) and no change signal (P2), the one signal that does exist has no
subscribers (P3), the view-model type it feeds is never refreshed (P4), and the transport
carrying everything is untyped (P7) and leaks across restarts (P9). Every one of these is
upstream of UI code. Fixing them is Phase 0 and is a prerequisite for everything else.

---

## 2. Recommended architecture

### Decision 1 — uGUI, not UI Toolkit, not hybrid

**Choice: stay on uGUI + TextMeshPro.**

Grounded in this codebase rather than general preference:

- TMP 3.0.7 and every existing UI prefab are uGUI. There are zero `.uxml` files, so a
  hybrid would mean a solo dev maintaining two UI systems with no existing UI Toolkit
  investment to preserve.
- UI Toolkit's runtime in 2022.3 has **no world-space canvas**. Floating damage numbers and
  enemy health bars over sprites are on the roadmap for an action roguelike; those need
  world-space, which would force uGUI back in alongside it.
- Per `docs/engine-reference/unity/VERSION.md`, this project's stated position is already
  "UI Toolkit ... not recommended for runtime UI in this project without explicit
  decision".
- P11 means adopting UI Toolkit forces resolving the ambiguous-using landmine first.

**Trade-off accepted:** uGUI's layout system is heavier at runtime than UI Toolkit's, and
authoring is prefab-based rather than markup-based. For the screen counts this project
will reach, that cost is not material. Revisit only on a Unity 6 migration.

### Decision 2 — MVVM-lite with a passive View

Three layers, thin:

```
Model                    ViewModel                  View
─────                    ─────────                  ────
StatsSO                  plain C# class             MonoBehaviour
PlayerData          →    no MonoBehaviour      →    reads ViewModel
runtime state            no UnityEngine.UI          writes TMP / Image / Slider
                         ← UNIT TEST TARGET         no game rules
```

**Why MVVM here specifically, not an imported preference:** `StatsSO` **already** exposes
`OnStatChanged` (`StatsSO.cs:18`) and **already** defines a `StatsViewDTO` type
(`StatsSO.cs:244`) whose entire job is to carry display-shaped data
(`Name`, `FinalValue`, `BonusValue`) to a view. The project is already reaching for a
ViewModel — it is just doing it inside the SO and never refreshing it (P4). This decision
formalizes what is already there rather than importing a foreign pattern.

**Why this satisfies the testability priority:** the ViewModel is plain C# with no Unity UI
dependency, so it runs in EditMode tests. Per `.claude/rules/test-standards.md`, logic
stories require a passing EditMode unit test as a **BLOCKING** gate — a ViewModel makes
that possible for UI logic, which is impossible today when the logic is inside
`StatsUI.OnOffUI` and `GetStatsUI`.

**Trade-off accepted, stated honestly:** MVVM adds one indirection layer versus putting
logic straight in the View. For a purely visual widget — a bar that only lerps a fill
toward a value — a View with **no** ViewModel is fine. Do not force the pattern where
there is no logic to test.

### Decision 3 — `Observable<T>` for UI binding; `EventManager` stays for gameplay

`EventManager` keeps its 24 working call sites across Map / Enemy / Character. It gets one
change only: a `Clear()` / scene-reset to fix P9. It is not made generic — refactoring
`Action<object>` into `Action<T>` would touch every call site in the dungeon flow that
currently works, for no UI benefit.

UI data binding instead uses a small `Observable<T>` (roughly 30 lines, no third-party
dependency) that raises only on actual change. This is type-safe, so P7 cannot recur, and
it is trivially unit-testable.

**The boundary must be documented and held:**

| Mechanism | Use for | Example |
|---|---|---|
| `EventManager` | discrete gameplay moments (thời điểm rời rạc) | room cleared, enemy died, player entered door |
| `Observable<T>` | continuous values the UI renders (giá trị liên tục) | current HP, mana, stat value, gold |

**Trade-off accepted:** two notification mechanisms in one project is a real cost in
onboarding clarity. It is chosen over one because unifying them means either untyped UI
binding (today's bug source) or a risky rewrite of working dungeon code.

### Decision 4 — `UIRootView`: Inspector-wired, not a singleton

`.claude/rules/manager-event-code.md` requires "`UIManager` must be wired via Inspector
reference, not found at runtime", and ADR-0002 permits only `MazeController` and
`EnemyManager` as singletons. `UIRootView` therefore holds no static instance.

It owns:

- **Layers** — a `UILayer` enum (HUD / Screen / Popup / Overlay), one Canvas per layer.
  This is also a draw-call win: a HUD value changing does not dirty the popup canvas.
- **A screen stack (ngăn xếp màn hình)** — push / pop, ESC pops the top, input blocking
  below the top, back-navigation. Needed for nesting such as shop → item detail → confirm.
- **Show/hide via `CanvasGroup.alpha`**, per `.claude/rules/ui-code.md`, not `SetActive`.

### Decision 5 — One-way data flow, commands back

```
gameplay → writes Model → Model raises change → ViewModel recomputes → View renders
                                                       ↑
                                          UI input = command on ViewModel
                                          (never a direct call into gameplay)
```

UI never holds a `GameObject` reference to the Player or an Enemy — required by
`.claude/rules/ui-code.md`. This is what keeps the UI testable: a ViewModel driven by a
fake Model needs no scene.

### Decision 6 — Health lives in `StatsSO`

`StatsSO` gains `CurrentHP` as the single source of truth. `NegativeReciver` writes through
to it instead of owning its own `int`.

**Why:** `OnStatChanged` (`:18`) already exists, so the UI gets binding with no new event
plumbing; `MaxHP` is already a derived stat, so equipment and buffs applied through
`StatModifierGroupSO` automatically move the health bar's maximum with no extra code.

**Trade-off, stated explicitly:** run state now lives on an asset that persists between
Editor Play sessions. `StatsSO.OnEnable()` (`:20-29`) already clears modifiers on load; the
reset must be extended to cover `CurrentHP`, or health carries over between playtests. This
is a known Unity SO hazard and must be covered by an EditMode test.

---

## 3. Phased implementation plan

Every phase ends with the game still playable. No phase requires a big-bang rewrite.

---

### Phase 0 — Foundation & safety (no new UI)

**Goal:** make the data and event layer observable, so UI has something to bind to.
Nothing visual changes.

- [ ] Add `Clear()` / scene-load reset to `EventManager` (fixes **P9**)
- [ ] Add `CurrentHP` to `StatsSO` as single source of truth; extend `OnEnable()` reset to cover it (**P1**, Decision 6)
- [ ] Route `NegativeReciver.TakeDamage()` through `StatsSO`; remove its own `currentHealth` field (**P1/P2**)
- [ ] Fix the `StatsViewDTO` write-back in `RecalculateDerived()` (**P4**)
- [ ] Delete the four stray `using UnityEngine.UIElements;` lines (**P11**)
- [ ] **ADR: Event Bus** — record the static-bus contract, `EventID`-enum-only extension rule, register/unregister lifecycle, and the new `Clear()` semantics *(HIGH-priority gap TR-fnd-EVENT from the 2026-07-13 review)*
- [ ] **ADR: Damage & Health** — record `INegativeReceiver.TakeDamage(int, Vector2)` and health-value ownership landing on `StatsSO` *(HIGH-priority gap TR-char-003; the review names HUD as a dependent of this contract)*

**Files touched:** `Manager/EventManager.cs`, `StatSystem/StatsSO.cs`,
`Character/Player/CoreComponent/NegativeReciver.cs`, `Character/Player/PlayerData.cs`,
plus the four P11 files; two new files under `docs/architecture/`.

**Why the ADRs sit here and not in a follow-up section:** these are the only two
HIGH-priority gaps in the architecture review, and Phase 0 changes both systems. Writing
the code without the ADR leaves the same informal-decision gap the review already rated
**CONCERNS**.

**Stays runnable:** no UI is touched. Behaviour is unchanged except that stat changes now
propagate correctly and events no longer leak across restarts.

**Test evidence:** EditMode tests — DTO refreshes after a derived-stat recalc;
`EventManager.Clear()` empties the table; `CurrentHP` resets on SO reload.

**Open question to settle during this phase:** whether `PlayerData.currentHealth` is
deleted outright or kept as a deprecated field. `PlayerData.Reborn()` is named in the demo
checklist and in `.claude/rules/scriptableobject-data.md` as the canonical reset, so
`Reborn()` must keep working either way.

---

### Phase 1 — UI core framework (no game feature yet)

**Goal:** build the skeleton every later screen plugs into.

- [ ] `Assets/Script/UI/Core/Observable.cs` — `Observable<T>`, raises only on change
- [ ] `UIView` base MonoBehaviour; `UIScreen`, `UIPopup` subclasses; `UILayer` enum
- [ ] `ScreenStack` — plain C#, push / pop / peek, so it is unit-testable
- [ ] `UIRootView` — Inspector-wired, owns one Canvas per layer, drives the stack
- [ ] Retire the `Manager/UI/UIManager.cs` stub into `UIRootView`
- [ ] *(Optional but recommended)* asmdef split so ViewModels compile without Unity UI refs

**Files touched:** new folder `Assets/Script/UI/Core/`; delete
`Assets/Script/Manager/UI/UIManager.cs`.

**Stays runnable:** the framework is inert until a screen registers with it. Nothing in the
existing scenes references `UIManager` today (it is an empty stub), so removing it breaks
nothing.

**Test evidence:** EditMode — `ScreenStack` push/pop ordering and ESC-pops-top;
`Observable<T>` does not raise when assigned an equal value.

---

### Phase 2 — Migrate `StatsUI` as the reference implementation

**Goal:** prove the pattern on real existing code before building anything new. Closes
**P3, P5, P6, P7, P8, P10**.

- [ ] `StatsViewModel` — plain C#, subscribes `StatsSO.OnStatChanged`, exposes `Observable<StatsViewDTO>` per `StatType`
- [ ] `StatsScreen : UIScreen` — builds slots **once**, reuses them on reopen
- [ ] `StatSlotView` — passive, `[SerializeField] private`, no logic
- [ ] Wire a real open/close trigger (nothing emits `ON_OPEN_STATS_PLAYER_UI` today)
- [ ] Delete `UI/StatsUI.cs` and `UI/StatSlot.cs` once the replacement is verified in Play Mode

**Files touched:** `Assets/Script/UI/StatsUI.cs`, `Assets/Script/UI/StatSlot.cs` →
replaced; prefabs under `Prefab/UI/StatsSystem/**` need their script references
re-assigned.

**Stays runnable:** keep the old `StatsUI` in place until the new screen is confirmed
working in Play Mode, then delete. The prefab script re-assignment is manual Inspector work
— small, and a solo dev can do it in one sitting. Do not rename the MonoBehaviour and the
file in the same commit, or Unity loses the GUID binding.

**Test evidence:** EditMode — a stat change on a fake `StatsSO` produces the correct DTO on
the ViewModel; opening the screen twice does not double the slot count.

---

### Phase 3 — HUD + `GameManager`

**Goal:** first player-visible win. Closes demo checklist item 9 (HUD) and item 6 (player
death).

- [ ] `GameManager` — Inspector-wired, **no singleton**; state machine Menu / Playing / Paused / Dead (**P12**)
- [ ] `PlayerHudViewModel` + `HealthBarView`, driving the existing script-less `HealthBar.prefab` / `ManaBar.prefab`
- [ ] Subscribe `ON_PLAYER_DEATH` → death screen → `PlayerData.Reborn()` + reload (**P8**)

**Files touched:** new `Assets/Script/Manager/GameManager.cs`, new
`Assets/Script/UI/Hud/`; `Prefab/UI/HealthBar.prefab`, `ManaBar.prefab` get their scripts.

**⚠ Ordering dependency:** this phase **must not start before sprint-10 story `S10-01`
lands**. Until `EntityNegativeReciver.cs` is deleted, enemy death emits `ON_PLAYER_DEATH`
(P14) and the death screen will appear on every mob kill.

**Stays runnable:** the HUD is additive. `GameManager` starts in `Playing` so existing
scenes behave exactly as before until menus exist.

**Test evidence:** EditMode on the state machine transitions; Play Mode / documented
playtest for the death → restart loop (integration story, BLOCKING per
`.claude/rules/test-standards.md`).

---

### Phase 4 — Popup stack + upgrade cards

**Goal:** exercise the screen stack with real nesting. Closes demo checklist item 10.

- [ ] Pause menu (ESC), settings screen
- [ ] Room-clear three-card upgrade popup — `ChoiceText.prefab` and `Content.prefab` already exist
- [ ] Time-scale handling owned by `GameManager`, never by a UI script

**Files touched:** new `Assets/Script/UI/Screens/`; hooks into `ON_CLEAR_ENEMY` /
`ON_ROOM_CLEAR`.

**Stays runnable:** each popup is independent; ship them one at a time.

**Test evidence:** EditMode on upgrade-card selection logic (which card applies which
modifier); manual walkthrough doc for the menus (ADVISORY per test standards).

---

### Phase 5 — Inventory / equipment / shop / talent

**Goal:** the scale test — the point at which a weak architecture would show.

- [ ] Item grid with pooled slots, reusing `Assets/Script/Poolable/ObjectPoolManager.cs`
- [ ] Tooltip and drag-drop
- [ ] Equip / unequip mapping onto the existing `StatsSO.AddModifiersFromSource` (`:94`) and `RemoveModifiersFromSource` (`:115`) with `StatModifierGroupSO`
- [ ] Shop screen; talent tree reading `InternalSkillSO`

**Note:** the equip path needs almost no new stat code — `AddModifiersFromSource` /
`RemoveModifiersFromSource` were built for exactly this and already batch their recalc.

**Stays runnable:** each screen is independent and gated behind its own open trigger.

**Test evidence:** EditMode on equip → stat delta → unequip → stat restored.

---

### Phase 6 — Mobile readiness (deferred)

**Goal:** honour "PC first, mobile later" without paying for it now.

- [ ] Input abstraction layer (touch vs. mouse)
- [ ] `CanvasScaler` presets and reference resolution
- [ ] Safe-area handling
- [ ] Touch target sizing pass

**Deliberately deferred** so it does not tax Phases 1–5. The constraint Phases 1–5 must
respect: **do not hardcode pixel sizes, and do not assume mouse hover exists.** A hover-only
tooltip in Phase 5 is the kind of thing that makes Phase 6 expensive.

---

## 4. Follow-up documentation

Two ADRs moved **into Phase 0** (see §0.0) because the architecture review already rates
them HIGH priority. What remains here is genuinely follow-up:

- [ ] **UI GDD** — `design/gdd/ui-system.md` does not exist. The 8-section format in
      `.claude/rules/design-docs.md` applies.
- [ ] **ADR: UI architecture** — the uGUI + MVVM-lite choice. Lower priority than the two
      Foundation ADRs; can follow Phase 1 once the framework shape is confirmed in code.
- [ ] **Resolve the platform-scope conflict** in `.claude/docs/technical-preferences.md`
      (§0.0) — owner decision, blocks nothing before Phase 6.
- [ ] **Not this plan's job, but adjacent:** all three existing ADRs are still `Proposed`.
      The review notes they gate Sprint 5/6 stories until `Accepted`.

---

## 5. Phase tracker

| Phase | Title | Status | Blocked by |
|---|---|---|---|
| 0 | Foundation & safety **+ 2 Foundation ADRs** | ☐ Not started | — |
| 1 | UI core framework | ☐ Not started | Phase 0 |
| 2 | Migrate StatsUI | ☐ Not started | Phase 1 |
| 3 | HUD + GameManager | ☐ Not started | Phase 1, **sprint-10 S10-01** |
| 4 | Popup stack + upgrade cards | ☐ Not started | Phase 3 |
| 5 | Inventory / shop / talent | ☐ Not started | Phase 2, Phase 4 |
| 6 | Mobile readiness | ☐ Deferred | Phase 5, **platform-scope conflict (§0.0)** |
