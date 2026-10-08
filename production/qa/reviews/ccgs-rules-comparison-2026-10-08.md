# CCGS Upstream Rules vs Project Rules — Detailed Comparison

> **Date**: 2026-10-08 · Companion to `ccgs-upgrade-2026-10-08.md` §5.
> **Purpose**: the owner asked to see, item by item, what the eight upstream rules that were *not* merged
> actually say, since "not needed now" does not mean "no effect". Nothing in `.claude/rules/` was changed
> by this document. Source of upstream text:
> `.claude/Claude-Code-Game-Studios-main/Claude-Code-Game-Studios-main/.claude/rules/`.

## How rules take effect

A rule file with a `paths:` frontmatter is loaded into Claude's context **only when a file matching one
of those globs is read or edited**. A rule with no frontmatter is loaded every session. So an extra rule
costs nothing until a matching file is touched — but once loaded, it is followed as an instruction, and
**if it contradicts a project rule, Claude gets two conflicting orders**. That is the real cost to weigh.

**Path check against this repo** — which upstream globs would actually fire today:

| Upstream rule | Upstream `paths:` | Matches in this repo? |
|---|---|---|
| gameplay-code | `Assets/**/{Gameplay,gameplay}/**/*.cs` | Only `Assets/Script/UIFlow/Gameplay/**` (UI code, not gameplay) — **wrong target** |
| ai-code | `Assets/**/{AI,Ai,ai}/**/*.cs` | **None** (enemy AI lives in `Character/Entity/`) |
| ui-code | `Assets/**/{UI,Ui,ui}/**/*.cs` | `Assets/Script/UI/**` |
| engine-code | `Assets/**/{Core,core}/**/*.cs` | `Character/Player/Core`, `Character/Entity/Core`, `System/Abilities/Core`, `UIFlow/Core` |
| data-files | `Assets/**/{Data,data}/**/*.json` | **`Assets/Data/Json/Room/*.json` — all 20 room files** |
| shader-code | `Assets/**/*.{shader,hlsl,cginc,compute,shadergraph}` | Only `Assets/TextMesh Pro/Shaders/*` (third-party) |
| narrative | `design/narrative/**` | Folder exists, holds only `.gitkeep` |
| network-code | `Assets/**/{Network…}/**/*.cs` | None |

## 1. `gameplay-code.md`

| # | Upstream rule | Local `gameplay-code.md` | Verdict |
|---|---|---|---|
| 1 | All gameplay values from config/data files, never hardcoded | ✅ Same ("ALL numeric gameplay values MUST live in ScriptableObjects") | Covered |
| 2 | Use delta time for all time-dependent calculations | ❌ Not stated | **Worth adding** — frame-rate independence; ability cooldowns and timers already use `Time.deltaTime`, but nothing requires it |
| 3 | No direct references to UI code — use events | ⚠️ Stated from the UI side only (`ui-code.md`: UI must not reference player) | **Worth adding** the gameplay → UI direction. Note: UIFlow's `RealPlayerDataProvider` subscribes to C# events on character components (`CurrentStatsChanged`) — compatible |
| 4 | Every gameplay system implements a clear interface | ✅ `engine-code.md` "Interface-First" | Covered |
| 5 | State machines must have **explicit transition tables** with documented states | ⚠️ Project uses state *classes* (`PlayerState` subclasses) with transitions inside `LogicUpdate()` | **Conflicts** with the current architecture. Could be adopted as "document each state's transitions in the GDD" instead |
| 6 | Write unit tests for all gameplay logic; separate logic from presentation | ⚠️ `test-standards.md` requires EditMode tests for Logic stories, but tests are impossible today (BUG-084) | Covered in intent; blocked in practice |
| 7 | Document which design doc each feature implements, in code comments | ❌ Not stated; conflicts mildly with the project's "no comments unless WHY is non-obvious" | **Optional** — could be satisfied by GDD → code links instead of code comments |
| 8 | No static singletons for game state — use dependency injection | ✅ Same (VContainer, ADR-0004; `MazeController` / `EnemyManager` exceptions) | Covered |

## 2. `ai-code.md`

