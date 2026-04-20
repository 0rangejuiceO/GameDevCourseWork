using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class FollowPlayer : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private int damage = 50;
    [SerializeField] private float punchCooldown = 1.5f;
    private Transform target;
    private UnityEngine.AI.NavMeshAgent agent;
    public bool follow = false;
    private int punchHash;
    private float sampleDistance = 2f;
    private float currentPunchCD;
    private bool timerRunning = false;


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

        if (timerRunning)
        {
            if(currentPunchCD > 0)
            {
                currentPunchCD -= Time.deltaTime;
            }
            else
            {
                timerRunning = false;
            }
        }


        Vector3 targetPos = target.position;

        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(targetPos, out hit, sampleDistance, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner)
        {
            return;
        }
        if(other.gameObject.tag == "Player")
        {
            if (!timerRunning)
            {
                StartCoroutine(DelayedDamage(other.gameObject));
            }

        }
    }

    private IEnumerator DelayedDamage(GameObject target)
    {
        timerRunning = true;
        currentPunchCD = punchCooldown;

        SendPunchAnimationRPC();

        yield return new WaitForSeconds(1f);

        if (target != null)
        {
            target.GetComponent<PlayerHealth>().RequestDamageRPC(damage);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SendPunchAnimationRPC()
    {
        Debug.Log("Calling Punch Trigger");
        animator.SetTrigger("Punch");
    }


}
