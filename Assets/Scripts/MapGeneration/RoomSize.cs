using UnityEngine;

public class RoomSize : MonoBehaviour
{
    public Vector3 roomSize= new Vector3(0,0,0);
    public Vector3 roomOffset = new Vector3(0,0,0);

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.pink;
        Gizmos.DrawWireCube(transform.position + roomOffset, roomSize);
    }
}
