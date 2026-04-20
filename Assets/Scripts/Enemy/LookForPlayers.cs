using Unity.Netcode;
using UnityEngine;

public class LookForPlayers : NetworkBehaviour
{
    [SerializeField] private float detectionDistance = 10f;
    [SerializeField] private float detectionAngle = 45f;

    [SerializeField] private float requiredTime = 3f;
    private float[] currentTimes;
    private Transform[] players;

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

    }

    private void Update()
    {
        if (IsOwner)
        {
            for (int i = 0; i < currentTimes.Length; i++)
            {
                CheckPlayerLocation(i);
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
                        currentTimes[i] += Time.deltaTime;

                        if (currentTimes[i] > requiredTime)
                        {
                            OnPlayerDetected(i);
                        }

                        return;
                    }
                }


            }

        }
        currentTimes[i] = 0f;
    }

    private void OnPlayerDetected(int i)
    {
        Debug.Log($"Player {i} found");
    }


}
