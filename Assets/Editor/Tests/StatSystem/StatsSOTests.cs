#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static Game.Tests.StatSystem.StatsProfileFactory;

namespace Game.Tests.StatSystem
{
    /// <summary>
    /// Bộ test EditMode cho StatsSO — chạy bằng Window ▸ General ▸ Test Runner ▸ EditMode.
    ///
    /// Quy ước:
    /// - Test KHÔNG có category: chốt hành vi hiện tại, phải XANH.
    /// - [Category("KnownBug")]: mô tả hành vi ĐÚNG theo tài liệu, hiện đang ĐỎ vì code còn lỗi.
    ///   Sửa xong bug thì test tự xanh, không cần sửa test. Muốn chạy bỏ qua nhóm này:
    ///   Test Runner ▸ ô filter ▸ gõ tên test, hoặc CLI -testCategory "!KnownBug".
    ///
    /// Mỗi test tự dựng StatsSO riêng (StatsProfileFactory) và huỷ trong TearDown —
    /// không bao giờ chạm vào asset StatsProfile thật trong project.
    /// </summary>
    [TestFixture]
    public class StatsSOTests
    {
        private const float TOLERANCE = 0.0001f;

        [TearDown]
        public void TearDown() => DestroyCreated();

        private static void AssertFloat(float actual, float expected, string message = null)
            => Assert.That(actual, Is.EqualTo(expected).Within(TOLERANCE), message);

        // =====================================================================
        // 1. Khởi tạo / Get
        // =====================================================================

        [Test]
        public void Get_EveryStatTypeInEnum_ReturnsNonNullStat()
        {
            StatsSO profile = Create();

            foreach (StatType type in Enum.GetValues(typeof(StatType)))
                Assert.That(profile.Get(type), Is.Not.Null, $"{type} không được seed.");
        }

