using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct RoomEntry
{
    public string roomName;
    public int count;
}


[CreateAssetMenu(fileName = "LevelSettings", menuName = "Map/Level")]
public class LevelSettings : ScriptableObject
{
    public int LevelID;
    public List<RoomEntry> RoomEntries = new List<RoomEntry>();
    public int MaxStaircases;

    public Dictionary<string, int> GetRoomDictionary()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();

        foreach (var room in RoomEntries)
        {
            if (dict.ContainsKey(room.roomName))
                dict[room.roomName] += room.count;
            else
                dict.Add(room.roomName, room.count);
        }

        return dict;
    }

}
