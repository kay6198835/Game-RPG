# EditMode Tests — Location Note

Unity only compiles C# that lives under `Assets/` or `Packages/`. Anything in this
repo-root `tests/` folder is invisible to the Unity compiler and to the Test Runner.

This project also has **no assembly definition files** — all gameplay code is compiled
into the predefined `Assembly-CSharp` assembly. An `.asmdef` test assembly cannot
reference a predefined assembly, so tests in their own asmdef would not be able to see
`StatsSO`, `Stat`, `GameConstants`, etc.

The only working location is therefore a folder named `Editor`, whose contents Unity
compiles into `Assembly-CSharp-Editor` — that assembly already references both
`Assembly-CSharp` and the NUnit / Test Framework assemblies.

| Suite | Path |
|---|---|
| StatsSO (EditMode) | `Assets/Editor/Tests/StatSystem/StatsSOTests.cs` |
| Shared test factory | `Assets/Editor/Tests/StatSystem/StatsProfileFactory.cs` |

Run them via **Window ▸ General ▸ Test Runner ▸ EditMode**.

All tests are expected to pass. The suite pins the StatSystem calculation contract:

```
AdjustedValue = BaseValue + LevelUpValue                    (excludes EquipmentValue)
Value         = (AdjustedValue + EquipmentValue + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult)
BonusValue    = Value − AdjustedValue
```

`EquipmentValue` is an *input* tier written only by `StatsSO.RecalculateDerived()` (the
contribution propagated from primary stats through `DerivedStatFormula`); primary stats
always keep it at 0. The bonus shown to the player is always `BonusValue`, computed by
subtraction and never stored.

If assembly definitions are ever introduced for `Assets/Script/`, these suites should be
moved into a proper `Tests.EditMode` asmdef and this note removed.
