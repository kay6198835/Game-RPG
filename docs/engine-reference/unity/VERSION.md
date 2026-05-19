# Unity — Version Reference

| Field | Value |
|-------|-------|
| **Engine Version** | Unity 2022.3.62f1 LTS |
| **Project Pinned** | 2026-05-19 |
| **LLM Knowledge Cutoff** | May 2025 |
| **Risk Level** | LOW — version is within LLM training data |
| **Last Docs Verified** | 2026-05-19 |

## Pinned Package Versions

| Package | Version | Notes |
|---------|---------|-------|
| Input System | 1.14.0 | New Input System — use `PlayerInput` / `InputAction`, not legacy `Input.GetAxis` |
| TextMeshPro | 3.0.7 | Use `TextMeshProUGUI`, never legacy `Text` component |
| 2D Feature Pack | current | 2D Renderer (URP), Tilemap, Sprite Atlas |
| Visual Scripting | 1.9.4 | Present but not used in primary gameplay code |

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

### UI Toolkit vs UGUI
- This project uses UGUI (Canvas-based) with TextMeshPro
- UI Toolkit (UXML/USS) is available in 2022.3 for Editor tools — not recommended for runtime UI in this project without explicit decision

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

## Note

This engine version is within the LLM's training data. Full reference docs
(breaking-changes.md, deprecated-apis.md) are not required. Run `/setup-engine refresh`
to populate full reference docs if agents suggest incorrect APIs.
