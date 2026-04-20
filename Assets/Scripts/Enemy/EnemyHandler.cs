using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHandler : NetworkBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject physicsShadow;

    public void CreateEnemy(int enemyId, Vector3 location, Quaternion rotation)
    {
        if (IsServer)
        {
            SpawnEnemyServerRPC(enemyId, location, rotation);
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnEnemyServerRPC(int enemyId, Vector3 location, Quaternion rotation)
    {
        var enemy = Instantiate(enemyPrefabs[enemyId],location,rotation);

        var shadow = Instantiate(physicsShadow, location, rotation);
        shadow.GetComponent<PhysicsShadow>().SetTransform(enemy.transform);

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(location, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning("No NavMesh nearby!");
        }


        enemy.GetComponent<NetworkObject>().Spawn();

    }
}
