using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReadyCheckListener : NetworkBehaviour
{
    [SerializeField] private InputActionReference readyAction;
    private bool isReady = false;
    private static Dictionary<ulong, bool> playerReadyStatus = new Dictionary<ulong, bool>();
    private static int playersInLobby = 1;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            readyAction.action.actionMap.Enable();
            readyAction.action.performed += OnReady;
            readyAction.action.Enable();
        }
        if (!IsServer && IsOwner)
        {
            notifyInGameRPC();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            readyAction.action.performed -= OnReady;
        }
    }


    private void OnReady(InputAction.CallbackContext context)
    {
        OnReady();
    }

    private void OnReady()
    {
        isReady = !isReady;

        Debug.Log($"[Client] I (Player {NetworkManager.LocalClientId}) set my status to: {isReady}");

        SendReadyStatusServerRpc(isReady);
    }


    [Rpc(SendTo.Server)]
    public void SendReadyStatusServerRpc(bool isReady)
    {

        playerReadyStatus[OwnerClientId] = isReady;

        // Handle the ready status on the server (e.g., update player state, check if all players are ready, etc.)
        Debug.Log($"Player {OwnerClientId} is {(isReady ? "ready" : "not ready")}");

        if(playersInLobby == playerReadyStatus.Count)
        {
            CheckIfAllReady();
        }

        
    }

    [Rpc(SendTo.Server)]
    private void notifyInGameRPC()
    {
        playersInLobby++;
        Debug.Log($"Player {OwnerClientId} is in the lobby. Total players in lobby: {playersInLobby}");
    }

    private void CheckIfAllReady()
    {
        foreach (var status in playerReadyStatus.Values)
        {
            if (!status)
            {
                Debug.Log("Not all players are ready yet.");
                return;
            }
        }
        Debug.Log("All players are ready! Starting the game...");


        SceneHandler.InvokeTeleportPlayers();

    }
}
