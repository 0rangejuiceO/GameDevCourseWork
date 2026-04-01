using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]private Enemy enemy;
    [SerializeField] private Transform spawnPoint;

    public void SpawnEnemy()
    {
        GameObject spawnedEnemy = Instantiate(enemy.prefab,spawnPoint,transform);

        var shadow = Instantiate(enemy.physicsShadow, spawnPoint, transform);
        shadow.GetComponent<PhysicsShadow>().SetTransform(spawnedEnemy.transform);

        NavMeshAgent agent = spawnedEnemy.GetComponent<NavMeshAgent>();

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPoint.position, out hit, 5f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogWarning("No NavMesh nearby!");
        }
    }

    void OnEnable()
    {
        SceneHandler.spawnEnemyEvent += SpawnEnemy;
    }

    void OnDisable()
    {
        SceneHandler.spawnEnemyEvent -= SpawnEnemy;
    }


}