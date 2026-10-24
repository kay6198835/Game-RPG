# Unity 6.3 → 2022.3 Filter for the CCGS Reference Docs

> **Added 2026-10-08** with the CCGS v1.1.2 framework upgrade. The files listed below come from the
> upstream template, which documents **Unity 6.3 LTS**. This project pins **Unity 2022.3.62f3 LTS**
> ([VERSION.md](VERSION.md)). This table is the filter: it says which parts apply here. Each file also
> carries the same verdicts inline, next to the section they cover.
>
> Authority order when sources disagree: `CLAUDE.md` and `.claude/rules/*.md` → `VERSION.md` → this
> filter → the upstream text.

## Legend

| Marker | Meaning |
|---|---|
| ⛔ NOT IN 2022.3 | Unity 6-only API or feature. Using it will not compile or does not exist |
| ✅ OK IN 2022.3 | Applies to 2022.3, even when the upstream text labels it "Unity 6+" |
| ❌ INCORRECT | The upstream claim does not hold (or has no source). Do not rely on it |
| ➖ NOT USED | Valid in principle, but the project has no such package or system |

## Highest-risk items (read these first)

| Upstream says | Reality for this project | Marker |
|---|---|---|
| Custom URP passes use `RecordRenderGraph(RenderGraph, ContextContainer)` | URP 14 (2022.3) uses `Execute(ScriptableRenderContext, ref RenderingData)` | ⛔ |
| `Rigidbody.velocity` is deprecated; write `linearVelocity` / use `AddForce` | `Rigidbody2D.velocity` is the correct 2022.3 API and the project's chosen movement pattern. `linearVelocity` does not exist in 2022.3 | ❌ / ⛔ |
| UGUI (Canvas, Image) is deprecated; migrate to UI Toolkit | UGUI is supported. UIFlow (menus, loading, HUD) is UGUI + TextMeshPro | ❌ |
| `[GenerateSerializer]` source-generated serialization | Not a Unity API (Microsoft Orleans) | ❌ |
| Physics module (Rigidbody, `OnTriggerEnter(Collider)`, `Physics.Raycast`) | 3D only. Project is 2D: `Rigidbody2D`, `OnTriggerEnter2D(Collider2D)`, `Physics2D.*NonAlloc`. A 3D callback compiles but never fires (BUG-075) | ⚠️ |
| `Awaitable`, `Object.InstantiateAsync`, GPU Resident Drawer, Muse | Unity 2023.1+ / 6 only | ⛔ |
| PhysX 5.1 in Unity 6; default solver iterations 6 → 8 | No source given; irrelevant to Physics2D | ❌ |
| Built-in Particle System deprecated | Not deprecated | ❌ |

## Per-file verdict

| File | Overall | Notes |
|---|---|---|
| `breaking-changes.md` | ⚠️ Read in reverse | Lists 2022 → 6 changes; the "NEW" side is Unity 6. Input System entry ✅; RenderGraph ⛔; UGUI / particles ❌ |
| `deprecated-apis.md` | ⚠️ Mixed | Input ✅, `Text` → TMP ✅, `RaycastAll` → NonAlloc ✅, `Resources.Load` ✅ (forbidden by project rules); Canvas/Image ❌; `Rigidbody.velocity` ❌; RenderGraph ⛔ |
| `current-best-practices.md` | ⚠️ Mixed | Input ✅, Test Framework + CLI ✅ (exit codes observed on Unity 6 — re-verify), logging ✅, GPU instancing ✅; Unity 6.3 upgrade ⛔, RenderGraph ⛔, `Awaitable` ⛔, `[GenerateSerializer]` ❌; DOTS / Addressables / Netcode ➖ |
| `PLUGINS.md` | ⚠️ Index | Input System ✅ in use, UI Toolkit partly in use, Timeline installed; Cinemachine / Addressables / DOTS / Netcode / Sentis ➖; Muse ⛔ |
| `modules/input.md` | ✅ | Matches project usage (Input System 1.14, generated `PlayerInput` class) |
| `modules/animation.md` | ✅ mostly | Mecanim, blend trees, events valid. Animation Rigging ✅ but ➖ (3D). Project rule on animation events wins |
| `modules/ui.md` | ✅ with caveat | Both stacks valid in 2022.3; project's main stack is UGUI + TMP |
| `modules/rendering.md` | ⚠️ Mixed | RenderGraph ⛔, GPU Resident Drawer ⛔; Volumes, Rendering Debugger, lightmapper ✅ |
| `modules/physics.md` | ⚠️ 3D only | Use the Physics2D equivalents; velocity advice ❌ for this project |
| `modules/audio.md` | ✅ ➖ | Version-independent; no audio code yet; keep `spatialBlend = 0` for 2D |
| `modules/navigation.md` | ➖ | Project uses its own A* (ADR-0002), not NavMesh |
| `modules/networking.md` | ➖ | Single-player |
| `plugins/addressables.md`, `plugins/cinemachine.md`, `plugins/dots-entities.md` | ➖ | Not installed |

## What was not filtered

- Upstream `VERSION.md` (Unity 6.3) was **not** copied; the project's own [VERSION.md](VERSION.md) stays
  authoritative and holds the real package list (Input System 1.14.0, TMP 3.0.7, VContainer 1.19.0, …).
- Code samples inside each section were left verbatim; the inline marker above them sets how to read them.
- Verdicts are from knowledge of Unity 2022.3 / URP 14 and the project's `Packages/manifest.json`, not
  from running each sample. When a sample matters, verify it in the Editor.
