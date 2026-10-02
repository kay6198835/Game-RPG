# Báo cáo kiến trúc — Hệ thống Boss Data-Driven

> **Phạm vi:** đề xuất kiến trúc, chưa viết code. Không file nào trong project bị tạo hoặc sửa.
> **Căn cứ:** đọc source tại HEAD `c7b02a8`, branch `origin/feature/boss-design` (2026-09-29).
> **Engine:** Unity 2022.3.62f3 LTS, VContainer 1.19.0.
> **Lưu ý:** trong repo không có "báo cáo kiến trúc boss" nào trước đó (chỉ còn
> `Assets/Script/Character/Boss/` gồm ba file `.meta` mồ côi từ commit boss đã bị revert). Vì vậy
> đây là **bản đầy đủ**, viết lại từ đầu và bao gồm toàn bộ yêu cầu "Fully Data-Driven Boss".

---

## 0. Tóm tắt (TL;DR)

1. **Được, nhưng có giới hạn rõ ràng.** Một `BossDefinition` ScriptableObject *có thể* là **điểm vào
   duy nhất** (single entry point) cho toàn bộ cấu hình của một boss. Nhưng nó **không thể là file
   duy nhất**, và **cũng không thể loại bỏ C# cho mechanic mới**. Clip animation, Animator Controller,
   prefab (sprite, collider, hurtbox) và projectile prefab vẫn là asset riêng. `BossDefinition` chỉ
   *tham chiếu* tới chúng.
2. **Ranh giới giữa data và code:** *số liệu, lựa chọn, thứ tự, điều kiện, trọng số, thời lượng* nằm
   trong data. *Hành vi thực thi* (cách một đợt bắn xoay tròn sinh ra đạn, cách một vụ nổ quét
   collider) là code, viết **một lần** dưới dạng `BossAction` tái sử dụng. Boss mới không cần code;
   **mechanic mới** thì cần đúng một class mới, và không phải sửa controller.
3. **Kiến trúc đề xuất:** HFSM nhỏ, cố định trong code (Brain / Attack / PhaseTransition / Death)
   + bộ chọn đòn đánh theo **priority + weight + condition** (một dạng "utility-lite") + **sequence**
   (chuỗi đòn). Mọi thứ bên trong state đều đọc từ data. **Không** dùng Behavior Tree và **không**
   làm graph editor ở giai đoạn này.
4. **Tái sử dụng tối đa hệ thống hiện có:** `BossController : Entity`, `BossDefinition : EntityData`.
   Nhờ đó dùng lại nguyên `EntityCore`, `EntityVitalStats`, `EntityStatsHandler` (đã clone stat SO
   theo từng instance), `EntityMovement` (A*), `EntityInput`, pool + VContainer injection,
   `ProjectileBody` / `IProjectilePayload` (commit `b7a0af5`), và luồng `ON_ENEMY_DEATH` →
   `RoomCell` → mở cửa.
5. **Polymorphism khi serialize:** `[SerializeReference]` cho `BossAction`, `BossCondition` và
   `BossMovementPolicy` (tham số nằm inline, không bị "bùng nổ" asset), cộng một PropertyDrawer
   chọn type (~100 dòng editor code, **bắt buộc**, vì Unity 2022.3 không có sẵn dropdown chọn type).
   Các attack là **asset SO** để dùng chung giữa các boss và phase.
6. **Có ba điểm tiền đề phải xử lý trước khi viết boss** (mục 1.3): BUG-066/070 (dictionary vitals
   không có guard), việc enemy lấy từ pool **không reset state machine**, và một chỗ lệch tên
   parameter trong Animator (`TakeDamge`).

---

## 1. Hiện trạng codebase liên quan đến boss

### 1.1 Những gì dùng lại được

| Hệ thống hiện có | File | Boss dùng thế nào |
|---|---|---|
| `Entity : CharacterBase<EntityCore>` | `Character/Entity/Entity.cs` | `BossController` kế thừa → được sẵn root, `EntityCore`, animation event, `ICharacter` |
| `EntityData : CharacterData` | `Character/Entity/EntityData.cs` | `BossDefinition` kế thừa → sẵn `Stats`, `AbilityBindings`, `DefaultWeapon`, `Aima`, `LayerMask` |
| `EntityStatsHandler` (clone `BaseStatsSO` theo từng instance) | `EntityStatsHandler.cs:761-768` | Buff/phase modifier không rò sang asset và không lây sang instance khác — **đúng yêu cầu Scenario 4** |
| `EntityVitalStats.Reborn()` từ `OnEnable` | `EntityVitalStats.cs:736-749` | Đầy máu, xoá modifier, xoá knockback/lock khi respawn từ pool |
| `MovementBase` / `IMovement` (lock theo source, speed multiplier, knockback) | `Character/Base/MovementBase.cs` | Khoá di chuyển trong lúc đánh, dash (qua `ApplyKnockback`), phase speed multiplier |
| `EntityMovement` (A* qua `EnemyManager`) | `EntityMovement.cs` | Chính sách di chuyển Chase / KeepDistance |
| `NegativeReceiverBase` → `EntityNegativeReciver` (`Mitigate` / `OnDamaged` là `protected virtual`) | `Character/Base/NegativeReceiverBase.cs` | `BossNegativeReceiver : EntityNegativeReciver` — thêm invulnerable + event HUD mà **không sửa file gốc** |
| `ProjectileBody` + `IProjectilePayload` + `ProjectileConfig` | `Runtime/SpawnMono/ProjectileBody.cs` | Đạn của boss: body lo phần bay/va chạm/pool, payload của boss lo damage |
| `IObjecPoolService`, `Pool.Spawn()` inject VContainer | `System/PoolableService/` | Boss, đạn, telegraph, minion đều đi qua pool |
| `EventManager` + `ON_ENEMY_DEATH` / `ON_SPAWN_EXTRA_ENEMY` | `Manager/EventManager.cs` | Boss chết → phòng được clear; minion triệu hồi được `RoomCell` đếm |
| Abilities v2 (`AbilityEffectDefinition`) | `System/Abilities/` | *Tuỳ chọn:* bridge action để boss dùng lại effect có sẵn (mục 5.3.5) |
| Mẫu "Definition (SO) → Instance (C# thuần)" | `AbilityDefinition` → `AbilityInstance` | Boss dùng cùng mẫu: `BossAttackDefinition` → `BossAttackExecutor` |
| Mẫu override Animator | `AttackSO.directionAttackAnimatorOV`, `AbilityDefinition.AnimatorOverride` | Boss dùng `AnimatorOverrideController` theo từng boss, không đổi theo từng đòn (mục 5.2) |

### 1.2 Những gì **không** dùng lại, và lý do

| Thứ | Lý do không dùng cho boss |
|---|---|
| Các state `EntityBasicState` / `EntityIdleState` / `EntityMoveState` / `EntityAttackState` | Transition hardcode (`EntityIdleState.cs:275` gán cứng `idleDurationTime = 3`; move timeout hardcode `10`). Mỗi hit đều chuyển sang `TakeDamageState` → boss bị stun-lock. Boss cần super armor. |
| `EntityWeaponHolder` + `Weapon` + `AttackSO` | Một `AttackSO` chỉ có range/damage/rate/override. Không có telegraph, không có nhiều cửa sổ damage, không có di chuyển, không sinh object. Quy tắc "no weapon, no attack" vẫn áp dụng cho enemy thường; boss là **một đường tấn công riêng**, cần ghi vào ADR. |
| `AbilityDefinition` làm định nghĩa đòn của boss | Vòng đời theo input (Active/Hold), không có stage có thời lượng. Còn bug mở (BUG-083: `HoldRatio` luôn bằng 0; BUG-089: effect tier là scaffolding). `SpawnEffectBase._context` là **state runtime lưu trên SO dùng chung** — đúng loại lỗi mà Scenario 4 cấm. Không nên xây boss lên đó; chỉ bridge khi thật cần. |
| `EntityFindTarget` min/max range | Range là field trên **component của prefab**, không nằm trong SO. Boss dùng range riêng của từng đòn trong data. |

### 1.3 Lỗi / khoảng trống hiện có ảnh hưởng trực tiếp đến boss

| # | Vấn đề | Bằng chứng | Ảnh hưởng tới boss | Đề xuất |
|---|---|---|---|---|
| P1 | Dictionary vitals không có guard (BUG-066 = BUG-070) | `VitalStatsBase.cs:28,39,41,45,52,54,58` | Boss đọc `HP` mỗi frame để check phase → `KeyNotFoundException` nếu profile thiếu key | Sửa trước (đã nằm trong sprint-16) |
| P2 | **Enemy lấy từ pool không reset state machine** | `Entity.Start()` chỉ chạy một lần; `Pool.Reload()` chỉ `SetActive(true)`; `Entity` không có `OnEnable` | Enemy lấy lại từ pool sẽ **kẹt ở `DeathState`** (`Status = None`). Boss sẽ kẹt y hệt, kèm phase / cooldown / attack đang chạy của đời trước | Thêm hook reset vào `Entity` (mục 7.4). Đây là lỗi của **enemy thường** nữa, nên cần file bug riêng |
| P3 | Parameter Animator bị lệch tên | `BasicEnemy2D.controller` khai báo `TakeDamge`; code gọi `SetBool("TakeDamage")` (`Entity.cs:56`) | Minh chứng cho việc map animation bằng chuỗi rất dễ vỡ và vỡ **âm thầm** → boss cần validator (mục 8) | File bug; validator phát hiện được |
| P4 | Field của HoT/DoT không được serialize | `RecoveryReductionPerTimeForDuration.cs:6-8` (`private`, không có `[SerializeField]`) | Chỉ ảnh hưởng nếu boss bridge sang effect này | Phần còn lại của BUG-092 |
| P5 | Method animation event là `private` trên `Entity` | `Entity.cs:67-72` | Unity gọi animation event bằng reflection theo tên trên type **runtime**. Ghi chú ở `CharacterInputBase.cs` cho thấy `Invoke()` không thấy method `private` của base class. **Cần xác minh trong Editor** xem animation event gọi trên `BossController` có tới được các method này không | Đổi thành `protected` (vô hại) |
| P6 | Chưa có GDD boss và ADR | `design/gdd/` | `design-docs.md` yêu cầu GDD 8 mục trước khi code | GDD boss + ADR-0006 (roadmap M0) |

---

## 2. Đánh giá bắt buộc — trả lời 8 câu hỏi

### Q1. Một Boss SO có thực sự quyết định được toàn bộ animation, attack, behavior, phase không?

**Với vai trò điểm vào: có. Với vai trò file duy nhất: không.**

- **Có:** mọi lựa chọn riêng của boss (đòn nào, khi nào, trọng số, phase nào, ngưỡng HP, clip nào
  cho tình huống nào, tốc độ, cooldown) đều truy cập được từ `BossDefinition`, trực tiếp hoặc qua
  tham chiếu.
- **Không**, vì ba lý do mang tính kỹ thuật chứ không phải do thiết kế:
  1. Clip, `AnimatorController` và `AnimatorOverrideController` là asset Unity riêng. SO chỉ tham
     chiếu được, không chứa được chúng.
  2. Prefab vẫn phải tồn tại: sprite renderer, collider, hurtbox layer và cây `EntityCore` là cấu trúc
     GameObject, không phải data. Thêm nữa, `Pool.Spawn()` gọi `Instantiate` → `Awake()` chạy **trước
     khi** spawner kịp gán data, nên prefab phải tự trỏ tới SO của nó.
  3. **Mechanic chưa từng tồn tại thì phải có code.** Không có cách "data thuần" nào mô tả "đạn xoay
     theo xoắn ốc Archimedes" nếu không có một action biết tính xoắn ốc.
- **Thoả hiệp đề xuất:** "Một SO là điểm vào + các SO tái sử dụng được tham chiếu + các action type
  viết bằng code, tự động xuất hiện trong Inspector". Người thiết kế boss chỉ mở **một** asset và từ
  đó thấy hết.

### Q2. Phần nào data-driven hoàn toàn, phần nào cần C#?

| Hoàn toàn bằng data (Inspector) | Cần C# (viết một lần, tái sử dụng) |
|---|---|
| Stats, HP, Defense, MoveSpeed (qua `EnemyStatSO`) | Mỗi **loại** action: hitbox, bắn pattern, AoE, dash, summon, play anim |
| Danh sách đòn, trọng số, priority, cooldown, range, số lần dùng tối đa | Mỗi **loại** condition: khoảng cách, % HP, line of sight, signal |
| Thứ tự stage, thời lượng, delay, damage, số đạn, góc, tốc độ | Mỗi **loại** chính sách di chuyển: chase, giữ khoảng cách, strafe |
| Phase: điều kiện vào, ngưỡng, bộ đòn, override animation, modifier, action khi vào/ra | Bộ khung runtime: HFSM, executor, selector, phase controller |
| Map animation key → state; override clip theo từng phase | Adapter Animator, validator, drawer chọn type |
| Chuỗi đòn (sequence / combo pattern) | Mọi mechanic cần **đọc thế giới theo cách mới** (ví dụ tương tác tilemap của arena) |

**Không được đưa vào data:** biểu thức logic tuỳ ý (kiểu `"hp < 0.5 && dist > 3 || ..."`), delegate,
`UnityEvent` trỏ vào object trong scene, và các graph state tuỳ ý. Đó chính là con đường trở thành một
visual scripting engine (Q5).

### Q3. Framework runtime tái sử dụng nhỏ nhất là gì?

