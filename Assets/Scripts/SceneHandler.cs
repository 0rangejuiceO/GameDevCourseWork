using UnityEngine;
using System.Threading.Tasks;

public class SceneHandler : MonoBehaviour
{
    [SerializeField] GameObject Player;
    [SerializeField] GameObject MapSpawner;
    [SerializeField] Vector3 spawnPosition;

    async void Start()
    {


        bool succeeded = false;
        bool doReplacement = false;

        while (!succeeded)
        {
            succeeded = await MapSpawner.GetComponent<MapGenerator>().BeginMapGeneration();
            if (succeeded)
            {
                doReplacement = MapSpawner.GetComponent<MapGenerator>().doReplacement;
            }
        }

        Debug.Log(doReplacement);

        if (doReplacement)
        {
            Player.transform.position = spawnPosition;
        }
        

    }

}
