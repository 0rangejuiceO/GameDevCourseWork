using UnityEngine;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using System.Collections;
using System;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject MapSpawner;
    [SerializeField] Vector3 spawnPosition;
    [SerializeField] NavMeshSurface surface;
    [SerializeField] FurnitureGenerator furnitureGenerator;

    public static event Action spawnEnemyEvent;

    private void Start()
    {

        StartCoroutine(DoMapGeneration());
    }

    IEnumerator DoMapGeneration()
    {
        bool succeeded = false;
        bool doReplacement = false;

        while (!succeeded)
        {
            var task = MapSpawner.GetComponent<MapGenerator>().BeginMapGeneration();

            yield return new WaitUntil(() => task.IsCompleted);

            succeeded = task.Result;
            if (succeeded)
            {
                doReplacement = MapSpawner.GetComponent<MapGenerator>().doReplacement;
            }
        }

        //Debug.Log(doReplacement);

        if (doReplacement)
        {
            Player.transform.position = spawnPosition;

            furnitureGenerator.GenerateFurniture();

            yield return null;

            surface.BuildNavMesh();

            yield return null;

            spawnEnemyEvent.Invoke();

        }
    }


}