Mười một type runtime + bốn base class để mở rộng:

```
Data (SO / [Serializable]):  BossDefinition, BossPhaseDefinition, BossBehaviorProfile,
                             BossAttackDefinition, BossAttackStage, BossAnimationProfile
Mở rộng ([SerializeReference]): BossAction, BossCondition, BossMovementPolicy
Runtime (C# thuần):          BossRuntime, BossAttackExecutor, BossDecisionMaker, BossAnimator,
                             BossActionContext
Runtime (MonoBehaviour):     BossController : Entity, BossNegativeReceiver : EntityNegativeReciver
States (EntityState):        BossBrainState, BossAttackState, BossPhaseTransitionState, BossDeathState
```

Stagger/poise, resistance, utility curve, Behavior Tree và Playables đều **để sau** (mục 12).

### Q4. Thêm mechanic tấn công mới mà không sửa controller trung tâm bằng cách nào?

**Strategy + serialized polymorphism + khám phá type tự động.** Mechanic mới = một class
`: BossAction` mới. Drawer dùng `TypeCache.GetTypesDerivedFrom<BossAction>()`, nên class vừa compile
xong là **tự hiện** trong menu "Add Action". Không cần registry, không có `switch`, không phải sửa
`BossController` / `BossAttackExecutor` / `BossDecisionMaker`. Xem ví dụ đầy đủ ở mục 7.2.

### Q5. Làm sao tránh xây một visual scripting engine quá tổng quát?

Năm quy tắc chặn trước:

1. **State là code cố định** (4 state). Data chỉ cấu hình *bên trong* state, không tạo state mới.
2. **Condition là class có tên và tham số kiểu cứng** (`HealthPercentBelow { threshold }`), không phải
   ngôn ngữ biểu thức. Kết hợp chỉ có AND (danh sách) và `invert`. Nếu thật sự cần OR, làm một
   `AnyOfCondition` — và dừng ở đó.
3. **Không có biến tuỳ ý / blackboard do designer tạo.** Chỉ có các đại lượng mà `BossQuery` expose
   (khoảng cách, % HP, thời gian trong phase, số lần dùng đòn, số minion còn sống, signal).
4. **Không có vòng lặp và nhánh lồng nhau trong data.** Stage chạy tuần tự; action trong một stage
   chạy song song. Muốn "lặp" thì chính action phải có tham số `waves`.
5. **Không có graph editor** cho tới khi nào có ≥ 3 boss thật sự chạm giới hạn của Inspector (mục 8.4).

### Q6. Tái sử dụng CoreComponent và state machine hiện có như thế nào?

- `BossController : Entity`. Override việc tạo state, giữ nguyên `EntityCore`, `StateMachine<T>`,
  `BaseEntity` tick, và các animation event (`AnimationOnAction`… → `Status`).
- State của boss kế thừa `EntityState` (**không** kế thừa `EntityBasicState`, để tránh transition
  hardcode và stun-lock).
- Sibling component vẫn lấy qua `Core.GetCoreComponent<T>` / `TryGetCapability<T>` (tuân thủ
  `engine-code.md`). Service cross-system (pool, player) vẫn qua VContainer `[Inject]` lúc
  `Pool.Spawn()` (tuân thủ ADR-0004). `GameLifetimeScope` **không** thay đổi.
- Damage vẫn qua `INegativeReceiver.TakeDamage(float, Vector2)`. Current vs max tách theo
  `EntityVitalStats` / `EntityStatsHandler`.

### Q7. Trade-off (tóm tắt — chi tiết ở mục 11)

| Khía cạnh | Chi phí | Kiểm soát |
|---|---|---|
| Performance | Selector chạy mỗi `decisionInterval` (0.2 s), không phải mỗi frame; mỗi frame có vài phép so sánh | Runner được tạo sẵn lúc init → 0 alloc/đòn; `OverlapCircleNonAlloc` |
| Memory | Mỗi instance boss có một bộ runner (~vài KB) | Không đáng kể với 1–3 boss |
| Serialization | `[SerializeReference]` **mất data khi rename/move class** | Luật `[MovedFrom]` + validator check "missing type" |
| Debugging | Data-driven thì khó đọc theo luồng hơn code thẳng | Bắt buộc có debug inspector runtime + log quyết định (`#if UNITY_EDITOR`) |
| Maintainability | Thêm lớp trừu tượng | Contract nhỏ (3 interface), mỗi action tự validate |

### Q8. Làm gì ngay cho boss đầu tiên, để lại gì?

**Làm ngay (M0–M4):** HFSM 4 state, attack theo stage, 8–10 action cơ bản (anim, wait, face,
lock movement, hitbox, projectile pattern, ground AoE, dash, summon, signal), 5 condition, 3 chính
sách di chuyển, phase theo HP + signal, transition sequence, invulnerable, HUD, validator, drawer,
debug view, reset khi lấy từ pool.

**Để sau:** poise/stagger, damage resistance theo loại, utility curve, Playables, BT, graph editor,
arena hazard, camera/cinematic, save/load tiến trình boss.

---

## 3. Tổng quan kiến trúc

### 3.1 Ba tầng

```
┌──────────────────────────── TẦNG DATA (asset, dùng chung, READ-ONLY lúc runtime) ────────────────────────────┐
│ BossDefinition (SO) ── kế thừa EntityData ── kế thừa CharacterData                                           │
│   ├─ Identity / Presentation / Targeting ([Serializable] inline)                                             │
│   ├─ Stats ─────────────────────────────► EnemyStatSO (SO, sẵn có)                                           │
│   ├─ Aima ──────────────────────────────► AnimatorOverrideController (asset, sẵn có trên EntityData)         │
│   ├─ AnimationProfile ──────────────────► BossAnimationProfile (SO) ──► map key → state của BossBase.controller │
│   └─ Phases: List<BossPhaseDefinition> ([Serializable] inline)                                               │
│         ├─ EnterWhen: [SerializeReference] List<BossCondition>                                               │
│         ├─ OnEnter / OnExit: [SerializeReference] List<BossAction>                                           │
│         ├─ TransitionSequence ──────────► BossAttackDefinition (SO)                                          │
│         └─ Behavior: BossBehaviorProfile ([Serializable])                                                    │
│               ├─ Movement: [SerializeReference] BossMovementPolicy                                           │
│               └─ Attacks: List<BossAttackEntry> ──► BossAttackDefinition (SO, dùng chung giữa boss/phase)    │
│                                                        └─ Stages: List<BossAttackStage>                       │
│                                                              └─ Actions: [SerializeReference] List<BossAction>│
└──────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
                          │ đọc (không bao giờ ghi)
┌─────────────────────────▼──────── TẦNG RUNTIME (theo từng instance, C# thuần) ───────────────────────────────┐
│ BossRuntime  (phase hiện tại, cooldown[], lịch sử đòn, RNG, signal, nguồn invulnerable, minion đang sống)   │
│   ├─ BossDecisionMaker   — lọc theo cooldown + condition → priority → weighted random                         │
│   ├─ BossAttackExecutor  — chạy các stage; giữ IBossActionRunner[] (tạo sẵn cho từng attack)                  │
│   ├─ IBossMovementRunner — chính sách di chuyển của phase hiện tại                                           │
│   ├─ BossAnimator        — adapter: key → hash (cache), Play, Direction, override swap                       │
│   └─ BossActionContext   — gói tham chiếu cho action (component, pool, target, runtime)                      │
└──────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
                          │ điều phối
┌─────────────────────────▼──────── TẦNG UNITY (MonoBehaviour + state sẵn có) ─────────────────────────────────┐
│ BossController : Entity   — tạo state, sở hữu BossRuntime, reset khi lấy từ pool                             │
│   StateMachine<EntityState>: BossBrainState ⇄ BossAttackState ⇄ BossPhaseTransitionState → BossDeathState    │
│ EntityCore (sẵn có) ─ EntityMovement · EntityInput · EntityVitalStats · EntityStatsHandler ·                 │
│                       BossNegativeReceiver (mới, : EntityNegativeReciver) · EntityFindTarget (giữ cho Movement)│
│ Dịch vụ: IObjecPoolService (inject) · EventManager (static) · EnemyManager (A*, singleton đã được phép)       │
└──────────────────────────────────────────────────────────────────────────────────────────────────────────────┘
```

### 3.2 Sơ đồ quan hệ class / component

```
                         ┌──────────────────┐
                         │   CharacterData  │ (SO, sẵn có)
                         └────────▲─────────┘
                         ┌────────┴─────────┐
                         │    EntityData    │ (SO, sẵn có)
                         └────────▲─────────┘
                         ┌────────┴─────────┐   1   ┌──────────────────────┐
                         │  BossDefinition  │──────►│ BossAnimationProfile │ (SO)
                         └────────┬─────────┘       └──────────────────────┘
                                  │ 1..n (inline)
                         ┌────────▼──────────┐  0..1  ┌──────────────────────┐
                         │BossPhaseDefinition│───────►│ BossAttackDefinition │◄──────┐
                         └────────┬──────────┘ (trans.)└──────────┬───────────┘       │ n (SO, dùng chung)
                                  │ 1 (inline)                    │ 1..n (inline)      │
                         ┌────────▼──────────┐          ┌─────────▼────────┐  ┌───────┴────────┐
                         │BossBehaviorProfile│──n──────►│ BossAttackStage  │  │BossAttackEntry │
                         └────────┬──────────┘ entries  └─────────┬────────┘  └────────────────┘
                                  │ 1                             │ 0..n
                   ┌──────────────▼──────┐                ┌───────▼───────┐        ┌────────────────┐
                   │ «SR» BossMovement-  │                │ «SR» BossAction│        │«SR» BossCondition│
                   │       Policy        │                └───────┬───────┘        └────────┬───────┘
                   └──────────┬──────────┘                        │ CreateRunner()           │ Evaluate(BossQuery)
                              │ CreateRunner()                    ▼                          │
                   ┌──────────▼──────────┐              ┌────────────────────┐               │
                   │ IBossMovementRunner │              │ IBossActionRunner  │               │
                   └──────────▲──────────┘              └─────────▲──────────┘               │
                              │                                   │ n                        │
 ┌──────────────┐  owns  ┌────┴──────────┐  owns  ┌───────────────┴──────┐                    │
 │BossController│───────►│  BossRuntime  │───────►│  BossAttackExecutor  │ (một cái mỗi attack)│
 │   : Entity   │        │               │───────►│  BossDecisionMaker   │────────────────────┘
 └──────┬───────┘        │               │───────►│  BossAnimator        │
        │ states         └───────┬───────┘        └──────────────────────┘
        ▼                        │ BossActionContext (tham chiếu tới ↓)
 BossBrainState / BossAttackState / BossPhaseTransitionState / BossDeathState  (: EntityState)
        │
        ▼ Core.GetCoreComponent / TryGetCapability
 EntityMovement(IMovement) · EntityInput · EntityVitalStats(IVitalComponent) · EntityStatsHandler(IStatService)
 BossNegativeReceiver(INegativeReceiver) · IObjecPoolService · EventManager

 «SR» = [SerializeReference] polymorphic, class C# thuần, tham số inline trong asset
```

### 3.3 Bảng type (năm câu hỏi cho mỗi type)

| Type | Loại | Shared config hay runtime state? | Ai tạo / ai sở hữu vòng đời | Giao tiếp | Mở rộng không cần sửa code cũ |
|---|---|---|---|---|---|
| `BossDefinition` | SO (`: EntityData`) | Shared config | Designer tạo asset; prefab `Entity.data` trỏ tới | Được `BossController` đọc; `Core.Data` trả về nó cho các component base | Thêm field (có default) không làm hỏng asset cũ |
| `BossPhaseDefinition` | `[Serializable]` class, inline | Shared config | Nằm trong `BossDefinition` | Được `BossRuntime` đọc theo index | Phase mới = một phần tử danh sách mới |
| `BossBehaviorProfile` | `[Serializable]` class, inline trong phase | Shared config | Như trên | `BossDecisionMaker` + movement runner đọc | Thêm entry / đổi policy trong Inspector |
| `BossAttackEntry` | `[Serializable]` class | Shared config | Trong behavior profile | Selector đọc: attack, priority, weight, conditions, chain | Như trên |
| `BossAttackDefinition` | SO | Shared config | Designer; dùng lại ở nhiều phase/boss | `BossAttackExecutor` đọc stage | Stage/action mới trong Inspector |
| `BossAttackStage` | `[Serializable]` class | Shared config | Trong attack | Executor đọc | — |
| `BossAnimationProfile` | SO | Shared config | Designer; một cái cho mỗi rig (dùng chung giữa các biến thể) | `BossAnimator` build cache hash lúc init | Thêm binding |
| `BossAction` | abstract `[Serializable]`, `[SerializeReference]` | **Chỉ config (phải stateless)** | Nằm trong asset | `CreateRunner()` → runner | **Class con mới = mechanic mới** |
| `IBossActionRunner` | interface; class C# thuần | **Runtime state** | `BossAttackExecutor` tạo **một lần lúc init**, tái dùng mỗi lần đánh | `Begin/Tick/End(BossActionContext)` | Mỗi action tự mang runner của nó |
| `BossCondition` | abstract `[Serializable]`, `[SerializeReference]` | Config, stateless | Trong asset | `Evaluate(BossQuery)` | Class con mới |
| `BossMovementPolicy` | abstract `[Serializable]`, `[SerializeReference]` | Config | Trong behavior profile | `CreateRunner()` → `IBossMovementRunner` | Class con mới |
| `BossRuntime` | C# thuần | **Runtime state** | `BossController.Awake` tạo; `ResetRuntime()` mỗi lần spawn | Sở hữu executor, selector, animator adapter | Không cần mở rộng |
| `BossAttackExecutor` | C# thuần | Runtime state | `BossRuntime`, một cái mỗi `BossAttackDefinition` | Chạy stage, gọi runner, báo "cancelable" | Không cần |
| `BossDecisionMaker` | C# thuần | Runtime (buffer tạm, RNG) | `BossRuntime` | Đọc phase profile + `BossQuery` → trả về attack | Không cần |
| `BossAnimator` | C# thuần (adapter) | Runtime (cache hash, override hiện tại) | `BossRuntime` | Bọc `Animator` | Không cần |
| `BossActionContext` | C# thuần | Runtime (tham chiếu) | `BossRuntime`, một cái mỗi boss, tái dùng | Truyền vào mọi runner | Thêm property khi action mới cần dịch vụ mới (thay đổi cộng thêm) |
| `BossController` | MonoBehaviour (`: Entity`) | Runtime | Prefab; pool | Tạo state, reset, animation event | Không cần thay theo từng boss |
| `BossBrainState` / `BossAttackState` / `BossPhaseTransitionState` / `BossDeathState` | C# thuần (`: EntityState`) | Runtime | `BossController.LoadState()` | `StateMachine<EntityState>` | Cố định (có chủ đích) |
| `BossNegativeReceiver` | MonoBehaviour (`: EntityNegativeReciver`) | Runtime | Prefab (thay thế `EntityNegativeReciver` trên hurtbox của boss) | `Mitigate` → check invulnerable; `OnDamaged` → emit event HUD | — |
| `BossHealthBarUI` | MonoBehaviour UGUI trong scene | Runtime (chỉ hiển thị) | Scene | Subscribe `ON_BOSS_*` | — |

