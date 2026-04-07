using UnityEditor;
using UnityEngine;

public class RoomCreator : EditorWindow
{
    float width = 5f;
    float height = 3f;
    float depth = 5f;

    [MenuItem("Tools/Room Generator")]
    public static void ShowWindow()
    {
        GetWindow<RoomCreator>("Room Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Room Settings", EditorStyles.boldLabel);

        width = EditorGUILayout.FloatField("Width", width);
        height = EditorGUILayout.FloatField("Height", height);
        depth = EditorGUILayout.FloatField("Depth", depth);

        if (GUILayout.Button("Create Room"))
        {
            CreateRoom();
        }
    }

    void CreateRoom()
    {
        GameObject parent = new GameObject("Room");

        // Floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(parent.transform);
        floor.transform.localScale = new Vector3(width, 0.1f, depth);
        floor.transform.localPosition = new Vector3(0, -0.05f, 0);

        // Ceiling
        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceiling.name = "Ceiling";
        ceiling.transform.SetParent(parent.transform);
        ceiling.transform.localScale = new Vector3(width, 0.1f, depth);
        ceiling.transform.localPosition = new Vector3(0, height, 0);

        // Walls
        CreateWall(parent.transform, "Wall_Front", new Vector3(0, height / 2, depth / 2), new Vector3(width, height, 0.1f));
        CreateWall(parent.transform, "Wall_Back", new Vector3(0, height / 2, -depth / 2), new Vector3(width, height, 0.1f));
        CreateWall(parent.transform, "Wall_Left", new Vector3(-width / 2, height / 2, 0), new Vector3(0.1f, height, depth));
        CreateWall(parent.transform, "Wall_Right", new Vector3(width / 2, height / 2, 0), new Vector3(0.1f, height, depth));

        Selection.activeGameObject = parent;
    }

    void CreateWall(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = name;
        wall.transform.SetParent(parent);
        wall.transform.localPosition = position;
        wall.transform.localScale = scale;
    }
}
