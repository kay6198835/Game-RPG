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

        if (GUILayout.Button("Save Level (Create)")) tool.SaveLevel();
        if (GUILayout.Button("Load Level (Read)")) tool.LoadLevel();
        if (GUILayout.Button("Edit Level (Update)")) tool.UpdateRoom();
        if (GUILayout.Button("Delete Level")) tool.DeleteRoom();
        if (GUILayout.Button("Import Room Json Files")) tool.ImportRoomJsonFiles();
    }
}
#endif
