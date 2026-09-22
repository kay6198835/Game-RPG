# Documentation Changelog

Every documentation change in this project, with the code change that caused it.

**Why this file exists.** Between 2026-08-22 and 2026-09-09 four structural changes landed in
`Assets/Script/` with no documentation update. By 2026-09-11 that had produced 23 drifted and
9 outdated living documents, including rules files that instructed contributors to write code
that would not compile. This log exists so a documentation change is never again silent about
which code change caused it.

**Conventions used here**
- Every entry names the **cause** (a commit, a file, or a verified source read), not just the effect.
- **Nothing is hard-deleted.** Outdated documents move to `docs/archive/` with a `⚠️ DEPRECATED`
  banner and their original content untouched.
- Dated snapshots (sprint logs, retros, triage reports, playtests) are **historical records** and
  are never rewritten — they were accurate on their own date.

---

## 2026-09-22 (sixth pass, same day) — Owner review round 4: BUG-089 closed by design

**Cause.** The seven-gap audit of the gain-tier flow was put to the owner. Five gaps were answered,
all five answers hold, and the bug closes. This entry records the answers and what was promoted out
of the bug rather than dropped with it.

### The answers

| Gap | Owner's position | Outcome |
|---|---|---|
| **1** — paying buys nothing; nothing carries "paid" to `Apply()` | The gain fields are not designed yet; the flow is deliberately laid down to open the path later | **Accepted — not a defect.** Re-classified as *planned, unimplemented* |
| **3** — `Costs` is one all-or-nothing bundle, not a tier ladder | Same: the data shape for tiers is future work | **Accepted — not a defect** |
| **2** — a refusal calls `CancelHold()` and terminates the channel | **Intentional.** Not enough resource → no tier up **and the ability is force-released**. `CancelHold()` is the mechanism | **Withdrawn.** The reading was wrong — the design is not "drop to default and keep channelling", it is "drop to default and fire now", which is exactly what `CastInstant():50-54` does |
| **4** — charged per animation event; sustain-vs-purchase undefined | Scaffolding; and the per-animation event **is required** — it is what allows the check to happen at runtime each loop | **Accepted — by design.** The animation event is the intended runtime checkpoint |
| **6** — effect list order decides who gets the resource | **Also intentional and important**, because the same order decides which effect runs first and last. Belongs with the per-effect cost as one shared authoring note | **Accepted — promoted to a documented rule** |
| **7** — a refused tier removes the Consecrate telegraph | Note it; only Active abilities are in use | **Recorded as a note.** Not filed, not scheduled |
| **5** — `HoldRatio` always `0f` | not raised | Unchanged — independent, tracked as **BUG-083** |

### Verified: nothing is affected today

| Ability asset | `ActivationType` |
|---|---|
| `Avatar of Light.asset` | `0` — Active |
| `Blessing.asset` | `0` — Active |
| `Consecrate.asset` | `0` — Active |
| `Paladin Blessed Slash Ability.asset` | `1` — Hold |
| `SpiritBomd.asset` | `1` — Hold (part of the broken ShootSpirit pair) |

Three of the four live Paladin abilities are Active, so the `Hold` branch at
`AbilityInstance.cs:50-53` is skipped for them entirely. Every effect asset has `SubConditions: []`
and no serialized `Costs` key, so `TryCast()` returns `true` unconditionally and `CancelHold()` at
`:79` is never reached. The owner's "current logic is not affected" claim checks out against the
assets.

### Promoted out of the bug — three new rules in `.claude/rules/weapon-skill-code.md`

1. **Effect list order is authored data, not an implementation detail.** `AbilityDefinition.Effects`
   order sets execution order (`Casting()` and `Execute()` both walk it by index) *and* resource
   priority (`Effects[0]` has first claim, so a later effect can be refused because an earlier one
   already spent the resource). Reordering in the Inspector changes both. Balance decision.
2. **Per-effect `Costs` is an upgrade-tier price, not a go/no-go gate** — with the force-release
   behaviour on `Hold` spelled out.
3. ⚠️ **Do not author a per-effect `Costs` list until the tier fields exist.** Nothing carries "paid"
   through to `Apply()` and no effect has a second power level, so a cost authored today would take
   the player's resources and give nothing back.

### One residual loose end, recorded and not pursued

`AbilityInstance.IsHolding` and `AbilityHolder.IsHolding` are two fields for one fact. The forced
release calls the instance's `CancelHold()` directly, so the holder still reports `true` afterwards.
It self-heals — the state has already moved to `Do`, so
`PlayerSkillWeaponState.AnimationOnAction():30-40` takes its `else` branch next event, and the
holder's flag is cleared on button release (`PlayerInputHandle.cs:243`/`:273`). Worth knowing before
anything new reads `AbilityHolder.IsHolding`; not worth a fix on its own.

### Documents changed

| Document | Change |
|---|---|
| `production/qa/bugs/BUG-089.md` | `## CLOSED — owner review round 4` appended: the answer table, the `ActivationType` audit, the rule promoted out of gap 6, the gap 7 note, the `IsHolding` loose end, and a five-step specification for whoever implements the tier feature. Status → CLOSED |
| `.claude/rules/weapon-skill-code.md` | Three rules added above the existing cost rules |
| `CLAUDE.md` | BUG-089 row rewritten as CLOSED, original text kept after "*Original entry:*" |
| `production/qa/open-issues-2026-09-22.md` | Counts updated (18 open), the §1 BUG-089 entry rewritten as closed-with-notes, the blocker table and I-1 adjusted |

### Constraint check

No `.cs` file changed in this pass. No commit beyond the documentation commit, no force push.

---


## 2026-09-22 (fifth pass, same day) — Owner review round 2: BUG-072 proposal applied, BUG-089 and BUG-091 item 1 restated

**Cause.** Three responses from the owner: approval of the BUG-072 buffer proposal, a request for a
clearer explanation of BUG-089, and a challenge to BUG-091 item 1 (*"mục 1 t trả return mà"*).
`git fetch --all` was re-run first — **remote unchanged at `2a83469`**; all `.cs` edits remain local
and uncommitted.

### Code changed — `LightningController.cs`, with owner approval

```diff
-    [SerializeField] private float radius;
-    [SerializeField] private Collider2D[] _buffer;
-    [SerializeField] private LayerMask layerMask;
+    [SerializeField] private float radius = 2f;
+    [SerializeField] private LayerMask layerMask;
+    [SerializeField] private int maxTargets = 16;
     [SerializeField] private int randomIndex;
+    private Collider2D[] _buffer;
+
+    protected override void Awake()
+    {
+        base.Awake();
+        _buffer = new Collider2D[maxTargets];
+    }
```

`Execute()` untouched — its shape was already correct. A serialized `Collider2D[]` is given
**length 0** by Unity, and `OverlapCircleNonAlloc` writes at most `results.Length` hits, so the query
was returning `0` on every call, silently. Allocating in `Awake()` also satisfies
`.claude/rules/engine-code.md`. `base.Awake()` is mandatory — `SpawnSummonBase.Awake()` is what
caches `animator`, which `Launch()` uses on the next frame.

The owner subsequently tuned `maxTargets` from `16` to `4`. Recorded, not changed: it caps how many
enemies one Consecrate strike can damage, which is a balance decision, and the field exists precisely
so that number is visible and editable.

⚠️ **BUG-072 does not close yet.** `layerMask` ships as `0` = *Nothing* and has no safe code default
(a literal would be a hardcoded layer index, forbidden by `engine-code.md` and `ai-code.md`), so it
must be set on `Lightning.prefab`. Confirm the fix by an enemy losing HP, not by a clean Console —
every failure mode in this bug has been silent.

### BUG-091 item 1 — the early return covers one case out of four

The owner is right that the guard returns. It is joined by `&&`, so it returns **only when both lists
are empty**:

| `Conditions` | `Costs` | `LEFT && RIGHT` | returns early? | what runs next |
|---|---|---|---|---|
| empty/null | empty/null | `true` | yes | — |
| 2 entries | 2 entries | `false` | no | both loops fine |
| **null** | **2 entries** | `false` | **no** | `foreach (… in Conditions)` → **NRE** |
| **2 entries** | **null** | `false` | **no** | `foreach (… in Costs)` → **NRE** |

The guard protects the case that needs no protection — an empty list iterates zero times and throws
nothing anyway — and skips the two that do. Fix recorded: one `if (list != null)` per loop, with the
combined early return deleted. A **fifth site** was found in the same file: `GetCostValues()`
(`:74-77`) walks `Costs` with neither a list guard nor an element guard.

### BUG-089 — restated as one worked example

Effect A costs 20 Mana (a per-effect `Costs` entry), player has 10. At `Cast`: `A.TryCast()` returns
`false`, so `PayCost(A.Costs)` is skipped — A is **not** charged — and `CancelHold()` runs. `:54`
then advances to `Do`, where `Execute():147` calls `A.Apply()` with no check of any kind.
**Refusing an effect currently makes it free, not blocked.**

Left as a question rather than a defect claim, because one reading makes it intentional: `TryCast()`
gates only the `Cast` phase, and `Apply()` is the ability resolving — which is exactly the Consecrate
telegraph/payload shape. Owner picks one: write that rule into `.claude/rules/weapon-skill-code.md`,
or give `Execute()` a record of which effects refused. Dormant either way — every live effect asset
has `SubConditions: []` and no serialized `Costs`, so `TryCast()` cannot return `false` today.

### Documents changed

