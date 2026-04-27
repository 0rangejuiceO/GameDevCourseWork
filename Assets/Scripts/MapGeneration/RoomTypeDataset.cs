using System.Collections.Generic;
using UnityEngine;

public class RoomTypeDataset : MonoBehaviour
{
    [Header("Map Settings")]
    public List<RoomType> roomPrefabs = new List<RoomType>();
    [SerializeField] private RoomType spawnRoomPrefab;
    public List<RoomType> GetRoomPrefabs() { return roomPrefabs; }
    public RoomType GetSpawnRoom() { return spawnRoomPrefab; }


}
