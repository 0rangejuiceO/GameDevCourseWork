using UnityEngine;

public class MapSpawnerSettingsDataset : MonoBehaviour
{
    [SerializeField] private float standardDeviation = 10f;
    [SerializeField] private int minHallwayGap = 5;
    [SerializeField] private Vector2 mapSize = new Vector2(100, 100);
    [SerializeField] private int maxPlacementAttempts = 1000;
    [Space(20)]
    [SerializeField]
    private IntFloatDictionary floorNumProbability = new IntFloatDictionary()
    {
        {1, 0.07f},
        {2, 0.18f},
        {3, 0.60f},
        {4, 0.12f},
        {5,0.03f }
    };
    [Space(20)]
    [SerializeField] private int roomHeight = 5;
    [SerializeField] private int minRoomsPerFloor = 4;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, 0.1f, mapSize.y));
    }

    public float GetStandardDeviation() { return standardDeviation; }
    public int GetRoomHeight() { return roomHeight; }
    public int GetMinRoomsPerFloor() { return minRoomsPerFloor; }
    public int GetMinHallwayGap() { return minHallwayGap; }
    public Vector2 GetMapSize() { return mapSize; }
    public int GetMaxPlacementAttempts() { return maxPlacementAttempts; }
    public IntFloatDictionary GetFloorNumProbability() { return floorNumProbability; }

}
