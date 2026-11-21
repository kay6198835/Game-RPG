# Inbox proposals - run 20261121-2050

Sprint `origin/sprint-23` at `bcedaebb`. Rows written to `production/qa/bug-inbox.md` on `sprint-23`.

| Note | Date | Source | Sev guess | Where | What | Evidence tier |
|---|---|---|---|---|---|---|
| NOTE-20261121-1 | 2026-11-21 | qa-auto | S1 | `Assets/Script/Character/Base/NegativeReceiverBase.cs:9`; `Assets/Script/Interface/INegativeReciver.cs:5-14`; lookups `Assets/Script/Weapons/MeleeWeapon/MeleeWeapon.cs:31`, `Assets/Script/Weapons/RangeWeapon/RangeWeapon.cs:75`, `Assets/Script/Character/Player/Projectile/Projectile.cs:46`, ability effects | QA-ENT-08 FAIL for all 10 enemy roles ("player HP unchanged after 3-4 attacks: player has no hurtbox") and QA-VIT-06 / 13 / 14 NRE on `QAPlayerRig.Hurtbox` (null), 1 of 1 runs at `bcedaebb` and 1 of 1 at `76a60d2f` (run `20261121-1931`). Cause by reading: `8005d27e` changed `NegativeReceiverBase<TCore>` to implement only the new `INegativeReceiver<TCore>` (`InvicisibleHurtbox`), not `INegativeReceiver` (`TakeDamage`). Every weapon, projectile and ability finds targets with `TryGetComponent(out INegativeReceiver)`, so neither the player nor any enemy can be damaged by weapons or abilities (only `TrainingDummy` / `QAProbeTarget` still match). Fix direction: `NegativeReceiverBase<TCore> : ..., INegativeReceiver, INegativeReceiver<TCore>`. New AUTO case QA-VIT-15 guards it. Report `qa-automation/reports/20261121-2050/report.md` | RUNTIME |

Other FAILs of this run are mapped, not new notes: QA-PLY-06.* -> BUG-130, QA-ENT-11 -> BUG-133,
QA-STATIC-09 / QA-UNIT ItemEffectDataTests -> BUG-132, QA-STATIC-13 -> BUG-122, QA-STATIC-01 new capture
scenes -> BUG-128, QA-STATIC-12 -> static baseline (owner OQ-10).