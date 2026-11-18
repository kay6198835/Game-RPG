#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelEditorTool))]
public class LevelEditorToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelEditorTool tool = (LevelEditorTool)target;

        if (GUILayout.Button("Save Level"))
        {
            tool.SaveLevel();
        }

        if (GUILayout.Button("Load Level"))
        {
            tool.LoadLevel();
        }
        if (GUILayout.Button("Edit Level"))
        {
            tool.UpdateRoom();
        }
        if (GUILayout.Button("Clear Enemy"))
        {
            EventManager.Emit(EventID.ON_OPEN_DOOR);
        }
        if (GUILayout.Button("Import Room Json Files"))
        {
            tool.ImportRoomJsonFiles();
        }
        if (GUILayout.Button("Spawn Enemy"))
        {
            tool.SpawnRoomEnemies();
        }
    }
}
#endif