using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]private int enemyId;
    [SerializeField] private Transform spawnPoint;

    public void SpawnEnemy()
    {
        EnemyHandler[] enemyHandlers = FindObjectsByType<EnemyHandler>(FindObjectsSortMode.None);
        EnemyHandler enemyHandler = enemyHandlers[0];

        enemyHandler.CreateEnemy(enemyId,spawnPoint.position,spawnPoint.rotation);

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