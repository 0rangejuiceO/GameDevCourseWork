using UnityEngine;

public class StopRotate : MonoBehaviour
{
    void LateUpdate()
    {
        Vector3 euler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0f, euler.y, 0f);
    }
}
