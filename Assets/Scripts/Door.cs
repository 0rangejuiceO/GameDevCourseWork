using UnityEngine;

public class Door : MonoBehaviour
{
    public bool canOpen = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void addForce(Vector3 direction,float force)
    {
        Debug.Log($"AddForce Called canOpen is {canOpen}");
        if (canOpen)
        {
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
        
    }
}