| Document | Change |
|---|---|
| `production/qa/bugs/BUG-072.md` | `## Proposal applied` appended: the diff, a before/after table with the reason for each change, the `base.Awake()` warning, and the remaining Inspector step spelled out. Status line rewritten |
| `production/qa/bugs/BUG-089.md` | `## Restated plainly` appended: the 20-Mana worked example step by step, the two readings in a decision table, and the per-asset audit proving it dormant |
| `production/qa/bugs/BUG-091.md` | `## Owner review round 2` appended: the four-row truth table for the `&&` guard, the per-list fix, the newly found `GetCostValues()` site, and a six-row status table for the bug's items |
| `CLAUDE.md` | BUG-072, BUG-089 and BUG-091 rows rewritten to match |


### Round 3, minutes later — BUG-082 and BUG-091 closed by the owner

Three more edits landed in the working tree while this entry was being written:

- `AbilityDefinition.cs:25` — `public List<StatCost> Costs = new();`
- `AbilityDefinition.cs:67-75` — `CheckPayCostValid()` **deleted** (zero callers, body already
  inlined into `TryStart()`)
- `AbilityEffectDefinition.cs:26` — operands swapped to `Costs == null || Costs.Count == 0`

**BUG-082 is FIXED** — both sites gone, one swapped and one deleted.
**BUG-091 is FIXED** — four of its six items fixed outright, and the `= new()` makes the other two
(the `&&` early return, and `GetCostValues()` walking `Costs` unguarded) **unreachable**: all three
lists on `AbilityDefinition` now carry initialisers, so no loop on that class can meet a null. What
is left is cosmetic — the `&&` block at `:52-55` is now redundant and reads as a null guard it no
longer needs to be. Recorded as a tidy-up, explicitly not recommended for sprint time.

One adjacent site found while verifying and **not** folded into either bug:
`AbilityEffectDefinition.SubConditions` (`:7`) has no `= new()` and is dereferenced at `:13`
(`if (SubConditions.Count > 0)`). Same shape, other class, one character.

The owner also tuned `LightningController.maxTargets` from `16` to `4` — a balance decision,
recorded and not changed.

### Constraint check

Two `.cs` files changed across passes four and five, both at explicit owner request:
`SpawnProjectileBase.cs` (BUG-075, one word) and `LightningController.cs` (BUG-072, buffer
allocation). `EntityMovement.cs` and `AbilityDefinition.cs` are the owner's own uncommitted work and
were not touched. No commit, no push.

---


## 2026-09-22 (fourth pass, same day) — Owner review of the post-fetch findings; BUG-075 fixed in code

**Cause.** The post-fetch findings from the entry below were put to the owner. Four were challenged
or decided, one fix was requested, and four uncommitted `.cs` edits had appeared in the working tree
since that entry was written. `git fetch --all` was re-run first: **the remote is unchanged at
`2a83469`** — every edit described here is local and uncommitted.

This is the first pass in this series that changed a `.cs` file, and it did so at explicit owner
request. That request supersedes the "no `.cs` edits" constraint for this one file only.

### Code changed — one file, at owner request

`Assets/Script/System/Abilities/Runtime/SpawnMono/SpawnProjectileBase.cs:29`

```diff
-    private void OnTriggerEnter(Collider other)
+    private void OnTriggerEnter2D(Collider2D other)
```

**BUG-075 → FIXED.** One word, no other change. The body (`:31-33`) was already correct and is the
reference implementation of the set-then-invoke contract; it had simply never been dispatched to.
No null guard was added, deliberately: `TryGetComponent` leaves the receiver null for a non-receiver
collider, and the effect's `if (negativeReceiver != null)` guard is what handles that.

Two things the fix exposes, recorded in `BUG-075.md` but not filed as bugs: the projectile does not
despawn on hit and so re-triggers on everything it passes through, and it carries no layer mask.

### Owner decisions and corrections

| Item | Owner position | Verified outcome |
|---|---|---|
| **BUG-088** — build break | *"t đâu thấy có issue gì"* | **Half right.** `Random.range` → `Random.Range`, `trasnform.postion` → `transform.position`, `Vector3.Lerp` → `Vector2.Lerp` are all fixed in the working tree and the project compiles, which is why nothing is visible. ❌ **`Random.Range(10, 100)/100` is still integer division.** Both operands are `int`, the largest numerator is 99, and `99 / 100 == 0` — so `rangeToCheck` is `0` on every call, `Vector2.Lerp(a, b, 0)` returns `a`, and the method assigns the entity its own position. The feature is dead and reports nothing. Fix: `/ 100f`. **S1 → S3 once the build fix is committed** |
| **BUG-089** — refused `TryCast()` | *"không return nữa đây là method void"*, *"casting đã chuyển sang void"* | **Accepted; claim re-framed and fix sketch withdrawn.** With `Casting()` `void`, `CancelHold()` reads as the channel terminator for a `Hold` ability and `CastInstant():52` is then correct — that half of the original finding is wrong and is withdrawn. What remains is a question, not an assertion: `Execute():139-148` calls `Apply()` on every effect unconditionally, so an effect that refused at `Cast` still fires at `Do`, and pays nothing. **Dormant** — audited every `.asset` under `Assets/SO/Skill/`: every live Paladin effect has `SubConditions: []` and no serialized `Costs`, so `TryCast()` cannot return `false` today. **S2 → S3** |
| **BUG-090** — orphaned condition asset | *"đã xóa script đó"* | **Correct, and that is the premise of the bug, not a refutation.** The **script** `HasEnoughManaCondition.cs` was deleted (BUG-077, correctly closed). The **asset** `Assets/SO/Skill/Conditions/Has Enough Mana Condition.asset` is still on disk — re-verified by `ls` and by `git status` showing no deletion — and its `m_Script` guid `6f896bb2…` now resolves to 0 files. `ShootSpirit.asset` and `SpiritBomd.asset` still reference it. **Open, unchanged** |
| **BUG-063** — `[SerializeField]` on `Stat.modifiers` | keep through development, remove at demo prep | **Recorded as ACCEPTED (deferred).** This is a deliberate trade and it supersedes the "one-line fix, no blocker, carried 29+ cycles" framing in this file and in `CLAUDE.md`. The cost is documented alongside it: Play Mode in the Editor *is* `UNITY_EDITOR`, the leak is silent, and it has already reached git once. Mitigation recorded: check `git status` for a dirty `Assets/SO/Stat/*.asset` after any Play Mode session. Re-evaluation triggers listed |
| **BUG-082 / BUG-091** — *"tại sao phải đảo toán hạng"* | question | **Answered in `BUG-091.md`.** `\|\|` evaluates left to right and evaluates the right operand only when the left is `false`. In `Costs.Count == 0 \|\| Costs == null`, `.Count` runs first, so when `Costs` is null it throws before the null test is ever reached: the guard is unreachable in the only case it exists for. Swapping works because `Costs == null` is safe on a null reference and short-circuits before `.Count`. Latent, not live — Unity's serializer materialises an empty list on asset load |

### Owner fixes found in the working tree (uncommitted, not by this pass)

| File | Change | Effect |
|---|---|---|
| `EntityMovement.cs` | three identifier/type fixes | **BUG-088 partial** — builds again; integer division survives |
| `AbilityDefinition.cs` | `if (condition == null) continue;` and `if (cost == null) continue;` added to `TryStart()` | **BUG-091 defect 2 closed.** `StatCost` is a `class` (`:98`) so `cost == null` is valid. Also absorbs BUG-090's one live consequence |
| `LightningController.cs` | overlap query implemented — `OverlapCircleNonAlloc`, assign receiver, `base.Execute()` per target | **BUG-072 partial.** Shape is correct. ⚠️ `_buffer` is a serialized `Collider2D[]` that nothing allocates, so Unity gives it **length 0** and `OverlapCircleNonAlloc` returns `0` every call, silently. `radius` and `layerMask` also default to `0` / *Nothing*, and none of the three fields exist yet in `Lightning.prefab:119-122`. Recommended: allocate in an `Awake()` override that calls `base.Awake()` (which caches the Animator) |

### Documents changed

| Document | Change |
|---|---|
| `production/qa/bugs/BUG-075.md` | `## FIXED — 2026-09-22, at owner request` appended with the diff, the reason no null guard was added, and the two exposed follow-ups. Status line rewritten, previous one kept inline |
| `production/qa/bugs/BUG-063.md` | `## Owner decision — ACCEPTED for the development phase` appended: the decision, what the acceptance costs, three mitigations, and the re-evaluation triggers |
| `production/qa/bugs/BUG-088.md` | `## Owner review` appended: which defects are fixed, then integer division demonstrated with a value table and a two-line proof. Severity path S1 → S3 stated |
| `production/qa/bugs/BUG-089.md` | `## Owner review` appended: `void` design accepted, `Hold`-path claim and `bool`-return sketch withdrawn, the remaining item restated as a design question, and the per-asset audit proving it dormant |
| `production/qa/bugs/BUG-090.md` | `## Owner review` appended: the script/asset distinction in a two-row table, with the `ls` and `grep` output re-run after the review |
| `production/qa/bugs/BUG-091.md` | `## Owner review` appended: the operand-order answer in full, the owner's element guards recorded as closing defect 2, and a four-row table of what is still open |
| `production/qa/bugs/BUG-072.md` | `## Owner fix` appended: the implementation recorded as correct in shape, then the three zero-defaulting fields, with the `Awake()` override sketch and the `base.Awake()` warning |
| `CLAUDE.md` | Header block extended with the four working-tree edits and the two decisions. Rows rewritten for BUG-063 (ACCEPTED), BUG-072 (PARTIAL), BUG-075 (FIXED), BUG-088 (PARTIAL), BUG-089 (S3, re-framed), BUG-091 (PARTIAL) — each keeping its original text after "*Original entry:*" |

