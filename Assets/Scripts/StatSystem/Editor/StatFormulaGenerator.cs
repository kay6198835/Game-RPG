#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Game.Stats.EditorTools
{
    /// <summary>
    /// Sinh tự động 12 asset DerivedStatFormula với bộ số mặc định.
    /// Chạy qua menu: Tools → Stats → Generate Default Formulas.
    /// Chạy lại sẽ ghi đè số liệu lên asset cũ (không tạo trùng).
    /// </summary>
    public static class StatFormulaGenerator
    {
        private const string OutputFolder = "Assets/GameData/StatFormulas";

        [MenuItem("Tools/Stats/Generate Default Formulas")]
        public static void Generate()
        {
            EnsureFolder();

            Create(StatType.MaxHP, 100f, 5f, (StatType.VIT, 10f));
            Create(StatType.MaxMana, 50f, 0f, (StatType.INT, 8f));
            Create(StatType.PhysicalDamage, 0f, 0f, (StatType.STR, 2f));
            Create(StatType.MagicDamage, 0f, 0f, (StatType.INT, 2.5f));
            Create(StatType.Defense, 0f, 0f, (StatType.VIT, 1.5f));
            Create(StatType.AttackSpeed, 1f, 0f, (StatType.DEX, 0.01f));
            Create(StatType.CritChance, 5f, 0f, (StatType.DEX, 0.2f), (StatType.LUK, 0.3f));
            Create(StatType.CritDamage, 150f, 0f, (StatType.LUK, 1f));
            Create(StatType.MoveSpeed, 5f, 0f, (StatType.DEX, 0.02f));
            Create(StatType.HPRegen, 0f, 0f, (StatType.VIT, 0.1f));
            Create(StatType.ManaRegen, 0f, 0f, (StatType.INT, 0.15f));
            Create(StatType.Evasion, 0f, 0f, (StatType.DEX, 0.15f));

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[StatFormulaGenerator] Đã tạo/cập nhật 12 formula assets tại {OutputFolder}");
        }

        private static void EnsureFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/GameData"))
                AssetDatabase.CreateFolder("Assets", "GameData");
            if (!AssetDatabase.IsValidFolder(OutputFolder))
                AssetDatabase.CreateFolder("Assets/GameData", "StatFormulas");
        }

        private static void Create(StatType target, float baseConstant, float perLevel,
            params (StatType stat, float coef)[] contribs)
        {
            string path = $"{OutputFolder}/Formula_{target}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<DerivedStatFormula>(path);
            bool isNew = asset == null;
            if (isNew) asset = ScriptableObject.CreateInstance<DerivedStatFormula>();

            asset.targetStat = target;
            asset.baseConstant = baseConstant;
            asset.perLevel = perLevel;
            asset.contributions = System.Array.ConvertAll(contribs,
                c => new DerivedStatFormula.StatContribution { sourceStat = c.stat, coefficient = c.coef });

            if (isNew) AssetDatabase.CreateAsset(asset, path);
            else EditorUtility.SetDirty(asset);
        }
    }
}
#endif
