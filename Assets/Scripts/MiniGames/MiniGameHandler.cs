using FPController;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class MiniGameHandler : MonoBehaviour
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
    [SerializeField] private FPController.FPController fpController;
    [SerializeField] private Transform canvas;

    private GameObject currentMiniGame;
    private GameObject machine;
    public void Interact()
    {
        fpController.LockMovement = true;
    }

    public void LeaveInteract()
    {
        fpController.LockMovement = false;
    }

    public void StartMiniGame(GameObject usedMachine)
    {
        GameObject game = SelectMiniGame();
        var newGame = Instantiate(game, transform.position, Quaternion.identity,canvas);
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
            var rewardItem = SelectReward();
            Transform location = machine.GetComponent<GameMachineOutputMarker>().outputLocation;
            Instantiate(rewardItem, location.position, Quaternion.identity);

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

}
