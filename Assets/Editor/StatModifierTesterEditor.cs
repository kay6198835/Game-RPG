#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StatModifierTester))]
public class StatModifierTesterEditor : Editor
{
    private static readonly StatType[] PrimaryTypes = BuildPrimaryTypes();
    private static readonly string[] PrimaryNames = BuildPrimaryNames();

    public override void OnInspectorGUI()
    {
        DrawConfigFields();

        StatModifierTester tester = (StatModifierTester)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        if (GUILayout.Button("Level Up (+1 level, +5 primary)"))
            tester.LevelUp();

        if (GUILayout.Button("Add Modifier (from selected source)"))
            tester.AddModifierFromSource();

        if (GUILayout.Button("Add Modifier Group (from selected source)"))
            tester.AddGroupFromSource();

        if (GUILayout.Button("Remove Modifiers (this tester — all sources)"))
            tester.RemoveAllTesterSources();

        if (GUILayout.Button("Remove Modifiers (selected source only)"))
            tester.RemoveCurrentSource();

        if (GUILayout.Button("Reset Profile"))
            tester.ResetProfile();

        DrawLiveReadout(tester);
    }

    // Vẽ các field mặc định, riêng primaryToAllocate thay bằng popup CHỈ primary.
    private void DrawConfigFields()
    {
        serializedObject.Update();

        SerializedProperty iterator = serializedObject.GetIterator();
        bool enterChildren = true;
        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (iterator.name == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(iterator);
                continue;
            }
            if (iterator.name == "primaryToAllocate")
            {
                DrawPrimaryOnlyPopup(iterator);
                continue;
            }
            EditorGUILayout.PropertyField(iterator, true);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPrimaryOnlyPopup(SerializedProperty prop)
    {
        int current = Mathf.Max(0, Array.IndexOf(PrimaryTypes, (StatType)prop.intValue));
        int selected = EditorGUILayout.Popup("Primary To Allocate", current, PrimaryNames);
        prop.intValue = (int)PrimaryTypes[selected];
    }

    private void DrawLiveReadout(StatModifierTester tester)
    {
        StatsSO stats = tester.Stats;
        if (stats == null) return;

        EditorGUILayout.Space();
        //EditorGUILayout.LabelField($"Live Values (Level {stats.Level})", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Primary", EditorStyles.miniBoldLabel);
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
            if (t.IsPrimary())
                DrawStatLine(stats, t);

        EditorGUILayout.LabelField("Derived", EditorStyles.miniBoldLabel);
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
            if (!t.IsPrimary())
                DrawStatLine(stats, t);
    }

    private void DrawStatLine(StatsSO stats, StatType t)
    {
        Stat stat = stats.Get(t);
        float value = stat != null ? stat.Value : 0f;
        EditorGUILayout.LabelField(t.ToString(), value.ToString("0.###"));
    }

    private static StatType[] BuildPrimaryTypes()
    {
        List<StatType> list = new List<StatType>();
        foreach (StatType t in Enum.GetValues(typeof(StatType)))
            if (t.IsPrimary()) list.Add(t);
        return list.ToArray();
    }

    private static string[] BuildPrimaryNames()
    {
        StatType[] types = BuildPrimaryTypes();
        string[] names = new string[types.Length];
        for (int i = 0; i < types.Length; i++) names[i] = types[i].ToString();
        return names;
    }
}
#endif