### Constraint check

One `.cs` file changed, at explicit owner request: `SpawnProjectileBase.cs`, one word. The three
other modified `.cs` files are the owner's own uncommitted work and were not touched. No commit, no
push.

---


## 2026-09-22 (third pass, same day) — Post-fetch re-verification against HEAD `2a83469`

**Cause.** `git fetch` brought two commits that landed after both of the day's earlier passes:
`723fab1` ("coding") and `2a83469` ("coding"). Both were re-read against source. Every statement
below cites the file and line it was read from. No `.cs` file was modified in this pass.

The two earlier entries for 2026-09-22 are left intact above, including the claims this entry
supersedes, so the drift stays auditable.

### Headline: the project does not compile at HEAD

`723fab1` added `EntityMovement.SetPositionToCheck()` (`EntityMovement.cs:161-165`) containing two
identifiers that do not exist — `Random.range` (the member is `Random.Range`) and
`trasnform.postion`. `Assembly-CSharp` fails with CS0117 and CS0103.

Consequence for this register: **no bug in it can be verified in the Editor until that is fixed.**
Everything recorded in this pass is static analysis. Filed as **BUG-088** (S1, Priority 1), with the
systemic cause filed as **TD-048** — there is no CI, no compiling pre-push hook, and no `.asmdef`,
so nothing in the pipeline compiles this project except a human opening Unity.

Two further defects live in the same five lines and are invisible until it builds:
`Random.Range(0, 100)` binds the `int` overload, so `/100` is integer division and the value is
always `0`, making `Vector3.Lerp` return the entity's own position; and a `Vector3` result is
assigned into a `Vector2` field, against `.claude/rules/gameplay-code.md`.

### Three bugs closed by `2a83469`

| Bug | Evidence |
|---|---|
| **BUG-076** (all four sub-items) | `AbilityDefinition.TryStart()` (`:50-64`) is a single gate that walks `Conditions` once and then compares each `Costs` entry against `abilityContext.Services.Vital.GetCurrentStatValue(cost.statType)` — generic over `StatType`, so Avatar of Light's 50 HP cost is validated where the Mana-only condition could not see it. That closes **(b′)**. `ValidateConditions()`, `TryPayCost()` and the `AbilityHolder.cs:109-112` walk are all deleted, leaving one walk — that closes **(c)**. (a) was already by design, (b) already withdrawn |
| **BUG-077** | `HasEnoughManaCondition.cs` and its `.meta` deleted. All four Paladin ability assets rewritten from a one-entry `Conditions` list to `Conditions: []`. No code path writes runtime values into a committed condition asset any more |
| **BUG-085** | `NotDeadCondition.cs` and its `.meta` deleted. `Assets/Script/System/Abilities/Conditions/` is now empty, so the `[CreateAssetMenu]` hazard — a designer authoring a silent no-op gate — is gone with it |

`Casting()` was also renamed `TryCast()` (`AbilityEffectDefinition.cs:11`), exactly as the owner said
it would be in the review recorded in the entry above.

### Four bugs filed

| ID | Sev | Summary |
|---|---|---|
| **BUG-088** | S1 | `EntityMovement.SetPositionToCheck()` does not compile. Blocks everything. Also: integer division makes the method a no-op once fixed, and `EntityNegativeReciver.cs:27` dereferences a `GetCoreComponent` result unguarded on the live damage path |
| **BUG-089** | S2 | A refused `TryCast()` does not stop the cast. `Casting():79` responds to a refusal by calling `CancelHold()` — which clears `IsHolding`, the very flag `CastInstant():52` tests — so the ability skips its hold check, falls through to `ChangeState(Do)`, and `Execute()` applies the refused effect with no gate and starts the cooldown. Also desynchronises `AbilityInstance.IsHolding` from `AbilityHolder.IsHolding` |
| **BUG-090** | S3 | `Assets/SO/Skill/Conditions/Has Enough Mana Condition.asset` survived the deletion of its script. Its `m_Script` guid `6f896bb258601fc4cb5fc183399618ea` resolves to 0 `.meta` files; `ShootSpirit.asset` and `SpiritBomd.asset` still reference it, and both are reachable from `PlayerTest.prefab` |
| **BUG-091** | S3 | `AbilityDefinition.TryStart()` lost the `if (condition == null) continue` guard the deleted `ValidateConditions()` had — which is what BUG-090's asset would trigger; its `&&` early-return lets a null `Costs` or `Conditions` reach a `foreach`; and `CheckPayCostValid()` (`:67-75`) was added with zero callers, reproducing BUG-082's null-check-after-dereference verbatim in a second file |

BUG-089 and BUG-091 are grouped as **TD-049** — one commit, one review pass closes all three loose
ends.

### Verified unchanged

`LightningController.cs` was touched by `723fab1`, but the change is one trailing-space line; the
overlap query is still three comment lines, so **BUG-072** stands exactly as the owner review scoped
it. Also re-read and unchanged: BUG-063 (`Stat.cs:63-66`), BUG-068 (`GetAbility()` still zero
callers), BUG-071, BUG-073 (plus a second broken reference, see BUG-090), BUG-074, BUG-075
(`OnTriggerEnter(Collider)` still 3D at `:29`), BUG-079, BUG-080, BUG-082, BUG-083, BUG-084
(`find Assets -name "*.asmdef"` still returns 0), BUG-086, BUG-087.

### Documents changed

| Document | Change |
|---|---|
| `production/qa/bugs/BUG-088.md` … `BUG-091.md` | **New.** Full evidence, reproduction, fix sketch and cross-references for each |
| `production/qa/bugs/BUG-076.md` | `## Re-verification — 2026-09-22 (post-fetch …)` appended: the `TryStart()` gate quoted, the three-walk table showing each site deleted, Status → **FIXED**. The owner-review section above it is untouched |
| `production/qa/bugs/BUG-077.md`, `BUG-085.md` | Same heading appended, deletion evidence quoted, Status → **FIXED**; BUG-077 points at BUG-090 and BUG-091 as the residuals it did *not* close |
| `production/qa/bugs/BUG-072.md` | Appended: `723fab1`'s change to the file is whitespace only; scope unchanged; note that it cannot be tested until BUG-088 lands |
| `production/qa/bugs/BUG-082.md` | Appended: the defect now exists at **two** sites, the second added by `2a83469`; BUG-091 recommends deleting rather than fixing the new one |
| `production/qa/bugs/BUG-068.md`, `BUG-073.md`, `BUG-079.md`, `BUG-083.md` | Appended dated re-verification blocks. BUG-073 gains a second missing-script reference; BUG-079 gains a note that BUG-089's fix sketch routes through `Exit()` and so inherits it |
| `CLAUDE.md` | New dated header block at the top, with the compile break first; the previous header demoted to "Previous entry" intact. BUG-076/077/085 rows rewritten as FIXED with their original text preserved after "*Original entry:*"; four new rows BUG-088…BUG-091; the "v2 does not currently work" block and the repo-layout annotations for `AbilityInstance` / `AbilityContext` / `AbilityEffectDefinition` / `Conditions/` updated; demo-checklist item 21 re-ordered behind BUG-088 with the old ordering kept beneath it |
| `docs/tech-debt-register.md` | **TD-048** (no compile gate anywhere — the root cause of BUG-088) and **TD-049** (the three loose ends from `2a83469`) added. Nothing removed |
| `docs/diagrams/ability-system-diagrams.md` | **§1–§8 left unedited.** New **§9** appended: what the two commits did, the build break, a redrawn activation-path diagram showing the single `TryStart()` gate and BUG-089's fall-through, a corrections table against §6/§8, revised strengths/weaknesses, and a revised fix order with BUG-088 as step 0. Navigation block at the top updated to point at §9 first |
| `.claude/rules/weapon-skill-code.md` | Rewritten where it now reads false: `Casting()` → `TryCast()` throughout, the three dispatch methods renamed, the ability-scope-cost warning replaced with the shipped `TryStart()` gate and the instruction to author costs in `AbilityDefinition.Costs` rather than a per-stat condition class, the shared-`ScriptableObject` rule updated to note its example was deleted (while the rule stands for `Stat.modifiers`), and two new warnings added for BUG-089 and BUG-091 |

### Constraint check

`git diff --stat -- '*.cs'` is empty. No `.cs` file was modified, no commit was made, no push. The
working tree contains documentation changes only.

---


## 2026-09-22 (later same day) — Owner review: three findings corrected or retracted

**Cause.** The findings from the re-verification pass earlier the same day were put to the owner.
Three were challenged. Each was re-checked against source and against the live `.asset` files; two
were wrong, one was misattributed. This entry records the corrections and, per the log's own
convention, **nothing from the earlier entry was deleted** — the claim and its rebuttal both stay on
the record.

### What changed

