# Unity 6.3 — Physics Module Reference

> ⚠️ **Project filter (Game-RPG, 2026-10-08).** This file comes from the CCGS v1.1.2 template and was written for **Unity 6.3 LTS**. This project pins **Unity 2022.3.62f3 LTS** ([VERSION.md](../VERSION.md)). Read every item through the filter table in [UNITY-2022.3-FILTER.md](../UNITY-2022.3-FILTER.md). Inline markers: **⛔ NOT IN 2022.3** (do not use), **✅ OK IN 2022.3** (applies despite a Unity 6 label), **❌ INCORRECT** (claim does not hold — do not rely on it), **➖ NOT USED** (valid but this project has no such system). Project rules in `.claude/rules/` and `CLAUDE.md` win over anything here.

> **⚠️ 3D ONLY.** This page documents `Physics` / `Rigidbody` / `Collider` (3D). The project is 2D: use `Physics2D`, `Rigidbody2D`, `Collider2D`, `OnTriggerEnter2D(Collider2D)` and `Physics2D.OverlapCircleNonAlloc` — the 3D callbacks compile but are never called on 2D objects (that was BUG-075).

**Last verified:** 2026-02-13
**Knowledge Gap:** Unity 6 physics improvements, solver changes

---

## Overview

> **❌ Unverified** — the PhysX 5.1 claim is not backed by a source here; it does not affect Physics2D (Box2D) either way.

Unity 6.3 uses **PhysX 5.1** (improved from PhysX 4.x in 2022 LTS):
- Better solver stability
- Improved performance
- Enhanced collision detection

---

## Key Changes from 2022 LTS

> **❌ Unverified** — do not tune solver iterations on this basis.

### Default Solver Iterations Increased
Unity 6 increased default solver iterations for better stability:

```csharp
// Default changed from 6 to 8 iterations
Physics.defaultSolverIterations = 8; // Check if relying on old behavior
```

### Enhanced Collision Detection

```csharp
// ✅ Unity 6: Improved Continuous Collision Detection (CCD)
rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
// Better handling of fast-moving objects
```

---

## Core Physics Components

### Rigidbody

> **❌ Wrong advice for this project** — writing `Rigidbody2D.velocity` in `PhysicsUpdate()` is the project's chosen pattern (top-down movement). In 2022.3 the property is `velocity`; **⛔ `linearVelocity` is Unity 6 only.**

```csharp
// ✅ Best practice: Use AddForce, not direct velocity writes
Rigidbody rb = GetComponent<Rigidbody>();
rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);

// ❌ Avoid: Direct velocity assignment (can cause instability)
rb.velocity = new Vector3(0, 10, 0); // Only use when necessary
```

### Colliders

```csharp
// Primitive colliders: Box, Sphere, Capsule (cheapest)
// Mesh colliders: Expensive, use only for static geometry

// ✅ Compound colliders (multiple primitives) > single mesh collider
```

---

## Raycasting

### Efficient Raycasting (Avoid Allocations)

```csharp
// ✅ Non-allocating raycast
if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance)) {
    Debug.Log($"Hit: {hit.collider.name}");
}

// ✅ Multiple hits (non-allocating)
RaycastHit[] results = new RaycastHit[10];
int hitCount = Physics.RaycastNonAlloc(origin, direction, results, maxDistance);
for (int i = 0; i < hitCount; i++) {
    Debug.Log($"Hit {i}: {results[i].collider.name}");
}

// ❌ Avoid: RaycastAll (allocates array every call)
RaycastHit[] hits = Physics.RaycastAll(origin, direction); // GC allocation!
```

### LayerMask for Selective Raycasting

```csharp
// ✅ Use LayerMask to filter collisions
int layerMask = 1 << LayerMask.NameToLayer("Enemy");
Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, layerMask);
```

---

## Physics Queries

### OverlapSphere (Check for nearby objects)

```csharp
// ✅ Non-allocating version
Collider[] results = new Collider[10];
int count = Physics.OverlapSphereNonAlloc(center, radius, results);
for (int i = 0; i < count; i++) {
    // Process results[i]
}
```