| # | Upstream rule | Local `ai-code.md` | Verdict |
|---|---|---|---|
| 1 | AI update budget 2 ms per frame total | ✅ Local: 0.1 ms **per enemy** (stricter per entity; no total) | Covered; a total cap could be added (e.g. 20 enemies × 0.1 ms) |
| 2 | All AI parameters tunable from data | ✅ Same (`EntityData` SO) | Covered |
| 3 | Visualization hooks for all AI state (paths, perception, decisions) | ✅ Local: `OnDrawGizmosSelected` for FOV, attack range, target | Covered (paths not mentioned — A* path gizmo would be the gap) |
| 4 | **AI should telegraph intentions** — players need time to read and react | ❌ Not stated | **Worth adding** — core to Cult-of-the-Lamb-style combat feel; today enemy attack wind-up is only an animation choice, not a rule |
| 5 | Prefer utility-based or behavior trees over if/else chains | ⚠️ Project uses a state machine per enemy (by design) | **Conflicts** with current architecture; skip |
| 6 | Group AI: formation, flanking, role assignment from data | ❌ Not in scope | Not needed for the demo |
| 7 | Log all AI state transitions | ✅ Local: log state changes in DEBUG only (`#if UNITY_EDITOR`) | Covered (local is stricter) |
| 8 | Never trust AI input from the network | — | Not applicable (single-player) |

## 3. `ui-code.md`

| # | Upstream rule | Local `ui-code.md` | Verdict |
|---|---|---|---|
| 1 | UI never owns or modifies game state | ✅ Same | Covered |
| 2 | All UI text through the localization system | ⚠️ Local: "constants or loaded from a data source" — weaker, no localization system | **Partly worth adding** — make it "no inline player-facing literals"; full localization is out of demo scope |
| 3 | Support keyboard/mouse **and gamepad** for all interactive elements | ❌ Project spec: "Gamepad Support: None (demo target)" | **Conflicts** with project scope; skip until gamepad is planned |
| 4 | All animations skippable; respect motion/accessibility preferences | ❌ Not stated | Useful later (UIFlow has splash/loading animations); low priority |
| 5 | UI sounds through the audio event system | ❌ No audio system yet | Not applicable yet |
| 6 | UI must never block the game thread | ⚠️ Implied by local performance rules | Covered in spirit |
| 7 | Scalable text and colorblind modes **mandatory** | ❌ Not stated | **Conflicts** with demo scope if "mandatory"; useful as a release-phase item |
| 8 | Test all screens at min and max resolution | ❌ Not stated | **Worth adding** — cheap, catches Canvas Scaler mistakes in UIFlow |

## 4. `engine-code.md`

| # | Upstream rule | Local `engine-code.md` | Verdict |
|---|---|---|---|
| 1 | Zero allocations in hot paths | ✅ Same ("Zero-Alloc Hot Paths") | Covered |
| 2 | APIs thread-safe or documented single-thread-only | ✅ Local: all Unity API on main thread | Covered |
| 3 | **Profile before and after every optimization; record the numbers** | ❌ Not stated | **Worth adding** — the project has no profiling evidence anywhere |
| 4 | **Engine code never depends on gameplay code** (engine ← gameplay) | ⚠️ Local: "Core components may not depend on specific state implementations" — narrower | **Worth adding** as a general dependency-direction rule |
| 5 | Every public API has usage examples in its doc comment | ⚠️ Local: one-line doc comment on public Core methods | Local is lighter on purpose; optional |
| 6 | Public interface changes need deprecation period + migration guide | ⚠️ Local: "no breaking changes without design review" | Covered in spirit (solo project) |
| 7 | RAII / deterministic cleanup | ⚠️ C# equivalent: `IDisposable`, `OnDisable` unsubscribe (`manager-event-code.md`) | Covered for events |
| 8 | Graceful degradation | ❌ Not stated | Low priority |
| 9 | **Consult `docs/engine-reference/` before writing engine API code** | ❌ Not stated | **Worth adding** now that the filtered Unity reference exists (`UNITY-2022.3-FILTER.md`) |

## 5. `data-files.md`

