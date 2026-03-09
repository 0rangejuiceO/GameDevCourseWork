using UnityEngine;

public class Door : MonoBehaviour
{
    public bool canOpen = false;
    public bool isLocked = false;
    public Rigidbody rb;
    private int lockedChance = 2; //1 in 100 chance

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        int num =  Random.Range(1, lockedChance);
        if(num == 1)
        {
            isLocked = true;
            rb.isKinematic = true;

        }
    }

    public void addForce(Vector3 direction,float force)
    {
        if (isLocked)
        {
            Debug.Log("Is Locked");
            return;
        }

        Debug.Log($"AddForce Called canOpen is {canOpen}");
        if (canOpen)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
        
    }
}