---

## 4. Schema dữ liệu (Deliverable 1)

> Code dưới đây là **phác thảo để duyệt kiến trúc**, không phải bản cài đặt cuối.
> Theo `gameplay-code.md`, bản thật sẽ dùng `[SerializeField] private` + property. Ở đây dùng field
> `public` cho ngắn gọn.

### 4.1 `BossDefinition`

```csharp
[CreateAssetMenu(menuName = "Game/Boss/Boss Definition")]
public class BossDefinition : EntityData          // → CharacterData: Stats, AbilityBindings, DefaultWeapon
{                                                // → EntityData: LayerMask, Aima (override controller), ranges...
    [Header("Identity")]
    [SerializeField] private string bossId;      // GUID tự sinh trong OnValidate (giống AbilityDefinition)
    public string DisplayName;
    public string Title;                          // "Kẻ canh giữ hầm mỏ"
    public Sprite Portrait;
    public GameObject Prefab;                     // để spawner dùng; validator check prefab.Entity.data == this

    [Header("Presentation")]
    public BossPresentation Presentation;         // [Serializable]: showHealthBar, barColor, introDelay,
                                                  //   phaseTickMarks (tự vẽ vạch theo ngưỡng HP của phase)
    [Header("Animation")]
    public BossAnimationProfile AnimationProfile; // SO; Aima (kế thừa) = AnimatorOverrideController của boss

    [Header("Targeting")]
    public BossTargetingSettings Targeting;       // aggroRange, requireLineOfSight, obstacleMask, targetMask

    [Header("Rules")]
    public bool SuperArmor = true;                // không vào TakeDamage state khi bị đánh
    public float DeathDuration = 2f;              // fallback nếu clip death thiếu animation event
    [SerializeReference] public List<BossAction> OnDeath = new();   // ví dụ DespawnSummons, SpawnLoot

    [Header("Phases")]
    public List<BossPhaseDefinition> Phases = new();                // [0] = phase mở đầu
}

[Serializable] public class BossTargetingSettings
{
    public float AggroRange = 12f;
    public bool RequireLineOfSight = true;
    public LayerMask ObstacleMask;   // bắt buộc set trong Inspector (engine-code.md: không hardcode layer)
    public LayerMask TargetMask;     // hurtbox của player — dùng làm default cho mọi action gây damage
}
```

**Vì sao kế thừa `EntityData` thay vì chứa một tham chiếu `EntityData`?**
`Entity`, `EntityState` (constructor) và `EntityCore.Data` đều nhận kiểu `EntityData` / `CharacterData`.
Kế thừa giúp mọi component base (`StatHandlerBase.ResolveProfile() => Core.Data.Stats`,
`WeaponHolderBase`, `AbilityHolderBase`) chạy **không cần sửa**, và giữ đúng **một asset**. Cái giá phải
trả: vài field của `EntityData` vô nghĩa với boss (`idleDurationTime`, `moveDurationTime`,
`rangeCheckAttack`). Validator sẽ cảnh báo, và custom inspector có thể ẩn chúng (để sau).

**Đối chiếu nhóm A trong yêu cầu:**

| Yêu cầu | Ở đâu |
|---|---|
| Identity, hiển thị | `BossDefinition` Identity |
| Prefab + component bắt buộc | `Prefab` + validator (mục 8.2) |
| Base stats, di chuyển, chỉ số chiến đấu | `Stats` → `EnemyStatSO` (HP, Defense, PhysicalDamage, MoveSpeed…) |
| Resistance | **Để sau.** `StatType` chưa có resistance. Đề xuất `DamageResistanceProfile` khi có damage type |
| Targeting, detection | `Targeting` |
| Presentation, health bar, UI | `Presentation` + `BossHealthBarUI` qua event |

### 4.2 `BossPhaseDefinition` và `BossBehaviorProfile`

```csharp
[Serializable] public class BossPhaseDefinition
{
    public string PhaseId = "phase-1";            // chuỗi ổn định cho log / signal, KHÔNG phải số thứ tự
    public string DisplayName;
    [SerializeReference] public List<BossCondition> EnterWhen = new();  // AND; phase [0] để trống
    public PhaseInterruptPolicy InterruptPolicy = PhaseInterruptPolicy.AtCancelWindow;
    public bool InvulnerableDuringTransition = true;
    public BossAttackDefinition TransitionSequence;                     // tuỳ chọn; chạy bằng executor chung
    [SerializeReference] public List<BossAction> OnEnter = new();       // tuỳ chọn, một lần
    [SerializeReference] public List<BossAction> OnExit  = new();
    public AnimatorOverrideController AnimatorOverride;                 // null = giữ override hiện tại
    public List<AnimationKeyRemap> AnimationRemaps = new();             // "smash.hit" → "smash.hit.enraged"
    public StatModifierGroup StatModifiers;                             // hệ thống modifier sẵn có
    public BossBehaviorProfile Behavior = new();
}

public enum PhaseInterruptPolicy { WaitForAttackEnd, AtCancelWindow, Immediately }

[Serializable] public class BossBehaviorProfile
{
    [SerializeReference] public BossMovementPolicy Movement;            // Chase / KeepDistance / Hold / Strafe
    public Vector2 IdleBetweenAttacks = new(0.4f, 1.0f);                // random trong [min, max]
    public float DecisionInterval = 0.2f;
    public int AvoidRepeatLast = 1;                                     // không chọn lại N đòn gần nhất nếu còn lựa chọn khác
    public List<BossAttackEntry> Attacks = new();
}

[Serializable] public class BossAttackEntry
{
    public BossAttackDefinition Attack;
    public int Priority = 0;                     // tier cao hơn được xét trước; weight chỉ so trong cùng tier
    [Min(0)] public float Weight = 1f;
    public float CooldownOverride = -1f;         // < 0 = dùng Attack.Cooldown
    public int MaxUsesPerPhase = 0;              // 0 = không giới hạn
    [SerializeReference] public List<BossCondition> Conditions = new();  // cộng thêm vào condition của attack
    public List<BossAttackDefinition> ChainAfter = new();                // sequence: chạy nối tiếp, bỏ qua selector
}
```

**Phase inline, attack là SO — vì sao?** Phase hầu như luôn riêng của từng boss; tách thành asset chỉ
làm Project window rối hơn. Attack thì được tái sử dụng (Golem A/B, phase 1/3), nên là SO.
**Một phase dùng lại attack có sẵn** = trỏ cùng asset. **Attack riêng của phase** = asset khác, hoặc
dùng asset gốc với `CooldownOverride` / `Conditions` riêng ở entry.

### 4.3 `BossAttackDefinition`, `BossAttackStage`

```csharp
[CreateAssetMenu(menuName = "Game/Boss/Attack Definition")]
public class BossAttackDefinition : ScriptableObject
{
    [SerializeField] private string attackId;    // GUID tự sinh
    public string DisplayName;
    public Sprite Icon;

    [Header("Selection")]
    public float Cooldown = 3f;
    public Vector2 Range = new(0f, 3f);          // khoảng cách tới target được phép chọn đòn này
    public bool RequireLineOfSight = false;
    [SerializeReference] public List<BossCondition> Conditions = new();

    [Header("Execution")]
    public bool FaceTargetOnStart = true;
    public List<BossAttackStage> Stages = new();
}

[Serializable] public class BossAttackStage
{
    public string StageName = "Telegraph";       // Telegraph / Startup / Active / Recovery — chỉ là nhãn
    public string AnimationKey;                  // key trong BossAnimationProfile; rỗng = giữ animation hiện tại
    public StageEndMode EndMode = StageEndMode.Duration;
    [Min(0)] public float Duration = 0.5f;
    public StatusAnimation EndSignal = StatusAnimation.End;   // khi EndMode == AnimationSignal
    public bool Cancelable = false;              // phase transition / death được phép cắt tại stage này
    public bool LockMovement = true;             // IMovement.Lock(source = executor) trong suốt stage
    [SerializeReference] public List<BossAction> Actions = new();    // tất cả Begin khi vào stage (tôn trọng Delay)
}

public enum StageEndMode { Duration, AnimationSignal, ActionsFinished, DurationOrSignal }
```

**Startup / telegraph / active / recovery** không phải enum cứng, mà là **các stage do designer đặt
tên**. Một đòn 3 hit có thể có 7 stage (Telegraph, Hit1, Gap, Hit2, Gap, Hit3, Recovery), hoặc 3 stage
với nhiều `HitboxDamageAction` được tách bằng `Delay`. Cả hai đều hợp lệ.

### 4.4 Các base mở rộng

```csharp
[Serializable] public abstract class BossAction
{
    [Min(0)] public float Delay;                 // giây, tính từ lúc vào stage
    public abstract IBossActionRunner CreateRunner();
    public virtual void Validate(BossValidationReport report, UnityEngine.Object owner) { }
}

public interface IBossActionRunner
{
    void Begin(BossActionContext ctx);
    bool Tick(BossActionContext ctx, float dt);          // true = xong
    void End(BossActionContext ctx, bool interrupted);   // dọn dẹp: unlock, thu hồi telegraph, ...
}

// Base tiện dụng cho action tức thời (90% trường hợp)
[Serializable] public abstract class InstantBossAction : BossAction
{
    protected abstract void Execute(BossActionContext ctx);
    public sealed override IBossActionRunner CreateRunner() => new Runner(this);
    private sealed class Runner : IBossActionRunner
    {
        private readonly InstantBossAction _def;
        public Runner(InstantBossAction def) => _def = def;
        public void Begin(BossActionContext ctx) => _def.Execute(ctx);
        public bool Tick(BossActionContext ctx, float dt) => true;
        public void End(BossActionContext ctx, bool interrupted) { }
    }
}

[Serializable] public abstract class BossCondition
{
    public bool Invert;
    public bool Evaluate(in BossQuery q) => Invert ^ EvaluateCore(q);
    protected abstract bool EvaluateCore(in BossQuery q);
    public virtual void Validate(BossValidationReport report, UnityEngine.Object owner) { }
}

[Serializable] public abstract class BossMovementPolicy
{
    public float SpeedMultiplier = 1f;
    public abstract IBossMovementRunner CreateRunner();
}
```

`BossQuery` là một `readonly struct` do `BossRuntime` dựng: `DistanceToTarget`, `HasLineOfSight`,
`HealthPercent`, `TimeInPhase`, `TimeInFight`, `UsesThisPhase(attack)`, `AliveSummons`,
`HasSignal(id)`. Condition **chỉ đọc**; không truy cập được component, nên không thể gây side-effect.

**Thư viện có sẵn đề xuất cho boss đầu tiên:**

| Actions | Conditions | Movement policies |
|---|---|---|
| `PlayAnimationAction` (key) | `DistanceToTarget(min,max)` | `ChaseTarget(stopDistance)` |
| `FaceTargetAction(trackDuring)` | `HealthPercentBelow(x)` | `KeepDistance(min,max)` |
| `HitboxDamageAction` (circle/box, offset, active window, hit-once) | `HasLineOfSight` | `HoldPosition` |
| `ProjectilePatternAction` (fan / ring / aimed, waves, rotation/wave) | `TimeInPhaseAtLeast(s)` | |
| `GroundAoEAction` (telegraph prefab, delay, radius, N điểm) | `SignalReceived(id)` | |
| `DashAction` (hướng target/tránh, speed, duration → `IMovement.ApplyKnockback`) | `AliveSummonsAtMost(n)` | |
| `SummonAction` (prefab, count, vòng tròn, emit `ON_SPAWN_EXTRA_ENEMY`) | `AnyOf(list)` | |
| `SetInvulnerableAction(on/off)` | | |
| `SpawnVfxAction` (pooled, gắn vào boss hoặc target point) | | |
| `EmitSignalAction(id)` | | |
| `DespawnSummonsAction` | | |

