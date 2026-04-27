#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(RoomTypeDataset))]

public class RoomTypesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomTypeDataset handler = (RoomTypeDataset)target;

        if (GUILayout.Button("Auto Assign Levels"))
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:RoomType",
                new[] { "Assets/Prefabs/Rooms" }
            );

            handler.roomPrefabs = guids
                .Select(guid => AssetDatabase.LoadAssetAtPath<RoomType>(
                    AssetDatabase.GUIDToAssetPath(guid)))
                .ToList();

            EditorUtility.SetDirty(handler);
        }
    }
}
#endif