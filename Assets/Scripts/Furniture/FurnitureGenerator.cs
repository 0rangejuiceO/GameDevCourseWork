using UnityEngine;

public class FurnitureGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;

    private FurnitureSpawnPoint[] spawnPoints;

    public void GenerateFurniture()
    {
        GetFurnitureSpawnPoints();

        foreach (FurnitureSpawnPoint spawnPoint in spawnPoints)
        {
            if(Random.Range(0,2) == 1)
            {
                //Debug.Log("Trying to spawn furniture at " + spawnPoint.name);
                Vector3 availableSpace = spawnPoint.cubeSize;

                int index = Random.Range(0, prefabs.Length);
                GameObject prefab = prefabs[index];
                //Debug.Log($"Prefab {prefab.name}");

                Vector3 neededSpace = prefab.GetComponent<FurnitureSize>().cubeSize;

                //Debug.Log($"Available space {availableSpace} , Needed space {neededSpace}");

                //Debug.Log($"Rotation {spawnPoint.gameObject.transform.rotation}");

                Vector3 offset = new Vector3(0,0,0);

                offset.y = -availableSpace.y/2f;
                offset.z = -availableSpace.z / 2f;
                if (spawnPoint.gameObject.transform.rotation.eulerAngles.y == 180)
                {
                    offset.z = -offset.z;
                }
                //Debug.Log($"Offset {offset}");

                if (neededSpace.x <= availableSpace.x && neededSpace.y <= availableSpace.y && neededSpace.z <= availableSpace.z )
                {
                    var furniture = Instantiate(prefab, spawnPoint.transform.position + spawnPoint.cubeOffset+ offset, spawnPoint.gameObject.transform.rotation);
                }

            }
        }

    }

    private void GetFurnitureSpawnPoints()
    {
        spawnPoints = Object.FindObjectsByType<FurnitureSpawnPoint>(FindObjectsSortMode.None);
    }
}
