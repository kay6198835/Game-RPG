#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DepotItem))]
public class DepotItemEditor : Editor
{
    private const int DEFAULT_SAMPLE_COUNT = 10000;

    // Ngưỡng "lệch thật": 3 sigma của phân phối nhị thức ở cỡ mẫu n.
    // Dưới ngưỡng này thì sai khác chỉ là nhiễu ngẫu nhiên, không phải lỗi cấu hình.
    private const float SIGMA_MULTIPLIER = 3f;

    private int _sampleCount = DEFAULT_SAMPLE_COUNT;
    private int _seed = 12345;
    private bool _useSeed;

    private bool _hasResult;
    private int _resultSamples;
    private int _noDropCount;
    private readonly Dictionary<RarityTierItem, int> _tierCounts = new Dictionary<RarityTierItem, int>();
    private readonly Dictionary<ItemSO, int> _itemCounts = new Dictionary<ItemSO, int>();
    private readonly Dictionary<ItemSO, RarityTierItem> _itemTier = new Dictionary<ItemSO, RarityTierItem>();

    public override void OnInspectorGUI()
    {
        DrawConfigFields();

        DepotItem depot = (DepotItem)target;

        EditorGUILayout.Space();
        DrawConfigCheck(depot);

        EditorGUILayout.Space();
        DrawRollTestControls(depot);

        if (_hasResult)
        {
            EditorGUILayout.Space();
            DrawTierResult(depot);
            EditorGUILayout.Space();
            DrawItemResult();
        }
    }

    private void DrawConfigFields()
    {
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (iterator.name == "m_Script" || iterator.name == "rateDepotDropItem")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(iterator, true);
                continue;
            }

