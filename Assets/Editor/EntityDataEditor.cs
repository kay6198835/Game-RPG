#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// EntityData Inspector plus a read-only preview of the AI values the multipliers produce from the
/// assigned stat profile (no buffs). In Play Mode each enemy computes them from its own stats.
/// </summary>
// NEED CHECK: new 2026-11-09 (stat split) — replaces the four values EntityData cached in OnValidate.
[CustomEditor(typeof(EntityData))]
public class EntityDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var data = (EntityData)target;
        if (data.Stats == null) return;

        StatSheet sheet = new StatSheet(data.Stats);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Preview (from Stats, read only)", EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.FloatField("Range View", data.RangeView(sheet));
            EditorGUILayout.FloatField("Angle View", data.AngleView(sheet));
            EditorGUILayout.FloatField("Idle Duration", data.IdleDurationTime(sheet));
            EditorGUILayout.FloatField("Move Duration", data.MoveDurationTime(sheet));
        }
    }
}
#endif
