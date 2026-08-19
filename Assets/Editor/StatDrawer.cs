#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Vẽ một Stat trong Inspector: hai tầng authored sửa được, ba giá trị dẫn xuất chỉ để xem.
///
/// AdjustedValue / EquipmentValue / FinalValue KHÔNG serialize (xem Stat.cs) nên không lấy được
/// qua SerializedProperty. Drawer lần theo propertyPath bằng reflection để cầm đúng instance
/// Stat đang sống, rồi đọc property trực tiếp — nhờ vậy số hiển thị bao gồm cả modifier
/// runtime lúc đang Play Mode, chứ không phải một bản chụp cũ nằm trong .asset.
/// </summary>
[CustomPropertyDrawer(typeof(Stat))]
public class StatDrawer : PropertyDrawer
{
    // Type là auto-property -> Unity serialize dưới tên backing field này, không phải "Type".
    private const string TYPE_FIELD = "<Type>k__BackingField";
    private const float PAD = 2f;
    private const int ROWS_EXPANDED = 7;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float row = EditorGUIUtility.singleLineHeight + PAD;
        return property.isExpanded ? row * ROWS_EXPANDED : row;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty typeProp = property.FindPropertyRelative(TYPE_FIELD);
        SerializedProperty baseProp = property.FindPropertyRelative("baseValue");
        SerializedProperty levelUpProp = property.FindPropertyRelative("levelUpValue");

        Stat stat = ResolveStat(property);

        // Fallback khi reflection không cầm được instance: tính lại từ dữ liệu đã serialize.
        // Bản fallback không thấy modifier runtime — chấp nhận được vì nó chỉ chạy ngoài Play Mode,
        // lúc chưa có modifier nào nên final trùng adjusted.
        float adjusted = stat != null
            ? stat.AdjustedValue
            : (baseProp?.floatValue ?? 0f) + (levelUpProp?.floatValue ?? 0f);
        float final = stat != null ? stat.Value : adjusted;

        Rect row = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        string title = typeProp != null ? ((StatType)typeProp.intValue).ToString() : label.text;
        property.isExpanded = EditorGUI.Foldout(row, property.isExpanded, $"{title}     {final:0.###}", true);

        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;

        row = NextRow(row);
        if (typeProp != null) EditorGUI.PropertyField(row, typeProp, new GUIContent("Type"));

        row = NextRow(row);
        if (baseProp != null) EditorGUI.PropertyField(row, baseProp, new GUIContent("Base Value"));

        row = NextRow(row);
        if (levelUpProp != null) EditorGUI.PropertyField(row, levelUpProp, new GUIContent("Level Up Value"));

        using (new EditorGUI.DisabledScope(true))
        {
            row = NextRow(row);
            EditorGUI.FloatField(row, new GUIContent("Adjusted Value", "Base + LevelUp — tính ra, không lưu"), adjusted);

            row = NextRow(row);
            EditorGUI.FloatField(row, new GUIContent("Equipment Value", "Final - Adjusted: đóng góp của trang bị và buff — tính ra, không lưu"), final - adjusted);

            row = NextRow(row);
            EditorGUI.FloatField(row, new GUIContent("Final Value", "Adjusted qua toàn bộ modifier — tính ra, không lưu"), final);
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    private static Rect NextRow(Rect r) =>
        new Rect(r.x, r.y + EditorGUIUtility.singleLineHeight + PAD, r.width, r.height);

    // ---------- lần theo propertyPath về đúng instance Stat ----------

    private static Stat ResolveStat(SerializedProperty property)
    {
        object current = property.serializedObject.targetObject;
        if (current == null) return null;

        // "stats.Array.data[3]" -> "stats[3]"
        string path = property.propertyPath.Replace(".Array.data[", "[");

        foreach (string step in path.Split('.'))
        {
            int bracket = step.IndexOf('[');
            if (bracket < 0)
            {
                current = GetMember(current, step);
            }
            else
            {
                string name = step.Substring(0, bracket);
                string indexText = step.Substring(bracket + 1).TrimEnd(']');
                if (!int.TryParse(indexText, out int index)) return null;
                current = GetElement(GetMember(current, name), index);
            }

            if (current == null) return null;
        }

        return current as Stat;
    }

    private static object GetMember(object source, string name)
    {
        if (source == null) return null;

        for (Type t = source.GetType(); t != null; t = t.BaseType)
        {
            FieldInfo field = t.GetField(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field != null) return field.GetValue(source);

            PropertyInfo prop = t.GetProperty(name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop != null) return prop.GetValue(source);
        }
        return null;
    }

    private static object GetElement(object source, int index)
    {
        if (source is IList list) return index >= 0 && index < list.Count ? list[index] : null;

        if (source is IEnumerable enumerable)
        {
            IEnumerator e = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
                if (!e.MoveNext()) return null;
            return e.Current;
        }
        return null;
    }
}
#endif