| # | Upstream rule | Project reality | Verdict |
|---|---|---|---|
| 1 | All JSON must be valid | Room JSON is loaded by `RoomGeneraterController`; the commit hook validates a non-existent path (`Assets/ScriptableObjects/*.json`) | **Worth adding** (and fix the hook path) |
| 2 | File names lowercase `[system]_[name].json` | Files are `CombatRoom_*.json`, `StartRoom_Entrance.json`, … and are referenced by `filePath: /Data/Json/Room/<Name>.json` in `Maze_Storage.asset` | **Conflicts** — renaming breaks asset references (and room order already broke once, BUG-097) |
| 3 | Documented schema for every data file | `LevelData` shape is only in code | **Worth adding** — schema section in `map-system.md` |
| 4 | Numeric values documented | Partly in `map-system.md` | Covered partly |
| 5 | Keys in `snake_case`, map via a DTO | Keys are serialized `LevelData` field names (camelCase via `JsonUtility`) | **Conflicts** — would require a DTO layer and rewriting all 20 files for no gameplay gain |
| 6 | No orphaned entries | 27 old room files (+ `.meta`) were deleted in `d09671fd`; no orphan check exists | Worth adding |
| 7 | Version data on breaking schema change | Not done | Worth adding |
| 8 | Sensible defaults for optional fields | Not done | Optional |

**Important:** this rule's glob **would load for all 20 room files today**. Adding it unchanged would make
Claude treat the existing, working room data as violating rules 2 and 5. If adopted, take rules 1, 3, 6, 7
only.

## 6. `shader-code.md` (vs local `shader-vfx-code.md`)

| Topic | Upstream | Local `shader-vfx-code.md` | Verdict |
|---|---|---|---|
| Authoring tool | — | Shader Graph first | Local covers |
| Naming | `SG_Env_Water` style prefixes | `PascalCase` (`FlashWhite`, `DissolveEnemy`) | **Conflicts** — pick one before the first custom shader |
| Parameters | Descriptive names, `[Header]` grouping, no magic numbers, header comment | Not stated | Worth adding when shaders exist |
| Performance | Precision (`half`), few texture samples, no branching / no loop texture reads, two-pass blur | No `discard` on mobile, draw-call budget | Complementary |
| Cross-platform | Fallbacks per quality tier, document target pipeline, do not mix pipelines | Fallback required; URP 2D only | Covered |
| Variants | Minimize, document keywords, strip, monitor count | Not stated | Worth adding when shaders exist |
| VFX / particles | — | Pooling, 500-particle cap, sort layer | Local covers |

Effect today: **none** — the project has no custom shaders (only TextMesh Pro's).

## 7. `narrative.md`

Rules: cross-check lore for contradictions; canon level per lore entry; dialogue matches character voice
profile; world rules documented; mysteries have documented answers; faction logic consistent; localization-
ready text (no idioms, named placeholders); **no dialogue line over 120 characters**.

Effect today: **none** — `design/narrative/` is empty. Note that UIFlow already ships a `DialoguePanel`
(`Assets/Script/UIFlow/Gameplay/Dialogue/`); if NPC dialogue is ever authored, the 120-character limit and
named placeholders become relevant.

## 8. `network-code.md`

Rules: server-authoritative state; versioned messages; client prediction + rollback; disconnect / reconnect /
host migration; rate-limited logging; replication strategy per value; bandwidth budget; packet validation.

Effect today: **none** — single-player, no networking package.

## Summary and options

| Rule | Unique, useful items | Conflicts with project | Recommendation |
|---|---|---|---|
| gameplay-code | delta time; gameplay → UI via events | transition tables; code comments | **Merge 2 bullets** into local `gameplay-code.md` |
| ai-code | telegraph intentions; total AI budget | behavior trees | **Merge 2 bullets** into local `ai-code.md` |
| ui-code | no inline player text; min/max resolution test | gamepad; mandatory colorblind | **Merge 2 bullets** into local `ui-code.md` |
| engine-code | profile before/after; dependency direction; consult engine reference | — | **Merge 3 bullets** into local `engine-code.md` |
| data-files | valid JSON; schema; no orphans; versioning | snake_case; lowercase names | **New local `data-files.md`** with only the 4 compatible rules, scoped to `Assets/Data/Json/**` |
| shader-code | parameters, variants, precision | naming prefix | **Defer** — add when the first custom shader is written; decide naming then |
| narrative | 120-char lines, placeholders | — | **Defer** — add when `design/narrative/` gets content |
| network-code | — | — | **Skip** |

Owner decision needed: which of the "Merge" rows to apply. Nothing has been changed yet.
