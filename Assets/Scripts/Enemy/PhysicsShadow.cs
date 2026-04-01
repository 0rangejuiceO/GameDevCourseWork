using UnityEngine;

public class PhysicsShadow : MonoBehaviour
{
    private Transform realTransform;

    public void SetTransform(Transform transform)
    {
        realTransform = transform;
    }

    private void Update()
    {
        if (realTransform != null)
        {
            transform.position = realTransform.position;
            transform.rotation = realTransform.rotation;
        }
    }

}
