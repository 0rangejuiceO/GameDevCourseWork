using UnityEngine;

public class GameMachineOutputMarker : MonoBehaviour
{
    public Transform outputLocation;

    public void playGame()
    {
        MiniGameHandler[] miniGameHandlerArray = FindObjectsByType<MiniGameHandler>(FindObjectsSortMode.None);
        MiniGameHandler miniGameHandler = miniGameHandlerArray[0];

        miniGameHandler.StartMiniGame(gameObject);
    }

}
