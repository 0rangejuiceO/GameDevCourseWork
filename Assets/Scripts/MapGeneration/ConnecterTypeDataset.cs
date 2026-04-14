using UnityEngine;

public class ConnecterTypeDataset : MonoBehaviour
{
    [SerializeField] private ConnecterType hallwayPrefab;
    [SerializeField] private ConnecterType hallwayCornerPrefab;
    [SerializeField] private ConnecterType hallwayLargeEntryPrefab;
    [SerializeField] private ConnecterType hallwayThreeJunctionPrefab;
    [SerializeField] private ConnecterType hallwayFourJunctionPrefab;
    [SerializeField] private float hallwayWidth = 5f;
    [SerializeField] private float hallwayHeight = 5f;
    [SerializeField] private float hallwayBaseLength = 5f;
    [SerializeField] private RoomType stairSegment;
    [SerializeField] private int stairSegmentHeight;
    [SerializeField] private int numStairs;
    [SerializeField] private int minStairsDistance;
    [SerializeField] private int maxStairHallwayDistance;
    [SerializeField] private GameObject doorframePrefab;
    [SerializeField] private GameObject doorPrefab;

    public ConnecterType GetHallwayPrefab() {  return hallwayPrefab; }

    public ConnecterType GetHallwayCornerPrefab() { return hallwayCornerPrefab; }
    public ConnecterType GetLargeEntryPrefab() { return hallwayLargeEntryPrefab; }
    public ConnecterType GetThreeJunctionPrefab() { return hallwayThreeJunctionPrefab;}
    public ConnecterType GetFourJunctionPrefab(){ return hallwayFourJunctionPrefab;}

    public float GetHallwayWidth() { return hallwayWidth; }
    public float GetHallwayHeight() { return hallwayHeight; }
    public float GetHallwayBaseLength() {return hallwayBaseLength;}
    public RoomType GetStairSegment() { return stairSegment; }
    public int GetStairSegmentHeight() { return stairSegmentHeight; }
    public int GetNumStairs() { return numStairs; } 
    public int GetMinStairsDistance() { return minStairsDistance; }
    public int GetMaxStairHallwayDistance() { return maxStairHallwayDistance; }
    public GameObject GetDoorframePrefab() { return doorframePrefab;}
    public GameObject GetDoorBodyPrefab() { return doorPrefab;}


}
