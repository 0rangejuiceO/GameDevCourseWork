using UnityEngine;
using System.Threading.Tasks;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject MapSpawner;
    [SerializeField] Vector3 spawnPosition;

    async void Start()
    {
        await MapSpawner.GetComponent<MapGenerator>().BeginMapGeneration();

        Player.transform.position = spawnPosition;

    }

}
