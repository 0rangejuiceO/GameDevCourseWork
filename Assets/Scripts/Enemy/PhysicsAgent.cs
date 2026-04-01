using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PhysicsAgent : MonoBehaviour
{
    private NavMeshAgent agent;
    private Rigidbody rb;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Let NavMeshAgent update position using Rigidbody
        agent.updatePosition = false;
        agent.updateRotation = true;
    }

    void FixedUpdate()
    {
        // Move Rigidbody manually toward NavMeshAgent's next position
        Vector3 nextPos = agent.nextPosition;
        Vector3 move = nextPos - rb.position;
        rb.MovePosition(rb.position + move);
    }

    void LateUpdate()
    {
        // Sync agent's position to Rigidbody
        agent.nextPosition = rb.position;
    }
}