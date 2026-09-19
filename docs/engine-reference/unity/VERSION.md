# Unity — Version Reference

| Field | Value |
|-------|-------|
| **Engine Version** | Unity 2022.3.62f3 LTS |
| **Project Pinned** | 2026-05-19 (bumped f1→f3 per ProjectSettings/ProjectVersion.txt, verified 2026-07-09) |
| **LLM Knowledge Cutoff** | May 2025 |
| **Risk Level** | LOW — version is within LLM training data |
| **Last Docs Verified** | 2026-09-11 |

## Pinned Package Versions

> **Table corrected 2026-09-11.** VContainer was added on 2026-08-22 (`aa4e620`) and was missing
> from this table for three sprints. Versions below are read from `Packages/manifest.json`.

| Package | Version | Notes |
|---------|---------|-------|
| Input System | 1.14.0 | New Input System — use `PlayerInput` / `InputAction`, not legacy `Input.GetAxis` |
| TextMeshPro | 3.0.7 | Use `TextMeshProUGUI`, never legacy `Text` component |
| 2D Feature Pack | 2.0.1 | 2D Renderer (URP), Tilemap, Sprite Atlas |
| **VContainer** | **1.19.0** | **Dependency injection.** Git package (`jp.hadashikick.vcontainer`), not on the Unity registry. See ADR-0004 |
| Test Framework | 1.1.33 | Present; `tests/EditMode` and `tests/PlayMode` are still empty (TD-014) |
| Timeline | 1.7.7 | Present, not used in gameplay code |
| Visual Scripting | 1.9.4 | Present but not used in primary gameplay code |
| uGUI | 1.0.0 | Canvas UI |
| DOTween | — | **Asset Store import under `Assets/Plugins/`, not in `manifest.json`** — it will not restore from a clean package cache |

## Rendering Pipeline

- **Pipeline**: Universal Render Pipeline (URP) — 2D Renderer
- **Shaders**: Use URP-compatible shaders only; legacy Built-In shaders will not render correctly
- **Shader Graph**: Supported — use for all new shader authoring

## Key API Notes (Unity 2022.3 LTS)

### Input System 1.14
- Use `InputSystem.onAnyButtonPress` for any-key detection, not `Input.anyKeyDown`
- `PlayerInput` component drives Unity Events or C# events — choose one pattern per project
- `InputAction.ReadValue<T>()` is the correct way to read axis values

### Physics2D
- `Physics2D.OverlapCircleNonAlloc(pos, radius, results, contactFilter)` — use NonAlloc variants in hot paths
- `Physics2D.OverlapCircle` allocates — only acceptable outside hot paths (Awake, editor buttons)
- Layer masks must be set via `LayerMask.GetMask("LayerName")` or Inspector — never hardcode layer indices

### Addressables (not currently used)
- If added in future: use `Addressables.LoadAssetAsync<T>` + `Addressables.Release` — never use `Resources.Load` for new assets

### VContainer 1.19.0 (added 2026-09-11)

- `LifetimeScope` is a MonoBehaviour — the scope must exist **in the scene**, and `Configure()`
  runs during `Awake()`
- `RegisterComponentInHierarchy<T>()` searches the scene and **never instantiates**. A component
  absent at `Awake()` throws immediately, before any gameplay code runs
- Objects created at runtime (pooled enemies, projectiles, items) cannot be registered this way —
  inject them at spawn time instead (`Pool.Spawn()` does this)
- Prefer `[Inject] public void Construct(...)` method injection for MonoBehaviours; constructor
  injection is not available to them
- Register behind interfaces so the container never hands out a concrete MonoBehaviour type

### UI Toolkit vs UGUI

> **Corrected 2026-09-11.** This section said runtime UI Toolkit was "not recommended … without
> explicit decision". Runtime UI Toolkit had in fact already shipped, so the guidance was being
> contradicted by the codebase every time anyone read it. The decision was never written down —
> that gap is tracked under BUG-052.

- This project uses **both**, by area:
  - **UI Toolkit (UXML/USS) at runtime** — `Assets/Script/UI/UIController.cs` loads MainMenu,
    Settings and Pause from `Assets/UI/Screens/*.uxml`
  - **UGUI + TextMeshPro** — in-world and HUD elements, including the enemy health bar
    (`EntityUIController`)
- Never use the legacy `Text` component; `TextMeshProUGUI` only
- New *menu* screens follow the UI Toolkit path; new *HUD* elements follow the UGUI path. Neither
  has a GDD yet

### Animator / Animation
- `Animator.SetBool` / `SetTrigger` are the correct calls — never use `CrossFade` without checking current state
- Animation Events call methods on GameObjects in the hierarchy — wire through `AnimationEventManager`, not direct state calls

## Deprecated / Avoid in 2022.3

| Avoid | Use Instead |
|-------|------------|
| `Input.GetAxis()` | `InputAction.ReadValue<Vector2>()` |
| Legacy `Text` component | `TextMeshProUGUI` |
| `FindObjectOfType<T>()` in hot paths | Inspector refs or `GetComponent<T>()` |
| `Resources.Load()` | Direct Inspector references |
| `Physics2D.OverlapCircle` in `Update()` | `Physics2D.OverlapCircleNonAlloc` |

## Migration Notes

No migration required — project is on 2022.3 LTS, a stable long-term support release.
2022.3 receives only bug fixes; no API breaking changes expected.

**Third-party risk note (2026-09-11):** VContainer is pinned by git URL to tag `1.19.0`, not by
the Unity package registry. A moved or deleted upstream repository breaks package resolution for
the whole project. DOTween is vendored under `Assets/Plugins/` and is therefore committed, but is
invisible to `manifest.json` — neither dependency is reproducible from the manifest alone.

## Note

This engine version is within the LLM's training data. Full reference docs
(breaking-changes.md, deprecated-apis.md) are not required. Run `/setup-engine refresh`
to populate full reference docs if agents suggest incorrect APIs.
