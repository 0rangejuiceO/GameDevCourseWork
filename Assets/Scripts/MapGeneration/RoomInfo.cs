using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [Header("Room Settings")]
    public int roomID;
    public string roomName;
    public Vector3 roomSize;
    public int minDistanceFromSpawn = 0;
}
