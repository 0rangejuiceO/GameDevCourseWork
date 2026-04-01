using UnityEngine;

[ExecuteAlways]
public class FurnitureSize : MonoBehaviour
{
    public Vector3 cubeSize = new Vector3(1, 1, 1);
    public Vector3 cubeOffset = Vector3.zero;
    [SerializeField] private Color gizmoColour = Color.cyan;
    [SerializeField] private float forwardLineLength = 2f;
    [SerializeField] private Color lineColour = Color.red;

    private void OnDrawGizmos()
    {

        Gizmos.color = gizmoColour;
        Gizmos.DrawWireCube(transform.position + cubeOffset, cubeSize);

        Gizmos.color = lineColour;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * forwardLineLength);
    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = gizmoColour * new Color(1, 1, 1, 0.3f);
        Gizmos.DrawCube(transform.position + cubeOffset, cubeSize);
    }
}
