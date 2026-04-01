using UnityEngine;

public class PushChair : MonoBehaviour
{
    [SerializeField]private float pushForce = 2f;
    public void pushChair()
    {
        Vector3 pushDirection = transform.forward;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(pushDirection * 2f, ForceMode.Impulse);
    }
}
