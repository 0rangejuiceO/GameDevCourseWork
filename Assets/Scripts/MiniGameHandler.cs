using FPController;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class MiniGameHandler : MonoBehaviour
{
    [Header("MiniGame Settings")]
    [SerializeField] private GameObjectFloatDictionary miniGames = new GameObjectFloatDictionary();
    [SerializeField] private GameObject defaultMiniGame;

    [Header("Global Refs")]
    [SerializeField] private FPController.FPController fpController;
    [SerializeField] private Transform canvas;

    private GameObject currentMiniGame;
    public void Interact()
    {
        fpController.LockMovement = true;
    }

    public void LeaveInteract()
    {
        fpController.LockMovement = false;
    }

    public void StartMiniGame()
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
    }

}