| Finding | Owner's position | Verified outcome |
|---|---|---|
| **BUG-072** — framed as an architectural defect: "`Action<AbilityContext>` cannot carry a hit target, so per-hit state is smuggled through a shared mutable field" | `SummonExecute` is a callback invoked elsewhere, after the caller has set `Services.NegativeReceiver`; the `!= null` guard is deliberate | **Owner right — escalation WITHDRAWN.** Set-then-invoke is the intended contract and is already correctly implemented at `SpawnProjectileBase.cs:31-33` (assign, then `_callback.Invoke` — adjacent and synchronous, so no interleaving window; the earlier "overwritten by whichever object last hit" claim was wrong). The summon invoke wiring exists too: `SpawnSummonBase.cs:12-15` fires the callback from a Unity Animation Event and `LightningController.cs:18` calls `base.Execute()`. **Only the overlap query is missing** — `LightningController.cs:12-19`, still three comment lines. Bug returns to its original scope |
| **BUG-078** — "double spawn from two prefab fields, still open" | Deliberate: the effect controls two objects within one effect | **Owner right — finding RETRACTED, bug CLOSED.** Verified in `Assets/SO/Skill/Paladin/Ability/Consecrate/Effect/Paladin Spawn Consecrat Effect.asset`: `Prefab` → `RuneCircle.prefab` (Cast-phase telegraph; `Execute()` = `//Do nothing`, correct for something that deals no damage), `summonPrefab` → `Lightning.prefab` (Do-phase payload, `SummonExecute`). Telegraph-then-strike. `RuneCircleController` being an empty subclass is consistent. The inverted-return half remains fixed in `73ab8e7`, so the whole bug is closed with no code change |
| **BUG-076 (a)** — "Hold re-charges per-effect cost every dispatch" | Correct flow, intended | **Accepted as by design** (channelled cast). Additionally verified **dormant**: no effect asset in the project carries a serialized `Costs` list — all seven predate the field added in `73ab8e7`, so `TryPayEffectCost()` returns at `statCosts.Count == 0`. Blessed Slash (the only `Hold` ability) pays its 5 Mana once from the ability-scope list |
| **BUG-076 (b)** — "`TryPayEffectCost` never checks affordability" | The check is already in the base class; `Casting()` will be renamed `TryCasting()` to make that legible | **Owner right for the effect-scope list — WITHDRAWN.** `AbilityEffectDefinition.Casting():20` calls `CheckPayCostValid()`, and `AbilityInstance.cs:83-86` pays only inside that gate. **But the ability-scope list has no gate at all**: `TryPayCost()` (`:164-178`) pays unconditionally and `HasEnoughManaCondition` reads `StatType.Mana` only. `Avatar of Light.asset` costs 40 Mana (`statType: 101`) **+ 50 HP** (`statType: 100`); the HP half is validated by nothing, and `Reduction()` clamps at 0. Re-filed as **BUG-076 (b′)**, S2, live |
| **BUG-076 (c)** — "conditions walked 3× per activation" | Not understood | **Stands.** Restated in full in `BUG-076.md` with the complete call chain: `AbilityHolder.cs:109-112` (walk 1), `AbilityInstance.cs:44` (walk 2), `AbilityInstance.cs:166-171` (walk 3 — a verbatim duplicate of walk 2, five lines later in the same call). Each walk writes to a committed `.asset` via BUG-077 ⇒ three asset writes per button press |

### Net effect on the bug list

- **BUG-078: CLOSED** (S1 → non-issue). Half fixed, half retracted.
- **BUG-072: still open, scope reduced** to its original filing — summon target resolution
  unimplemented. The "no v2 effect deals damage" *outcome* is unchanged and still true, but the
  *cause* is now correctly stated as two independent implementation gaps (BUG-075 for projectiles,
  BUG-072 for summons), not one design flaw. Either can be fixed without the other.
- **BUG-076: S1 → S2**, re-scoped a second time. (a) and (b) resolved; (b′) and (c) open.
- **TD-046: WITHDRAWN** — kept in the register as the record of a retracted claim, marked
  *do not action*.
- Two behaviours are now recorded as **confirmed by design** rather than defects: the channelled
  per-effect cost on `Hold` abilities, and the `if (negativeReceiver != null)` guard pattern in the
  spawn effects.

### Documents changed

| Document | Change |
|---|---|
| `production/qa/bugs/BUG-072.md` | `## Owner review — 2026-09-22` appended: escalation withdrawn, set-then-invoke confirmed correct with the reference implementation cited, scope returned to the missing overlap query, revised fix sketch using `OverlapCircleNonAlloc` per `.claude/rules/engine-code.md` |
| `production/qa/bugs/BUG-078.md` | `## Owner review` appended with the asset evidence; **Status → CLOSED**, severity annotated as downgraded-to-non-issue. Follow-up suggested (not filed): `[Header]`/`[Tooltip]` on `Prefab` vs `summonPrefab`, since the two are indistinguishable in the Inspector and that is what caused the misreading |
| `production/qa/bugs/BUG-076.md` | `## Owner review` appended: (a) accepted by design + the per-asset `Costs` audit proving it dormant; (b) withdrawn with the gate chain quoted; (b′) newly stated with the Avatar of Light asset evidence; (c) restated in full with the three-walk call chain. Severity S1 → S2, Priority 1 → 2, Status rewritten |
| `docs/tech-debt-register.md` | TD-046 rewritten as WITHDRAWN/Void with the reason, marked *do not action this entry* |
| `CLAUDE.md` | BUG-072, BUG-075, BUG-076 and BUG-078 rows rewritten. "v2 does not currently work" block rewritten around the two-independent-gaps table instead of the withdrawn architectural claim, and the closed/by-design items listed. Damage-chain ability branch rewritten to state the contract first, then the two gaps |
| `.claude/rules/weapon-skill-code.md` | Four rules rewritten: the spawn-damage contract is now stated positively as set-then-invoke with `SpawnProjectileBase.cs:31-33` named as the shape to copy; `Casting()` documented as a gate (rename to `TryCasting()` noted) with the two-object case called out as legitimate; the shared-`ScriptableObject` rule split so `_context`/`.dir` is described as load-bearing rather than a violation; the cost rule rewritten around one-off vs channelled cost, with the ability-scope affordability gap (b′) as the live warning |
| `docs/diagrams/ability-system-diagrams.md` | **§6 and §7 left unedited.** New **§8 Owner review** appended: corrections table, a corrected damage-path diagram (contract → two independent gaps), a corrected Consecrate sequence diagram (telegraph → payload), and revisions to §7 — weakness #1 withdrawn, #2 softened, #3 split, #5 re-aimed, a new strength added for the one-off/channelled cost distinction, and a revised fix order that no longer blocks on a design decision |
| `production/sprint-status.yaml` | Owner decision (option C) recorded — see below |

### Owner decision — S14-01 / S14-14 (option C)

Both stories cover bugs that are now fixed in code but have no Play Mode confirmation. The owner
chose **not to close them**.

Implemented conservatively: **no `status:` value was changed and no story was closed.** Only the
free-text `blocker:` field on S14-01 (BUG-067) and S14-14 (BUG-069) was filled in, recording what
landed, what evidence exists, and what is still missing. `needs-verification` was deliberately **not**
introduced as a status value — it is not in this file's vocabulary (`backlog` / `ready-for-dev` /
`done`) and `/story-done` would not understand it. A dated comment block explaining all of this was
added under the file's existing "DO NOT edit manually" header. Both stories should be closed through
`/story-done` after the first Play Mode session.

---

## 2026-09-22 — Bug-documentation re-verification pass

**Cause.** Two ability commits landed after the 2026-09-21 doc-sync and were never reviewed:
`73ab8e7` ("coding update flow ability, update logic cost") and `e2cb75e` ("done"), reaching `main`
through the merge `d17fcc5`. `CLAUDE.md` still declared HEAD `15242e6`. Separately, the 2026-09-21
pass had introduced three claims that source does not support. Every tracked bug and the
tech-debt register were re-read against `Assets/Script/` at `d17fcc5`.

**Scope constraint.** Documentation only. **No `.cs` file was changed** —
`git diff --stat` shows zero source files touched. `production/sprint-status.yaml` was deliberately
not edited: closing a story is a production decision for the owner, made through `/story-done`.
Status changes below therefore live in the bug files and `CLAUDE.md`, and the sprint files still
list the old state — see "Handover" at the end.

### Status changes, with evidence

| Bug | Was | Now | Decisive evidence |
|---|---|---|---|
| BUG-069 | Open ("apparently fixed, unverified") | **FIXED** | Full loop present: `AbilityHolder.cs:107` calls `CanStart()`; `AbilityInstance.cs:192` sets; `:23-30` ticks; driven by `AbilityHolder.cs:51-59` from `PlayerBasicState.cs:32` / `PlayerSkillWeaponState.cs:68`; gated at `PlayerInputHandle.cs:254` |
| BUG-067 | Apparently fixed (unverified) | **FIXED** | `ResourceReceiver.cs:17-24` polarity correct; consumer `RecoveryEffectDefinition.cs:13` confirms. Two *unrelated* defects in the same file split out as BUG-080 / BUG-081 rather than closed with it |
| BUG-078 | Open (two defects in one) | **PARTIAL** | Inverted return fixed in `73ab8e7` (`SpawnSummonEffect.cs:20-25`); double spawn still open — `Casting()` spawns `Prefab`, `Apply()` spawns `summonPrefab`, at states `Cast` and `Do` of the same cast |
| BUG-076 | Open (double-charge) | **RE-SCOPED, open** | Original defect fixed by a design change: two disjoint cost lists now exist (`AbilityDefinition.cs:25`, `AbilityEffectDefinition.cs:9`). Three new defects replace it — see below |
| BUG-072 | Open — PLAUSIBLE | **Open — CONFIRMED, widened** | The only writer of `Services.NegativeReceiver` repo-wide is `SpawnProjectileBase.cs:32`, inside the dead 3D callback; `AbilityContext.cs:37-47` never assigns it ⇒ always null ⇒ **no Abilities v2 effect deals any damage** |
| BUG-073 | Open (suspected) | **Open — CONFIRMED by GUID** | `ShootSpirit.asset:12` guid `ac9ac7c011812d042ac992007bc0cf48` resolves to **0** `.meta` files under `Assets/` |
| BUG-079 | Open, S2 | **Open, S3** | Consequence corrected — see below |
| BUG-066 / BUG-070 | Two separate entries | Open, **recorded as one defect, two instances** | Near-verbatim duplicate classes; neither file contains a single `TryGetValue`/`ContainsKey` |

