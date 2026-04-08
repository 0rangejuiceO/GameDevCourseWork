using UnityEngine;

public class DestroySnitch : MonoBehaviour
{
    void OnDestroy()
    {
        Debug.LogError($"{gameObject.name} is being DESTROYED! Trace: ", this);
        // This will print the stack trace to the console so you can see 
        // exactly which script called the Destroy command.
        Debug.Log(System.Environment.StackTrace);
    }
}