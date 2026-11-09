#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector of a stat profile: the editable data (level, primary base values, derived formulas), then a
/// read-only table of what a character built from it gets, calculated by a fresh <see cref="StatSheet"/>.
/// The asset itself stores no calculated value.
/// </summary>
// NEED CHECK: new 2026-11-09 (stat split).
[CustomEditor(typeof(BaseStatsSO), true)]
public class BaseStatsSOEditor : Editor
{
    private bool showPreview = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        showPreview = EditorGUILayout.Foldout(showPreview, "Preview (calculated, read only)", true);
        if (!showPreview) return;

        StatSheet sheet = new StatSheet((BaseStatsSO)target);
        StatPreviewTable.Draw(sheet);
    }
}

/// <summary>Shared read-only stat table for editor previews.</summary>
public static class StatPreviewTable
{
    private const float NameWidth = 120f;

    public static void Draw(StatSheet sheet)
    {
        using (new EditorGUI.DisabledScope(true))
        {
            Row("Stat", "Base", "LevelUp", "Bonus", "Final", EditorStyles.miniBoldLabel);
            foreach (Stat stat in sheet.All)
            {
                Row(stat.Type.ToString(),
                    F(stat.BaseValue), F(stat.LevelUpValue), F(stat.EquipmentValue), F(stat.FinalValue),
                    EditorStyles.label);
            }
        }
    }

    private static void Row(string name, string a, string b, string c, string d, GUIStyle style)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(name, style, GUILayout.Width(NameWidth));
            EditorGUILayout.LabelField(a, style);
            EditorGUILayout.LabelField(b, style);
            EditorGUILayout.LabelField(c, style);
            EditorGUILayout.LabelField(d, style);
        }
    }

    private static string F(float value) => value.ToString("0.##");
}
#endif