### Claims corrected — all three originated in the 2026-09-21 pass

1. **BUG-063 — the `#if UNITY_EDITOR` guard is not a mitigation.** `CLAUDE.md` framed it as
   narrowing the blast radius. Play Mode in the Editor *is* `UNITY_EDITOR`, which is precisely the
   leak path the capitalised comment at `Stat.cs:49-62` warns about; the guard only protects the
   player build, where `.asset` files are read-only anyway. The framing is why a three-line deletion
   has been carried 29+ cycles. Corrected in `CLAUDE.md` (Known Bugs row, tree annotation) and
   `production/qa/bugs/BUG-063.md`.
2. **BUG-079 — "castable once per scene load" is false.** `AbilityHolder.StartHold()` (`:143-148`)
   forces `ChangeState(AbilityState.Start)` on every fresh press via `PlayerInputHandle.cs:254-256`,
   so abilities do re-cast. What remains is a layering defect: the instance cannot reset itself.
   Severity lowered S2 → S3. Also propagated: `docs/diagrams/ability-system-diagrams.md` lines 226
   and 248 still carry the wrong claim and are flagged in the handover below.
3. **BUG-074 — there is no work-in-progress fix.** An earlier review recorded an uncommitted,
   ineffective edit in `Assets/Script/Utility/Utility.cs` (a dead `finalValue` variable). Verified
   2026-09-22: `git status --porcelain` is empty and `ModifierStatsCalculate` (`Utility.cs:274-304`)
   contains no such variable. Anyone picking BUG-074 up starts from the untouched version.

### Eight defects filed for the first time

| ID | Severity | Summary |
|---|---|---|
| BUG-080 | S1 | `ResourceReceiver.vitalStatsComponent` (`:5`) is assigned only inside `ReceverModifierGroup()` (`:13`); `Recovery()`/`Reduction()`/`BuffDebuffForDuration()` dereference it unresolved. Live on the item-pickup path via `RecoveryEffectDefinition.cs:13`. `TakeDamage()` escapes only via a shadowing local |
| BUG-081 | S2 | Two `INegativeReceiver` implementers with byte-identical bodies, **both on `PlayerTest.prefab`** (verified by GUID). Player-side twin of the closed BUG-053. Secondary: `EnemyPrefab.prefab` carries the *player* `NegativeReciver` and no `EntityNegativeReciver` |
| BUG-082 | S3 | `if (Costs.Count == 0 \|\| Costs == null)` — `\|\|` short-circuits, so the null test is unreachable. Latent only because Unity's serializer materialises empty lists; becomes live the moment code-constructed instances appear (i.e. in tests) |
| BUG-083 | S2 | `AbilityContext.HoldTime` / `.HoldRatio` are plain fields snapshotted by `BuildContext()` *before* `StartHold()` zeroes the counter, and never rebuilt — always `0f`. Masked because all four Paladin abilities have `MaxHoldTime = 0` |
| BUG-084 | S2 | **No `.asmdef` anywhere under `Assets/`**, and `tests/` sits outside the Unity asset tree. Test Runner can neither compile nor discover a test. Precondition of TD-014, not a symptom |
| BUG-085 | S3 | `NotDeadCondition.IsMet()` body commented out, always `true`. Dormant (no asset references it) but carries `[CreateAssetMenu]` |
| BUG-086 | S2 | `PlayerDeathState.LogicUpdate()` never consumes `Status` and never changes state ⇒ `ON_PLAYER_DEATH` emitted every frame. Violates the durable-`Status` contract in `manager-event-code.md`. Was only a parenthetical inside BUG-065 |
| BUG-087 | S1 | `ON_PLAYER_DEATH` has zero subscribers; no `GameManager` exists; player `VitalStatsComponent` has no reset path; `ON_REALOAD_GAME` has 0 emitters and 0 subscribers. **Death is a permanent hard lock.** Closes the open half of Bug #6 / S10-08 |

BUG-076's three replacement defects are recorded inside `BUG-076.md` rather than as new IDs, since
they occupy the same code and the same open design question: (a) Hold abilities re-charge
per-effect cost every dispatch (`AbilityInstance.cs:57-65`, live on Blessed Slash); (b)
`TryPayEffectCost()` (`:155-162`) never checks affordability and `Reduction` clamps at 0, so an
unaffordable cast drains to zero instead of being refused; (c) conditions are evaluated 3× per
activation (`AbilityHolder.cs:109-112`, `AbilityInstance.cs:44`, `:166-171`), each evaluation
writing to a committed asset via BUG-077.

### Documents changed

| Document | Change |
|---|---|
| `CLAUDE.md` | New header entry (HEAD corrected `15242e6` → `d17fcc5`). Known Bugs preamble rewritten with this pass's status changes and the three corrections. All fifteen BUG-063…BUG-079 rows rewritten from source; eight rows added (BUG-080…BUG-087). "v2 is not healthy" block rewritten around BUG-072 as the subsuming defect. Damage-chain section: player branch rewritten (death is a dead end), new "Player ability hits anything — ❌ DOES NOT WORK AT ALL" branch added. Repo-layout annotations updated on 12 lines. Demo checklist: items 6, 20 and 21 rewritten; items 23 (test pipeline) and 24 (death recovery path) added |
| `production/qa/bugs/BUG-063…079.md` | Dated `## Re-verification — 2026-09-22` section appended to each, with file:line evidence. Status lines updated on 067, 069, 072, 074, 076, 078, 079, 066, 070. `**Priority**` field added to BUG-075…079, which had none and appeared in no sprint or carry-over list. BUG-079 severity S2 → S3 |
| `production/qa/bugs/BUG-080…087.md` | New, in the existing BUG-*.md format |
| `docs/tech-debt-register.md` | Header date and counts updated (43 → 47 items). TD-014 re-scoped as blocked by TD-044. TD-044 (asmdef/test pipeline), TD-045 (death recovery path), TD-046 (`NegativeReceiver` shared mutable state / callback signature), TD-047 (duplicate player `INegativeReceiver`) added |
| `.claude/rules/weapon-skill-code.md` | "Rules that exist because of open bugs" block rewritten. Corrected a wrong bug ID (`SpawnEffectBase._context`/`.dir` is BUG-078, not BUG-079). Four rules added: no v2 effect deals damage today; `Casting()` and `Apply()` run at different phases of one cast; the 3D trigger form compiles clean with no warning; `HoldTime`/`HoldRatio` are always `0f`; cost now comes from two disjoint lists |
| `.claude/rules/test-standards.md` | Warning added under "Unity Test Naming": the `tests/EditMode/` / `tests/PlayMode/` paths this file prescribes do not compile, and no test can be written to them until BUG-084 / TD-044 is resolved. This file has prescribed non-working paths since it was written |
| `docs/diagrams/ability-system-diagrams.md` | **Additive only — §1–§5 untouched.** Pointer block added under the existing defect list, then **§6 Current Version** (five stale §1–§3 annotations tabulated, plus three new source-drawn diagrams: the two-list cost model, the damage dead end, the `SpawnSummonEffect` double spawn) and **§7 System assessment** (7 strengths / 11 weaknesses, ranked, with a recommended fix order that puts the two undefined contracts first) appended |
| `docs/CHANGELOG-DOCS.md` | This entry |

### Handover — deliberately left for the owner

- **`production/sprint-status.yaml` not touched.** Its own header requires `/story-done`. BUG-067
  and BUG-069 are now FIXED and their stories should be closed there by the owner.
- **`production/sprints/sprint-15.md` line 39** still reads "Open bugs: 13 (BUG-052, 063-066,
  070-074 + verify-only 067/069)". BUG-075…079 were never added to any sprint or carry-over list,
  and BUG-080…087 are new. Sprint files are dated records and were not rewritten.
- **`docs/diagrams/ability-system-diagrams.md`** — **addressed 2026-09-22 by addition, not rewrite**,
  per owner instruction. Lines 200, 207, 226 and 248 still carry the pre-`73ab8e7` cost model and the
  wrong BUG-079 consequence; they are deliberately left in place and are instead tabulated as stale
  in the new §6.1, so the drift remains auditable. §6 supplies the current diagrams and §7 the
  strengths/weaknesses assessment. Regenerating §1–§3 in place is still open, and is a larger job.
- **Estimates now known to be wrong**: sprint-14 S14-13 and the sprint-15 test story estimate
  "write the first EditMode test" at 0.3d; BUG-084 must land first, and it entails splitting
  `Assembly-CSharp`.

---

## 2026-09-21 — Abilities v2 re-synchronisation + Paladin direction renumber

