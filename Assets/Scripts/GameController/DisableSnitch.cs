using UnityEngine;

public class DisableSnitch : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.LogError($"{gameObject.name} is being Disabled! Trace: ", this);
        Debug.Log(System.Environment.StackTrace);
    }
}
