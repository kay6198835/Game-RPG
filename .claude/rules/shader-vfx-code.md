---
description: Shader and VFX standards for Unity URP 2D
globs: ["Assets/Shader/**/*", "Assets/Sprite/VFX/**/*", "Assets/Particle Effect/**/*"]
---

# Shader and VFX Standards

## Unity URP 2D Context
This project uses Unity 2022.3 LTS with the 2D Feature Pack.
All visual effects must be compatible with the 2D Renderer pipeline.

## Shader Rules
- Use Shader Graph for new shaders — no hand-written ShaderLab unless Shader Graph cannot achieve the effect
- All shaders must have a fallback defined for low-end hardware
- Sample textures with UV variations, not hardcoded UVs
- No `discard` in fragment shaders on mobile-target platforms (affects batching)

## VFX / Particle Systems
- Particle systems must use the `Pooling/` system — never `Instantiate` at runtime
- Maximum 500 particles per system for 2D gameplay effects
- Stop Action must be set to `Destroy` only if not pooled; pooled systems use `Disable`
- Sort layer must be explicitly set — never leave at default

## Performance Budget
- Draw calls from VFX: max +5 per active effect on screen
- No VFX that allocate GPU memory per-frame
- Prefer sprite sheet animation over particle simulation for frequently-used small effects (hits, sparks)

## Naming
- Shader assets: `PascalCase` — `FlashWhite`, `DissolveEnemy`
- Particle assets: `PascalCase` + type suffix — `HitSpark`, `DeathExplosion`, `DashTrail`
