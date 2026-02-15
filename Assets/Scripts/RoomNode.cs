using UnityEngine;

public class RoomNode
{
    public GameObject roomObject;
    public Vector3 Position => roomObject.transform.position;

    public RoomNode(GameObject room)
    {
        roomObject = room;
    }
}