**Cause.** Branch `origin/feature/fix-player-control`, HEAD `15242e6`. Between `8295539`
("update spawn effect", 2026-09-17) and `7cceda2` ("done paladin consecrate ability", 2026-09-19)
the Abilities v2 **effect layer and runtime layer were replaced wholesale**, and the Paladin sprite
set was imported. No documentation entry accompanied either. Verified by direct source read of all
23 files under `Assets/Script/System/Abilities/`, plus image inspection of the Paladin sprite sheets
against `paladin_8dir_controller` state names.

### What the code did that no document recorded

| Change | Evidence |
|---|---|
| `SkillState` enum renamed **`AbilityState`**, `None` member removed | `AbilityDefinition.cs:74-80` |
| `DamageInFrontEffect`, `LungeForwardEffect`, `ShootObjectEffect`, `PlayDebugLogEffect` deleted | `Effects/` now holds seven different files |
| `AbilityRuntimeHelpers`, `SpiritOrbProjectile`, `SpiritDoTBehaviour` deleted | `Runtime/` restructured into `BaseController/` + `SpawnMono/` |
| New `SpawnEffectBase` → `SpawnProjectileEffect` / `SpawnSummonEffect` | `Effects/SpawnEffectBase.cs` |
| New `StatsEffectBase` → `RecoveryReductionStatsEffect` / `RecoveryReductionPerTimeForDuration`; new `BuffDebuffStatsForDuration` | `Effects/StatEffectBase.cs` (⚠️ file/class name mismatch) |
| New runtime layer `SpawnMono` → `SpawnProjectileBase` / `SpawnSummonBase` → `SlashProjectile`, `LightningController`, `RuneCircleController` | `Runtime/` |
| **New and never documented anywhere**: `IAbilityServices` / `AbilityServices` — the service bundle carried on every `AbilityContext` | `AbilityContext.cs:20-47` |
| `AbilityEffectDefinition` gained `SubConditions`, `SubEffects` and the `Casting()` hook | `AbilityEffectDefinition.cs` |
| v2's live content became the **Paladin ability set** (Consecrate, Blessed Slash, Blessing, Avatar of Light) | `Assets/SO/Skill/Paladin/Ability/` |
| Paladin sprite sets imported numbered `dir0 = N`, counter-clockwise — against `DirectionResolver` | `Assets/Sprite/Paladin/Sprites/` |

### Documents changed

| Document | Change |
|---|---|
| `CLAUDE.md` | Header entry for this pass. `System/Abilities/` subtree re-listed file-by-file against source. `AbilityHolder` row corrected (`AbilityState`, `AbilityServices`, `abilityBindings` overwritten in `Start()`). "Skill / Ability — TWO frameworks" section rewritten: lifecycle box, effect-hierarchy table, runtime layer, live asset paths, open-defect list. `SO/Skill/` inventory corrected. Thirteen rows added to Known Bugs (BUG-067…BUG-079). Demo checklist items 21 and 22 added. |
| `docs/diagrams/ability-system-diagrams.md` | **§1–§3 rewritten from source.** The 2026-09-11 pass corrected this file's status banner but left the diagrams describing the deleted Spirit Orb prototype. New architecture flowchart, activation sequence and lifecycle state diagram, each annotated with the open bug that breaks that step. §4–§5 kept and explicitly marked HISTORICAL. Deletion table added at the top. |
| `.claude/rules/weapon-skill-code.md` | Abilities v2 section: `SkillState` → `AbilityState`; effect-hierarchy table added; `abilityBindings` authoring location corrected; `IAbilityServices` documented; three new prohibitions added (no per-cast state on an effect/condition SO, no new `Casting()` override until its contract is defined, 2D trigger callbacks are `OnTriggerEnter2D`). |
| `design/gdd/skill-ability-system.md` | v1/v2 comparison table: lifecycle corrected to `AbilityState: Start → Cast → Do → Exit`; live v2 assets updated to the Paladin set. Cross-system table: the "`PlayerInputHandle` provides a `SkillState` enum" row was wrong and is corrected. |
| `design/gdd/animation-system.md` | **New section "8-direction index convention"** under Formulas — the `DirectionResolver` index table (0 = down-left, clockwise), the asset classes it binds, and the Paladin renumber precedent. This convention had never been written down, which is how the Paladin set was imported against a different one. Header re-verification banner updated. |
| `production/qa/bugs/BUG-075.md` … `BUG-079.md` | New. See below. |

### Bugs filed this pass

| ID | Severity | Summary |
|---|---|---|
| BUG-075 | S1 | `SpawnProjectileBase` uses the 3D `OnTriggerEnter(Collider)` signature in a 2D project — Unity never dispatches it, so every projectile ability deals zero damage |
| BUG-076 | S1 | Ability cost paid in `TryActivateInstant()` **and** again per effect inside `Casting()` — a 3-effect ability charges its full `Costs` four times per cast |
| BUG-077 | S1 | `HasEnoughManaCondition.currentMana` / `.costMana` are public serialized fields written on every `IsMet()` — runtime state committed into a `.asset`, same class as BUG-063 |
| BUG-078 | S1 | `SpawnSummonEffect.Casting()` returns its gate inverted (charges cost on the failure path) and spawns the summon in both `Casting()` and `Apply()` |
| BUG-079 | S2 | `AbilityInstance.Exit()` body is commented out — `State` never returns to `Start`, so an `Active` ability is castable once per scene load. Also collects three minor findings not separately filed |

Per the owner's decision on 2026-09-21, **no ability code was changed in this pass** — BUG-076 and
BUG-078 in particular cannot be fixed without first defining what `AbilityEffectDefinition.Casting()`
is for, and v2 still has no GDD (BUG-052, demo-checklist item 18).

### Asset change: Paladin direction renumber

`DirectionResolver.Calculate()` yields index 0 = **down-left (SW)**, running clockwise. The Knight
sprite sets already follow it (verified by rendering `KnightSnS/Block/Direction_0..7.png`). The
imported Paladin `cast` and `idle` sets were numbered `dir0 = N`, counter-clockwise — confirmed both
by rendering the sheets and by the compass suffixes on `paladin_8dir_controller`'s own state names
(`a1_cast_dir0_n`, `a1_cast_dir1_nw`, …).

Renumbered with the involution `0↔3, 1↔2, 4↔7, 5↔6`, applied to:

- 32 PNGs (`paladin_{cast,idle}_dirN.png` + `_NormalMap.png`) and their `.meta`
- the sprite names and `nameFileIdTable` keys inside every `.png.meta`
- 16 generated `.anim` and 16 `.mat` (files, and `m_Name` inside)
- `paladin_8dir_controller`: 16 state names **and** the 16 `direction` parameter thresholds that
  select them, so state and condition moved together

Unity references are GUID-based, so content followed each file through the rename and nothing broke.
Confirmed afterwards: `paladin_cast_dir0.png` is the SW sheet, `a1_cast_dir0_sw` is reached by
`direction == 0`, and the three pre-existing `Knight_Consecrate_State*_dir 0` clips (which already
pointed at the SW sheet by GUID) still resolve to it.

### Asset change: Consecrate directional clips

`Paladin Consecrate.overrideController` previously overrode only three clips — `dir 0` of
`Knight_SpinAttack_State1/2/3`. Generated the remaining 21 clips (`dir 1`…`dir 7` × 3 states) by
remapping each frame of the `dir 0` clips onto the corresponding sheet by **sprite ordinal**, so the
frame ranges, sample rate and Animation Events are identical across all eight directions:

| State | Sheet frames | Animation events |
|---|---|---|
| State1 (wind-up) | 0–30 | `AnimationStart` @0, `AnimationOnAction` @0.4167, `AnimationFinishTrigger` @0.625 |
| State2 (hold loop) | 29–30 | inherited from `dir 0` |
| State3 (release) | 31–88 | inherited from `dir 0` |

All 24 entries are now wired in the override controller.

---

## 2026-09-11 — Full documentation/code re-synchronisation

**Scope:** all living documentation. **Verified against:** HEAD `6d6a8e4`, branch
`origin/feature/synce-doc-and-code`. **Code changed:** none — this was a documentation-only pass.

### Root causes

Four structural code changes drove almost every edit below.

| ID | Change | Commit(s) | Date |
|---|---|---|---|
| **R1** | Seven directories moved under `Assets/Script/System/` — `Enemy/`, `Pathfinding/`, `Poolable/`→`PoolableService/`, `StatSystem/`, `Skill_Ability/`, `Item/`, `LifetimeScope/`. 77 renames | `1c0742e` (also `853fe39`, `f7acffc`, `0686619`) | 2026-09-03 |
| **R2** | **VContainer 1.19.0** dependency injection adopted; `GameLifetimeScope` composition root | `aa4e620` | 2026-08-22 |
| **R3** | `StatsSO.cs` deleted → `BaseStatsSO` + `EnemyStatSO`; `StatPointAllocator` added | `b0512f4`, `1c0742e` | 2026-08-28 / 09-03 |
| **R4** | `prototypes/skill-enhance-abilities/Scripts/` (17 files) promoted into `Assets/Script/System/Abilities/`; `AbilityHolder` rewritten to drive it | `9b8d40f`, `5c7afba` | 2026-09-09 |

Secondary causes: `INegativeReceiver.TakeDamage` changed `int` → `float` (`f3f5f08`); `EventID`
grew 20 → 23; a boss system was added and reverted (`ffe1976` → `4421fdc`, `ff67f4d`).

---

### Updated

#### `CLAUDE.md` — rewritten (563 → 743 lines)
**Cause:** R1, R2, R3, R4 + three false bug statuses.
**Changed:**
- Repository Layout rebuilt against the real tree (R1), with a Was→Is migration table so older
  documents remain readable.
