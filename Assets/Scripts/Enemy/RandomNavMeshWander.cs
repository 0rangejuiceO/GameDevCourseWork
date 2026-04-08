using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class RandomNavMeshWander : NetworkBehaviour
{
    public float wanderRadius = 10f;
    public float waitTime = 2f;

    private NavMeshAgent agent;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = waitTime;
    }

    void Update()
    {
/*        if (!IsOwner)
        {
            return;
        }
        Debug.Log("Wandering...");*/
        timer += Time.deltaTime;

        if (timer >= waitTime && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            Vector3 newPos = GetRandomPoint(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0f;
        }
    }

    Vector3 GetRandomPoint(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, distance, NavMesh.AllAreas);

        return hit.position;
    }
}