### SphereCast (Thick raycast)

```csharp
// Useful for character controllers
if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, maxDistance)) {
    // Hit something with a sphere-shaped ray
}
```

---

## Collision Events

### OnCollisionEnter / Stay / Exit

```csharp
void OnCollisionEnter(Collision collision) {
    // Triggered when collision starts
    Debug.Log($"Collided with {collision.gameObject.name}");

    // Access contact points
    foreach (ContactPoint contact in collision.contacts) {
        Debug.DrawRay(contact.point, contact.normal, Color.red, 2f);
    }
}
```

### OnTriggerEnter / Stay / Exit

```csharp
void OnTriggerEnter(Collider other) {
    // Trigger collider (Is Trigger = true)
    if (other.CompareTag("Pickup")) {
        Destroy(other.gameObject);
    }
}
```

---

## Character Controllers

### CharacterController Component

```csharp
CharacterController controller = GetComponent<CharacterController>();

// ✅ Move with collision detection
Vector3 move = transform.forward * speed * Time.deltaTime;
controller.Move(move);

// Apply gravity manually
if (!controller.isGrounded) {
    velocity.y += Physics.gravity.y * Time.deltaTime;
}
controller.Move(velocity * Time.deltaTime);
```

---

## Physics Materials

### Friction & Bounciness

```csharp
// Create: Assets > Create > Physic Material
// Assign to collider: Collider > Material

// PhysicMaterial settings:
// - Dynamic Friction: 0.6 (sliding friction)
// - Static Friction: 0.6 (starting friction)
// - Bounciness: 0.0 - 1.0
// - Friction Combine: Average, Minimum, Maximum, Multiply
// - Bounce Combine: Average, Minimum, Maximum, Multiply
```

---

## Joints

### Fixed Joint (Attach two rigidbodies)

```csharp
FixedJoint joint = gameObject.AddComponent<FixedJoint>();
joint.connectedBody = otherRigidbody;
```

### Hinge Joint (Door, wheel)

```csharp
HingeJoint hinge = gameObject.AddComponent<HingeJoint>();
hinge.axis = Vector3.up; // Rotation axis
hinge.useLimits = true;
hinge.limits = new JointLimits { min = -90, max = 90 };
```

---

## Performance Optimization

### Physics Layer Collision Matrix
`Edit > Project Settings > Physics > Layer Collision Matrix`
- Disable unnecessary collision checks between layers
- Massive performance gain

### Fixed Timestep
`Edit > Project Settings > Time > Fixed Timestep`
- Default: 0.02 (50 FPS physics)
- Lower = more accurate, higher CPU cost
- Match game's target framerate if possible

### Simplified Collision Geometry
- Use primitive colliders (box, sphere, capsule) over mesh colliders
- Bake mesh colliders at build time, not runtime

---

## Common Patterns

### Ground Check (Character Controller)

```csharp
bool IsGrounded() {
    float rayLength = 0.1f;
    return Physics.Raycast(transform.position, Vector3.down, rayLength);
}
```

### Apply Explosion Force

```csharp
void ApplyExplosion(Vector3 explosionPos, float radius, float force) {
    Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
    foreach (Collider hit in colliders) {
        Rigidbody rb = hit.GetComponent<Rigidbody>();
        if (rb != null) {
            rb.AddExplosionForce(force, explosionPos, radius);
        }
    }
}
```

---

## Debugging

### Physics Debugger (Unity 6+)

> **✅ OK IN 2022.3** — Window > Analysis > Physics Debugger exists (3D). For 2D, use Gizmos.
- `Window > Analysis > Physics Debugger`
- Visualize colliders, contacts, queries

### Gizmos

```csharp
void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, detectionRadius);
}
```

---

## Sources
- https://docs.unity3d.com/6000.0/Documentation/Manual/PhysicsOverview.html
- https://docs.unity3d.com/ScriptReference/Physics.html
