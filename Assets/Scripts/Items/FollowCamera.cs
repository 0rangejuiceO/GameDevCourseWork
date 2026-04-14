using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Vector3 heldLocalPos = new Vector3(0.65f, -0.5f, 1.1f);
    [SerializeField] private Quaternion heldLocalRot = Quaternion.identity;
    public bool isFollowing = false;
    public Transform camera;

    private void Update()
    {
        if (!isFollowing)
        {
            return;
        }
        transform.localPosition = camera.localPosition + heldLocalPos;
        transform.localRotation = camera.localRotation;
    }
}
