using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// AbilityDefinition.TryStart (the single condition + cost gate since 2a83469) and GetCostValues.
// Added 2026-11-20 (coverage review). Synthetic definitions only; destroyed in TearDown.
public class AbilityDefinitionTests
{
    private readonly List<Object> _created = new();

    private sealed class FakeCaster : IAbilityOwner
    {
        public float Hp;
        public float Mana;
        public Transform Transform => null;
        public float GetCurrentStatValue(VitalStatType statType) => statType == VitalStatType.HP ? Hp : statType == VitalStatType.Mana ? Mana : 0f;
        public float GetCurrentStatValue(StatType statType) => 0f;
        public void PayCost(VitalStatType statType, float amount) { }
        public Vector2 DirectorForward() => Vector2.right;
        public int DirectorForwardIndex() => 0;
        public Vector2 TargetPosition() => Vector2.zero;
    }

    [TearDown]
    public void TearDown() => TestSupport.DestroyAll(_created);

    private AbilityDefinition NewAbility(params (VitalStatType type, float value)[] costs)
    {
        AbilityDefinition ability = ScriptableObject.CreateInstance<AbilityDefinition>();
        _created.Add(ability);
        foreach (var (type, value) in costs) ability.Costs.Add(new StatCost { statType = type, value = value });
        return ability;
    }

    private static AbilityContext Context(float hp, float mana) => new AbilityContext { Caster = new FakeCaster { Hp = hp, Mana = mana } };

    [Test]
    public void TryStart_NoCostNoCondition_AlwaysStarts()
    {
        Assert.IsTrue(NewAbility().TryStart(Context(0f, 0f)));
    }

    [Test]
    public void TryStart_ManaCostAffordable_Starts()
    {
        Assert.IsTrue(NewAbility((VitalStatType.Mana, 15f)).TryStart(Context(100f, 15f)), "exactly enough Mana was refused");
    }

    [Test]
    public void TryStart_ManaCostTooHigh_IsRefused()
    {
        Assert.IsFalse(NewAbility((VitalStatType.Mana, 15f)).TryStart(Context(100f, 14.9f)));
    }

    [Test]
    public void TryStart_HpCostTooHigh_IsRefused()
    {
        // Avatar of Light pays HP; the gate is generic over the stat type (BUG-076 fix).
        Assert.IsFalse(NewAbility((VitalStatType.HP, 40f), (VitalStatType.Mana, 50f)).TryStart(Context(39f, 100f)));
    }

    [Test]
    public void TryStart_OneOfTwoCostsUnaffordable_IsRefused()
    {
        Assert.IsFalse(NewAbility((VitalStatType.HP, 40f), (VitalStatType.Mana, 50f)).TryStart(Context(100f, 49f)));
        Assert.IsTrue(NewAbility((VitalStatType.HP, 40f), (VitalStatType.Mana, 50f)).TryStart(Context(100f, 50f)));
    }

    [Test]
    public void TryStart_NullCostOrConditionEntry_IsSkipped()
    {
        AbilityDefinition ability = NewAbility((VitalStatType.Mana, 5f));
        ability.Costs.Add(null);
        ability.Conditions.Add(null);

        Assert.DoesNotThrow(() => ability.TryStart(Context(100f, 100f)));
        Assert.IsTrue(ability.TryStart(Context(100f, 100f)));
    }

    [Test]
    public void GetCostValues_ReturnsTheCostOfThatStatOrZero()
    {
        AbilityDefinition ability = NewAbility((VitalStatType.HP, 40f), (VitalStatType.Mana, 50f));

        Assert.AreEqual(40f, ability.GetCostValues(VitalStatType.HP), 0.0001f);
        Assert.AreEqual(50f, ability.GetCostValues(VitalStatType.Mana), 0.0001f);
        Assert.AreEqual(0f, NewAbility().GetCostValues(VitalStatType.Mana), 0.0001f);
    }

    [Test]
    public void NewDefinition_HasNonNullListsAndAPositiveCooldown()
    {
        AbilityDefinition ability = NewAbility();

        Assert.IsNotNull(ability.Costs);
        Assert.IsNotNull(ability.Conditions);
        Assert.IsNotNull(ability.Effects);
        Assert.Greater(ability.Cooldown, 0f);
    }
}
