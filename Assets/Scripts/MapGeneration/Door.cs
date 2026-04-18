using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class Door : NetworkBehaviour
{
    public bool canOpen = false;
    public bool isLocked = false;
    public Rigidbody rb;
    private int lockedChance = 20; //1 in 20 chance
    bool openDirection = false;
    float openForce = 2f;
    public bool isSideWays = false;


    public override void OnNetworkSpawn()
    {
        //Debug.Log($"Location local: {transform.localPosition} Location Global: {transform.position} Rotation local: {transform.localRotation} Rotation Global: {transform.rotation}");


        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {

            int num = Random.Range(1, lockedChance);
            if (num == 1)
            {
                isLocked = true;
                rb.isKinematic = true;
                gameObject.GetComponent<NavMeshObstacle>().enabled = true;
                TellDoorIsLockedRPC();
            }
        }
        else
        {
            rb.isKinematic = true;
        }


        if (transform.rotation.y == 0)
        {
            isSideWays = true;
        }

    }

    [Rpc(SendTo.Everyone)]
    private void TellDoorIsLockedRPC()
    {
        isLocked = true;
        rb.isKinematic = true;
        gameObject.GetComponent<NavMeshObstacle>().enabled = true;
    }


    [Rpc(SendTo.Server)]
    public void addForceRPC()
    {
        addForce();
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

    [Rpc(SendTo.Everyone)]
    private void TellEveryoneDoorIsUnlockedRPC()
    {
        if (isLocked)
        {
            if (IsServer)
            {
                rb.isKinematic = false;
            }
            isLocked = false;
            Debug.Log("Door Unlocked By Another Player");
            gameObject.GetComponent<NavMeshObstacle>().enabled = false;
        }
    }

    public void unlockDoor(string interactionMessage)
    {
        Debug.Log(interactionMessage);

        if (isLocked)
        {
            if (IsServer)
            {
                rb.isKinematic = false;
            }
            isLocked = false;
            gameObject.GetComponent<NavMeshObstacle>().enabled = false;
            InventoryHandler.onDestroyItem();
            Debug.Log("Door Unlocked");
            TellEveryoneDoorIsUnlockedRPC();
        }
    }
}
