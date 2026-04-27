using UnityEngine;

[ExecuteAlways]
public class FurnitureSize : MonoBehaviour
{
    public Vector3 cubeSize = new Vector3(1, 1, 1);
    public Vector3 cubeOffset = new Vector3(0f, 0.5f, 0.5f);
    public Vector3 rotation = new Vector3(0f, 0f, 0f);
    [SerializeField] private Color gizmoColour = Color.cyan;
    [SerializeField] private float forwardLineLength = 2f;
    [SerializeField] private Color lineColour = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;

        // Create rotation from your Vector3
        Quaternion rot = Quaternion.Euler(rotation);

        // Apply transformation matrix
        Gizmos.matrix = Matrix4x4.TRS(transform.position + cubeOffset, rot, Vector3.one);

        Gizmos.DrawWireCube(Vector3.zero, cubeSize);

        // Reset matrix so other gizmos aren't affected
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = lineColour;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * forwardLineLength);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColour * new Color(1, 1, 1, 0.3f);

        Quaternion rot = Quaternion.Euler(rotation);
        Gizmos.matrix = Matrix4x4.TRS(transform.position + cubeOffset, rot, Vector3.one);

        Gizmos.DrawCube(Vector3.zero, cubeSize);

        Gizmos.matrix = Matrix4x4.identity;
    }
}