---

## 5. Thiết kế chi tiết từng mảng

### 5.1 Tách bạch shared config và runtime state (xuyên suốt)

Ba quy tắc, và validator + code review sẽ kiểm tra:

1. **Không field nào trên SO / `BossAction` / `BossCondition` bị ghi lúc runtime.** Mọi state nằm
   trong runner / `BossRuntime`. Ví dụ ngược **không được chép**: `SpawnEffectBase._context`
   (state runtime lưu trên SO dùng chung).
2. **Runner tạo một lần cho mỗi instance boss**, trong `BossRuntime.Build()`, và được `Begin()` reset
   mỗi lần dùng → không alloc theo từng đòn, và hai boss cùng loại có hai bộ runner riêng.
3. **Modifier áp lên clone stat của instance** (`EntityStatsHandler` đã làm sẵn), với `source` = object
   runtime của phase, nên remove được chính xác.

### 5.2 Animation (nhóm B)

#### 5.2.1 So sánh các phương án

| Phương án | Ưu | Nhược | Kết luận |
|---|---|---|---|
| **Animator Controller riêng cho từng boss** | Tự do tuyệt đối | Mỗi boss một controller → tên state/parameter lại do code hardcode, hoặc thành "chuỗi ma thuật" trong data; lỗi kiểu `TakeDamge` lặp lại | ❌ |
| **Override controller theo từng đòn** (mẫu hiện tại: `AttackSO` / `AbilityDefinition`) | Đã quen, đã có | Swap `runtimeAnimatorController` giữa các stage → animator bị rebind, dễ giật frame; phải tạo một override cho mỗi đòn × mỗi boss | ⚠️ chỉ dùng ở mức **phase** |
| **Base controller có slot + một override theo từng boss + profile map key** | Một controller duy nhất cho mọi boss; code chỉ biết một bộ tên cố định; clip đổi theo boss qua override; đòn nói bằng **key ngữ nghĩa** nên dùng lại được giữa các boss | Số slot có giới hạn (chọn 8–12); với 8 hướng, mỗi slot là một blend tree 8 clip placeholder | ✅ **Đề xuất** |
| **Chỉ hash / tên clip trong data, gọi `Play(clipName)`** | Đơn giản | `Play` cần tên **state**, không phải tên clip; không có hướng; vỡ âm thầm | ❌ |
| **Playables API** (`AnimationClipPlayable`) | Phát bất kỳ clip nào từ data, không cần controller | Phải tự làm mixer 8 hướng, crossfade, event; đội chưa từng dùng; debug khó hơn | ⏳ Để sau, nếu slot không đủ |
| **Hệ mapping tuỳ biến hoàn toàn** | — | Chính là phương án 3 nhưng tự viết thêm | Phương án 3 **chính là** mapping tối thiểu |

#### 5.2.2 Phương án đề xuất — phân chia trách nhiệm

| Nằm ở đâu | Chứa gì |
|---|---|
| **`BossBase.controller`** (một file, dùng chung cho mọi boss) | Parameter cố định: `Direction` (float, quy ước `DirectionResolver`), `Speed` (float). State cố định: `Locomotion` (blend Idle/Move), `Action_01…Action_10`, `Hit`, `PhaseTransition`, `Death` — mỗi state là blend tree 8 hướng trên `Direction` với **clip placeholder**. Không có transition phụ thuộc parameter cho action (runtime gọi `Play` trực tiếp). Tên state/parameter nằm trong `GameConstants.BossAnimation`. |
| **`AnimatorOverrideController` của boss** (= `EntityData.Aima`, field đã có) | Thay clip placeholder → clip thật của boss. Không có logic. |
| **`BossAnimationProfile`** (SO, một cái cho mỗi rig) | `List<AnimationBinding>`: `key` (ngữ nghĩa, ví dụ `smash.windup`) → `stateName` (ví dụ `Action_02`), `layer`, `crossFade`, `speed`. Cộng thêm key bắt buộc: `locomotion`, `hit`, `phase.transition`, `death`. |
| **Stage của attack** | Chỉ ghi **key** (`smash.windup`). Attack không biết slot, không biết clip → **cùng một attack asset chạy được trên nhiều boss.** |
| **Phase** | `AnimatorOverride` (swap một lần lúc chuyển phase) và/hoặc `AnimationRemaps` (key → key). |
| **`BossAnimator`** (adapter runtime) | Lúc init: `Animator.StringToHash` toàn bộ binding → `Dictionary<string, BindingRuntime>`. `Play(key)` → tra remap của phase → `animator.CrossFadeInFixedTime(hash, fade, layer)` hoặc `Play(hash, layer, 0)`. `SetDirection(int)`. Key không tồn tại → log cảnh báo **một lần** + fallback về `locomotion`. |

Ghi chú về `VERSION.md` ("không dùng `CrossFade` mà không kiểm tra state hiện tại"): action của boss
**được thiết kế để cắt ngang**, nên `Play(hash, layer, 0f)` là mặc định; `CrossFade` chỉ dùng khi
binding có `crossFade > 0` và state hiện tại khác state đích — adapter kiểm tra điều này.

**Đồng bộ thời lượng stage với clip:** mặc định `EndMode = Duration` (tune bằng data, test được,
không phụ thuộc animation event). Validator so `Duration` với độ dài clip trong override và cảnh báo
khi lệch > 10%. Khi cần khớp khung hình tuyệt đối (một cú chém phải trúng đúng frame 7), dùng
`EndMode = AnimationSignal`: clip gọi `AnimationOnAction` / `AnimationEnd` (method sẵn có trên
`Entity`) → `Status` của `BossAttackState` → executor nhận "signal".

**Parameter tuỳ biến theo boss** (escape hatch): `SetAnimatorParameterAction { name, type, value }`.
Validator kiểm tra parameter có tồn tại trong controller. Không dùng cho luồng chính.

**Boss chỉ có 1 hướng** (sprite to quay mặt về trước): dùng `BossBase_1Dir.controller` cùng tên state
nhưng không có blend tree. `BossAnimationProfile.BaseController` chỉ định controller nào, validator
kiểm tra tương ứng.

### 5.3 Attack và skill (nhóm C)

#### 5.3.1 Mô hình thực thi

```
BossAttackDefinition ──(executor chạy)──► Stage[0] ─► Stage[1] ─► … ─► Stage[n] ─► Done
                                            │
                                            ├─ vào stage: Animator.Play(key); nếu LockMovement → IMovement.Lock(executor)
                                            ├─ mỗi action: đợi Delay → runner.Begin → runner.Tick… đến khi true
                                            ├─ kết thúc stage theo EndMode (Duration | Signal | ActionsFinished)
                                            └─ rời stage: runner.End(interrupted:false) cho runner chưa xong; Unlock
Interrupt() bất kỳ lúc nào: End(interrupted:true) cho tất cả runner đang chạy + Unlock → sạch sẽ
```

#### 5.3.2 Ranh giới data / code, nhìn theo từng yêu cầu

| Yêu cầu | Data | Code (action có sẵn) |
|---|---|---|
| Identifier, hiển thị | `attackId`, `DisplayName`, `Icon` | — |
| Animation mapping | `AnimationKey` theo stage | `BossAnimator` |
| Range, targeting, chọn target | `Range`, `RequireLineOfSight`, `Conditions`; `TargetPointMode` trong action (`Target`, `Predicted(lead)`, `Self`, `AroundTarget(radius)`) | Condition + resolver điểm |
| Cooldown, giới hạn dùng | `Cooldown`, `CooldownOverride`, `MaxUsesPerPhase` | `BossRuntime` |
| Thời lượng telegraph, VFX | Stage + `SpawnVfxAction` / `GroundAoEAction.TelegraphPrefab` | Action |
| Thời lượng startup / active / recovery | Stage `Duration` | Executor |
| Damage, hit detection | `HitboxDamageAction { shape, offset, radius, activeTime, baseDamage, statScaling, targetMask }` | Action (mẫu `MeleeWeapon.OnActivate`) |
| Projectile, object sinh ra | `ProjectilePatternAction { prefab, pattern, count, spread, waves, interval, speed }` | Action + `ProjectileBody` |
| Di chuyển trong đòn | `DashAction`, `LockMovement` của stage, `FaceTargetAction(track)` | `IMovement` |
| Chuỗi / các stage thực thi | Danh sách `Stages` + `ChainAfter` ở entry | Executor |
| Hoàn thành / cắt ngang / huỷ | `EndMode`, `Cancelable` theo stage; `InterruptPolicy` của phase | Executor + state |
| Callback / event | `EmitSignalAction(id)` (không dùng `UnityEvent` trong SO — SO không trỏ được object trong scene) | `EventManager` |

**Công thức damage chung** (theo `design-docs.md`, dùng tên field):

```
finalDamage = action.baseDamage + casterCurrent(PhysicalDamage) × action.statScaling
receivedDamage = max(0, finalDamage − target.Defense)        // mitigation nằm ở receiver, như hiện tại
```

#### 5.3.3 Ví dụ: `HitboxDamageAction` (có state → có runner riêng)

```csharp
[Serializable] public class HitboxDamageAction : BossAction
{
    public HitShape Shape = HitShape.Circle;
    public Vector2 Offset = new(1f, 0f);        // theo hướng boss đang nhìn
    [Min(0.05f)] public float Radius = 1.2f;
    public float ActiveTime = 0.1f;              // 0 = một frame
    public float BaseDamage = 20f;
    public float StatScaling = 1f;
    public LayerMask TargetMask;                 // 0 → dùng Targeting.TargetMask của boss (validator cảnh báo)
    public int MaxTargets = 8;

    public override IBossActionRunner CreateRunner() => new Runner(this);

    private sealed class Runner : IBossActionRunner
    {
        private readonly HitboxDamageAction _d;
        private readonly Collider2D[] _hits;
        private readonly HashSet<INegativeReceiver> _alreadyHit = new();
        private float _t;
        public Runner(HitboxDamageAction d) { _d = d; _hits = new Collider2D[d.MaxTargets]; }
        public void Begin(BossActionContext c) { _t = 0f; _alreadyHit.Clear(); Sweep(c); }
        public bool Tick(BossActionContext c, float dt) { _t += dt; if (_t < _d.ActiveTime) { Sweep(c); return false; } return true; }
        public void End(BossActionContext c, bool interrupted) => _alreadyHit.Clear();
        private void Sweep(BossActionContext c)
        {
            Vector2 center = c.Position + c.Rotate(_d.Offset);
            int n = Physics2D.OverlapCircleNonAlloc(center, _d.Radius, _hits, c.MaskOrDefault(_d.TargetMask));
            float dmg = _d.BaseDamage + c.CurrentStat(StatType.PhysicalDamage) * _d.StatScaling;
            for (int i = 0; i < n; i++)
                if (_hits[i].TryGetComponent(out INegativeReceiver r) && _alreadyHit.Add(r))
                    r.TakeDamage(dmg, c.Position);
        }
    }
}
```

`TryGetComponent` trên chính collider bị trúng: đúng quy tắc hurtbox của ADR-0005 Amendment 1.

#### 5.3.4 Cơ chế thêm hành vi mới — đã chọn gì và vì sao

| Cơ chế | Mô tả | Dùng cho |
|---|---|---|
| **Serialized polymorphism (`[SerializeReference]`)** | Class C# thuần; tham số inline trong asset của attack; drawer chọn type bằng `TypeCache` | ✅ `BossAction`, `BossCondition`, `BossMovementPolicy` |
| **Strategy SO** (như `AbilityEffectDefinition`) | Mỗi action là một asset | ⚠️ Phương án dự phòng. Không cần editor code, nhưng một đòn 5 action = 5 asset, và sửa tham số phải nhảy qua lại giữa các asset |
| **Registry theo id / chuỗi** | `"ring_barrage"` → factory | ❌ Chuỗi dễ gõ sai, lại cần bảng đăng ký; `TypeCache` đã làm việc này |
| **Composition SO cho data nặng** | Ví dụ `ProjectilePatternDefinition` (SO) được nhiều action tham chiếu | ✅ Khi một bộ tham số được dùng ở ≥ 3 chỗ |

Vì sao chọn `[SerializeReference]` dù dự án đang dùng strategy SO: số asset của attack boss sẽ nhân
lên nhanh (3 boss × 3 phase × 6 đòn × 4 action), và tham số của một action gần như luôn riêng cho
đòn đó. Nếu chủ dự án muốn tránh editor code, **cùng interface đó** vẫn chạy với strategy SO
(`BossActionAsset : ScriptableObject` bọc một `BossAction`). Executor không phân biệt hai loại.

#### 5.3.5 (Tuỳ chọn, để sau) Bridge sang Abilities v2

`ApplyAbilityEffectAction { AbilityEffectDefinition effect }` dựng một `AbilityContext`
(`Caster` = `EntityAbilityHolder` trên prefab boss, vốn đã là `IAbilityOwner`) rồi gọi
`effect.Apply(ctx)`. Nhờ đó boss dùng lại được `BuffDebuffStatsForDuration`, `SpawnSummonEffect`…
**Chưa đưa vào boss đầu tiên**, vì v2 còn bug mở và còn state runtime trên SO.

### 5.4 Behavior và ra quyết định (nhóm D)

#### 5.4.1 So sánh

