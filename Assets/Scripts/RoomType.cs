using UnityEngine;

[CreateAssetMenu(fileName = "RoomType", menuName = "Map/Room Type")]

public class RoomType : ScriptableObject
{
    public string displayName;
    public GameObject roomPrefab;
    public GameObject actualPrefab;
    public GameObject actualPrefabInverted;
    public int minDistanceFromSpawn = 0;
    public int minDistanceFromSameType = 0;
}
