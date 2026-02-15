using UnityEngine;
using System.Threading.Tasks;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject MapSpawner;
    [SerializeField] Vector3 spawnPosition;

    async void Start()
    {
        bool doReplacement = await MapSpawner.GetComponent<MapGenerator>().BeginMapGeneration();

        if (doReplacement)
        {
            Player.transform.position = spawnPosition;
        }
        

    }

}