| Mô hình | Hợp với yêu cầu? | Chi phí authoring | Debug | Kết luận |
|---|---|---|---|---|
| FSM thuần trong data | Boss A/B được, boss C (phase + pattern) làm số state bùng nổ | Graph state trong Inspector khó đọc | Dễ | ❌ làm lớp data |
| **HFSM cố định trong code** | Khung Brain/Attack/Transition/Death dùng chung cho mọi boss | 0 (không cấu hình) | Dễ | ✅ **làm khung** |
| Behavior Tree trong data | Rất linh hoạt | Cây lồng nhau trong Inspector gần như không đọc được nếu thiếu graph editor → kéo theo việc phải làm graph editor | Khó nếu thiếu tooling | ❌ bây giờ; ⏳ là đường nâng cấp |
| Utility AI đầy đủ (curve, normalization) | Rất tốt cho "chọn đòn" | Tune curve khó với người mới | Trung bình | ⚠️ quá tay cho boss đầu |
| **Priority + weight + condition ("utility-lite") + sequence** | Đủ cho A (melee chase), B (kite + ranged), C (luân phiên pattern, summon, đa phase) | Danh sách phẳng, dễ đọc | Log được "vì sao chọn đòn X" | ✅ **làm lớp data** |

**Hybrid đề xuất:** HFSM 4 state trong code, và *bên trong* `BossBrainState` là selector đọc từ
`BossBehaviorProfile` của phase hiện tại.

#### 5.4.2 HFSM

```
            ┌───────────────────────────── HP ≤ 0 (kiểm tra ở mọi state) ─────────────────────────────┐
            │                                                                                          ▼
 spawn ─► BossBrainState ── selector chọn attack ──► BossAttackState ── executor xong ──► BossBrainState   BossDeathState
            │   ▲  (movement policy chạy ở đây,          │  (ChainAfter còn? → chạy tiếp đòn kế)
            │   │   chờ IdleBetweenAttacks)              │
            │   └──────── transition xong ───────┐       │ phase chờ + stage Cancelable / hết đòn / Immediately
            │                                     │       ▼
            └── phase chờ (khi đang ở Brain) ──► BossPhaseTransitionState
```

Boss state kế thừa `EntityState` (không phải `EntityBasicState`) → **super armor mặc định**: một hit chỉ
trừ HP (qua `BossNegativeReceiver`), không đổi state. Stagger/poise để sau, sẽ là state thứ 5 kèm
`BossDefinition.PoiseMax`.

#### 5.4.3 Thuật toán selector (`BossDecisionMaker.TrySelect`)

```
candidates ← entries của phase hiện tại
lọc bỏ: attack null | cooldown chưa hết | usesThisPhase ≥ MaxUsesPerPhase (nếu > 0)
        | distance ∉ attack.Range | (RequireLineOfSight && !LoS)
        | bất kỳ condition nào của attack hoặc của entry trả false
        | nằm trong AvoidRepeatLast (chỉ khi sau khi bỏ đi vẫn còn ứng viên)
nếu rỗng → không đánh (movement policy tiếp tục chạy) 
topTier ← max(Priority) trong candidates
pick ← weighted random (System.Random có seed riêng mỗi instance) trong các candidate có Priority == topTier
```

- Buffer ứng viên là `List<>` tạo sẵn → 0 alloc.
- **RNG có seed theo instance** → EditMode test cố định được seed (`test-standards.md`: không random
  trong test).
- Log quyết định (`#if UNITY_EDITOR`, bật/tắt theo instance): *"phase-2 chọn RockBarrage (tier 0,
  w=3/7); loại Smash: distance 6.2 ∉ [0,3]"*.

**Boss A / B / C chỉ khác nhau về data:**

| | Movement | Entries |
|---|---|---|
| **A — melee, đuổi** | `ChaseTarget(stop 1.5)` | Combo (w 5, range 0–2.5), Lunge (w 2, range 2.5–6) |
| **B — giữ khoảng cách, bắn xa** | `KeepDistance(5, 8)` | Barrage (w 4, range 4–10), Retreat Dash (priority 1, `DistanceToTarget(0,3)`), Aimed Shot (w 3) |
| **C — pattern + summon + đa phase** | Phase 1 `Chase` → phase 2 `Hold` | Entry có `ChainAfter [Slam, Barrage]`; `Summon` với `AliveSummonsAtMost(1)` + `MaxUsesPerPhase 2`; phase 3 chuyển bằng `SignalReceived("all-pillars-broken")` |

#### 5.4.4 Serialize, validate, execute — tóm tắt

- **Biểu diễn / serialize:** danh sách phẳng trong phase (`List<BossAttackEntry>`) + condition dạng
  `[SerializeReference]`. Không có graph, không có id tham chiếu chéo trong một asset.
- **Validate:** mục 8.
- **Execute:** `BossBrainState.LogicUpdate` → tick movement runner → hết idle timer và đến
  `DecisionInterval` → `TrySelect` → `ChangeState(AttackState)` với attack đã chọn.

### 5.5 Phase (nhóm E)

- **Thứ tự:** phase xếp theo danh sách; runtime chỉ xét **phase kế tiếp** (`current + 1`). Lý do: đơn
  giản, dự đoán được, và khớp mọi ví dụ trong yêu cầu. Nếu burst damage làm HP rơi từ 70% xuống 20%
  trong một frame, các phase sẽ **nối tiếp nhau** (P2 rồi P3), mỗi phase chạy đủ `OnEnter`; nhưng
  `TransitionSequence` của phase *trung gian* bị bỏ qua khi phase sau đã đủ điều kiện (cờ
  `skipIntermediateSequences`, mặc định bật).
- **Ba loại transition** đều là `BossCondition`:
  - theo HP: `HealthPercentBelow(0.6)`
  - theo event: `SignalReceived("pillars-broken")` — signal đến từ `EmitSignalAction` hoặc từ
    `EventManager` qua một EventID mới `ON_BOSS_SIGNAL` (payload `BossSignal { GameObject boss, string id }`;
    `boss == null` nghĩa là broadcast)
  - theo gameplay: `TimeInPhaseAtLeast(45)`, `AliveSummonsAtMost(0)`, kết hợp AND.
- **Không hardcode** số phase, ngưỡng hay hành vi: controller chỉ biết "danh sách phase" và "phase
  hiện tại".

**Tương tác với attack đang chạy (`InterruptPolicy`):**

| Policy | Đang ở Brain | Đang ở Attack |
|---|---|---|
| `WaitForAttackEnd` | Chuyển ngay | Chờ executor xong (bỏ qua `ChainAfter`) |
| `AtCancelWindow` (mặc định) | Chuyển ngay | Chuyển khi stage hiện tại có `Cancelable = true`, hoặc khi hết đòn |
| `Immediately` | Chuyển ngay | `executor.Interrupt()` ngay lập tức |

Death luôn thắng mọi policy.

**`BossPhaseTransitionState.Enter` → `Exit`, theo đúng thứ tự:**

1. `runtime.AddInvulnerable(this)` nếu `InvulnerableDuringTransition`
2. chạy `OnExit` của phase cũ; gỡ `StatModifiers` của phase cũ (source = runtime object của phase cũ)
3. `currentPhase = next`; reset `timeInPhase`, `usesThisPhase`; **cooldown giữ nguyên** (cờ
   `resetCooldownsOnEnter` trên phase nếu muốn reset)
4. swap `AnimatorOverride` (nếu có), áp `AnimationRemaps`, áp `StatModifiers` mới, đổi movement runner
5. `EventManager.Emit(ON_BOSS_PHASE_CHANGED, BossPhaseChanged{…})`
6. chạy `TransitionSequence` bằng **chính `BossAttackExecutor`** (một "attack" không gây damage: roar,
   rung màn hình, summon)
7. chạy `OnEnter` của phase mới
8. `RemoveInvulnerable(this)` → `ChangeState(Brain)`

Invulnerable được hiện thực bằng tập "source" (giống `MovementBase.Lock`) trong `BossRuntime`;
`BossNegativeReceiver.Mitigate()` trả `0` khi tập đó không rỗng.

---

## 6. Runtime đọc và thực thi định nghĩa như thế nào (Deliverable 6)

| Thời điểm | Đọc gì | Cache hay đọc live? |
|---|---|---|
| `Awake` (một lần mỗi instance) | Toàn bộ cây phase → attack → stage → action | **Cache cấu trúc:** tạo `BossAttackExecutor` + runner cho mọi attack xuất hiện trong mọi phase (kể cả `TransitionSequence`, `ChainAfter`); `BossAnimator` hash toàn bộ binding; mảng cooldown đánh index theo attack |
| Mỗi lần ra quyết định | `Weight`, `Priority`, `Range`, `Cooldown`, condition | **Đọc live** qua tham chiếu → chỉnh trong Play Mode có tác dụng ngay |
| Mỗi stage | `Duration`, `EndMode`, `AnimationKey`, `Delay`, tham số action | **Đọc live** (runner giữ tham chiếu tới definition, không copy số liệu) |
| Mỗi frame | Condition của phase kế tiếp | Live |

Khác biệt với `AbilityInstance`: `AbilityInstance` tạo `AbilityContext` mới mỗi lần `CanStart()`
(alloc). Boss tái sử dụng một `BossActionContext` duy nhất.

---

## 7. Bốn kịch bản mở rộng

### 7.1 Scenario 1 — Boss hoàn toàn mới (quy trình authoring chính xác)

1. **Art:** import sprite sheet, tạo clip (Idle/Move/Hit/Death + clip cho từng đòn, 8 hướng hoặc 1 hướng).
2. **Override:** `Create → Animator Override Controller`, base = `BossBase.controller`, kéo clip vào
   từng placeholder (`Action_01_Down`, …).
3. **Profile:** `Create → Game/Boss/Animation Profile`, base controller = `BossBase.controller`, thêm
   binding `key → state` (nút "Add required keys" tự điền `locomotion`, `hit`, `phase.transition`, `death`).
4. **Stats:** `Create → EnemyStatSO` (hoặc dùng `Enemy/Boss/BossStats.asset` có sẵn).
5. **Attack:** tạo hoặc dùng lại các `BossAttackDefinition` (mục 9.2).
6. **Boss SO:** `Create → Game/Boss/Boss Definition`. Điền Identity, `Stats`, `Aima` = override ở
   bước 2, `AnimationProfile`, `Targeting` (hai layer mask!), rồi các phase với behavior và entry.
7. **Prefab:** duplicate `BossTemplate.prefab` (mục 13.3). Prefab này đã có đủ `BossController` +
   `EntityCore` + core component + hurtbox với `BossNegativeReceiver`. Chỉ thay sprite, collider, và
   `Entity.data` → Boss SO. Đặt `BossDefinition.Prefab` = prefab này.
8. **Validate:** nút **Validate** trên Inspector của Boss SO (hoặc `Tools/Boss/Validate All`).
9. **Spawn:** thêm prefab vào `EnemySpawnEntry` của một `RoomModel` "boss room" (luồng spawn hiện có),
   hoặc kéo thẳng vào scene test.

**Không có file `.cs` mới.**

### 7.2 Scenario 2 — Mechanic mới: "Rotating Projectile Barrage"

Đúng **một** class mới. Không sửa controller, executor, selector hay boss asset nào đang có:

```csharp
[Serializable, BossActionMenu("Projectile/Rotating Barrage")]
public class RotatingBarrageAction : BossAction
{
    public GameObject ProjectilePrefab;          // phải có ProjectileBody + collider trigger
    [Min(1)] public int ProjectilesPerWave = 8;
    [Min(1)] public int Waves = 5;
    [Min(0.01f)] public float WaveInterval = 0.25f;
    public float DegreesPerWave = 15f;           // phần "xoay"
    public ProjectileConfig Flight = new() { speed = 8f, lifetime = 4f };
    public float BaseDamage = 12f;
    public float StatScaling = 0.5f;

    public override IBossActionRunner CreateRunner() => new Runner(this);

    public override void Validate(BossValidationReport r, UnityEngine.Object owner)
    {
        if (ProjectilePrefab == null || !ProjectilePrefab.TryGetComponent(out ProjectileBody _))
            r.Error(owner, "RotatingBarrage: prefab thiếu ProjectileBody");
        if (Flight.targetMask == 0) r.Error(owner, "RotatingBarrage: targetMask = Nothing");  // bài học BUG-072
    }

    private sealed class Runner : IBossActionRunner, IProjectilePayload
    {
        private readonly RotatingBarrageAction _d;
        private BossActionContext _c; private int _wave; private float _timer; private float _damage;
        public Runner(RotatingBarrageAction d) => _d = d;

        public void Begin(BossActionContext c)
        {
            _c = c; _wave = 0; _timer = 0f;
            _damage = _d.BaseDamage + c.CurrentStat(StatType.PhysicalDamage) * _d.StatScaling;  // snapshot lúc bắn
        }
        public bool Tick(BossActionContext c, float dt)
        {
            _timer -= dt;
            if (_timer > 0f) return false;
            float step = 360f / _d.ProjectilesPerWave, offset = _wave * _d.DegreesPerWave;
            for (int i = 0; i < _d.ProjectilesPerWave; i++)
            {
                Vector2 dir = Quaternion.Euler(0, 0, offset + i * step) * Vector2.right;
                GameObject go = c.Pool.Spawn(_d.ProjectilePrefab, c.Position, Quaternion.identity);
                go.GetComponent<ProjectileBody>().Launch(dir, _d.Flight, this);   // runner chính là payload
            }
            _timer = _d.WaveInterval;
            return ++_wave >= _d.Waves;
        }
        public void End(BossActionContext c, bool interrupted) { }    // đạn đã bắn vẫn bay tiếp — có chủ đích

        public void OnHit(Collider2D target, Vector2 hitPos)
        {
            if (target.TryGetComponent(out INegativeReceiver r)) r.TakeDamage(_damage, hitPos);
        }
    }
}
```

