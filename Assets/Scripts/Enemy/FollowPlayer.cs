using UnityEngine;
using Unity.Netcode;

public class FollowPlayer : NetworkBehaviour
{
    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    public bool follow = false;
    private float sampleDistance = 2f;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        }

    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Update()
    {
        if (!IsOwner || !follow)
        {
            return;
        }

        Vector3 targetPos = target.position;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, sampleDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

    }


}
