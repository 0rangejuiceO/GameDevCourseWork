using TMPro;
using UnityEngine;

public class LobbyNameHolder : MonoBehaviour
{
    [SerializeField]private TMP_Text lobbyNameText;

    public void SetLobbyName(string name,string code)
    {
        lobbyNameText.text = $"Lobby Name: {name}\nLobby Code: {code}";
    }

    public void Hide()
    {
        lobbyNameText.gameObject.SetActive(false);
    }
}
