#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManager levelManager = (LevelManager)target;

        if (GUILayout.Button("Save Level"))
        {
            levelManager.SaveLevel();
        }

        if (GUILayout.Button("Load Level"))
        {
            levelManager.LoadLevel();
        }
        if (GUILayout.Button("Clear Enemy"))
        {
            EventManager.Emit(EventID.ON_CLEAR_ENEMY);
        }
        if (GUILayout.Button("Import Room Json Files"))
        {
            levelManager.ImportRoomJsonFiles();
        }
    }
}
#endif