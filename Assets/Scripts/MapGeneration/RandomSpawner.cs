using UnityEngine;

public class ClusteredGridSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    public int spawnCount = 20;
    public float standardDeviation = 10f;
    public float safetyGap = 5.0f;
    public int maxAttemptsPerObject = 50; // Increased to help find spots in clusters

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        // 1. Get the actual size of the prefab's collider or renderer
        Vector3 objectSize = GetBounds(objectToSpawn).size;

        for (int i = 0; i < spawnCount; i++)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxAttemptsPerObject)
            {
                attempts++;

                // 2. Generate and Snap Position
                float rawX = NextGaussian(0, standardDeviation);
                float rawZ = NextGaussian(0, standardDeviation);
                Vector3 snappedPos = new Vector3(Mathf.Round(rawX), 0, Mathf.Round(rawZ));

                // 3. Define Check Area: (Size + Gap) 
                // We use (Size + Gap) / 2 because CheckBox uses 'halfExtents'
                Vector3 checkHalfExtents = (objectSize + (Vector3.one * safetyGap)) / 2f;

                // 4. Perform the check
                // We subtract a tiny margin (0.01) to avoid "edge-to-edge" false positives
                if (!Physics.CheckBox(snappedPos, checkHalfExtents - (Vector3.one * 0.01f), Quaternion.identity))
                {
                    Instantiate(objectToSpawn, snappedPos, Quaternion.identity);

                    // CRITICAL: Tell Unity to update the physics world immediately 
                    // so the next 'CheckBox' knows this object is here.
                    Physics.SyncTransforms();

                    placed = true;
                }
            }
        }
    }

    Bounds GetBounds(GameObject obj)
    {
        var renderer = obj.GetComponentInChildren<Renderer>();
        return renderer != null ? renderer.bounds : new Bounds(Vector3.zero, Vector3.one);
    }

    float NextGaussian(float mean, float stdDev)
    {
        float v1 = Random.value;
        float v2 = Random.value;
        float stdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(v1)) * Mathf.Sin(2.0f * Mathf.PI * v2);
        return mean + stdDev * stdNormal;
    }
}