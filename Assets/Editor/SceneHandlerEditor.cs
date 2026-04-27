#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(MapGenerator))]
public class SceneHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapGenerator handler = (MapGenerator)target;

        if (GUILayout.Button("Auto Assign Levels"))
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:LevelSettings",
                new[] { "Assets/Levels" }
            );

            handler.levels = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<LevelSettings>(
                    AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            EditorUtility.SetDirty(handler);
        }
    }
}
#endif