- New sections: **Dependency Injection** (R2), **Player stat / vitals split**, **Skill / Ability —
  TWO frameworks now coexist** (R4), **Orphan directories**.
- `EventID` corrected 20 → **23** (`ON_RESET_STATS_UI_SESSION`, `ON_DROP_ITEM`, `ON_COLLECT_ITEM`),
  with the count history extended rather than replaced.
- Damage Chain rewritten: `float` signature, BUG-053 path now working, Defense mitigation.
- Package list: added VContainer 1.19.0, Timeline 1.7.7; noted DOTween is an Asset Store import.
- **Three bug-status corrections** — each was actively misleading:
  - `BUG-053` listed OPEN → it is **FIXED** (`f3f5f08`; `production/qa/bugs/BUG-053.md` had said so
    since 2026-09-06 — this table never followed the bug file).
  - `BUG-044` claimed the fix "properly stops `PlayerMovement`" → **false**;
    `PlayerDeathState.Enter()` only calls `base.Enter()`. Split out as `BUG-065`.
  - `NEW-4` claimed `Stat.modifiers` is no longer serialized → **regressed**; `Stat.cs:63-65`
    re-serializes it behind `#if UNITY_EDITOR`. Now `BUG-063`.
- Resolved a self-contradiction: the header claimed `EntityFindTarget` was deleted while the
  wiring section required it. The file exists — the header was wrong.
- Demo checklist gained items 18–20 (ability-framework decision, BUG-063, zero tests).
**Evidence:** `EventManager.cs`; `INegativeReciver.cs:7`; `Stat.cs:63-65`; `PlayerDeathState.cs:10-13`;
`EntityNegativeReciver.cs:18-25`; `Player.cs:58`; `GameLifetimeScope.cs`.

#### `README.md` — links repaired
**Cause:** R1.
**Changed:** 7 deep links to `Assets/Script/{Pathfinding,StatSystem}/…` were **404 on GitHub** after
the move — on the repository's public-facing page. All repointed and each verified to resolve on
disk. Project-structure block rebuilt; size corrected `~10k lines / 174 files` → `~11k / 194`.
**Note:** the commit that broke these links was `f7acffc`, *"chore(portfolio): prep repo for clean
public release"*.

#### `memory/project_state.md` — rewritten
**Cause:** R1–R4; three sprints stale (last updated 2026-08-21).
**Changed:** every row of the open-bug table was wrong — 7 bugs listed as open are fixed, 2 pointed
at deleted files (`EntityStatsSO.cs`, `EntityWeaponMelee.cs`), and it stated `prototypes/` "does not
exist". Rewritten with a structural-changes table, a key-API-changes table, and a re-prioritised
fix list.

#### `.claude/rules/gameplay-code.md`
**Cause:** `f3f5f08` (signature), R2.
**Changed:** `TakeDamage(int …)` → **`float`** — this rule would have produced non-compiling code
for three sprints. Added a **Dependency Resolution** section distinguishing the three mechanisms
(Core hub / DI / Inspector), and a max-vs-current stat ownership rule.

#### `.claude/rules/weapon-skill-code.md`
**Cause:** `f3f5f08`, BUG-043/046 closure, R4.
**Changed:** `int` → `float`; removed guidance referencing the deleted `EntityWeaponMelee`;
**"Skill Lifecycle Contract (ActivateSkill)" rewritten as a two-framework section** — the player
no longer uses the lifecycle this rule described.

#### `.claude/rules/engine-code.md`
**Cause:** R2.
**Changed:** "Components discover siblings via `Core.GetCoreComponent<T>()`, **never direct field
references**" was contradicted by shipped DI. Amended to give DI and the Core hub disjoint scopes,
and to constrain `GameLifetimeScope` from becoming a hub.

#### `.claude/rules/ai-code.md`
**Cause:** Bug #7 closed long ago; BUG-053/066; R3.
**Changed:** removed the stale instruction *"`EntityDeathState` must extend `EntityState` — fix the
base class bug before shipping"*. Added a **Damage Reception** section (one `INegativeReceiver` per
enemy, Defense in `DamageCalculate()`, the BUG-066 key-guard warning) and corrected the stat source
to `EnemyStatSO` / `EntityStatsHandler`.

#### `.claude/rules/scriptableobject-data.md`
**Cause:** R3 + the Sprint 8–10 rename.
**Changed:** `WeaponMeleeStats` → `MeleeWeaponStats`; `StatsCharacter` base → **`BaseStatsSO`**.
Added a **Runtime vs Authored Serialization** table — the distinction behind the project's only
committed data-corruption incident, currently regressed as BUG-063.

#### `.claude/rules/manager-event-code.md`
**Cause:** `EventID` 20 → 23.
**Changed:** added an **EventID Count** section. Flagged explicitly that
`ON_PLAYER_TAKE_DAMAGE` — which this rule instructs the health bar to bind to — **has never
existed**, and pointed to `EntityUIController` as the working push-based pattern.