Sau khi compile, "Projectile/Rotating Barrage" xuất hiện trong menu *Add Action* của **mọi** stage
thuộc **mọi** boss. "Ground Explosion" làm tương tự: một class `GroundExplosionAction` (spawn marker
qua pool → đợi `Delay` → `OverlapCircleNonAlloc` → `TakeDamage` → thu hồi marker trong `End`, kể cả khi
bị cắt ngang).

**Quy tắc cho người viết action mới:**
1. Class action chỉ chứa tham số (không ghi field nào lúc runtime).
2. State nằm trong runner.
3. `End()` phải dọn sạch khi `interrupted == true`.
4. Override `Validate()`.
5. **Không bao giờ đổi tên hoặc di chuyển class** mà không thêm `[MovedFrom]` (mục 11.3).

### 7.3 Scenario 3 — Đổi hành vi mà không sửa code

| Thay đổi trong SO | Có tác dụng khi | Ghi chú |
|---|---|---|
| Weight, priority, cooldown, range, `IdleBetweenAttacks`, `DecisionInterval` | **Ngay lập tức** (lần quyết định kế tiếp) | Đọc live |
| Ngưỡng HP của phase, tham số condition | **Ngay lập tức** (frame kế tiếp) | Nếu phase đã qua thì không quay lại |
| Duration, delay, damage, số đạn, góc, tốc độ | **Đòn kế tiếp** | Runner đọc definition lúc `Begin` |
| Đổi `AnimationKey` của stage sang key **đã có** | Ngay lập tức | |
| Thêm/sửa binding trong `BossAnimationProfile` | **Reinit** | Hash được cache; ở Editor có thể rebuild cache trong `OnValidate` (tuỳ chọn) |
| Đổi clip trong `AnimatorOverrideController` | Thường là ngay (Unity cập nhật override) | Nếu không thấy thì reinit |
| Thêm/bớt entry, stage, action; đổi **loại** action / condition | **Reinit** | Runner được tạo sẵn lúc init |
| Thêm/bớt/sắp xếp lại phase | **Reinit** | |
| Stats trong `EnemyStatSO` | **Respawn** | `EntityStatsHandler` clone profile ở lần dùng đầu |
| Class action/condition mới | **Recompile** (thoát Play Mode) | |

- "Reinit" = context menu **`Reinitialize Boss`** trên `BossController` (debug) hoặc respawn.
- ⚠️ Chỉnh SO trong Play Mode **được giữ lại sau khi thoát Play Mode** (khác với chỉnh component) —
  tiện cho việc tune, và cũng là lý do runtime **tuyệt đối không ghi vào SO** (cùng loại lỗi với
  BUG-063).

### 7.4 Scenario 4 — Nhiều instance và object pooling

**Những gì là theo từng instance (không bao giờ nằm trên SO):**

| State | Nằm ở |
|---|---|
| Phase hiện tại, thời gian trong phase / trong trận, số lần dùng mỗi đòn | `BossRuntime` |
| Cooldown từng đòn | `BossRuntime.cooldowns[]` (index theo attack) |
| Target, khoảng cách, line of sight | `BossRuntime` / `EntityInput` |
| Attack, stage, timer đang chạy; runner | `BossAttackExecutor` (một bộ mỗi instance) |
| RNG | `BossRuntime.rng` (seed = hash(bossId) ^ instanceId, hoặc seed cố định cho test) |
| Signal, nguồn invulnerable, danh sách minion | `BossRuntime` |
| Current HP / Mana | `EntityVitalStats` (sẵn có) |
| Modifier | Clone `BaseStatsSO` của instance (`EntityStatsHandler`, sẵn có) |
| Override Animator hiện tại | `Animator` của instance |

**Trách nhiệm reset — `BossController.ResetRuntime()`:**

```
OnEnable()           → _resetPending = true     // KHÔNG đọc HP ở đây: thứ tự OnEnable giữa các GameObject con không xác định
Update() đầu tiên    → nếu _resetPending:
    executor.Interrupt() nếu còn chạy (dọn lock, telegraph)
    runtime.Reset(): phase = 0, timers = 0, uses = 0, cooldowns = Phase0.InitialCooldowns (mặc định 0),
                     history.Clear(), signals.Clear(), invulnerable.Clear(), summons.Clear(), rng reseed
    animator.runtimeAnimatorController = Definition.Aima; BossAnimator.ClearRemaps()
    gỡ StatModifiers của phase (đã được EntityVitalStats.Reborn → ResetRuntimeModifiers xoá sẵn; gọi lại cho chắc, idempotent)
    stateMachine.Initialize(BrainState)     // ← thứ mà Entity hiện tại KHÔNG làm (P2)
    Emit(ON_BOSS_SPAWNED)
OnDisable()          → executor.Interrupt(); StopAllCoroutines()   // không để telegraph/lock mồ côi
```

Sẵn có và vẫn đúng: `EntityVitalStats.OnEnable → Reborn()` (đầy HP, xoá modifier, `ClearImpacts()`).

**Những thứ hệ thống pool hiện tại *không* làm (cần biết):**
- `Pool.Reload()` **không inject lại** — không sao, vì dependency đã được inject ở lần `Instantiate` đầu.
- `Awake` / `Start` **không chạy lại**. Mọi việc khởi tạo theo từng lần spawn phải nằm trong đường reset ở trên.
- `ON_ENEMY_DEATH` → `EnemySpawner.ReleaseEnemy` trả boss về pool. Minion do boss triệu hồi đi cùng
  luồng này, và phải được emit `ON_SPAWN_EXTRA_ENEMY` lúc sinh ra để `RoomCell` đếm đúng.

---

## 8. Authoring trong Inspector và validation

### 8.1 Editor tooling — làm vs không làm

| Tool | Làm? | Vì sao |
|---|---|---|
| **`BossTypePickerDrawer`** cho `[SerializeReference]` (nút "+" → menu type lấy từ `TypeCache`, đường dẫn menu từ `[BossActionMenu("Projectile/…")]`) | ✅ **Bắt buộc** | Unity 2022.3 không có dropdown chọn type cho `[SerializeReference]`; thiếu nó thì designer không thêm được action |
| **Nút "Validate"** trên Inspector của `BossDefinition` / `BossAttackDefinition` + menu `Tools/Boss/Validate All` | ✅ | Bắt lỗi thiếu clip, sai key, mask = Nothing, tham chiếu null |
| **Debug view runtime** (custom Inspector của `BossController` khi đang Play: phase, state, attack/stage, bảng cooldown, lý do chọn đòn gần nhất) + gizmo range/hitbox | ✅ | Data-driven mà không có công cụ này thì gần như không debug được |
| Nút "Add required animation keys" trên profile | ✅ (nhỏ) | Giảm lỗi cho người mới |
| Preview timeline của attack (thanh stage theo thời gian) | ⏳ Để sau | Đẹp nhưng không bắt buộc |
| Graph editor (GraphView / Node) | ❌ | Mục 8.4 |

### 8.2 Danh sách kiểm tra của validator

| Nhóm | Kiểm tra | Mức độ |
|---|---|---|
| Boss | `Stats` null; `Phases.Count == 0`; phase [0] có `EnterWhen`; `PhaseId` trùng | Error |
| Boss | Ngưỡng `HealthPercentBelow` không giảm dần theo thứ tự phase | Warning |
| Boss | `Prefab` null, hoặc `prefab.Entity.data != this` | Error |
| Prefab | Thiếu `BossController`, `EntityCore`, `EntityMovement`, `EntityInput`, `EntityVitalStats`, `EntityStatsHandler`, `EntityFindTarget`, `BossNegativeReceiver`; hurtbox không có collider | Error |
| Targeting | `TargetMask` / `ObstacleMask` = 0 (*Nothing*) | Error (bài học BUG-072) |
| Phase | Behavior không có entry nào; entry có `Attack` null; `Weight == 0` ở mọi entry của một tier | Error / Warning |
| Attack | `Stages.Count == 0`; `Range.x > Range.y`; stage `Duration`-mode có `Duration == 0` và không có action | Error |
| Animation | Key của stage không có trong profile (sau khi tính remap của **mọi** phase dùng attack đó) | Error |
| Animation | `stateName` không tồn tại trong base controller; placeholder chưa được override (clip vẫn là placeholder) | Error / Warning |
| Animation | `Duration` lệch > 10% so với độ dài clip | Warning |
| Serialization | `SerializationUtility.HasManagedReferencesWithMissingTypes(asset)` (class bị đổi tên hoặc xoá) | Error |
| Action | `Validate()` riêng của từng action (prefab thiếu component, mask = 0, count ≤ 0, …) | Theo action |

`OnValidate()` chỉ làm những việc rẻ (sinh GUID, clamp). Validation đầy đủ chạy khi bấm nút, và nên
chạy trong pre-push compile check khi TD-048 được làm.

### 8.3 Người mới nhìn thấy gì

Mở `GolemWarden.asset` → Identity → Stats → Animation → Targeting → **Phases** (danh sách gập được,
header hiện `PhaseId` + tóm tắt "HP < 60% · 4 attacks"). Mở một phase → Movement (dropdown) → Attacks
(danh sách: tên attack · tier · weight · range). Double-click attack → thấy stage và action. Không có
khái niệm nào ngoài *phase → attack → stage → action*.

### 8.4 Inspector vs graph editor

| | Inspector (đề xuất) | Graph editor |
|---|---|---|
| Chi phí xây | ~1–2 ngày (drawer + validator + debug view) | 2–4 tuần (GraphView còn experimental trong 2022.3, lưu graph, undo, copy/paste) |
| Hợp với | Danh sách phẳng: phase → entry → stage | Logic phân nhánh sâu (BT, dialogue) |
| Rủi ro | Danh sách dài khó nhìn khi > 15 entry mỗi phase | Tự xây, tự bảo trì; dễ trượt thành visual scripting |
| Tiêu chí để cân nhắc lại | ≥ 3 boss mà designer thật sự cần nhánh lồng nhau selector không biểu diễn được | — |

---

## 9. Ví dụ hoàn chỉnh — "Golem Warden" (Deliverables 3, 4, 5)

Boss giả định, dùng rig **Golem** có sẵn (3 phase art). Top-down 2D, 8 hướng.

### 9.1 Cấu trúc asset

```
Assets/SO/Boss/GolemWarden/
  GolemWarden.asset                       (BossDefinition)
  GolemWarden_Stats.asset                 (EnemyStatSO)       HP 1500, Defense 5, PhysicalDamage 10, MoveSpeed 2.2
  GolemWarden_Anim.asset                  (BossAnimationProfile)
  GolemWarden.overrideController          (Aima, base = BossBase.controller)
  GolemWarden_Enraged.overrideController  (override của phase 3)
  Attacks/
    Atk_TripleSmash.asset                 (BossAttackDefinition — melee combo)
    Atk_RockBarrage.asset                 (projectile barrage)
    Atk_SeismicSlam.asset                 (AoE)
    Seq_Awaken.asset                      (transition sequence phase 2)
    Seq_Enrage.asset                      (transition sequence phase 3)
Assets/Prefab/Boss/GolemWarden.prefab
Assets/Prefab/Boss/Projectiles/RockShard.prefab        (ProjectileBody + CircleCollider2D trigger + PoolMember)
Assets/Prefab/Boss/Telegraph/GroundCrackMarker.prefab  (sprite vòng tròn, pooled)
```

### 9.2 Ba attack mẫu (dạng Inspector)

**A. `Atk_TripleSmash` — melee combo nhiều hit, có bước tiến**

```
DisplayName:        "Triple Smash"
Cooldown:           4.0
Range:              (0, 2.8)
RequireLineOfSight: false
FaceTargetOnStart:  true
Stages:
  [0] Telegraph   key=smash.windup   Duration 0.45   Lock ✓  Cancelable ✓
        Actions:  SpawnVfxAction { prefab: DustPuff, attach: Self }
  [1] Hit1        key=smash.hit1     Duration 0.30   Lock ✓
        Actions:  DashAction { direction: TowardTarget, speed: 6, duration: 0.12 }
                  HitboxDamageAction { Delay 0.08, Circle r=1.2, offset (1.1,0), active 0.08, base 18, scaling 1.0 }
  [2] Hit2        key=smash.hit2     Duration 0.30   Lock ✓
        Actions:  FaceTargetAction { track: false }
                  HitboxDamageAction { Delay 0.08, Circle r=1.2, offset (1.1,0), active 0.08, base 18, scaling 1.0 }
  [3] Hit3        key=smash.hit3     Duration 0.55   Lock ✓
        Actions:  HitboxDamageAction { Delay 0.20, Circle r=1.8, offset (1.3,0), active 0.10, base 30, scaling 1.4 }
                  SpawnVfxAction { Delay 0.20, prefab: ImpactCrack, at: HitboxCenter }
  [4] Recovery    key=smash.recover  Duration 0.60   Lock ✓  Cancelable ✓
```

**B. `Atk_RockBarrage` — projectile barrage (xoay dần qua mỗi đợt)**

