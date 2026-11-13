using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;

// Shared helpers for the EditMode tests: private-member access for building synthetic data, and loading
// committed assets (always used through an Object.Instantiate copy by the caller).
internal static class TestSupport
{
    private const BindingFlags Members = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

    public static void SetField(object target, string name, object value)
    {
        FindField(target, name).SetValue(target, value);
    }

    public static T GetField<T>(object target, string name)
    {
        return (T)FindField(target, name).GetValue(target);
    }

    // For auto-properties with a private setter ([field: SerializeField] public X Y { get; private set; }).
    public static void SetProperty(object target, string name, object value)
    {
        for (Type type = target.GetType(); type != null; type = type.BaseType)
        {
            PropertyInfo property = type.GetProperty(name, Members);
            if (property == null) continue;
            MethodInfo setter = property.GetSetMethod(true);
            Assert.IsNotNull(setter, $"{target.GetType().Name}.{name} has no setter (test assumption out of date)");
            setter.Invoke(target, new[] { value });
            return;
        }
        Assert.Fail($"{target.GetType().Name} has no property '{name}' (test assumption out of date)");
    }

    public static T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        Assert.IsNotNull(asset, $"asset not found: {path}");
        return asset;
    }

    public static void DestroyAll(List<UnityEngine.Object> created)
    {
        foreach (UnityEngine.Object o in created)
        {
            if (o != null) UnityEngine.Object.DestroyImmediate(o);
        }
        created.Clear();
    }

    private static FieldInfo FindField(object target, string name)
    {
        for (Type type = target.GetType(); type != null; type = type.BaseType)
        {
            FieldInfo field = type.GetField(name, Members);
            if (field != null) return field;
        }
        Assert.Fail($"{target.GetType().Name} has no field '{name}' (test assumption out of date)");
        return null;
    }
}
