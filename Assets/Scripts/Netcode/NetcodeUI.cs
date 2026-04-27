using NUnit.Framework;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NetcodeUI : MonoBehaviour
{
    [SerializeField]private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Camera UICamera;
    [SerializeField]private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Button joinWithCodeButton;
    [SerializeField]private TMP_InputField lobbyCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;


    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            Debug.Log("HOST");
            lobbyCreateUI.Show();
        });

        joinButton.onClick.AddListener(() =>
        {
            Debug.Log("JOIN");
            GameLobby.Instance.QuickJoin();
        });

        joinWithCodeButton.onClick.AddListener(() =>
        {
            Debug.Log("JOIN WITH CODE");
            GameLobby.Instance.JoinWithCode(lobbyCodeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        GameLobby.Instance.OnLobbyListChanged += OnUpdateLobbyList;
        UpdateLobbyList(new List<Lobby>());
    }

    private void OnUpdateLobbyList(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach(Transform child in lobbyContainer)
        {
            if(child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);

            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