#### `.claude/rules/ui-code.md`
**Cause:** R2, R3, the stat/vitals split.
**Changed:** added a table of where player values actually come from (current → `VitalStatsComponent`,
max → `StatHandler`), noted `PlayerData.currentHealth` is not a valid source (Bug #6), documented
the two deliberately-unregistered DI components, and recorded that `StatsUIController` still writes
gameplay state — a standing `ui-code.md` violation that survived the DI refactor.

#### `.claude/rules/prototype-code.md`
**Cause:** R4.
**Changed:** added a **Promotion Log**. The 2026-09-09 promotion bypassed this file's own Promotion
Rules (rewrite to production standards), and the residual prototype-grade code is named.

#### `.claude/rules/map-code.md`
**Cause:** verification only.
**Changed:** a re-verification stamp. **All five bugs it lists were checked in source and are still
open exactly as described** — this file needed no correction.

#### `docs/engine-reference/unity/VERSION.md`
**Cause:** R2.
**Changed:** package table corrected from `manifest.json` — **VContainer 1.19.0 was missing
entirely**, plus Timeline, Test Framework, uGUI and a note that DOTween is vendored and invisible to
the manifest. The **"UI Toolkit … not recommended for runtime UI"** guidance was **reversed**: runtime
UI Toolkit had already shipped (`UIController.cs`), so the file was contradicting the codebase every
time it was read. Added a VContainer API-notes section and a git-URL supply-chain risk note.

#### `docs/architecture/adr-0001-statsystem-dual-data-structure.md`
**Cause:** R3.
**Changed:** header warning + a **2026-09-11 Amendment**. The class the ADR is written about
(`StatsSO`) no longer exists; `BaseStatsSO` keeps the same API surface, so **the decision itself
stands unmodified** — this was a rename, not a redesign. The amendment also records that the
data-corruption hazard the ADR warns about has **regressed** (BUG-063), and that the source comment
saying "NEVER add `[SerializeField]`" did not prevent it.

#### `docs/architecture/adr-0002-enemymanager-singleton-exception.md`
**Cause:** R1, R2.
**Changed:** second amendment — path move, and a note that **ADR-0004 supersedes this ADR's
premise**. A singleton is no longer *necessary* for cross-system access; the exception is retained
only because `EntityMovement` reads it on runtime-spawned enemies, the case DI handles worst.

#### `design/gdd/stat-system.md`
**Cause:** R3.
**Changed:** all `StatsSO` references → `BaseStatsSO` + a scope banner. **Rules, formulas and
acceptance criteria are unchanged and still valid** — only the type name and path moved. Banner also
flags what this GDD does *not* yet cover: `StatPointAllocator`, the max/current split, and BUG-063.

#### `design/gdd/skill-ability-system.md`
**Cause:** R4.
**Changed:** scope banner. This GDD is **not obsolete but now partial** — it documents v1
(`ActivateSkill`), which still drives weapons and enemies, while the **player** moved to v2. A
comparison table names which is which. v2 is deliberately *not* retro-designed here: one system =
one GDD (`.claude/rules/design-docs.md`), and no ADR has decided whether the two converge.

#### `design/gdd/character-system.md`
**Cause:** the Sprint 12 stat/vitals split; `f3f5f08`.
**Changed:** banner mapping the three-component split for both player and enemy, the `float`
signature, Defense mitigation, the ninth player state, and confirmation that death/restart remains
`[PLANNED]`.

#### `design/gdd/enemy-spawn-system.md`
**Cause:** BUG-033/BUG-053 closure, R1, R2.
**Changed:** status banner. **The algorithm did not drift** — `GetSpawnSet()` still matches. Four
status corrections (BUG-033 fixed, epic unblocked, path moved, DI-resolved); the three genuinely
open items are restated.

#### `design/gdd/weapons-system.md`, `attack-speed-system.md`, `map-system.md`, `animation-system.md`
**Cause:** R1, R3, `f3f5f08`; verification.
**Changed:** verification banners. `map-system.md` and `animation-system.md` had **no drift** — all
five map bugs re-confirmed open, and the `StatusAnimation` handoff unchanged. `weapons-system.md`
and `attack-speed-system.md` took `float` / `BaseStatsSO` / path corrections only.

#### `design/gdd/systems-index.md`
**Cause:** R2, R4, the Item system.
**Changed:** **three systems added** (24 DI, 25 Abilities v2, 26 Item) — all shipped without ever
entering the index. "Built without a GDD" corrected 4 → **7**. "Next Systems to Design" re-ordered:
the ability-framework decision is now the top blocker.

#### `docs/diagrams/ability-system-diagrams.md` — banner **inverted**
**Cause:** R4.
**Changed:** the banner said *"These diagrams do NOT describe the ability system the game runs"* and
*"ships no SO assets … nothing can instantiate it"*. **Both became false on 2026-09-09.** This is now
the most accurate description of the player's live ability system. Rewritten with the three deltas
that occurred during promotion (`AbilitySystem` absorbed into `AbilityHolder`;
`ShootSpiritOrbEffect` → `ShootObjectEffect`; `DamageInFrontEffect.Apply()` uncommented).
**Evidence that it is wired:** `Assets/SO/Skill/ShootSpirit/{ShootSpirit,SpiritBomd}.asset`,
`Assets/SO/Skill/Conditions/New Has Enough Mana Condition.asset`.

#### `prototypes/skill-enhance-abilities/README.md`
**Cause:** R4.
**Changed:** Result `[IN PROGRESS — never finished, never wired]` → **`[VALIDATED — promoted 2026-09-09]`**.
Original body retained unedited as the historical record; a **Promotion (2026-09-09)** section audits
the promotion against this README's own four "if you pick this back up" steps — steps 1–3 done,
step 4 (rewrite to production standards) **partial**.

#### `docs/tech-debt-register.md`
**Cause:** R1–R4 + source re-verification of every entry.
**Changed:** 39 → 43 items.
- **TD-011 → VOID** — carried at Critical/Priority-1 for 14 weeks against `EntityStatsSO.cs`,
  a file deleted in `b0512f4`.
- **TD-036 → CLOSED** — every sub-item verified fixed; two residuals re-filed as TD-041/TD-042
  rather than left buried.
- **TD-038 → REOPENED (regression)** — now BUG-063.
- **TD-005 re-scoped**, **TD-003 widened**, **TD-031** corrected (`prototypes/` does exist).
- **New:** TD-040 (two ability frameworks, no ADR), TD-041 (BUG-066 key guard),
  TD-042 (BUG-043 residual), TD-043 (orphan `.meta`-only directories).

#### `docs/registry/architecture.yaml`
**Cause:** R2 + a pre-existing syntax defect.
**Changed:** added the `vcontainer_dependency_injection` decision (ADR-0004). Corrected the
forbidden-patterns note: the allocating-`OverlapCircle` entry pointed at the **deleted**
`EntityWeaponMelee.cs`; `GameObject.Find` re-confirmed as the only remaining instance
(`FollowPlayer.cs:24`); the UI-mutates-state violation re-confirmed — it survived the DI refactor
and now writes through `IPlayerStatService` (`StatsUIController.cs:110,158,172`).
**Also fixed a pre-existing defect:** this file **did not parse as YAML** — an unquoted `:` inside a
`scope:` value, present at HEAD before this pass. A machine-readable registry that no parser can
read is worse than none. Converted to a block scalar; the file now parses.

#### `production/epics/index.md`
**Cause:** TD-036 closure; R2, R4, Item system.
**Changed:** the enemy-spawn epic's end-to-end blocker is recorded as gone. "Systems with code but
no epic" grew 4 → **7**; all three new entries shipped through un-storied commits.

#### `production/qa/bugs/BUG-052.md`
**Cause:** R2 (partial closure), R4, Item system.
**Changed:** one subsystem left the list (DI, now ADR-0004); three joined (Item, Abilities v2, UI).
The gap **grew rather than closed**.

#### `.claude/agent-memory/lead-programmer/MEMORY.md`
**Cause:** R1–R4.
**Changed:** re-synced. It had gone stale within three sprints of its last rewrite — noted in the
header as a recurring property of shortcut files, with a seven-line delta summary.

#### `docs/skill-reference.md`
**Cause:** file count drift.
**Changed:** 79 → **80** skills; **3 → 4 ADRs**, with ADR-0004's 20-day retroactive gap named.

#### `.claude/skills/module-quality-audit/SKILL.md`
**Cause:** R1.
**Changed:** path note. This skill's path-based scans would **silently return nothing** for moved
modules, reporting them as unchanged rather than as missing.

#### `Assets/Animation/DUPLICATE_NOTE.md`
**Cause:** `.claude/rules/language-reporting.md` (stored `.md` must be English) + re-verification.
**Changed:** rewritten in English with the original Vietnamese preserved verbatim below. Counts
**re-verified: 155 `.anim` in each folder**, still unresolved after 13 sprints. Added why this cannot
be settled from the repository alone (`.anim` files do not record their referencing Animator).

---

### Created

#### `docs/architecture/adr-0004-vcontainer-dependency-injection.md` — **new**
**Cause:** R2 — the largest architectural change in the project shipped with no ADR.
**Contents:** context (three competing resolution mechanisms, and the singleton pressure that
produced `LevelManager`/TD-023), the decision as implemented, five alternatives with reasons for
rejection — including `ScriptableObject`-as-service-locator, rejected because this project has
already been burned by SO persistence (TD-038/BUG-063) — consequences, the git-URL supply-chain
risk, a source-verified checklist, and three open questions.
**Status:** Accepted (retroactively documented). The 20-day gap between `aa4e620` and this ADR is
recorded in the ADR itself as the reason it exists.

#### `docs/CHANGELOG-DOCS.md` — this file.

---

### Archived (moved, not deleted)

| From | To | Reason |
|---|---|---|
| `docs/reviews/melee-combat-review.md` | `docs/archive/reviews/melee-combat-review.md` | Every class it reviews is deleted or rewritten. `WeaponMelee.Attack()` (section A) became `MeleeWeapon.OnActivate()` and is now the project's *reference* implementation — the opposite of the review's finding. `EntityWeaponMelee.cs` (section B) was **deleted**. Its header verdict ("both melee directions deal no reliable damage", "0 of 16 issues fixed") was true at HEAD `7995066` on 2026-07-30 and is false today. Content untouched; banner names each superseding document. |
| `docs/adoption-plan-2026-05-19.md` | `docs/archive/adoption-plan-2026-05-19.md` | A one-time `/adopt` onboarding checklist from project start, 13 sprints ago. Living in `docs/` invited reading it as an outstanding to-do list. Content untouched; banner points to the four documents that now carry its concerns. |

`docs/reviews/` is now empty and was removed. `docs/archive/` was created by this pass.

---

### Historical snapshots — banner added, content untouched

These are accurate records of their own date. They were **not rewritten**; each received a
`📅 HISTORICAL SNAPSHOT` banner because each was being read as a current assessment.

| Document | Why the banner |
|---|---|
| `docs/architecture/architecture-review-2026-07-13.md` | Its CONCERNS verdict and "8 GDDs · 3 ADRs" counts are from 2026-07-13; there are now 4 ADRs, ADR-0001's subject class is deleted, and three further systems shipped undocumented. |
| `production/qa/module-health-2026-08.md` | **Every** CRITICAL verdict in its scorecard has since been resolved; its "skill-ability-system HEALTHY" verdict is also stale after R4. |
| `design/balance/combat-balance-2026-07-07.md` | Two of its four cited source files no longer exist at those paths (`StatsSO.cs` deleted; `DerivedStatFormula.cs` moved). The **numbers were not re-derived** in this pass and may still be valid — the banner says so explicitly and names what to re-validate against. |

---

### Deliberately NOT changed

| Set | Count | Reason |
|---|---|---|
| `production/sprints/`, `production/retros/`, `production/qa/bug-triage-*`, `production/qa/playtests/`, `production/session-state/`, `design/gdd/gdd-cross-review-*` | ~59 | **Point-in-time records.** Their stale paths and bug statuses were correct on their own date. Rewriting them would destroy the project's history — the opposite of this pass's purpose. |
| `.claude/docs/*.md` (9 files), `.claude/docs/templates/` (38), `.claude/docs/hooks-reference/` (6) | 53 | Checked for drift (stale paths, `StatsSO`, `EntityWeaponMelee`, `TakeDamage(int`): **zero hits**. Generic tooling and blank templates, not project documentation. |
| `.claude/skills/` (79 of 80), `.claude/agents/` (26) | 105 | Engine- and project-agnostic. Only `module-quality-audit/SKILL.md` hardcoded project paths. |
| All source code | 194 `.cs` | **This pass was documentation-only by instruction.** Every defect found in code — BUG-063, BUG-065, BUG-066, BUG-064 sub-item 7, the orphan directories, the `AbilityHolder` null dereference — was **documented, not fixed**, and is tracked in `CLAUDE.md`, `docs/tech-debt-register.md` and `production/qa/bugs/`. |

---

### Findings that need a decision (documentation cannot resolve these)

1. **BUG-063** — `Stat.modifiers` re-serialized (`Stat.cs:63-65`). A one-line fix, no technical
   blocker, carried 24+ triage cycles, and it can corrupt committed `.asset` files. It has already
   happened once.
2. **Two ability frameworks, no ADR** (TD-040) — v1 drives weapons/enemies, v2 drives the player.
   Until this is decided, `design/gdd/skill-ability-system.md` cannot be made authoritative and
   Abilities v2 cannot be designed at all.
3. **The documentation gap is systemic, not incidental.** Four structural changes shipped across
   three sprints with zero doc updates, in a project that runs weekly wrap-ups and monthly module
   audits. Both rituals read documents rather than diffing the tree, so neither could see R1–R4.
   Consider a pre-commit or weekly check that diffs `Assets/Script/`'s directory list against
   `CLAUDE.md`'s Repository Layout.
