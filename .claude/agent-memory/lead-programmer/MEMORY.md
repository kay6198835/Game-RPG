# Lead Programmer Memory — Game-RPG

## Project Context

Unity 2022.3.62f3 LTS action roguelike. Combat inspired by Cult of the Lamb.
Main dev scene: `Assets/Scenes/Main/Test/RandomMaze.unity`

## Active Known Bugs (block demo)

1. `WeaponMelee.Attack()` is EMPTY — player deals no damage → mirror EntityWeaponMelee.Attack()
2. `RoomMapController.GetNextRoom()` compile error — missing `Columns` property, bad self-ref
3. `MainMapController.LoadRoom()` typo: `cuonefsakdjfhnasdklfhjasdrrent` → `current`
4. `MainMapController.Start()` calls missing method — use `.RoomMapController.GetStartRoom()`
5. `EntityDeathState` extends MonoBehaviour — must extend EntityState
6. `EntityMoveState` NullRef when target lost — add null guard at top of Update
7. No player death check — `currentHealth <= 0` triggers nothing in `Core.TakeDamage()`

## Architecture Invariants

- State machine for all characters — no inline if/else in Update
- ScriptableObject-first: all gameplay values in SO assets
- INegativeReciver interface for all damage (player, enemy, projectile)
- EventManager static bus for cross-system events (note: typo "Resgister" is intentional)
- MazeController is the ONLY singleton — no new singletons ever
- Core.cs / EntityCore.cs are the ONLY component hubs

## Coding Standards Shortcuts

- `[SerializeField] private` not `public` for inspector fields
- Cache GetComponent in Awake, never in Update
- OverlapCircleNonAlloc with pre-allocated array — never OverlapCircle in hot paths
- Damage chain: Weapon.Attack() → OverlapCircleNonAlloc → INegativeReciver.TakeDamage(amount, pos)
