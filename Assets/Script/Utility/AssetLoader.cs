using UnityEngine;

public static class AssetLoader
{
    public static T Load<T>(string path) where T : Object
    {
        T asset = Resources.Load<T>(path);

        if (asset == null)
        {
            Debug.LogError($"Asset not found at path: {path}");
        }

        return asset;
    }
}