```
DisplayName:   "Rock Barrage"
Cooldown:      6.0
Range:         (3, 10)
RequireLineOfSight: true
Stages:
  [0] Telegraph  key=barrage.charge  Duration 0.70  Lock ✓  Cancelable ✓
        Actions: SpawnVfxAction { prefab: ChargeGlow, attach: Self, lifetime 0.7 }
  [1] Fire       key=barrage.loop    EndMode ActionsFinished   Lock ✓
        Actions: RotatingBarrageAction { prefab: RockShard, perWave 10, waves 4, interval 0.3,
                                          degreesPerWave 9, flight { speed 7, lifetime 3,
                                          targetMask: PlayerHurtbox, blockMask: Wall }, base 12, scaling 0.5 }
  [2] Recovery   key=barrage.end     Duration 0.50  Lock ✓  Cancelable ✓
```

(Boss đầu tiên có thể dùng `ProjectilePatternAction { pattern: Ring, rotationPerWave: 9 }` —
`RotatingBarrageAction` ở 7.2 là ví dụ cho *mechanic mới*.)

**C. `Atk_SeismicSlam` — AoE telegraph có trễ, nhiều điểm nổ**

```
DisplayName:   "Seismic Slam"
Cooldown:      8.0
Range:         (0, 7)
Conditions:    [ TimeInPhaseAtLeast { seconds: 5 } ]
Stages:
  [0] Raise      key=slam.raise     Duration 0.60  Lock ✓  Cancelable ✓
        Actions: GroundAoEAction { points: 3, placement: AroundTarget(radius 2.0) + OnTarget,
                                   telegraph: GroundCrackMarker, telegraphTime: 1.1,
                                   radius: 1.6, base 26, scaling 1.2, targetMask: (default) }
  [1] Slam       key=slam.impact    Duration 0.60  Lock ✓
        Actions: SpawnVfxAction { prefab: ShockRing, attach: Self }
                 HitboxDamageAction { Circle r=1.5, offset (0,0), active 0.1, base 15, scaling 0.8 }  ← vùng dưới chân
  [2] Recovery   key=slam.recover   Duration 0.80  Lock ✓  Cancelable ✓
```

`GroundAoEAction` sống **xuyên qua ranh giới stage**: nó được `Begin` ở stage 0, và nổ sau 1.1 s
(lúc đó đang ở stage 1). Executor cho phép runner của stage trước chạy tiếp nếu action được đánh dấu
`OutlivesStage = true`. Mặc định là `false`: runner nhận `End(interrupted:false)` khi rời stage.

**Cùng một executor chạy cả ba** — nó chỉ biết "stage có thời lượng + danh sách action". Melee, đạn
hay AoE là khác biệt nằm hoàn toàn trong các action.

### 9.3 `GolemWarden.asset` — cấu hình Inspector đầy đủ, đa phase

```
── CharacterData ───────────────────────────────────────────────
Stats:              GolemWarden_Stats
Ability Bindings:   []                     (không dùng Abilities v2)
Default Weapon:     None                   (boss không dùng WeaponHolder)
── EntityData ──────────────────────────────────────────────────
Layer Mask:         Enemy
Aima:               GolemWarden.overrideController
Range Check FOV:    12                     (aggro, dùng bởi EntityFindTarget)
Movement Velocities: 2.2
── BossDefinition ──────────────────────────────────────────────
Identity:           bossId=auto  DisplayName="Golem Warden"  Title="Guardian of the Deep Vault"  Portrait=…
Prefab:             GolemWarden.prefab
Presentation:       showHealthBar ✓  barColor #C9803A  introDelay 1.0  phaseTickMarks ✓
Animation Profile:  GolemWarden_Anim
Targeting:          aggroRange 12  requireLoS ✓  obstacleMask=Wall  targetMask=PlayerHurtbox
Super Armor:        ✓
Death Duration:     2.5
On Death:           [ DespawnSummonsAction ]

Phases:
 [0] PhaseId "p1-awake"      DisplayName "Stone Sentinel"
     EnterWhen:              []
     Behavior:
       Movement:             ChaseTarget { stopDistance 2.2, speedMultiplier 1.0 }
       IdleBetweenAttacks:   (0.6, 1.2)     AvoidRepeatLast 1
       Attacks:
         - Atk_TripleSmash   priority 0  weight 5
         - Atk_RockBarrage   priority 0  weight 2
 [1] PhaseId "p2-fractured"  DisplayName "Fractured"
     EnterWhen:              [ HealthPercentBelow { 0.60 } ]
     InterruptPolicy:        AtCancelWindow   InvulnerableDuringTransition ✓
     TransitionSequence:     Seq_Awaken       (roar 1.2 s + ShockRing, không damage)
     OnEnter:                [ SummonAction { prefab: Pebble, count 3, ring r=3, emitExtraEnemy ✓ } ]
     StatModifiers:          [ AttackSpeed +15% ]
     Behavior:
       Movement:             ChaseTarget { stopDistance 2.5, speedMultiplier 1.15 }
       IdleBetweenAttacks:   (0.4, 0.9)
       Attacks:
         - Atk_TripleSmash   priority 0  weight 4
         - Atk_RockBarrage   priority 0  weight 3
         - Atk_SeismicSlam   priority 0  weight 3
         - Atk_SeismicSlam   priority 1  weight 1  Conditions [ DistanceToTarget { 0, 1.5 } ]  MaxUsesPerPhase 2
                             (“phạt” người chơi đứng sát — tier 1 luôn thắng khi đủ điều kiện)
 [2] PhaseId "p3-enraged"    DisplayName "Enraged Core"
     EnterWhen:              [ HealthPercentBelow { 0.25 } ]
     InterruptPolicy:        Immediately
     TransitionSequence:     Seq_Enrage
     AnimatorOverride:       GolemWarden_Enraged.overrideController
     AnimationRemaps:        [ smash.hit3 → smash.hit3.enraged ]
     StatModifiers:          [ PhysicalDamage +25%, MoveSpeed +20% ]
     Behavior:
       Movement:             ChaseTarget { stopDistance 2.0, speedMultiplier 1.3 }
       IdleBetweenAttacks:   (0.2, 0.5)
       Attacks:
         - Atk_SeismicSlam   priority 0  weight 3  ChainAfter [ Atk_TripleSmash, Atk_RockBarrage ]   ← pattern
         - Atk_TripleSmash   priority 0  weight 2  CooldownOverride 2.5
         - Atk_RockBarrage   priority 0  weight 2
```

`GolemWarden_Anim.asset` (trích):

```
BaseController: BossBase.controller
Bindings:
  locomotion          → Locomotion
  hit                 → Hit
  phase.transition    → PhaseTransition
  death               → Death
  smash.windup        → Action_01     fade 0.05
  smash.hit1          → Action_02
  smash.hit2          → Action_03
  smash.hit3          → Action_04
  smash.hit3.enraged  → Action_05
  smash.recover       → Action_06     fade 0.1
  barrage.charge      → Action_07
  barrage.loop        → Action_08
  barrage.end         → Action_06     (dùng lại clip recover)
  slam.raise          → Action_09
  slam.impact         → Action_10
  slam.recover        → Action_06
```

---

## 10. Vòng đời đầy đủ (Deliverable 7)

```
① SPAWN
  RoomGeneraterController ─ON_GET_SPAWN_POSITIONS─► EnemySpawner ─► RoomModel.GetSpawnSet() (entry của boss room)
  ─► ObjectPoolManager.Spawn(GolemWarden.prefab)
      └─ Pool: Instantiate
           ├─ Awake: Entity.LoadEntity()  (anim.runtimeAnimatorController = Definition.Aima)
           │         BossController.LoadState() → new BossBrainState / AttackState / PhaseTransitionState / DeathState
           │         BossController.BuildRuntime() → BossRuntime{ Animator adapter (hash cache),
           │                                          executors + runners cho mọi attack, cooldowns[], rng }
           ├─ Awake: EntityCore.Setup() gom core component
           └─ resolver.InjectGameObject → EntityInput(IPlayerService), BossController(IObjecPoolService)
  ─► Emit(ON_DONE_SPAWN_ENEMY, n) → RoomCell.EnemyCount = n (boss được tính như một enemy)

② RESET (lần đầu và mỗi lần lấy lại từ pool)
  OnEnable: EntityVitalStats.Reborn() (HP đầy, xoá modifier, xoá impact) · BossController._resetPending = true
  Update đầu tiên: BossController.ResetRuntime() → phase 0 → stateMachine.Initialize(Brain) → Emit(ON_BOSS_SPAWNED)
  BossHealthBarUI hiện tên, title, thanh máu 100%, vạch 60% / 25% (lấy từ condition của phase)

③ BRAIN (mỗi frame)
  kiểm tra HP ≤ 0 → Death
  runtime.Tick(dt): cooldown −= dt, timeInPhase += dt, cập nhật distance/LoS (EntityInput target)
  phase kế tiếp đủ điều kiện? → PhaseTransition
  movementRunner.Tick → EntityMovement.ChaseToTarget / MoveToNodeTarget (A*)
  idleTimer hết && decisionTimer ≥ 0.2 → DecisionMaker.TrySelect(phaseProfile, query)
     → (ví dụ) distance 1.9: TripleSmash (w5) vs RockBarrage (bị loại: 1.9 ∉ [3,10]) → chọn TripleSmash
  → ChangeState(Attack, TripleSmash)

④ ATTACK
  Enter: FaceTarget; executor.Begin(TripleSmash); cooldowns[TripleSmash] = 4.0; uses++; history.Push
  Stage 0 Telegraph: Animator.Play("smash.windup") · Lock(executor) · Dust VFX · 0.45 s
  Stage 1 Hit1: Dash 0.12 s (IMovement.ApplyKnockback) · +0.08 s: OverlapCircleNonAlloc → player NegativeReciver.TakeDamage(18 + 10×1.0)
  … Hit2, Hit3, Recovery …
  executor xong → entry có ChainAfter? → chạy đòn kế tiếp : ChangeState(Brain), idleTimer = rand(0.6, 1.2)

⑤ BỊ ĐÁNH
  Player MeleeWeapon.OnActivate → BossNegativeReceiver.TakeDamage
     Mitigate: invulnerable? 0 : max(0, dmg − Defense) → EntityVitalStats.Reduction(HP)
     OnDamaged: Emit(ON_BOSS_HEALTH_CHANGED, ratio) → HUD  (không SetPositionToCheck, không đổi state: super armor)

⑥ CHUYỂN PHASE (HP 900 → 890 / 1500 = 59.3%)
  Brain phát hiện phase [1] đủ điều kiện (nếu đang Attack: chờ stage Cancelable, theo AtCancelWindow)
  → PhaseTransitionState: invulnerable+ · OnExit(p1) · gỡ modifier p1 · phase = 1 · áp modifier p2 (AttackSpeed +15%)
     · movement runner mới · Emit(ON_BOSS_PHASE_CHANGED) · executor.Begin(Seq_Awaken) · OnEnter: Summon 3 Pebble
       (mỗi con: Pool.Spawn + Emit(ON_SPAWN_EXTRA_ENEMY) → RoomCell.EnemyCount++) · invulnerable− → Brain

⑦ CHẾT
  HP ≤ 0 ở bất kỳ state nào → executor.Interrupt() (Unlock, thu hồi telegraph) → BossDeathState
     Play("death") · Stop movement · OnDeath: DespawnSummons · Emit(ON_BOSS_DEFEATED) → HUD ẩn
     sau DeathDuration (hoặc Status OnActivate từ clip): Emit(ON_ENEMY_DEATH, gameObject)
        → EnemySpawner.ReleaseEnemy → Pool.Release (SetActive false) · OnDisable: dọn dẹp
        → RoomCell.EnemyCount-- → 0 → ON_CLEAR_ENEMY → OpenDoors()

⑧ TÁI SỬ DỤNG → quay lại ② (không có Awake / Start)
```

Lưu ý về đếm enemy: `DespawnSummonsAction` thu hồi minion mà **không** emit `ON_ENEMY_DEATH` cho
từng con sẽ làm `RoomCell.EnemyCount` không bao giờ về 0 → cửa không mở. Action này phải emit
`ON_ENEMY_DEATH` cho mỗi minion bị thu hồi. Cần ghi rõ trong GDD.

---

## 11. Trade-off chi tiết

### 11.1 Performance
- Boss chỉ có 1–3 instance mỗi phòng. Chi phí mỗi frame: một lần evaluate condition của phase kế
  tiếp (1–3 condition) + tick movement + tick runner đang chạy. Không đáng kể.
- Selector chạy mỗi 0.2 s; tối đa ~10 entry × ~3 condition.
- **0 alloc theo từng đòn:** runner, buffer ứng viên, `Collider2D[]`, `HashSet` đều tạo sẵn.
  Chỗ duy nhất có alloc là `Pool.Spawn` khi pool rỗng (hành vi sẵn có).
- Swap `runtimeAnimatorController` chỉ xảy ra lúc chuyển phase / reset, không xảy ra theo từng đòn
  (khác với mẫu `EntityAttackState` hiện tại, vốn swap mỗi đòn).

### 11.2 Memory
- Asset: dữ liệu `[SerializeReference]` nằm inline trong asset của attack, nhẹ hơn một SO riêng cho mỗi action.
- Runtime mỗi instance: vài chục runner + mảng cooldown ≈ vài KB.

### 11.3 Serialization (rủi ro lớn nhất)
- **Đổi tên / namespace / assembly của class dùng trong `[SerializeReference]` sẽ làm mất data một
  cách âm thầm** (Unity lưu tên type). Biện pháp: `[UnityEngine.Scripting.APIUpdating.MovedFrom(...)]`
  mỗi khi đổi tên; validator dùng `SerializationUtility.HasManagedReferencesWithMissingTypes`.
  Quy tắc này phải vào `weapon-skill-code.md` (hoặc một file rule mới cho boss).
