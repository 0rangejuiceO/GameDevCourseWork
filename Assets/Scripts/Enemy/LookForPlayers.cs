using Unity.Netcode;
using UnityEngine;

public class LookForPlayers : NetworkBehaviour
{
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private float detectionAngle = 45f;

    [SerializeField] private float requiredTime = 3f;
    [SerializeField] private FollowPlayer followPlayer;
    [SerializeField] private RandomNavMeshWander navMeshWander;
    private UnityEngine.AI.NavMeshAgent agent;
    private float[] currentTimes;
    private Transform[] players;
    private bool[] canSee;
    private int numCanSee = 0;
    private bool searching = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        PlayerHealth[] playerHealths = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);

        players = new Transform[playerHealths.Length];

        for (int i = 0; i < playerHealths.Length; i++) {
            players[i] = playerHealths[i].gameObject.transform;
        }

        currentTimes = new float[playerHealths.Length];
        for (int i = 0; i < playerHealths.Length; i++) { currentTimes[i] = 0f; }

        canSee = new bool[playerHealths.Length];
        for(int i =0; i < playerHealths.Length; i++) {  canSee[i] = false; }

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

    }

    private void Update()
    {
        if (IsOwner)
        {
            for (int i = 0; i < currentTimes.Length; i++)
            {
                CheckPlayerLocation(i);
            }

            if (searching)
            {
                float min = float.MaxValue;

                for (int i = 0; i < currentTimes.Length; i++)
                {
                    if (currentTimes[i] < min)
                        min = currentTimes[i];
                }

                if(min > 30)
                {
                    OnWander();
                }
            }

        }
    }

    private void CheckPlayerLocation(int i)
    {
        Transform player = players[i];

        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        float distance = Vector3.Distance(transform.position, player.position);

        if(distance <= detectionDistance)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);

            if(angle <= detectionAngle)
            {

                RaycastHit hit;

                if(Physics.Raycast(transform.position, directionToPlayer, out hit, detectionDistance))
                {
                    Debug.DrawRay(transform.position, directionToPlayer * hit.distance, Color.red);
                    if(hit.transform.CompareTag("Player"))
                    {
                        if (canSee[i])
                        {
                            currentTimes[i] = 0f;
                        }
                        else
                        {
                            currentTimes[i] += Time.deltaTime;

                            if (currentTimes[i] > requiredTime)
                            {
                                OnPlayerDetected(i);
                            }

                        }
                        return;

                    }
                }


            }

        }
        if (canSee[i])
        {
            currentTimes[i] += Time.deltaTime;

            if (currentTimes[i] > requiredTime)
            {
                OnPlayerLost(i);
            }
        }
        else
        {
            currentTimes[i] = 0f;
        }

            
    }

    private void OnPlayerDetected(int i)
    {
        canSee[i] = true;
        Debug.Log($"Player {i} found");
        followPlayer.SetTarget(players[i]);
        followPlayer.follow = true;
        navMeshWander.wander = false;
        agent.speed = 3.5f;
        numCanSee++;
        searching = false;
    }

    private void OnPlayerLost(int i)
    {
        Debug.Log($"Player {i} lost");
        canSee[i] = false;
        numCanSee--;
        if(numCanSee == 0)
        {
            OnSearch();
        }

    }

    private void OnSearch()
    {
        agent.speed = 2.25f;
        followPlayer.follow = false;
        navMeshWander.wander = true;
        navMeshWander.wanderRadius = 30f;
        searching = true;
    }

    private void OnWander()
    {
        agent.speed = 1.75f;
        searching = false;
        navMeshWander.wanderRadius = 180f;
    }

}
