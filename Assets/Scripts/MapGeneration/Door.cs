using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public bool canOpen = false;
    public bool isLocked = false;
    public Rigidbody rb;
    private int lockedChance = 20; //1 in 20 chance
    bool openDirection = false;
    float openForce = 2f;
    public bool isSideWays = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        int num =  Random.Range(1, lockedChance);
        if(num == 1)
        {
            isLocked = true;
            rb.isKinematic = true;
            gameObject.GetComponent<NavMeshObstacle>().enabled = true;

        }

        if(transform.parent.parent.transform.rotation.y == 0)
        {
            isSideWays = true;
        }

    }

    public void addForce()
    {
        if (isLocked)
        {
            Debug.Log("Is Locked");
            return;
        }

        Debug.Log($"AddForce Called canOpen is {canOpen}");
        if (canOpen)
        {
            if (isSideWays)
            {
                if (openDirection)
                {
                    rb.AddForce(Vector3.right * openForce, ForceMode.Impulse);
                    openDirection = false;

                }
                else
                {
                    rb.AddForce(Vector3.left * openForce, ForceMode.Impulse);
                    openDirection = true;
                }
            }
            else
            {
                if (openDirection)
                {
                    rb.AddForce(Vector3.forward * openForce, ForceMode.Impulse);
                    openDirection = false;
                }
                else
                {
                    rb.AddForce(Vector3.back * openForce, ForceMode.Impulse);
                    openDirection = true;
                }
            }


            
        }
        
    }

    public void unlockDoor(string interactionMessage)
    {
        Debug.Log(interactionMessage);

        if (isLocked)
        {
            isLocked = false;
            rb.isKinematic = false;
            gameObject.GetComponent<NavMeshObstacle>().enabled = false;
            InventoryHandler.onDestroyItem();
            Debug.Log("Door Unlocked");

        }
    }
}
