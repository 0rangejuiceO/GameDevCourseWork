using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    [Space(15)]
    [SerializeField] private RoomTypeIntDictionary roomPrefabs = new RoomTypeIntDictionary();
    [Space(20)]
    [SerializeField] private float standardDeviation = 10f;
    [SerializeField] private int minHallwayGap = 5;
    [SerializeField] private Vector2 mapSize = new Vector2(100, 100);
    [SerializeField] private int maxPlacementAttempts = 1000;
    [SerializeField] private RoomType spawnRoomPrefab;
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
    public bool doReplacement = false;



    private Dictionary<RoomType, List<GameObject>> roomObjectLists = new Dictionary<RoomType, List<GameObject>>();
    private Dictionary<RoomType, List<GameObject>> actualRoomObjectLists = new Dictionary<RoomType, List<GameObject>>();
    private List<GameObject> stairsList = new List<GameObject>();
    private Dictionary<int, int> floorYPositions;
    private int totalRoomsNum = 0;
    private int numFloors = 0;
    private int spawnFloor = 0;
    private int floorHeightZero = 0;
    private bool stairsInverted = false;
    private GameObject spawnRoom;
    private Dictionary<ConnecterType, List<GameObject>> connecterObjectLists = new Dictionary<ConnecterType, List<GameObject>>();
    private Dictionary<int, List<RoomNode>> floorRooms = new Dictionary<int, List<RoomNode>>();

    public async Task<bool> BeginMapGeneration()
    {


        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        await Task.Yield();

        Physics.SyncTransforms();

        roomObjectLists = new Dictionary<RoomType, List<GameObject>>();
        actualRoomObjectLists = new Dictionary<RoomType, List<GameObject>>();
        stairsList = new List<GameObject>();
        floorYPositions = new Dictionary<int, int>();
        totalRoomsNum = 0;
        numFloors = 0;
        spawnFloor = 0;
        floorHeightZero = 0;
        stairsInverted = false;
        spawnRoom = null;
        connecterObjectLists = new Dictionary<ConnecterType, List<GameObject>>();
        floorRooms = new Dictionary<int, List<RoomNode>>();

        totalRoomsNum = roomPrefabs.Values.Sum();

        foreach (RoomType key in roomPrefabs.Keys)
        {
            roomObjectLists.Add(key, new List<GameObject>());
        }

        connecterObjectLists.Add(hallwayPrefab, new List<GameObject>());
        connecterObjectLists.Add(hallwayCornerPrefab, new List<GameObject>());
        connecterObjectLists.Add(hallwayLargeEntryPrefab, new List<GameObject>());
        connecterObjectLists.Add(hallwayThreeJunctionPrefab, new List<GameObject>());
        connecterObjectLists.Add(hallwayFourJunctionPrefab, new List<GameObject>());

        numFloors = SetNumOfFloors();

        spawnFloor = Random.Range(1, numFloors + 1);

        floorHeightZero = Random.Range(1, numFloors + 1);

        floorYPositions = new Dictionary<int, int>();

        for (int i = 1; i <= numFloors; i++)
        {
            float yPos = (i - floorHeightZero) * roomHeight;

            Vector3 floorPosition = new Vector3(0, yPos, 0);

            floorYPositions.Add(i, (int)yPos);
            Debug.Log($"Floor {i} Y Position: {yPos}");
        }

        for (int i = 1; i <= numFloors; i++)
        {
            floorRooms.Add(i, new List<RoomNode>());
        }



        var newRoom = Instantiate(spawnRoomPrefab.roomPrefab, Vector3.zero, Quaternion.identity, transform);
        spawnRoom = newRoom;
        floorRooms[floorHeightZero].Add(new RoomNode(newRoom));


        if (numFloors > 1)
        {
            MakeStairs();

        }
        GenerateMap();
        if (numFloors > 1)
        {
            MakeStairHallway();
        }
        bool failed = MakeHallways();

        if (failed)
        {
            Debug.LogError("Failed to create hallways");


            return false;
        }

        if (doReplacement)
        {
            Debug.Log("Replacing Generations Rooms with Actual Rooms");
            DoReplacement();
            OpenDoors();
        }

        await Task.Yield();
        Debug.Log("Map Generation Complete");

        return true;
    }

    private void DoReplacement()
    {
        foreach (RoomType key in roomPrefabs.Keys)
        {
            actualRoomObjectLists.Add(key, new List<GameObject>());
        }



        Vector3 position = spawnRoom.transform.position;
        Quaternion rotation = spawnRoom.transform.rotation;
        Transform parent = spawnRoom.transform.parent;
        List<string> doorsToOpen = spawnRoom.GetComponent<DoorHandler>().doorsToOpen;
        Destroy(spawnRoom);
        var newRoom = Instantiate(spawnRoomPrefab.actualPrefab, position, rotation, parent);
        spawnRoom = newRoom;
        newRoom.GetComponent<DoorHandler>().doorsToOpen = doorsToOpen;

        foreach (var roomList in roomObjectLists)
        {
            foreach (GameObject room in roomList.Value)
            {
                position = room.transform.position;
                rotation = room.transform.rotation;
                parent = room.transform.parent;
                doorsToOpen = room.GetComponent<DoorHandler>().doorsToOpen;
                Destroy(room);
                newRoom = Instantiate(roomList.Key.actualPrefab, position, rotation, parent);
                actualRoomObjectLists[roomList.Key].Add(newRoom);
                newRoom.GetComponent<DoorHandler>().doorsToOpen = doorsToOpen;
            }
        }

        Debug.Log($"Replacing {connecterObjectLists[hallwayPrefab].Count} hallways, {connecterObjectLists[hallwayCornerPrefab].Count} corners, {connecterObjectLists[hallwayLargeEntryPrefab].Count} large entries, {connecterObjectLists[hallwayThreeJunctionPrefab].Count} three junctions, and {connecterObjectLists[hallwayFourJunctionPrefab].Count} four junctions.");

        foreach (var connecterList in connecterObjectLists)
        {
            Vector3 originalScale = connecterList.Key.creationPrefab.transform.localScale;
            foreach (GameObject connecter in connecterList.Value)
            {
                position = connecter.transform.position;
                rotation = connecter.transform.rotation;
                Vector3 scale = connecter.transform.localScale;
                parent = connecter.transform.parent;
                LargeEntryCreationHandler creationHandler = null;
                if (connecter.name.Contains("LargeEntry"))
                {
                    creationHandler = connecter.GetComponent<LargeEntryCreationHandler>();
                }
                Destroy(connecter);
                var newConnecter = Instantiate(connecterList.Key.actualPrefab, position, rotation, parent);

                float x = scale.x / originalScale.x;
                float y = scale.y / originalScale.y;
                float z = scale.z / originalScale.z;

                Vector3 newScale = new Vector3(x, y, z);

                //Debug.Log($"Original Scale: {originalScale}, Current Scale: {scale}, New Scale: {newScale}");

                if (connecter.name.Contains("LargeEntry") && creationHandler != null)
                {
                    LargeEntryHandler handler = newConnecter.GetComponent<LargeEntryHandler>();
                    handler.scaler.localScale = newScale;
                    handler.setDoorFrameScale(true, creationHandler.entryLocation, creationHandler.direction);

                    handler.setDoorFrameScale(false, creationHandler.exitLocation, creationHandler.direction * -1);

                }
                else
                {
                    newConnecter.transform.localScale = newScale;
                }


            }
        }

        actualRoomObjectLists.Add(stairSegment, new List<GameObject>());

        foreach (GameObject stair in stairsList)
        {
            position = stair.transform.position;
            rotation = stair.transform.rotation;
            parent = stair.transform.parent;
            doorsToOpen = stair.GetComponent<DoorHandler>().doorsToOpen;
            bool isTopStair = stair.GetComponent<StairCreationHandler>().isTopStair;
            bool isBottomStair = stair.GetComponent<StairCreationHandler>().isBottomStair;
            Destroy(stair);
            if (stairsInverted)
            {
                newRoom = Instantiate(stairSegment.actualPrefabInverted, position, rotation, parent);
            }
            else
            {
                newRoom = Instantiate(stairSegment.actualPrefab, position, rotation, parent);
            }

            actualRoomObjectLists[stairSegment].Add(newRoom);
            newRoom.GetComponent<DoorHandler>().doorsToOpen = doorsToOpen;
            if (isTopStair)
            {
                newRoom.GetComponent<StairHandler>().isTopCeiling();
            }
            if (isBottomStair)
            {
                newRoom.GetComponent<StairHandler>().isBottomFloor();
            }
        }
    }

    private bool MakeHallways()
    {
        for (int i = 1; i < numFloors + 1; i++)
        {
            var edges = GenerateMST(floorRooms[i]);

            foreach (var edge in edges)
            {
                bool failed = CreateHallway(edge.Item1, edge.Item2);

                if (failed)
                {
                    return true;
                }
            }
        }

        return false;

    }

    private void MakeStairs()
    {


        for (int i = 0; i < numStairs; i++)
        {
            //Debug.Log($"Creating Stair {i}/{numStairs}");

            bool placed = false;
            int attempts = 0;
            float currentStandardDeviation = standardDeviation / 10f;


            while (!placed && (attempts < maxPlacementAttempts))
            {
                attempts++;

                if (attempts >= maxPlacementAttempts)
                {
                    Debug.LogWarning($"Failed to place stairs {maxPlacementAttempts} attempts.");
                    break;
                }

                float rawX = NextGaussian(0, currentStandardDeviation);
                float rawZ = NextGaussian(0, currentStandardDeviation);

                float clampedX = Mathf.Clamp(Mathf.Round(rawX), -mapSize.x / 2, mapSize.x / 2);
                float clampedZ = Mathf.Clamp(Mathf.Round(rawZ), -mapSize.y / 2, mapSize.y / 2);



                Vector3 snappedPosition = new Vector3(clampedX, 0, clampedZ);

                bool tooCloseToSameType = false;
                if (stairsList.Count > 0)
                {
                    foreach (GameObject existingRoom in stairsList)
                    {
                        if (Vector3.Distance(existingRoom.transform.position, snappedPosition) < minStairsDistance)
                        {
                            tooCloseToSameType = true;
                        }
                    }
                }

                if (tooCloseToSameType)
                {
                    //Debug.Log($"Attempt {attempts}: Position too close to same type (" + clampedX + "," + clampedZ + "), sd " + currentStandardDeviation + " retrying...");
                    currentStandardDeviation *= 1.05f;
                    continue;
                }

                int totalHeight = (numFloors - 1) * roomHeight + stairSegmentHeight;

                Vector3 checkHalfExtents = (stairSegment.roomPrefab.transform.localScale / 2) + (Vector3.one * minHallwayGap);


                checkHalfExtents.y = totalHeight;


                if (!Physics.CheckBox(snappedPosition, checkHalfExtents - (Vector3.one * 0.01f), Quaternion.identity))
                {
                    for (int m = 0; m < numFloors; m++)
                    {
                        for (int n = 0; n < roomHeight / stairSegmentHeight; n++)
                        {
                            snappedPosition.y = floorYPositions[m + 1] + n * stairSegmentHeight;

                            var newStairs = Instantiate(stairSegment.roomPrefab, snappedPosition, Quaternion.identity, transform);
                            stairsList.Add(newStairs);
                            if (n == 0)
                            {
                                floorRooms[m + 1].Add(new RoomNode(newStairs));
                            }

                            if (m == numFloors - 1)
                            {
                                newStairs.GetComponent<StairCreationHandler>().isTopStair = true;
                                break;
                            }
                            if (m == 0 && n == 0)
                            {
                                newStairs.GetComponent<StairCreationHandler>().isBottomStair = true;
                            }
                        }

                    }


                    Physics.SyncTransforms();
                    placed = true;
                    //Debug.Log($"Placed {room.displayName} at {snappedPosition} after {attempts} attempts.");
                }

                currentStandardDeviation *= 1.02f;

            }
        }

    }

    private void MakeStairHallway()
    {
        // 1. Group all stair segments by their XZ position to identify unique shafts
        var stairShafts = stairsList
            .GroupBy(s => new Vector2(s.transform.position.x, s.transform.position.z))
            .ToList();

        foreach (var shaft in stairShafts)
        {
            // Track connections for this specific vertical shaft
            List<(int floorIndex, RoomNode closestRoom, float distance)> potentialConnections = new List<(int, RoomNode, float)>();

            for (int i = 1; i <= numFloors; i++)
            {
                if (!floorRooms.ContainsKey(i) || floorRooms[i].Count == 0) continue;

                // Find the closest room on this floor to the shaft's horizontal position
                Vector3 shaftPos = new Vector3(shaft.Key.x, floorYPositions[i], shaft.Key.y);

                var closest = floorRooms[i]
                    .Select(room => new { room, dist = Vector3.Distance(room.roomObject.transform.position, shaftPos) })
                    .OrderBy(x => x.dist)
                    .FirstOrDefault();

                if (closest != null)
                {
                    potentialConnections.Add((i, closest.room, closest.dist));
                }
            }

            // 2. Sort connections by distance to find the "Top 2" closest rooms
            var sortedConnections = potentialConnections.OrderBy(c => c.distance).ToList();

            for (int j = 0; j < sortedConnections.Count; j++)
            {
                var connection = sortedConnections[j];

                // Condition: Connect if it's one of the 2 closest floors OR distance is < 150
                if (j < 2 || connection.distance <= maxStairHallwayDistance)
                {
                    // Find the specific stair segment object at this floor's height
                    GameObject segmentOnFloor = shaft.FirstOrDefault(s =>
                        Mathf.Approximately(s.transform.position.y, floorYPositions[connection.floorIndex]));

                    if (segmentOnFloor != null)
                    {
                        CreateHallway(connection.closestRoom, new RoomNode(segmentOnFloor));
                    }
                }
            }
        }
    }

    private void GenerateMap()
    {


        int[] floorBuckets = new int[numFloors];

        int remainingRooms = totalRoomsNum - (numFloors * minRoomsPerFloor);

        for (int i = 0; i < numFloors; i++)
        {
            floorBuckets[i] = minRoomsPerFloor;
        }

        for (int i = 0; i < remainingRooms; i++)
        {
            int randomFloor = Random.Range(0, numFloors);

            floorBuckets[randomFloor]++;
        }


        List<RoomType> roomTypes = new List<RoomType>();
        foreach (var kvp in roomPrefabs)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                roomTypes.Add(kvp.Key);
            }
        }

        Shuffle(roomTypes);

        int currentFloor = 0;
        foreach (RoomType room in roomTypes)
        {
            bool placed = false;
            int attempts = 0;
            float currentStandardDeviation = standardDeviation;
            while (!placed && (attempts < maxPlacementAttempts))
            {
                attempts++;

                if (attempts >= maxPlacementAttempts)
                {
                    Debug.LogWarning($"Failed to place {room.displayName} after {maxPlacementAttempts} attempts.");
                    break;
                }

                float rawX = NextGaussian(0, currentStandardDeviation);
                float rawZ = NextGaussian(0, currentStandardDeviation);

                float clampedX = Mathf.Clamp(Mathf.Round(rawX), -mapSize.x / 2, mapSize.x / 2);
                float clampedZ = Mathf.Clamp(Mathf.Round(rawZ), -mapSize.y / 2, mapSize.y / 2);

                if (new Vector2(clampedX, clampedZ).magnitude < room.minDistanceFromSpawn)
                {
                    //Debug.Log($"Attempt {attempts}: Position too close to spawn ("+clampedX+","+clampedZ + "), sd "+currentStandardDeviation+" retrying...");
                    currentStandardDeviation *= 1.05f;
                    continue;
                }

                Vector3 snappedPosition = new Vector3(clampedX, 0, clampedZ);

                bool tooCloseToSameType = false;
                if (roomObjectLists[room].Count > 0)
                {
                    foreach (GameObject existingRoom in roomObjectLists[room])
                    {
                        if (Vector3.Distance(existingRoom.transform.position, snappedPosition) < room.minDistanceFromSameType)
                        {
                            tooCloseToSameType = true;
                        }
                    }
                }

                if (tooCloseToSameType)
                {
                    //Debug.Log($"Attempt {attempts}: Position too close to same type (" + clampedX + "," + clampedZ + "), sd " + currentStandardDeviation + " retrying...");
                    currentStandardDeviation *= 1.05f;
                    continue;
                }

                if (floorBuckets[currentFloor] <= 0)
                {
                    currentFloor++;
                    if (currentFloor >= numFloors)
                    {
                        Debug.LogWarning("All floors are full, but there are still rooms to place.");
                        break;
                    }
                }

                snappedPosition.y = floorYPositions[currentFloor + 1];

                Vector3 checkHalfExtents = (room.roomPrefab.transform.localScale / 2) + (Vector3.one * minHallwayGap);




                if (!Physics.CheckBox(snappedPosition, checkHalfExtents - (Vector3.one * 0.01f), Quaternion.identity))
                {
                    var newRoom = Instantiate(room.roomPrefab, snappedPosition, Quaternion.identity, transform);
                    roomObjectLists[room].Add(newRoom);
                    floorRooms[currentFloor + 1].Add(new RoomNode(newRoom));

                    Physics.SyncTransforms();
                    placed = true;
                    floorBuckets[currentFloor]--;
                    //Debug.Log($"Placed {room.displayName} at {snappedPosition} after {attempts} attempts.");
                }

                currentStandardDeviation *= 1.05f;

            }
        }

    }

    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    float NextGaussian(float mean, float stdDev)
    {
        float v1 = Random.value;
        float v2 = Random.value;
        float stdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(v1)) * Mathf.Sin(2.0f * Mathf.PI * v2);
        return mean + stdDev * stdNormal;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(mapSize.x, 0.1f, mapSize.y));
    }

    private int SetNumOfFloors()
    {
        float totalWeight = 0f;
        foreach (float weight in floorNumProbability.Values)
        {
            totalWeight += weight;
        }


        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in floorNumProbability)
        {
            cumulative += entry.Value;

            if (rand <= cumulative)
            {
                Debug.Log($"Selected number of floors: {entry.Key}");
                return entry.Key;
            }
        }

        return 5;
    }


    private List<(RoomNode, RoomNode)> GenerateMST(List<RoomNode> nodes)
    {
        if (nodes.Count == 0)
            return new List<(RoomNode, RoomNode)>();

        List<RoomNode> connected = new List<RoomNode>();
        List<RoomNode> unconnected = new List<RoomNode>(nodes);
        List<(RoomNode, RoomNode)> edges = new List<(RoomNode, RoomNode)>();

        connected.Add(unconnected[0]);
        unconnected.RemoveAt(0);

        while (unconnected.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            RoomNode from = null;
            RoomNode to = null;

            foreach (RoomNode c in connected)
            {
                foreach (RoomNode u in unconnected)
                {
                    // Use the room's actual position
                    float dist = Vector3.Distance(c.roomObject.transform.position, u.roomObject.transform.position);
                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        from = c;
                        to = u;
                    }
                }
            }

            if (from != null && to != null)
            {
                edges.Add((from, to));
                connected.Add(to);
                unconnected.Remove(to);
            }
            else
            {
                Debug.LogWarning("MST generation: no valid edge found. This should not happen.");
                break;
            }
        }

        return edges;
    }


    private bool CreateStraightHallway(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;
        float length = dir.magnitude;

        if (length < 0.1f)
            return true;

        Vector3 mid = (start + end) * 0.5f;
        Quaternion rotation = Quaternion.LookRotation(dir);


        Vector3 checkHalfExtents = new Vector3(
            (hallwayPrefab.creationPrefab.transform.localScale.x),
            (hallwayPrefab.creationPrefab.transform.localScale.y),
            (length * 0.5f) // Half the calculated length
        );

        float hallwayXScale = hallwayPrefab.creationPrefab.transform.localScale.x / 2;
        float hallwayZScale = hallwayPrefab.creationPrefab.transform.localScale.z / 2;

        Collider[] hits = Physics.OverlapBox(mid, checkHalfExtents - (Vector3.one * 0.1f), rotation);

        bool cornerOverlap = false, dontCreate = false;
        Vector3 cornerOverlapPosition = Vector3.zero;

        string holdLog = "";

        if (hits.Length > 0)
        {
            foreach (Collider col in hits)
            {
                holdLog += ($"\nOverlap detected with: {col.gameObject.name} at {col.transform.position} from Hallway at {mid}");



                float roomXScale = col.gameObject.transform.localScale.x / 2;
                float roomZScale = col.gameObject.transform.localScale.z / 2;

                float roomXTop = col.transform.position.x + roomXScale;
                float roomXBottom = col.transform.position.x - roomXScale;

                float roomZLeft = col.transform.position.z + roomZScale;
                float roomZRight = col.transform.position.z - roomZScale;

                float hallwayXTop = 0, hallwayXBottom = 0, hallwayZLeft = 0, hallwayZRight = 0;

                if (rotation.eulerAngles.y == 0 || rotation.eulerAngles.y == 180)
                {
                    holdLog += ($"\nHallway direction: {(dir.z > 0 ? "Forward" : "Backward")}");

                    hallwayXTop = mid.x + hallwayXScale - 0.1f;
                    hallwayXBottom = mid.x - hallwayXScale + 0.1f;


                    hallwayZLeft = mid.z + hallwayZScale - 0.1f;
                    hallwayZRight = mid.z - hallwayZScale + 0.1f;





                }
                else if (rotation.eulerAngles.y == 90 || rotation.eulerAngles.y == 270)
                {
                    holdLog += ($"\nHallway direction: {(dir.x > 0 ? "Right" : "Left")}");

                    hallwayXTop = mid.x + hallwayZScale - 0.1f;
                    hallwayXBottom = mid.x - hallwayZScale + 0.1f;


                    hallwayZLeft = mid.z + hallwayXScale - 0.1f;
                    hallwayZRight = mid.z - hallwayXScale + 0.1f;

                }



                bool xOverlap = false;
                bool zOverlap = false;

                holdLog += ($"\nHallway X Top: {hallwayXTop} Room X Bottom: {roomXBottom}");

                holdLog += ($"\nHallway X Bottom: {hallwayXBottom} Room X Top: {roomXTop}");

                holdLog += ($"\nHallway Z Left: {hallwayZLeft} Room Z Right: {roomZRight}");

                holdLog += ($"\nHallway Z Right: {hallwayZRight} Room Z Left: {roomZLeft}");

                if (mid.x > col.transform.position.x)
                {
                    holdLog += ($"\nHallway above room");

                    if (hallwayXBottom < roomXTop)
                    {
                        holdLog += ($"\nBottom of hallway overlaps with top of room!");
                        xOverlap = true;
                    }
                }
                else
                {
                    holdLog += ($"\nHallway below room");

                    if (hallwayXTop > roomXBottom)
                    {
                        holdLog += ($"\nTop of hallway overlaps with bottom of room!");
                        xOverlap = true;
                    }
                }

                if (mid.z > col.transform.position.z)
                {
                    holdLog += ($"\nHallway is Left of room");

                    if (hallwayZRight < roomZLeft)
                    {
                        holdLog += ($"\nRight of hallway overlaps with left of room!");
                        zOverlap = true;
                    }

                }
                else
                {
                    holdLog += ($"\nHallway is Right of room");
                    if (hallwayZLeft > roomZRight)
                    {
                        holdLog += ($"\nLeft of hallway overlaps with right of room!");
                        zOverlap = true;
                    }

                }

                if (xOverlap && zOverlap)
                {
                    Debug.LogWarning(holdLog);

                    Debug.LogWarning($"Hallway overlaps with room on both X and Z axes!\nRan into {col.gameObject.name} located at {col.transform.position}");
                    if (col.gameObject.name.Contains("Corner"))
                    {
                        cornerOverlap = true;
                        cornerOverlapPosition = col.transform.position;
                        CreateThreeJunction(col.gameObject);
                    }
                    else if (col.gameObject.name.Contains("Hallway"))
                    {
                        if (col.transform.position == mid && col.transform.localScale.z == length * 0.5f)
                        {
                            Debug.LogWarning("Two identical hallways overlapping.");
                            dontCreate = true;
                        }
                        else
                        {
                            Debug.LogError("Hallway overlaps with another hallway. This should not happen. Check the map generation logic for errors.");

                        }

                    }
                    else
                    {
                        Debug.LogError("Hallway overlaps with non-corner object. This should not happen. Check the map generation logic for errors.");
                    }
                    return true;
                }

            }
        }

        if (cornerOverlap)
        {
            Debug.Log($"Hallway overlaps with corner at {cornerOverlapPosition}. Adjusting hallway to create a junction.");

            end = cornerOverlapPosition - (dir.normalized * (hallwayCornerPrefab.creationPrefab.transform.localScale.x / 2f));

            dir = end - start;
            length = dir.magnitude;

            if (length < 0.1f)
                return true;

            mid = (start + end) * 0.5f;
        }
        if (!dontCreate)
        {
            GameObject hallway = Instantiate(hallwayPrefab.creationPrefab, mid, Quaternion.LookRotation(dir), transform);
            connecterObjectLists[hallwayPrefab].Add(hallway);

            hallway.transform.localScale = new Vector3(hallway.transform.localScale.x, hallway.transform.localScale.y, length / hallwayBaseLength);
            hallway.GetComponent<Hallway>().start = start;
            hallway.GetComponent<Hallway>().end = end;
        }

        return false;

    }

    private bool CreateHallway(RoomNode a, RoomNode b)
    {
        bool failed = false;

        if (a == null || b == null || (a.Position == b.Position))
        {
            return true;
        }

        Vector3 start = GetRoomEdgePoint(a.roomObject, b.Position);
        Vector3 end = GetRoomEdgePoint(b.roomObject, a.Position);

        start.y = a.roomObject.transform.position.y;
        end.y = b.roomObject.transform.position.y;


        float xDistance = Mathf.Abs(start.x - end.x);
        float zDistance = Mathf.Abs(start.z - end.z);



        Vector3 corner = new Vector3(end.x, start.y, start.z);


        float cornerOffset = hallwayWidth;


        float roomOffset = hallwayWidth / 2f;

        Vector3 dirToCorner = (corner - start).normalized;
        Vector3 dirFromCorner = (end - corner).normalized;
        Vector3 dirFromStartRoom = (start - a.Position).normalized;
        Vector3 dirToEndRoom = (end - b.Position).normalized;

        string[] colours = new string[] { "Red", "Green", "Blue", "Yellow" };
        Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };

        string startDoor = "unknown";
        string endDoor = "unknown";

        for (int i = 0; i < directions.Length; i++)
        {
            if (dirFromStartRoom == directions[i])
            {
                startDoor = colours[i];
                a.roomObject.GetComponent<DoorHandler>().doorsToOpen.Add(colours[i]);
            }
        }

        for (int i = 0; i < directions.Length; i++)
        {
            if (dirToEndRoom == directions[i])
            {
                endDoor = colours[i];
                b.roomObject.GetComponent<DoorHandler>().doorsToOpen.Add(colours[i]);
            }
        }

        //if (a.roomObject.name.Contains("Stair") || b.roomObject.name.Contains("Stair"))
        //{
        //    Debug.Log($"Predicted door Colours: Start {startDoor}, End {endDoor} for rooms {a.roomObject.name} at {a.Position} and {b.roomObject.name} at {b.Position}");
        //}

        //Debug.Log($"{dirToCorner}   | {dirFromCorner}");


        if (xDistance <= 5f || zDistance <= 5f)
        {
            //Debug.Log($"Creating large entry between {a.roomObject.name} and {b.roomObject.name} due to short distance (x: {xDistance}, z: {zDistance}) direction: {dirFromStartRoom}");
            CreateLargeEntry(((start + end) / 2f), dirFromStartRoom, xDistance, zDistance, start, end);
            return false;
        }

        if (dirToCorner == new Vector3(1, 0, 0) || dirToCorner == new Vector3(-1, 0, 0))
        {



            start += dirFromCorner * roomOffset;
            corner = new Vector3(end.x, start.y, start.z);

            dirToCorner = (corner - start).normalized;
            dirFromCorner = (end - corner).normalized;
        }


        Vector3 adjStart = start;

        if (dirFromStartRoom != dirToCorner)
        {
            failed = CreateCorner(start, a.roomObject.transform.position, corner, "ExitCorner");
            if(failed)
            {
                return true;
            }
            adjStart = start + (dirToCorner * roomOffset);
        }
        else
        {

            adjStart = start - (dirFromCorner * roomOffset);
            corner = corner - (dirToCorner * roomOffset);
            corner = corner - (dirFromCorner * roomOffset);
        }


        Vector3 segment1End = corner - (dirToCorner * roomOffset);



        Vector3 segment2Start = corner + (dirFromCorner * roomOffset);
        Vector3 adjEnd = end + (dirToEndRoom * roomOffset);


        //Corner corner
        failed = CreateCorner(corner, segment1End, segment2Start, "Corner");

        if (failed)
        {
            return true; 
        }

        //Debug.Log((dirToCorner * roomOffset) + "  |   " + (dirToCorner * cornerOffset) + "  |   " + (dirFromCorner * cornerOffset) + "  |   " + (dirToCorner * roomOffset));
        //Debug.Log($"Start: {start}, Segment1End: {segment1End}, Corner: {corner}, Segment2Start: {segment2Start}, End: {end}, AdjEnd: {adjEnd}");


        if (dirToEndRoom != -dirFromCorner)
        {
            //Debug.Log($"dirToEndRoom: {dirToEndRoom}  dirFromCorner: {dirFromCorner}");
            //Entry Corner
            failed = CreateCorner(adjEnd, segment2Start, end, "EntryCorner");
            if(failed)
            {
                return true;
            }

            adjEnd = adjEnd - (dirFromCorner * roomOffset);
        }
        else
        {
            adjEnd = end;
        }


        if (Vector3.Distance(adjStart, segment1End) > 0.1f)
            failed = CreateStraightHallway(adjStart, segment1End);

        if (failed)
        {
            return true;
        }

        //Segment 2
        if (Vector3.Distance(segment2Start, adjEnd) > 0.1f)
            failed = CreateStraightHallway(segment2Start, adjEnd);

        return failed;
    }


    Vector3 GetRoomEdgePoint(GameObject room, Vector3 targetPosition)
    {

        // Compute world-space center
        Vector3 center = room.transform.position;

        // Create bounds manually
        Bounds bounds = new Bounds(center, room.transform.localScale);

        Vector3 direction = (targetPosition - bounds.center).normalized;

        float xExtent = bounds.extents.x;
        float zExtent = bounds.extents.z;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            // Exit on left or right side
            return bounds.center + new Vector3(
                Mathf.Sign(direction.x) * xExtent,
                0,
                0
            );
        }
        else
        {
            // Exit on top or bottom
            return bounds.center + new Vector3(
                0,
                0,
                Mathf.Sign(direction.z) * zExtent
            );
        }
    }

    private bool CreateCorner(Vector3 cornerPos, Vector3 from, Vector3 to, string newName)
    {

        Vector3 dir1 = (from - cornerPos).normalized;
        Vector3 dir2 = (to - cornerPos).normalized;

        //Debug.Log("Dir1: " + dir1 + " | Dir2: " + dir2);

        dir1.y = 0;
        dir2.y = 0;

        float angle = 0;

        if ((dir1 == new Vector3(1, 0, 0) || dir2 == new Vector3(1, 0, 0)) && (dir2 == new Vector3(0, 0, -1) || dir1 == new Vector3(0, 0, -1)))
        {
            angle = 90;
        }
        else if ((dir1 == new Vector3(1, 0, 0) || dir2 == new Vector3(1, 0, 0)) && (dir2 == new Vector3(0, 0, 1) || dir1 == new Vector3(0, 0, 1)))
        {
            angle = 0;
        }
        else if ((dir1 == new Vector3(-1, 0, 0) || dir2 == new Vector3(-1, 0, 0)) && (dir2 == new Vector3(0, 0, -1) || dir1 == new Vector3(0, 0, -1)))
        {
            angle = 180;
        }
        else if ((dir1 == new Vector3(-1, 0, 0) || dir2 == new Vector3(-1, 0, 0)) && (dir2 == new Vector3(0, 0, 1) || dir1 == new Vector3(0, 0, 1)))
        {
            angle = 270;
        }
        else
        {
            Debug.LogWarning($"Unexpected corner direction combination: dir1 {dir1}, dir2 {dir2}. Defaulting angle to 0.");
        }


        Quaternion rotation = Quaternion.Euler(0, angle, 0);

        Vector3 checkHalfExtents = (hallwayCornerPrefab.creationPrefab.transform.localScale / 2) + (Vector3.one * minHallwayGap);


        float hallwayXScale = hallwayCornerPrefab.creationPrefab.transform.localScale.x / 2;
        float hallwayZScale = hallwayCornerPrefab.creationPrefab.transform.localScale.z / 2;

        Collider[] hits = Physics.OverlapBox(cornerPos, checkHalfExtents - (Vector3.one * 0.01f), Quaternion.identity);


        string holdLog = "";

        foreach (Collider col in hits)
        {
            holdLog += ($"\nCorner overlap detected with {col.gameObject.name} at {col.transform.position} when trying to place corner at {cornerPos} with rotation {rotation.eulerAngles}");




            float roomXScale = col.gameObject.transform.localScale.x / 2;
            float roomZScale = col.gameObject.transform.localScale.z / 2;

            float roomXTop = col.transform.position.x + roomXScale;
            float roomXBottom = col.transform.position.x - roomXScale;

            float roomZLeft = col.transform.position.z + roomZScale;
            float roomZRight = col.transform.position.z - roomZScale;

            float hallwayXTop = 0, hallwayXBottom = 0, hallwayZLeft = 0, hallwayZRight = 0;

            hallwayXTop = cornerPos.x + hallwayXScale - 0.1f;
            hallwayXBottom = cornerPos.x - hallwayXScale + 0.1f;


            hallwayZLeft = cornerPos.z + hallwayZScale - 0.1f;
            hallwayZRight = cornerPos.z - hallwayZScale + 0.1f;

            bool xOverlap = false;
            bool zOverlap = false;

            if (cornerPos.x > col.transform.position.x)
            {
                holdLog += ($"\nHallway above room");

                if (hallwayXBottom < roomXTop)
                {
                    holdLog += ($"\nBottom of hallway overlaps with top of room!");
                    xOverlap = true;
                }
            }
            else
            {
                holdLog += ($"\nHallway below room");

                if (hallwayXTop > roomXBottom)
                {
                    holdLog += ($"\nTop of hallway overlaps with bottom of room!");
                    xOverlap = true;
                }
            }

            if (cornerPos.z > col.transform.position.z)
            {
                holdLog += ($"\nHallway is Left of room");

                if (hallwayZRight < roomZLeft)
                {
                    holdLog += ($"\nRight of hallway overlaps with left of room!");
                    zOverlap = true;
                }

            }
            else
            {
                holdLog += ($"\nHallway is Right of room");
                if (hallwayZLeft > roomZRight)
                {
                    holdLog += ($"\nLeft of hallway overlaps with right of room!");
                    zOverlap = true;
                }

            }

            if ((xOverlap && zOverlap))
            {
                Debug.LogWarning(holdLog);
                Debug.LogWarning($"Corner overlaps with room on both X and Z axes!\nRan into {col.gameObject.name} located at {col.transform.position}");

                if (col.transform.position == cornerPos)
                {
                    var threeJunction = Instantiate(hallwayThreeJunctionPrefab.creationPrefab, cornerPos, rotation, transform);
                    Debug.LogWarning($"Exact overlap with corner detected. Placing three junction at {cornerPos} instead of regular corner.");
                }
                else
                {
                    Debug.LogError("Corner overlaps with non-corner object. This should not happen. Check the map generation logic for errors.");
                }

                return true;
            }


        }

        var corner = Instantiate(hallwayCornerPrefab.creationPrefab, cornerPos, rotation, transform);
        connecterObjectLists[hallwayCornerPrefab].Add(corner);
        corner.name = newName;


        return false;
    }

    void ChangeHallway(GameObject hallway, Vector3 newStart, Vector3 newEnd)
    {
        Vector3 dir = newEnd - newStart;
        float length = dir.magnitude;
        if (length < 0.1f)
            return;
        Vector3 mid = (newStart + newEnd) * 0.5f;
        hallway.transform.position = mid;
        hallway.transform.rotation = Quaternion.LookRotation(dir);
        hallway.transform.localScale = new Vector3(hallway.transform.localScale.x, hallway.transform.localScale.y, length / hallwayBaseLength);
        hallway.GetComponent<Hallway>().start = newStart;
        hallway.GetComponent<Hallway>().end = newEnd;
    }

    void CreateLargeEntry(Vector3 position, Vector3 direction, float XScale, float ZScale, Vector3 exitLocation, Vector3 entryLocation)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);



        if (direction == new Vector3(1, 0, 0) || direction == new Vector3(-1, 0, 0))
        {
            ZScale += 5f;
            float temp = ZScale;
            ZScale = XScale;
            XScale = temp;
        }
        else
        {
            XScale += 5f;
        }




        if (XScale >= 5 && ZScale >= 5)
        {
            var entry = Instantiate(hallwayLargeEntryPrefab.creationPrefab, position, rotation, transform);
            connecterObjectLists[hallwayLargeEntryPrefab].Add(entry);
            entry.name = "LargeEntry";
            entry.transform.localScale = new Vector3(XScale, hallwayHeight, ZScale);
            LargeEntryCreationHandler handler = entry.GetComponent<LargeEntryCreationHandler>();
            handler.direction = direction;
            handler.entryLocation = entryLocation;
            handler.exitLocation = exitLocation;

        }

    }

    private void CreateThreeJunction(GameObject cornerToReplace)
    {
        var junction = Instantiate(hallwayThreeJunctionPrefab.creationPrefab, cornerToReplace.transform.position, cornerToReplace.transform.rotation, transform);
        junction.transform.localScale = cornerToReplace.transform.localScale;
        Destroy(cornerToReplace);

    }

    private void OpenDoors()
    {

        spawnRoom.GetComponent<DoorHandler>().OpenDoors();

        foreach (var roomList in actualRoomObjectLists)
        {
            foreach (GameObject room in roomList.Value)
            {
                room.GetComponent<DoorHandler>().OpenDoors();
            }
        }
    }

}
