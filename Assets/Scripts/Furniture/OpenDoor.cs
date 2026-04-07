using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private int playersInTrigger = 0;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField]private Vector3 openPosition;
    [SerializeField] private float speed = 1f;
    [SerializeField] private GameObject door;
    private float t = 0f;
    private bool moveDoor = false;
    private Vector3 targetLocation;


    private void Update()
    {
        if (!moveDoor)
        {
            return;
        }
        t += Time.deltaTime * speed;
        
        door.transform.localPosition = Vector3.Lerp(door.transform.localPosition, targetLocation, t);

        if(door.transform.localPosition.y == targetLocation.y)
        {
            moveDoor = false;
            t = 0f;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playersInTrigger++;
            targetLocation = openPosition;
            moveDoor = true;
            t = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playersInTrigger--;
            
            if(playersInTrigger <= 0)
            {
                targetLocation = closedPosition;
                moveDoor = true;
                t = 0;
            }
            
        }
    }

    public void MoveDoor(bool direction)
    {
        if (direction)
        {
            targetLocation = closedPosition;
        }
        else
        {
            targetLocation = openPosition;
        }
        moveDoor = true;
        t = 0;
    }
}
