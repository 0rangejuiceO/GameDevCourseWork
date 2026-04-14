using UnityEngine;

public class RoomTypeDataset : MonoBehaviour
{
    [Header("Map Settings")]
    [Space(15)]
    [SerializeField] private RoomTypeIntDictionary roomPrefabs = new RoomTypeIntDictionary();
    [Space(150)]
    [SerializeField] private RoomType spawnRoomPrefab;
    public RoomTypeIntDictionary GetRoomPrefabs() { return roomPrefabs; }
    public RoomType GetSpawnRoom() { return spawnRoomPrefab; }


}
