using UnityEngine;
using UnityEditor;

public class StairsCreator : EditorWindow
{
    float targetHeight = 5.0f;
    float stepHeight = 0.25f;
    float stepDepth = 0.4f;
    float stepWidth = 1.0f;
    Material stepMaterial;

    [MenuItem("Tools/Stair Generator")]
    public static void ShowWindow()
    {
        GetWindow<StairsCreator>("Stair Creator");
    }

    void OnGUI()
    {
        GUILayout.Label("Stair Settings (Centered Anchor)", EditorStyles.boldLabel);

        targetHeight = EditorGUILayout.FloatField("Total Height", targetHeight);
        stepHeight = EditorGUILayout.FloatField("Step Height", stepHeight);
        stepDepth = EditorGUILayout.FloatField("Step Depth", stepDepth);
        stepWidth = EditorGUILayout.FloatField("Step Width", stepWidth);
        stepMaterial = (Material)EditorGUILayout.ObjectField("Step Material", stepMaterial, typeof(Material), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Stairs"))
        {
            CreateStairs();
        }
    }

    void CreateStairs()
    {
        int numSteps = Mathf.CeilToInt(targetHeight / stepHeight);

        // Create the parent at the current Scene View focus point or 0,0,0
        GameObject stairParent = new GameObject("GeneratedStaircase_Centered");
        Undo.RegisterCreatedObjectUndo(stairParent, "Create Stairs");

        // Calculate the total span to find the center offset
        float totalDepth = numSteps * stepDepth;
        float totalHeight = numSteps * stepHeight;

        // Offset values to shift steps so the parent is in the middle
        float zOffset = (totalDepth / 2f) - (stepDepth / 2f);
        float yOffset = (totalHeight / 2f) - (stepHeight / 2f);

        for (int i = 0; i < numSteps; i++)
        {
            GameObject step = GameObject.CreatePrimitive(PrimitiveType.Cube);
            step.name = $"Step_{i}";
            step.transform.parent = stairParent.transform;

            step.transform.localScale = new Vector3(stepWidth, stepHeight, stepDepth);

            // Calculate local position
            float localY = (i * stepHeight);
            float localZ = (i * stepDepth);

            // Apply the offset so i=0 starts behind/below the pivot 
            // and the middle step sits near the pivot
            step.transform.localPosition = new Vector3(0, localY - yOffset, localZ - zOffset);

            if (stepMaterial != null)
            {
                step.GetComponent<Renderer>().material = stepMaterial;
            }
        }

        Selection.activeGameObject = stairParent;
    }
}