- Nếu sau này thêm `.asmdef` (BUG-084), việc chuyển assembly cũng tính là "đổi tên": phải có
  `[MovedFrom(sourceAssembly: "Assembly-CSharp")]`.
- Thêm field mới vào action sẽ lấy default → an toàn. Xoá field thì data của field đó mất (bình thường).
- `BossDefinition : EntityData` — nếu `EntityData` đổi field thì boss bị ảnh hưởng theo. Chấp nhận
  được, vì `CharacterData` đã có sẵn `FormerlySerializedAs` theo đúng thói quen đó.

### 11.4 Debugging
- Rủi ro: "tại sao boss không đánh?" có thể nằm ở 6 chỗ (cooldown, range, LoS, condition, uses, tier).
  **Bắt buộc** debug view hiển thị *lý do bị loại* cho từng entry trong lần quyết định gần nhất.
- Gizmo: range của từng entry (vòng tròn màu theo tier), hitbox đang active, điểm AoE.
- Log theo `ai-code.md`: chỉ `#if UNITY_EDITOR`, bật/tắt theo instance.

### 11.5 Maintainability
- Contract nhỏ và ổn định: `BossAction.CreateRunner`, `IBossActionRunner` (3 method),
  `BossCondition.Evaluate`, `BossMovementPolicy.CreateRunner`. Thêm tính năng = thêm class.
- `BossActionContext` là điểm dễ "phình" nhất. Chỉ thêm property khi có ≥ 2 action cần tới nó.
- Hai đường tấn công song song (weapon cho enemy thường, action cho boss) → cần ADR ghi rõ, để không
  ai "hợp nhất" chúng một cách vô tình.

### 11.6 Giới hạn thật của cách tiếp cận (không đồng ý hoàn toàn với yêu cầu)
1. **Boss dạng puzzle / scripted** (đổi arena, tương tác tilemap, cutscene nhiều nhân vật) sẽ cần action
   riêng, và có thể là action *chỉ dùng cho boss đó*. Việc đó chấp nhận được: mechanic đó vẫn nằm
   trong một class tái sử dụng được, không nằm trong controller.
2. **Timing khớp frame** giữa data và clip là trách nhiệm của hai nơi (data `Duration` và clip). Validator
   chỉ cảnh báo được.
3. **Selector không biểu diễn được "kế hoạch" nhiều bước có điều kiện rẽ nhánh giữa chừng.**
   `ChainAfter` là tuần tự cứng. Nếu cần rẽ nhánh giữa chuỗi, đó là tín hiệu để nâng cấp lên BT.
4. **Số slot animation có hạn** (đề xuất 10). Boss cần > 10 clip hành động riêng biệt thì cần
   controller variant nhiều slot hơn, hoặc Playables.

---

## 12. Roadmap theo thứ tự phụ thuộc (Deliverable 8)

| Mốc | Nội dung | Phụ thuộc | Ước lượng | Kết quả kiểm chứng |
|---|---|---|---|---|
| **M0 — Tiền đề** | (a) Sửa BUG-066/070 (guard vitals). (b) File bug + sửa P2 (reset state machine khi lấy lại từ pool) — cho cả enemy thường. (c) File bug P3 (`TakeDamge`). (d) `design/gdd/boss-system.md` (8 mục) qua `/design-system`. (e) **ADR-0006** "Data-driven boss framework" (đường tấn công riêng của boss, `[SerializeReference]`, quy tắc `[MovedFrom]`). (f) Đổi method animation event của `Entity` thành `protected`, xác minh trong Editor | — | 1.5–2 ngày | GDD + ADR được duyệt; Play Mode: enemy lấy lại từ pool không kẹt ở Death |
| **M1 — Khung + melee** | `BossDefinition`, `BossPhaseDefinition` (chỉ phase 0), `BossBehaviorProfile`, `BossAttackDefinition`/`Stage`; `BossAction`/`BossCondition`/`BossMovementPolicy` base; `BossRuntime`, `BossAttackExecutor`, `BossDecisionMaker`, `BossAnimator`, `BossActionContext`; `BossController : Entity` + 3 state (Brain/Attack/Death); `BossNegativeReceiver`; `BossBase.controller`; actions: PlayAnimation, FaceTarget, HitboxDamage, Dash, SpawnVfx; conditions: DistanceToTarget, HealthPercentBelow; movement: ChaseTarget, HoldPosition; **type-picker drawer** | M0 | 4–5 ngày | Golem Warden phase 1 chỉ với TripleSmash: đuổi, đánh, gây damage, chết, phòng được clear |
| **M2 — Đạn + AoE** | `ProjectilePatternAction` (dùng `ProjectileBody`), `GroundAoEAction` + telegraph prefab, `OutlivesStage`; condition LoS; movement KeepDistance | M1 | 2 ngày | RockBarrage + SeismicSlam chạy trên cùng executor |
| **M3 — Phase + HUD** | Danh sách phase, `BossPhaseTransitionState`, `InterruptPolicy`, invulnerable, `StatModifiers`, override/remap, `TransitionSequence`, OnEnter/OnExit; `SummonAction` + `DespawnSummonsAction`; `SignalReceived` + `EmitSignalAction`; EventID mới; `BossHealthBarUI` | M2 | 3 ngày | Golem Warden đủ 3 phase trong Play Mode |
| **M4 — An toàn authoring** | Validator đầy đủ + nút + menu; debug view runtime + gizmo; context menu Reinitialize; kiểm thử pooling (spawn → giết → spawn lại ×3); `ChainAfter`, `AvoidRepeatLast`, `MaxUsesPerPhase` | M3 | 2–3 ngày | Checklist Scenario 4 pass; validator bắt được 10 lỗi được cố ý cài vào |
| **M5 — Tuỳ chọn / để sau** | Poise/stagger state, resistance, utility curve / weight modifier, bridge Abilities v2, Playables, preview timeline, camera shake, BT + graph editor | M4 + nhu cầu thật | — | — |

**Boss đầu tiên = M0 → M4 ≈ 12–15 ngày làm việc.** Với velocity gần đây (sprint 16 chỉ đặt ~1.1 ngày
Must-Have), con số này tương ứng nhiều sprint. Nên chia M1 thành 2 story.

**Test:** EditMode test cho `BossDecisionMaker` (seed cố định), `BossAttackExecutor` (tick tay, không
dùng `Time.time`) và condition là lý tưởng — nhưng **hiện chưa viết được do BUG-084** (không có
`.asmdef`). Cho tới khi đó, bằng chứng là một playtest session có ghi chép (`test-standards.md`:
Integration → PlayMode test hoặc playtest có tài liệu).

---

## 13. File cần thay đổi và file / asset mới (Deliverable 9)

### 13.1 File hiện có cần sửa (ít, đều là thay đổi cộng thêm)

| File | Thay đổi | Lý do |
|---|---|---|
| `Assets/Script/Character/Entity/Entity.cs` | `LoadState()` → `protected virtual`; state khởi đầu qua `protected virtual EntityState InitialState`; thêm `protected virtual void OnEnable()` / reset hook; 6 method animation event → `protected` | `BossController` override; sửa P2 cho mọi enemy; P5 |
| `Assets/Script/Character/Entity/EntityState.cs` | `Enter`/`Exit`: bỏ qua `SetBool` khi `animBoolName` rỗng | State của boss dùng `BossAnimator.Play`, không dùng bool |
| `Assets/Script/Character/Base/VitalStatsBase.cs` | Guard dictionary (BUG-066/070) | P1 |
| `Assets/Script/Manager/EventManager.cs` | Thêm `ON_BOSS_SPAWNED`, `ON_BOSS_HEALTH_CHANGED`, `ON_BOSS_PHASE_CHANGED`, `ON_BOSS_DEFEATED`, `ON_BOSS_SIGNAL` (23 → 28) | `manager-event-code.md`: event mới chỉ thêm vào enum |
| `Assets/Script/Utility/GameConstants.cs` | `GameConstants.BossAnimation` (tên state slot, `Direction`, `Speed`) | Không có magic string |
| `Assets/Script/Character/Entity/CoreComponent/EntityMovement.cs` | *Có thể* thêm `RequestPathTo(Vector2)` public cho `KeepDistance` (hiện `FleeTarget` phụ thuộc `EntityFindTarget.IsNearPlayer()` với min range nằm trên prefab) | Cần xác minh khi làm M2 |
| `.claude/rules/weapon-skill-code.md` (hoặc rule mới `boss-code.md`) | Quy tắc: action stateless, runner giữ state, `[MovedFrom]`, không ghi SO | Phòng lỗi serialize |
| `CLAUDE.md`, `design/gdd/systems-index.md` | Đăng ký hệ thống mới, xoá ghi chú thư mục Boss mồ côi | Đồng bộ tài liệu |

**Không thay đổi:** `GameLifetimeScope` (boss được pool inject), `EnemySpawner` (boss là một spawn
entry bình thường), `ProjectileBody`, `EntityNegativeReciver` (boss subclass), `EntityVitalStats`,
`EntityStatsHandler`, `MovementBase`, toàn bộ Abilities v2.

### 13.2 Class mới

```
Assets/Script/System/Boss/                      (các .meta mồ côi ở Character/Boss/ nên xoá hoặc tận dụng — quyết định ở M0)
  Definitions/
    BossDefinition.cs            BossPhaseDefinition.cs       BossBehaviorProfile.cs (+ BossAttackEntry)
    BossAttackDefinition.cs      BossAttackStage.cs           BossAnimationProfile.cs
    BossPresentation.cs          BossTargetingSettings.cs     Enums.cs (StageEndMode, PhaseInterruptPolicy, TargetPointMode)
  Extensibility/
    BossAction.cs  InstantBossAction.cs  IBossActionRunner.cs  BossActionMenuAttribute.cs
    BossCondition.cs  BossQuery.cs  BossMovementPolicy.cs  IBossMovementRunner.cs
  Runtime/
    BossRuntime.cs  BossAttackExecutor.cs  BossDecisionMaker.cs  BossAnimator.cs  BossActionContext.cs
    BossSignal.cs (payload)  BossHudData.cs (payload)
  Actions/        PlayAnimationAction  FaceTargetAction  HitboxDamageAction  DashAction  SpawnVfxAction
                  ProjectilePatternAction  GroundAoEAction  SummonAction  DespawnSummonsAction
                  SetInvulnerableAction  EmitSignalAction  SetAnimatorParameterAction
  Conditions/     DistanceToTargetCondition  HealthPercentBelowCondition  HasLineOfSightCondition
                  TimeInPhaseAtLeastCondition  SignalReceivedCondition  AliveSummonsAtMostCondition  AnyOfCondition
  Movement/       ChaseTargetPolicy  KeepDistancePolicy  HoldPositionPolicy
Assets/Script/Character/Boss/
  BossController.cs              BossNegativeReceiver.cs
  States/ BossBrainState.cs  BossAttackState.cs  BossPhaseTransitionState.cs  BossDeathState.cs
Assets/Script/UI/
  BossHealthBarUI.cs             (UGUI + TextMeshProUGUI, subscribe ON_BOSS_*)
Assets/Editor/Boss/
  BossTypePickerDrawer.cs        BossDefinitionEditor.cs (nút Validate, debug view khi Play)
  BossValidator.cs  BossValidationReport.cs  BossControllerEditor.cs (debug runtime)
```

### 13.3 Asset mới

```
Assets/AnimationController/Boss/BossBase.controller          (+ BossBase_1Dir.controller nếu cần)
Assets/AnimationController/Boss/Placeholders/*.anim          (clip rỗng cho các slot)
Assets/Prefab/Boss/BossTemplate.prefab                        (root + EntityCore + đủ core component — mẫu để duplicate)
Assets/Prefab/Boss/GolemWarden.prefab, Projectiles/RockShard.prefab, Telegraph/GroundCrackMarker.prefab
Assets/SO/Boss/GolemWarden/…                                  (mục 9.1)
Assets/UI/… BossHealthBar prefab (Canvas)
design/gdd/boss-system.md
docs/architecture/adr-0006-data-driven-boss-framework.md
```

---

## 14. Câu hỏi cần chủ dự án quyết định trước khi code

1. **`[SerializeReference]` + drawer (đề xuất)** hay **strategy SO** (không có editor code, nhiều asset hơn)?
2. `BossDefinition` **kế thừa `EntityData`** (đề xuất: một asset, component base chạy không cần sửa)
   hay **tách riêng** (sạch hơn nhưng phải sửa `EntityCore.Data` / constructor của `EntityState`)?
3. Boss spawn qua **`RoomModel` spawn entry có sẵn** (đề xuất cho boss đầu) hay cần một "boss room"
   thật sự (`RoomType` — hiện không được đọc lúc runtime, Bug #16)?
4. Chấp nhận **super armor mặc định** (không có hit-stun) cho boss đầu, và để poise sau?
5. Sửa P2 (reset khi lấy lại từ pool) ngay trong `Entity` — thay đổi này cũng ảnh hưởng enemy thường. Đồng ý không?
6. Thư mục `Assets/Script/Character/Boss/` (ba `.meta` mồ côi): dùng lại hay xoá rồi tạo mới?

Khi các điểm trên được duyệt, bước kế tiếp là **M0**: GDD + ADR-0006 + ba bug tiền đề. Chưa viết code
boss trước khi GDD/ADR được chấp thuận.
