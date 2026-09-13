using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return ShouldShow(property) ? EditorGUI.GetPropertyHeight(property, label) : 0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
            EditorGUI.PropertyField(position, property, label, true);
    }

    private bool ShouldShow(SerializedProperty property)
    {
        var showIf = attribute as ShowIfAttribute;
        string path = property.propertyPath.Replace(property.name, showIf.conditionField);
        var conditionProp = property.serializedObject.FindProperty(path);

        if (conditionProp == null) return true;

        switch (conditionProp.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return conditionProp.boolValue.Equals(showIf.compareValue);
            case SerializedPropertyType.Enum:
                return conditionProp.enumValueIndex.Equals((int)showIf.compareValue);
            default:
                return true;
        }
    }
}
