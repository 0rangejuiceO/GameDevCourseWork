using FPController;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class MiniGameHandler : NetworkBehaviour
{
    [Header("MiniGame Settings")]
    [Space(15)]
    [SerializeField] private GameObjectFloatDictionary miniGames = new GameObjectFloatDictionary();
    [Space(20)]
    [SerializeField] private GameObject defaultMiniGame;

    [Header("Loot Table")]
    [Space(15)]
    [SerializeField]private GameObjectFloatDictionary lootTable = new GameObjectFloatDictionary();
    [Space(20)]
    [SerializeField] private GameObject defaultReward;

    [Header("Global Refs")]
    private FPController.FPController fpController;
    [SerializeField] private GameObject canvas;

    private GameObject currentMiniGame;
    private GameObject machine;

    public override void OnNetworkSpawn()
    {
        fpController = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<FPController.FPController>();
        if(fpController == null)
        {
            Debug.Log("Couldnt find fpController");
        }
    }

    public void Interact()
    {
        if (fpController != null)
        {
            fpController.LockMovement = true;
        }
        else
        {
            Debug.Log("Couldnt Find FPController");
        }
    }

    public void LeaveInteract()
    {
        if (fpController != null)
        {
            fpController.LockMovement = false;
        }
        else
        {
            Debug.Log("Couldnt Find FPController");
        }
        
    }

    public void StartMiniGame(GameObject usedMachine)
    {
        fpController = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<FPController.FPController>();
        canvas.SetActive(true);
        GameObject game = SelectMiniGame();
        var newGame = Instantiate(game, transform.position, Quaternion.identity,canvas.transform);
        RectTransform rt = newGame.GetComponent<RectTransform>();
        Vector2 pos = rt.anchoredPosition;
        pos.x = 0f;
        pos.y = 0f;
        rt.anchoredPosition = pos;
        Interact();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        machine = usedMachine;
        currentMiniGame = newGame;
    }

    private GameObject SelectMiniGame()
    {
        float totalWeight = 0f;
        foreach (float weight in miniGames.Values)
        {
            totalWeight += weight;
        }

        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in miniGames)
        {
            cumulative += entry.Value;

            if (rand <= cumulative)
            {
                Debug.Log($"Selected mini game {entry.Key.name}");
                return entry.Key;
            }
        }

        return defaultMiniGame;

    }

    public void EndMiniGame(bool win)
    {
        LeaveInteract();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
        Destroy(currentMiniGame);

        if (win)
        {
            Transform location = machine.GetComponent<GameMachineOutputMarker>().outputLocation;
            SpawnRewardServerRPC(location.position);
        }

    }


    private GameObject SelectReward()
    {
        float totalWeight = 0f;
        foreach (float weight in lootTable.Values)
        {
            totalWeight += weight;
        }

        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in lootTable)
        {
            cumulative += entry.Value;

            if (rand <= cumulative)
            {
                Debug.Log($"Selected mini game {entry.Key.name}");
                return entry.Key;
            }
        }

        return defaultReward;

    }

    [Rpc(SendTo.Server,InvokePermission = RpcInvokePermission.Everyone)]
    private void SpawnRewardServerRPC(Vector3 location)
    {
        var rewardItem = SelectReward();
        
        var reward = Instantiate(rewardItem, location, Quaternion.identity);
        reward.GetComponent<NetworkObject>().Spawn();
    }

}