            EditorGUILayout.PropertyField(iterator, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawConfigCheck(DepotItem depot)
    {
        EditorGUILayout.LabelField("Config Check", EditorStyles.boldLabel);

        depot.InvalidateCache();

        float declared = 0f;
        var excluded = new List<string>();

        foreach (var tier in depot.DeclaredTiers)
        {
            if (tier == null) continue;
            declared += tier.TierRate;

            if (tier.TierRate > 0f && !depot.HasItemOfTier(tier.RarityTier))
                excluded.Add($"{tier.RarityTier} ({tier.TierRate:0.##}%)");
        }

        float effective = depot.TotalDropRate;

        EditorGUILayout.LabelField($"Declared total: {declared:0.##}%");
        EditorGUILayout.LabelField($"Effective drop rate: {effective:0.##}%   →   no-drop {100f - effective:0.##}%");

        if (declared > 100f)
        {
            EditorGUILayout.HelpBox(
                $"Tổng TierRate khai là {declared:0.##} > 100. Phần vượt 100 không bao giờ trúng — " +
                "các tier ở CUỐI danh sách bị cắt cụt. Giảm số lại cho tổng ≤ 100.",
                MessageType.Error);
        }

        if (excluded.Count > 0)
        {
            EditorGUILayout.HelpBox(
                "Tier bị loại vì listItems không có item nào thuộc tier đó: " + string.Join(", ", excluded) +
                ".\nTỷ lệ rơi thực tế đã trừ đi phần này.",
                MessageType.Warning);
        }

        if (depot.RollableTiers.Count == 0)
        {
            EditorGUILayout.HelpBox(
                "Không có tier nào roll được — kho này sẽ không bao giờ rơi đồ. " +
                "Cần khai TierRate > 0 và có ít nhất một item thuộc tier đó trong listItems.",
                MessageType.Warning);
        }

        // Đếm item theo từng tier để designer thấy ngay bảng nào đang trống.
        var perTier = new Dictionary<RarityTierItem, int>();
        foreach (var slot in depot.listItems)
        {
            if (slot == null || slot.DepotItem == null) continue;
            perTier.TryGetValue(slot.RarityTier, out int c);
            perTier[slot.RarityTier] = c + 1;
        }

        foreach (RarityTierItem tier in System.Enum.GetValues(typeof(RarityTierItem)))
        {
            perTier.TryGetValue(tier, out int count);
            EditorGUILayout.LabelField($"    {tier}: {count} item");
        }
    }

    private void DrawRollTestControls(DepotItem depot)
    {
        EditorGUILayout.LabelField("Roll Test", EditorStyles.boldLabel);

        _sampleCount = Mathf.Max(1, EditorGUILayout.IntField("Sample Count", _sampleCount));
        _useSeed = EditorGUILayout.Toggle("Use Fixed Seed", _useSeed);
        using (new EditorGUI.DisabledScope(!_useSeed))
            _seed = EditorGUILayout.IntField("Seed", _seed);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button($"Roll Test ({_sampleCount} samples)"))
                RunRollTest(depot);

            using (new EditorGUI.DisabledScope(!_hasResult))
            {
                if (GUILayout.Button("Clear Result"))
                    ClearResult();
            }
        }
    }

    private void RunRollTest(DepotItem depot)
    {
        ClearResult();

        // Bọc RNG state để phiên test không làm lệch chuỗi ngẫu nhiên của game.
        Random.State savedState = Random.state;
        if (_useSeed) Random.InitState(_seed);

        depot.InvalidateCache();

        for (int i = 0; i < _sampleCount; i++)
        {
            if (!depot.TryRollItem(out ItemSO item, out RarityTierItem tier))
            {
                _noDropCount++;
                continue;
            }

            _tierCounts.TryGetValue(tier, out int tc);
            _tierCounts[tier] = tc + 1;

            _itemCounts.TryGetValue(item, out int ic);
            _itemCounts[item] = ic + 1;
            _itemTier[item] = tier;
        }

        Random.state = savedState;

        _resultSamples = _sampleCount;
        _hasResult = true;
    }

    private void ClearResult()
    {
        _tierCounts.Clear();
        _itemCounts.Clear();
        _itemTier.Clear();
        _noDropCount = 0;
        _resultSamples = 0;
        _hasResult = false;
    }

    private void DrawTierResult(DepotItem depot)
    {
        EditorGUILayout.LabelField($"Result — by tier ({_resultSamples} samples)", EditorStyles.boldLabel);
        DrawRow("Tier", "Expected", "Actual", "Delta", "Count", EditorStyles.miniBoldLabel);

        foreach (var tier in depot.RollableTiers)
        {
            _tierCounts.TryGetValue(tier.RarityTier, out int count);
            DrawStatRow(tier.RarityTier.ToString(), tier.TierRate, count);
        }

        // Tier khai nhưng bị loại: kỳ vọng 0, phải thực sự bằng 0.
        foreach (var tier in depot.DeclaredTiers)
        {
            if (tier == null || depot.HasItemOfTier(tier.RarityTier)) continue;
            _tierCounts.TryGetValue(tier.RarityTier, out int count);
            DrawStatRow($"{tier.RarityTier} (excluded)", 0f, count);
        }

        DrawStatRow("NO DROP", 100f - depot.TotalDropRate, _noDropCount);
    }

    private void DrawItemResult()
    {
        EditorGUILayout.LabelField("Result — by item", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(
            "Trong cùng một tier các item phải xấp xỉ bằng nhau (GetRandomItemByRarity chọn đều).",
            EditorStyles.miniLabel);
        DrawRow("Item", "Tier", "Share of tier", "Of all rolls", "Count", EditorStyles.miniBoldLabel);

        foreach (var pair in _itemCounts)
        {
            RarityTierItem tier = _itemTier[pair.Key];
            _tierCounts.TryGetValue(tier, out int tierTotal);

            float shareOfTier = tierTotal > 0 ? 100f * pair.Value / tierTotal : 0f;
            float shareOfAll = 100f * pair.Value / _resultSamples;

            DrawRow(
                pair.Key != null ? pair.Key.name : "<null>",
                tier.ToString(),
                $"{shareOfTier:0.00}%",
                $"{shareOfAll:0.00}%",
                pair.Value.ToString(),
                EditorStyles.label);
        }
    }

    private void DrawStatRow(string label, float expectedPercent, int count)
    {
        float actual = 100f * count / _resultSamples;
        float delta = actual - expectedPercent;

        float p = expectedPercent / 100f;
        float tolerance = SIGMA_MULTIPLIER * Mathf.Sqrt(Mathf.Max(p * (1f - p), 0f) / _resultSamples) * 100f;
        bool outOfTolerance = Mathf.Abs(delta) > tolerance;

        var deltaStyle = new GUIStyle(EditorStyles.label);
        deltaStyle.normal.textColor = outOfTolerance ? Color.red : EditorStyles.label.normal.textColor;

        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(160));
            EditorGUILayout.LabelField($"{expectedPercent:0.00}%", GUILayout.Width(80));
            EditorGUILayout.LabelField($"{actual:0.00}%", GUILayout.Width(80));
            EditorGUILayout.LabelField($"{delta:+0.00;-0.00;0.00}", deltaStyle, GUILayout.Width(70));
            EditorGUILayout.LabelField(count.ToString(), GUILayout.Width(70));
        }
    }

    private void DrawRow(string c0, string c1, string c2, string c3, string c4, GUIStyle style)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(c0, style, GUILayout.Width(160));
            EditorGUILayout.LabelField(c1, style, GUILayout.Width(80));
            EditorGUILayout.LabelField(c2, style, GUILayout.Width(80));
            EditorGUILayout.LabelField(c3, style, GUILayout.Width(70));
            EditorGUILayout.LabelField(c4, style, GUILayout.Width(70));
        }
    }
}
#endif