        [Test]
        public void Get_AuthoredStat_KeepsAuthoredBaseValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 42f)));

            AssertFloat(profile.Get(StatType.STR).BaseValue, 42f);
        }

        [Test]
        public void Get_StatTypeMissingFromList_IsSeededWithZero()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f)));

            Stat seeded = profile.Get(StatType.LUK);

            Assert.That(seeded, Is.Not.Null);
            AssertFloat(seeded.BaseValue, 0f);
        }

        [Test]
        public void Get_CalledTwice_ReturnsSameInstance()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f)));

            Assert.That(profile.Get(StatType.STR), Is.SameAs(profile.Get(StatType.STR)));
        }

        [Test]
        public void Get_UndefinedStatType_ReturnsNull()
        {
            StatsSO profile = Create();

            Assert.That(profile.Get((StatType)9999), Is.Null);
        }

        [Test]
        public void GetStat_UndefinedStatType_CreatesStatInsteadOfReturningNull()
        {
            // Get() và GetStat() KHÔNG đối xứng: Get trả null, GetStat gọi GetOrCreate.
            StatsSO profile = Create();

            Stat created = profile.GetStat((StatType)9999);

            Assert.That(created, Is.Not.Null);
            AssertFloat(created.BaseValue, 0f);
        }

        [Test]
        public void EnsureInitialized_DuplicateStatType_KeepsFirstEntry()
        {
            // Console sẽ có warning "... khai báo trùng tại index 1 — giữ bản đầu, gỡ bản này."
            StatsSO profile = Create(stats: StatList(
                NewStat(StatType.STR, 10f),
                NewStat(StatType.STR, 99f)));

            AssertFloat(profile.Get(StatType.STR).BaseValue, 10f, "Phải giữ bản author đầu tiên, không phải bản trùng.");
        }

        [Test]
        public void EnsureInitialized_NullEntryInList_IsDropped()
        {
            StatsSO profile = Create(stats: StatList(null, NewStat(StatType.STR, 10f), null));

            AssertFloat(profile.Get(StatType.STR).BaseValue, 10f);
            Assert.That(profile.Get(StatType.DEX), Is.Not.Null);
        }

        [Test]
        public void StatTypeName_CoversEveryStatType()
        {
            // StatsViewDTO index thẳng vào dictionary này -> thiếu key nào là KeyNotFoundException lúc runtime.
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
                Assert.That(GameConstants.StatTypeName.ContainsKey(type), Is.True,
                    $"GameConstants.StatTypeName thiếu '{type}'.");
        }

        // =====================================================================
        // 2. SeedAllStats
        // =====================================================================

        [Test]
        public void SeedAllStats_AllStats_ResetToZero()
        {
            StatsSO profile = Create(stats: StatList(
                NewStat(StatType.STR, 100f, 20f),
                NewStat(StatType.VIT, 50f)));

            profile.SeedAllStats();

            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                Stat stat = profile.Get(type);
                AssertFloat(stat.BaseValue, 0f, $"{type}.BaseValue");
                AssertFloat(stat.LevelUpValue, 0f, $"{type}.LevelUpValue");
            }
        }

        [Test]
        public void SeedAllStats_ExistingModifiers_AreDropped()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 25f)));

            profile.SeedAllStats();

            Assert.That(profile.Get(StatType.STR).Modifiers, Is.Empty);
            AssertFloat(profile.Get(StatType.STR).Value, 0f);
        }

        [Test]
        public void SeedAllStats_ViewDTOs_AreCleared()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.Get(StatType.STR);

            profile.SeedAllStats();

            Assert.That(profile.statViewDTOs, Is.Empty);
        }

        // =====================================================================
        // 3. Level và StatUnusedBonus
        // =====================================================================

        [Test]
        public void Level_SetToZero_ClampsToOne()
        {
            StatsSO profile = Create(level: 1);

            profile.Level = 0;

            Assert.That(profile.Level, Is.EqualTo(1));
        }

        [Test]
        public void Level_SetNegative_ClampsToOne()
        {
            StatsSO profile = Create(level: 5);

            profile.Level = -20;

            Assert.That(profile.Level, Is.EqualTo(1));
        }

        [Test]
        public void Level_Increase_UpdatesStatUnusedBonus()
        {
            // Điểm chưa tiêu = level - Σ(LevelUpValue của primary) - 1 (level 1 không có điểm).
            StatsSO profile = Create(level: 1, stats: StatList(NewStat(StatType.STR, 10f)));

            profile.Level = 10;

            Assert.That(profile.StatUnusedBonus, Is.EqualTo(9));
        }

        [Test]
        public void Level_WithAllocatedPoints_SubtractsThemFromBonus()
        {
            StatsSO profile = Create(level: 1, stats: StatList(
                NewStat(StatType.STR, 10f, 5f),
                NewStat(StatType.VIT, 10f, 3f)));

            profile.Level = 10;

            Assert.That(profile.StatUnusedBonus, Is.EqualTo(1)); // 10 - (5+3) - 1
        }

        [Test]
        public void Level_WhenAllocatedPointsExceedLevel_ClampsBonusToZero()
        {
            StatsSO profile = Create(level: 1, stats: StatList(NewStat(StatType.STR, 10f, 20f)));

            profile.Level = 5;

            Assert.That(profile.StatUnusedBonus, Is.EqualTo(0));
        }

        [Test]
        public void Level_SetSameValue_RaisesNoStatChanged()
        {
            StatsSO profile = Create(level: 4, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 100f) });

            int changes = 0;
            profile.OnStatChanged += _ => changes++;
            profile.Level = 4;

            Assert.That(changes, Is.EqualTo(0), "Set Level bằng giá trị cũ phải thoát sớm, không recalc.");
        }

        [Test]
        public void Level_SetDifferentValue_RaisesStatChangedForDerivedStats()
        {
            StatsSO profile = Create(level: 4, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 100f, perLevel: 5f) });
            profile.Get(StatType.STR);

            int maxHpChanges = 0;
            profile.OnStatChanged += type => { if (type == StatType.MaxHP) maxHpChanges++; };
            profile.Level = 9;

            Assert.That(maxHpChanges, Is.GreaterThan(0));
        }

        [Test]
        public void Level_Increase_RecalculatesDerivedBaseValue()
        {
            // isDevMode = true để bỏ qua guard "giá trị không đổi" trong RecalculateDerived.
            StatsSO profile = Create(level: 1, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 0f, perLevel: 10f) });

            profile.Level = 5;

            AssertFloat(profile.Get(StatType.MaxHP).BaseValue, 50f);
        }

        [Test]
        public void StatUnusedBonus_SetBelowOne_ClampsToOne()
        {
            // Setter kẹp sàn 1 (khớp [Min(1)] trên field), trong khi CalculateStatUnusedBonus
            // ghi thẳng vào field nên VẪN có thể ra 0. Test này chốt sự bất đối xứng đó.
            StatsSO profile = Create(level: 1);
            profile.StatUnusedBonus = 7;

            profile.StatUnusedBonus = 0;

            Assert.That(profile.StatUnusedBonus, Is.EqualTo(1));
        }

        [Test]
        public void StatUnusedBonus_SetPositiveValue_IsStored()
        {
            StatsSO profile = Create(level: 1);

            profile.StatUnusedBonus = 12;

            Assert.That(profile.StatUnusedBonus, Is.EqualTo(12));
        }

        // =====================================================================
        // 4. AddPrimaryPoint
        // =====================================================================

        [Test]
        public void AddPrimaryPoint_PrimaryStat_IncreasesLevelUpValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f, 2f)));

            profile.AddPrimaryPoint(StatType.STR, 5f);

            AssertFloat(profile.Get(StatType.STR).LevelUpValue, 7f);
            AssertFloat(profile.Get(StatType.STR).AdjustedValue, 17f);
        }

        [Test]
        public void AddPrimaryPoint_DefaultAmount_AddsOnePoint()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f)));

            profile.AddPrimaryPoint(StatType.STR);

            AssertFloat(profile.Get(StatType.STR).LevelUpValue, 1f);
        }

        [Test]
        public void AddPrimaryPoint_NegativeAmount_DecreasesLevelUpValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f, 5f)));

            profile.AddPrimaryPoint(StatType.STR, -3f);

            AssertFloat(profile.Get(StatType.STR).LevelUpValue, 2f);
        }

        [Test]
        public void AddPrimaryPoint_DerivedStat_LogsWarningAndLeavesStatUnchanged()
        {
            // Console sẽ có warning "[StatsSO] MaxHP không phải primary stat."
            StatsSO profile = Create(stats: StatList(NewStat(StatType.MaxHP, 100f)));

            profile.AddPrimaryPoint(StatType.MaxHP, 5f);

            AssertFloat(profile.Get(StatType.MaxHP).LevelUpValue, 0f);
        }

        [Test]
        public void AddPrimaryPoint_UpdatesViewDTO()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.Get(StatType.STR);

            profile.AddPrimaryPoint(StatType.STR, 5f);

            StatsViewDTO dto = profile.statViewDTOs[StatType.STR];
            AssertFloat(dto.AdjustedValue, 105f);
            AssertFloat(dto.FinalValue, 105f);
            AssertFloat(dto.BonusValue, 0f);
        }

        [Test]
        public void AddPrimaryPoint_RaisesNoOnStatChanged()
        {
            // Hành vi hiện tại: gọi thẳng AfterChanged, KHÔNG bắn OnStatChanged
            // -> UI subscribe qua OnStatChanged sẽ không thấy việc cộng điểm.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.Get(StatType.STR);

            int changes = 0;
            profile.OnStatChanged += _ => changes++;
            profile.AddPrimaryPoint(StatType.STR, 5f);

            Assert.That(changes, Is.EqualTo(0));
        }

        [Test]
        public void AddPrimaryPoint_DoesNotRefreshStatUnusedBonus()
        {
            // Điểm chưa tiêu chỉ được tính lại khi Level đổi, không phải khi tiêu điểm.
            StatsSO profile = Create(level: 1, stats: StatList(NewStat(StatType.STR, 10f)));
            profile.Level = 10;

            profile.AddPrimaryPoint(StatType.STR, 3f);

            Assert.That(profile.StatUnusedBonus, Is.EqualTo(9));
        }

        [Test]
        [Category("KnownBug")]
        public void AddPrimaryPoint_WithDerivedFormula_RecalculatesDerivedStat()
        {
            // ĐÚNG: cộng điểm primary phải kéo theo derived tính lại.
            // HIỆN TẠI: AddPrimaryPoint chỉ gọi AfterChanged, không gọi RecalculateDerived
            // -> MaxHP giữ nguyên giá trị cũ cho tới lần đổi Level kế tiếp.
            StatsSO profile = Create(level: 1, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[]
                {
                    Formula(StatType.MaxHP, baseConstant: 0f, perLevel: 0f,
                        Contribution(StatType.STR, 2f)),
                });
            float before = profile.Get(StatType.MaxHP).AdjustedValue;

            profile.AddPrimaryPoint(StatType.STR, 10f);

            Assert.That(profile.Get(StatType.MaxHP).AdjustedValue, Is.Not.EqualTo(before).Within(TOLERANCE));
        }

        // =====================================================================
        // 5. AddModifiersFromSource
        // =====================================================================

        [Test]
        public void AddModifiersFromSource_FlatModifier_IncreasesFinalValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 10f)));

            AssertFloat(profile.Get(StatType.STR).Value, 110f);
        }

        [Test]
        public void AddModifiersFromSource_NullList_LeavesStatsUnchanged()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            Assert.DoesNotThrow(() => profile.AddModifiersFromSource(new object(), null));
            AssertFloat(profile.Get(StatType.STR).Value, 100f);
        }

        [Test]
        public void AddModifiersFromSource_ListContainingNull_SkipsNullEntry()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(),
                ModifierList(null, NewModifier(StatType.STR, 10f), null));

            AssertFloat(profile.Get(StatType.STR).Value, 110f);
            Assert.That(profile.Get(StatType.STR).Modifiers.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddModifiersFromSource_DoesNotMutateAuthoredModifier()
        {
            // Group SO là dữ liệu dùng chung: phải clone qua WithSource(), không đóng dấu lên bản gốc.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            StatModifier authored = NewModifier(StatType.STR, 10f);
            object source = new object();

            profile.AddModifiersFromSource(source, ModifierList(authored));

            Assert.That(authored.Source, Is.Null);
            Assert.That(profile.Get(StatType.STR).Modifiers[0], Is.Not.SameAs(authored));
        }

        [Test]
        public void AddModifiersFromSource_StoresCloneStampedWithSource()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            object source = new object();

            profile.AddModifiersFromSource(source, ModifierList(NewModifier(StatType.STR, 10f, ModifierType.PercentAdd)));

            StatModifier stored = profile.Get(StatType.STR).Modifiers[0];
            Assert.That(stored.Source, Is.SameAs(source));
            Assert.That(stored.TargetStat, Is.EqualTo(StatType.STR));
            Assert.That(stored.Type, Is.EqualTo(ModifierType.PercentAdd));
            AssertFloat(stored.Value, 10f);
        }

        [Test]
        public void AddModifiersFromSource_AppliesEachModifierToItsOwnTargetStat()
        {
            StatsSO profile = Create(stats: StatList(
                NewStat(StatType.STR, 100f),
                NewStat(StatType.DEX, 50f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.DEX, 5f)));

            AssertFloat(profile.Get(StatType.STR).Value, 110f);
            AssertFloat(profile.Get(StatType.DEX).Value, 55f);
        }

        [Test]
        public void AddModifiersFromSource_RaisesOnStatChangedOncePerModifier()
        {
            StatsSO profile = Create(stats: StatList(
                NewStat(StatType.STR, 100f),
                NewStat(StatType.DEX, 50f)));
            profile.Get(StatType.STR);

            int changes = 0;
            profile.OnStatChanged += _ => changes++;
            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.STR, 5f),
                NewModifier(StatType.DEX, 5f)));

            Assert.That(changes, Is.EqualTo(3));
        }

        [Test]
        public void AddModifiersFromSource_SameListTwice_StacksBothCopies()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            object source = new object();
            List<StatModifier> group = ModifierList(NewModifier(StatType.STR, 10f));

            profile.AddModifiersFromSource(source, group);
            profile.AddModifiersFromSource(source, group);

            AssertFloat(profile.Get(StatType.STR).Value, 120f);
            Assert.That(profile.Get(StatType.STR).Modifiers.Count, Is.EqualTo(2));
        }

        [Test]
        public void AddModifiersFromSource_NullSource_IsAccepted()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(null, ModifierList(NewModifier(StatType.STR, 10f)));

            AssertFloat(profile.Get(StatType.STR).Value, 110f);
            Assert.That(profile.Get(StatType.STR).Modifiers[0].Source, Is.Null);
        }

        [Test]
        public void AddModifiersFromSource_PrimaryTarget_TriggersDerivedRecalculation()
        {
            StatsSO profile = Create(devMode: true,
                stats: StatList(NewStat(StatType.STR, 100f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 10f) });
            profile.Get(StatType.STR);

            int maxHpChanges = 0;
            profile.OnStatChanged += type => { if (type == StatType.MaxHP) maxHpChanges++; };
            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 10f)));

            Assert.That(maxHpChanges, Is.EqualTo(1));
        }

        [Test]
        public void AddModifiersFromSource_DerivedTargetOnly_DoesNotTriggerDerivedRecalculation()
        {
            StatsSO profile = Create(devMode: true,
                stats: StatList(NewStat(StatType.STR, 100f), NewStat(StatType.Defense, 5f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 10f) });
            profile.Get(StatType.STR);

            int maxHpChanges = 0;
            profile.OnStatChanged += type => { if (type == StatType.MaxHP) maxHpChanges++; };
            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.Defense, 10f)));

            Assert.That(maxHpChanges, Is.EqualTo(0));
        }

        // =====================================================================
        // 6. RemoveModifiersFromSource
        // =====================================================================

        [Test]
        public void RemoveModifiersFromSource_RestoresOriginalValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            object source = new object();
            profile.AddModifiersFromSource(source, ModifierList(NewModifier(StatType.STR, 10f)));

            profile.RemoveModifiersFromSource(source);

            AssertFloat(profile.Get(StatType.STR).Value, 100f);
            Assert.That(profile.Get(StatType.STR).Modifiers, Is.Empty);
        }

        [Test]
        public void RemoveModifiersFromSource_OtherSource_IsKept()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            object weapon = new object();
            object buff = new object();
            profile.AddModifiersFromSource(weapon, ModifierList(NewModifier(StatType.STR, 5f)));
            profile.AddModifiersFromSource(buff, ModifierList(NewModifier(StatType.STR, 7f)));

            profile.RemoveModifiersFromSource(weapon);

            AssertFloat(profile.Get(StatType.STR).Value, 107f);
        }

        [Test]
        public void RemoveModifiersFromSource_RemovesAcrossAllStats()
        {
            StatsSO profile = Create(stats: StatList(
                NewStat(StatType.STR, 100f),
                NewStat(StatType.DEX, 50f),
                NewStat(StatType.VIT, 20f)));
            object source = new object();
            profile.AddModifiersFromSource(source, ModifierList(
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.DEX, 10f),
                NewModifier(StatType.VIT, 10f)));

            profile.RemoveModifiersFromSource(source);

            AssertFloat(profile.Get(StatType.STR).Value, 100f);
            AssertFloat(profile.Get(StatType.DEX).Value, 50f);
            AssertFloat(profile.Get(StatType.VIT).Value, 20f);
        }

        [Test]
        public void RemoveModifiersFromSource_MultipleModifiersFromSameSource_AllRemoved()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            object source = new object();
            profile.AddModifiersFromSource(source, ModifierList(
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.STR, 0.5f, ModifierType.PercentAdd),
                NewModifier(StatType.STR, 0.2f, ModifierType.PercentMult)));

            profile.RemoveModifiersFromSource(source);

            AssertFloat(profile.Get(StatType.STR).Value, 100f);
        }

        [Test]
        public void RemoveModifiersFromSource_UnknownSource_RaisesNoEvent()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 10f)));

            int changes = 0;
            profile.OnStatChanged += _ => changes++;
            profile.RemoveModifiersFromSource(new object());

            Assert.That(changes, Is.EqualTo(0));
            AssertFloat(profile.Get(StatType.STR).Value, 110f);
        }

        [Test]
        public void RemoveModifiersFromSource_EqualModifiersDifferentSources_RemovesOnlyMatchingOne()
        {
            // So khớp bằng ReferenceEquals -> hai modifier giá trị y hệt nhưng khác nguồn
            // phải tháo được độc lập.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            object swordA = new object();
            object swordB = new object();
            profile.AddModifiersFromSource(swordA, ModifierList(NewModifier(StatType.STR, 10f)));
            profile.AddModifiersFromSource(swordB, ModifierList(NewModifier(StatType.STR, 10f)));

            profile.RemoveModifiersFromSource(swordA);

            AssertFloat(profile.Get(StatType.STR).Value, 110f);
            Assert.That(profile.Get(StatType.STR).Modifiers.Count, Is.EqualTo(1));
            Assert.That(profile.Get(StatType.STR).Modifiers[0].Source, Is.SameAs(swordB));
        }

        [Test]
        public void RemoveModifiersFromSource_NullSource_RemovesModifiersAppliedWithNullSource()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.AddModifiersFromSource(null, ModifierList(NewModifier(StatType.STR, 10f)));

            profile.RemoveModifiersFromSource(null);

            AssertFloat(profile.Get(StatType.STR).Value, 100f);
        }

        [Test]
        public void RemoveModifiersFromSource_PrimaryTarget_TriggersDerivedRecalculation()
        {
            StatsSO profile = Create(devMode: true,
                stats: StatList(NewStat(StatType.STR, 100f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 10f) });
            object source = new object();
            profile.AddModifiersFromSource(source, ModifierList(NewModifier(StatType.STR, 10f)));

            int maxHpChanges = 0;
            profile.OnStatChanged += type => { if (type == StatType.MaxHP) maxHpChanges++; };
            profile.RemoveModifiersFromSource(source);

            Assert.That(maxHpChanges, Is.EqualTo(1));
        }

        [Test]
        public void RemoveModifiersFromSource_OnEmptyProfile_DoesNotThrow()
        {
            StatsSO profile = Create();

            Assert.DoesNotThrow(() => profile.RemoveModifiersFromSource(new object()));
        }

        // =====================================================================
        // 7. Công thức modifier (Flat → PercentAdd → PercentMult)
        // =====================================================================

        [Test]
        public void Value_FlatModifiers_AreSummed()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.STR, 15f)));

            AssertFloat(profile.Get(StatType.STR).Value, 125f);
        }

        [Test]
        public void Value_PercentAddModifiers_AreSummedThenMultipliedOnce()
        {
            // 100 × (1 + 0.10 + 0.20) = 130 — KHÔNG phải 100 × 1.1 × 1.2.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 0.10f, ModifierType.PercentAdd),
                NewModifier(StatType.STR, 0.20f, ModifierType.PercentAdd)));

            AssertFloat(profile.Get(StatType.STR).Value, 130f);
        }

        [Test]
        public void Value_PercentMultModifiers_AreMultipliedSeparately()
        {
            // 100 × 1.1 × 1.2 = 132.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 0.10f, ModifierType.PercentMult),
                NewModifier(StatType.STR, 0.20f, ModifierType.PercentMult)));

            AssertFloat(profile.Get(StatType.STR).Value, 132f);
        }

        [Test]
        public void Value_MixedModifiers_AppliedInFlatThenPercentAddThenPercentMultOrder()
        {
            // (100 + 10) × (1 + 0.10 + 0.20) × (1 + 0.50) = 214.5
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.STR, 0.10f, ModifierType.PercentAdd),
                NewModifier(StatType.STR, 0.20f, ModifierType.PercentAdd),
                NewModifier(StatType.STR, 0.50f, ModifierType.PercentMult)));

            AssertFloat(profile.Get(StatType.STR).Value, 214.5f);
        }

        [Test]
        public void Value_ModifiersAddedOutOfOrder_ProduceSameResult()
        {
            // Stat.AddModifier sort theo Order -> thứ tự gắn không ảnh hưởng kết quả.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 0.50f, ModifierType.PercentMult),
                NewModifier(StatType.STR, 0.20f, ModifierType.PercentAdd),
                NewModifier(StatType.STR, 10f),
                NewModifier(StatType.STR, 0.10f, ModifierType.PercentAdd)));

            AssertFloat(profile.Get(StatType.STR).Value, 214.5f);
        }

        [Test]
        public void Value_ZeroValueModifier_LeavesValueUnchanged()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(
                NewModifier(StatType.STR, 0f),
                NewModifier(StatType.STR, 0f, ModifierType.PercentAdd)));

            AssertFloat(profile.Get(StatType.STR).Value, 100f);
        }

        [Test]
        public void Value_NegativeFlatModifier_ReducesValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, -30f)));

            AssertFloat(profile.Get(StatType.STR).Value, 70f);
        }

        [Test]
        public void Value_NoModifiers_EqualsAdjustedValue()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f, 20f)));

            AssertFloat(profile.Get(StatType.STR).Value, 120f);
        }

        // =====================================================================
        // 8. Accessor và StatsViewDTO
        // =====================================================================

        [Test]
        public void GetStatValue_ReturnsAdjustedValue_IgnoringModifiers()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f, 5f)));
            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 100f)));

            AssertFloat(profile.GetStatValue(StatType.STR), 15f);
        }

        [Test]
        public void GetStat_ReturnsSameInstanceAsGet()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f)));

            Assert.That(profile.GetStat(StatType.STR), Is.SameAs(profile.Get(StatType.STR)));
        }

        [Test]
        public void GetStatEquipValue_PrimaryStatWithFlatModifier_ReturnsBonusPart()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 7f)));

            AssertFloat(profile.GetStatEquipValue(StatType.STR), 7f);
        }

        [Test]
        public void GetStatEquipValue_NoModifiers_ReturnsZero()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.Get(StatType.STR);

            AssertFloat(profile.GetStatEquipValue(StatType.STR), 0f);
        }

        [Test]
        public void EquipmentValue_SetOnPrimaryStat_IsOverwrittenByRecalculation()
        {
            // EquipmentValue của primary là giá trị DẪN XUẤT (Value - AdjustedValue), không phải
            // tầng author được: gán tay xong bị CalculateFinalValue ghi đè ngay.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));

            profile.Get(StatType.STR).EquipmentValue = 50f;

            AssertFloat(profile.GetStatEquipValue(StatType.STR), 0f);
        }

        [Test]
        public void ViewDTO_AfterModifier_ReportsAdjustedFinalAndBonus()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 100f)));
            profile.Get(StatType.STR);

            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.STR, 10f)));

            StatsViewDTO dto = profile.statViewDTOs[StatType.STR];
            AssertFloat(dto.AdjustedValue, 100f);
            AssertFloat(dto.FinalValue, 110f);
            AssertFloat(dto.BonusValue, 10f);
        }

        [Test]
        public void ViewDTO_HasDisplayNameFromGameConstants()
        {
            StatsSO profile = Create(stats: StatList(NewStat(StatType.MaxHP, 100f)));
            profile.Get(StatType.MaxHP);

            Assert.That(profile.statViewDTOs[StatType.MaxHP].Name, Is.EqualTo(GameConstants.StatTypeName[StatType.MaxHP]));
        }

        [Test]
        public void Modifiers_OnFreshStat_IsEmptyNotNull()
        {
            StatsSO profile = Create();

            Assert.That(profile.Get(StatType.STR).Modifiers, Is.Not.Null.And.Empty);
        }

        // =====================================================================
        // 9. Derived stat / DerivedStatFormula
        // =====================================================================

        [Test]
        public void RecalculateDerived_NullFormulaArray_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
            {
                StatsSO profile = Create(stats: StatList(NewStat(StatType.STR, 10f)), formulas: null);
                profile.Get(StatType.MaxHP);
            });
        }

        [Test]
        public void RecalculateDerived_NullFormulaEntry_IsSkipped()
        {
            StatsSO profile = Create(level: 1, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { null, Formula(StatType.MaxHP, baseConstant: 30f) });

            AssertFloat(profile.Get(StatType.MaxHP).BaseValue, 30f);
        }

        [Test]
        public void RecalculateDerived_LevelZeroOnAsset_ClampsToLevelOne()
        {
            // Asset cũ có thể lưu level 0; perLevel × 0 sẽ xoá sạch bảng cân bằng nếu không kẹp sàn.
            StatsSO profile = Create(level: 0, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 0f, perLevel: 10f) });

            AssertFloat(profile.Get(StatType.MaxHP).BaseValue, 10f);
        }

        [Test]
        [Category("KnownBug")]
        public void RecalculateDerived_WithDevModeOff_UpdatesDerivedStats()
        {
            // ĐÚNG: derived stat phải được công thức ghi đè kể cả khi isDevMode = false.
            // HIỆN TẠI: guard `Mathf.Approximately(target.Value, newBase.Value)` luôn so 0 với 0
            // (Stat.Value của mọi stat non-primary trả về field finalValue chưa bao giờ được gán)
            // -> `continue` cho MỌI công thức, RecalculateDerived thành no-op hoàn toàn khi tắt dev mode.
            StatsSO profile = Create(level: 1, devMode: false,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 250f) });

            AssertFloat(profile.Get(StatType.MaxHP).BaseValue, 250f);
        }

        [Test]
        [Category("KnownBug")]
        public void Get_DerivedStat_ValueReturnsCalculatedFinalValue()
        {
            // ĐÚNG: Value = AdjustedValue khi không có modifier.
            // HIỆN TẠI: nhánh non-primary của Stat.CalculateFinalValue() không gán this.finalValue
            // rồi vẫn `return this.finalValue` -> mọi derived stat luôn trả 0.
            StatsSO profile = Create(stats: StatList(NewStat(StatType.MaxHP, 100f)));

            AssertFloat(profile.Get(StatType.MaxHP).Value, 100f);
        }

        [Test]
        [Category("KnownBug")]
        public void GetStatEquipValue_DerivedStat_DoesNotAccumulateAcrossRecalculations()
        {
            // ĐÚNG: một modifier Flat +10 -> EquipmentValue = 10.
            // HIỆN TẠI: nhánh non-primary dùng `+=` và SetDirty() gọi Recaulate() thêm một lần
            // ngoài lần tính trong Value -> phần chênh bị cộng dồn (ra 20).
            StatsSO profile = Create(stats: StatList(NewStat(StatType.MaxHP, 100f)));

            profile.AddModifiersFromSource(new object(), ModifierList(NewModifier(StatType.MaxHP, 10f)));

            AssertFloat(profile.GetStatEquipValue(StatType.MaxHP), 10f);
        }

        [Test]
        [Category("KnownBug")]
        public void Evaluate_NoContributions_PutsConstantInBaseValueOnly()
        {
            // ĐÚNG: BaseValue = baseConstant + level × perLevel; LevelUpValue / EquipmentValue = 0.
            // HIỆN TẠI: Evaluate khởi tạo cả ba tầng bằng cùng một `value`
            // -> hằng số bị đếm ba lần, AdjustedValue ra gấp đôi.
            StatsSO profile = Create(level: 3, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f)),
                formulas: new[] { Formula(StatType.MaxHP, baseConstant: 50f, perLevel: 10f) });

            Stat maxHp = profile.Get(StatType.MaxHP);
            AssertFloat(maxHp.BaseValue, 80f, "BaseValue");
            AssertFloat(maxHp.LevelUpValue, 0f, "LevelUpValue");
            AssertFloat(maxHp.EquipmentValue, 0f, "EquipmentValue");
            AssertFloat(maxHp.AdjustedValue, 80f, "AdjustedValue");
        }

        [Test]
        [Category("KnownBug")]
        public void Evaluate_SingleContribution_AppliesItsCoefficient()
        {
            // ĐÚNG: BaseValue = 50 + STR.BaseValue × 2 = 70; LevelUpValue = STR.LevelUpValue × 2 = 8.
            // HIỆN TẠI: vòng lặp cộng `sourceBaseValue` của vòng TRƯỚC rồi mới đọc stat của vòng
            // hiện tại -> contribution cuối cùng luôn bị bỏ; với 1 contribution thì mất sạch.
            StatsSO profile = Create(level: 1, devMode: true,
                stats: StatList(NewStat(StatType.STR, 10f, 4f)),
                formulas: new[]
                {
                    Formula(StatType.MaxHP, baseConstant: 50f, perLevel: 0f,
                        Contribution(StatType.STR, 2f)),
                });

            Stat maxHp = profile.Get(StatType.MaxHP);
            AssertFloat(maxHp.BaseValue, 70f, "BaseValue");
            AssertFloat(maxHp.LevelUpValue, 8f, "LevelUpValue");
        }

        [Test]
        [Category("KnownBug")]
        public void Evaluate_MultipleContributions_PairEachSourceWithItsOwnCoefficient()
        {
            // ĐÚNG: BaseValue = 50 + STR(10)×2 + DEX(20)×3 = 130; LevelUpValue = 4×2 + 6×3 = 26.
            // HIỆN TẠI: lệch một nhịp -> STR bị nhân hệ số của DEX, DEX bị bỏ hẳn (ra 80 / 62).
            StatsSO profile = Create(level: 1, devMode: true,
                stats: StatList(
                    NewStat(StatType.STR, 10f, 4f),
                    NewStat(StatType.DEX, 20f, 6f)),
                formulas: new[]
                {
                    Formula(StatType.MaxHP, baseConstant: 50f, perLevel: 0f,
                        Contribution(StatType.STR, 2f),
                        Contribution(StatType.DEX, 3f)),
                });

            Stat maxHp = profile.Get(StatType.MaxHP);
            AssertFloat(maxHp.BaseValue, 130f, "BaseValue");
            AssertFloat(maxHp.LevelUpValue, 26f, "LevelUpValue");
        }
    }
}
#endif
