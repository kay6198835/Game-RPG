#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Tests.StatSystem
{
    /// <summary>
    /// Dựng StatsSO "sạch" cho test: mỗi test một instance riêng, KHÔNG đụng vào asset trong project.
    ///
    /// Vì sao phải dùng reflection: level / stats / statFormulas / isDevMode đều là private
    /// [SerializeField] — trong game chúng được author ở Inspector, không có API set từ code.
    ///
    /// Vì sao phải hạ cờ `initialized`: ScriptableObject.CreateInstance() gọi OnEnable ngay,
    /// mà OnEnable đã chạy EnsureInitialized() trên list rỗng. Bơm dữ liệu xong mà không hạ cờ
    /// thì lookup vẫn trỏ vào đám Stat 0 sinh ra lúc đó.
    /// </summary>
    public static class StatsProfileFactory
    {
        private const BindingFlags FIELD_FLAGS =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private static readonly List<StatsSO> created = new List<StatsSO>();

        public static StatsSO Create(
            int level = 1,
            bool devMode = false,
            List<Stat> stats = null,
            DerivedStatFormula[] formulas = null)
        {
            StatsSO profile = ScriptableObject.CreateInstance<StatsSO>();
            profile.name = "TestStatsProfile";

            SetField(profile, "level", level);
            SetField(profile, "statUnusedBonus", 0);
            SetField(profile, "stats", stats ?? new List<Stat>());
            SetField(profile, "statFormulas", formulas);
            SetField(profile, "isDevMode", devMode);
            SetField(profile, "initialized", false);

            // EnsureInitialized là private và chỉ chạy lười qua API public. Phải ép chạy ngay:
            // lookup lúc này vẫn đang giữ đám Stat 0 do OnEnable sinh ra, mà CalculateStatUnusedBonus
            // duyệt lookup chứ không duyệt list -> không ép thì test đọc phải dữ liệu cũ.
            profile.Get(StatType.STR);

            created.Add(profile);
            return profile;
        }

        /// <summary>Dọn mọi profile đã tạo — gọi trong TearDown để không rò state giữa các test.</summary>
        public static void DestroyCreated()
        {
            for (int i = 0; i < created.Count; i++)
                if (created[i] != null)
                    UnityEngine.Object.DestroyImmediate(created[i]);
            created.Clear();
        }

        // ------------------------- Dựng dữ liệu -------------------------

        public static Stat NewStat(StatType type, float baseValue = 0f, float levelUpValue = 0f, float equipmentValue = 0f)
            => new Stat(type, baseValue, levelUpValue, equipmentValue);

        public static List<Stat> StatList(params Stat[] stats) => new List<Stat>(stats);

        public static StatModifier NewModifier(StatType target, float value, ModifierType type = ModifierType.Flat)
            => new StatModifier(target, value, type);

        public static List<StatModifier> ModifierList(params StatModifier[] modifiers)
            => new List<StatModifier>(modifiers);

        public static DerivedStatFormula Formula(
            StatType target,
            float baseConstant = 0f,
            float perLevel = 0f,
            params DerivedStatFormula.StatContribution[] contributions)
        {
            return new DerivedStatFormula
            {
                targetStat = target,
                baseConstant = baseConstant,
                perLevel = perLevel,
                contributions = contributions,
            };
        }

        public static DerivedStatFormula.StatContribution Contribution(StatType source, float coefficient)
            => new DerivedStatFormula.StatContribution { sourceStat = source, coefficient = coefficient };

        // ------------------------- Reflection -------------------------

        private static void SetField(object target, string name, object value)
        {
            FieldInfo field = FindField(target.GetType(), name);
            if (field == null)
                throw new MissingFieldException($"{target.GetType().Name} không có field '{name}' — test helper đã lệch khỏi code.");
            field.SetValue(target, value);
        }

        // Auto-property ([field: SerializeField]) serialize dưới key `<name>k__BackingField`,
        // nên phải thử cả hai dạng tên.
        private static FieldInfo FindField(Type type, string name)
        {
            for (Type current = type; current != null; current = current.BaseType)
            {
                FieldInfo field = current.GetField(name, FIELD_FLAGS)
                                  ?? current.GetField($"<{name}>k__BackingField", FIELD_FLAGS);
                if (field != null) return field;
            }
            return null;
        }
    }
}
#endif
