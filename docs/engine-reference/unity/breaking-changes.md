# Unity 6.3 LTS — Breaking Changes

> ⚠️ **Project filter (Game-RPG, 2026-10-08).** This file comes from the CCGS v1.1.2 template and was written for **Unity 6.3 LTS**. This project pins **Unity 2022.3.62f3 LTS** ([VERSION.md](VERSION.md)). Read every item through the filter table in [UNITY-2022.3-FILTER.md](UNITY-2022.3-FILTER.md). Inline markers: **⛔ NOT IN 2022.3** (do not use), **✅ OK IN 2022.3** (applies despite a Unity 6 label), **❌ INCORRECT** (claim does not hold — do not rely on it), **➖ NOT USED** (valid but this project has no such system). Project rules in `.claude/rules/` and `CLAUDE.md` win over anything here.

> **How to read this file here:** it lists what changed from 2022 LTS **to** Unity 6. For this project the direction is reversed: every "✅ NEW" example below is a Unity 6 API that **does not exist in 2022.3** unless marked otherwise. The "❌ OLD" side is what 2022.3 actually has.

**Last verified:** 2026-02-13

This document tracks breaking API changes and behavioral differences between Unity 2022 LTS
(likely in model training) and Unity 6.3 LTS (current version). Organized by risk level.

## HIGH RISK — Will Break Existing Code

### Entities/DOTS API Complete Overhaul

> **➖ NOT USED** — no Entities package. (Entities 1.x also supports 2022.3, so the Unity 6-only framing is not accurate.)
**Versions:** Entities 1.0+ (Unity 6.0+)

```csharp
// ❌ OLD (pre-Unity 6, GameObjectEntity pattern)
public class HealthComponent : ComponentData {
    public float Value;
}

// ✅ NEW (Unity 6+, IComponentData)
public struct HealthComponent : IComponentData {
    public float Value;
}

// ❌ OLD: ComponentSystem
public class DamageSystem : ComponentSystem { }

// ✅ NEW: ISystem (unmanaged, Burst-compatible)
public partial struct DamageSystem : ISystem {
    public void OnCreate(ref SystemState state) { }
    public void OnUpdate(ref SystemState state) { }
}
```

**Migration:** Follow Unity's ECS migration guide. Major architectural changes required.

---

### Input System — Legacy Input Deprecated

> **✅ OK IN 2022.3** — the project already uses Input System 1.14; `Keyboard.current` / `InputAction` work in 2022.3.
**Versions:** Unity 6.0+

```csharp
// ❌ OLD: Input class (deprecated)
if (Input.GetKeyDown(KeyCode.Space)) { }

// ✅ NEW: Input System package
using UnityEngine.InputSystem;
if (Keyboard.current.spaceKey.wasPressedThisFrame) { }
```

**Migration:** Install Input System package, replace all `Input.*` calls with new API.

---

### URP/HDRP Renderer Feature API Changes

> **⛔ NOT IN 2022.3** — 2022.3 ships URP 14, where custom passes override `Execute(ScriptableRenderContext, ref RenderingData)`. `RecordRenderGraph` is URP 17 (Unity 6). Use the OLD form.
**Versions:** Unity 6.0+

```csharp
// ❌ OLD: ScriptableRenderPass.Execute signature
public override void Execute(ScriptableRenderContext context, ref RenderingData data)

// ✅ NEW: Uses RenderGraph API
public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
```

**Migration:** Update custom render passes to use RenderGraph API.

---

## MEDIUM RISK — Behavioral Changes

### Addressables — Asset Loading Returns

> **➖ NOT USED** — no Addressables package. The "throws by default in 6.2+" claim is unverified.
**Versions:** Unity 6.2+

Asset loading failures now throw exceptions by default instead of returning null.
Add proper exception handling or use `TryLoad` variants.

```csharp
// ❌ OLD: Silent null on failure
var handle = Addressables.LoadAssetAsync<Sprite>("key");
var sprite = handle.Result; // null if failed

// ✅ NEW: Throws on failure, use try/catch or TryLoad
try {
    var handle = Addressables.LoadAssetAsync<Sprite>("key");
    var sprite = await handle.Task;
} catch (Exception e) {
    Debug.LogError($"Failed to load: {e}");
}
```

---

### Physics — Default Solver Iterations Changed

> **❌ Unverified** — no source given; do not change solver iterations on this basis. The project uses Physics2D, which this does not cover.
**Versions:** Unity 6.0+

Default solver iterations increased for better stability.
Check `Physics.defaultSolverIterations` if you rely on old behavior.

---

## LOW RISK — Deprecations (Still Functional)

### UGUI (Legacy UI)

> **❌ INCORRECT** — UGUI is not deprecated in 2022.3 or Unity 6. This project's whole UIFlow (HUD, menus) is UGUI + TextMeshPro. Do not migrate it.
**Status:** Deprecated but supported
**Replacement:** UI Toolkit

UGUI still works but UI Toolkit is recommended for new projects.

---

### Legacy Particle System

> **❌ INCORRECT** — the Built-in Particle System (Shuriken) is not deprecated; VFX Graph is an alternative for GPU effects.
**Status:** Deprecated
**Replacement:** Visual Effect Graph (VFX Graph)

---

### Old Animation System

> **✅ OK IN 2022.3** — use Animator/Mecanim (the project already does).
**Status:** Deprecated
**Replacement:** Animator Controller (Mecanim)

---

## Platform-Specific Breaking Changes

> **⛔ NOT IN 2022.3** — Unity 6 platform minimums. Project target is PC (Windows) only.

### WebGL
- **Unity 6.0+**: WebGPU is now the default (WebGL 2.0 fallback available)
- Update shaders for WebGPU compatibility

### Android
- **Unity 6.0+**: Minimum API level raised to 24 (Android 7.0)

### iOS
- **Unity 6.0+**: Minimum deployment target raised to iOS 13

---

## Migration Checklist

> **➖ NOT APPLICABLE** — only for a future upgrade to Unity 6. No upgrade is planned.

When upgrading from 2022 LTS to Unity 6.3 LTS:

- [ ] Audit all DOTS/ECS code (complete rewrite likely needed)
- [ ] Replace `Input` class with Input System package
- [ ] Update custom render passes to RenderGraph API
- [ ] Add exception handling to Addressables calls
- [ ] Test physics behavior (solver iterations changed)
- [ ] Consider migrating UGUI to UI Toolkit for new UI
- [ ] Update WebGL shaders for WebGPU
- [ ] Verify minimum platform versions (Android/iOS)

---

**Sources:**
- https://docs.unity3d.com/6000.0/Documentation/Manual/upgrade-guides.html
- https://docs.unity3d.com/Packages/com.unity.entities@1.3/manual/upgrade-guide.html
