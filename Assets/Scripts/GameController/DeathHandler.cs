using FPController;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class DeathHandler : NetworkBehaviour
{
    [SerializeField] private GameObject deathScreen;

    private Vector3 deathTubPosition = new Vector3(0f, 158f, 0f);
    public static event Action OnDeath;
    private GameObject[] playerObjects;
    private GameObject currentTarget;
    private Vector3[] spawnLiftLocations = new Vector3[12];

    public override void OnNetworkSpawn()
    {
        DeathHandler.OnDeath += HandleDeath;
    }
    public override void OnNetworkDespawn()
    {
        DeathHandler.OnDeath -= HandleDeath;
    }

    public static void OnPlayerDied(GameObject playerObject)
    {
        OnDeath?.Invoke();
    }

    private void HandleDeath()
    {
        InventoryHandler.InvokeDropAllItems();

        NetworkObject localPlayerNetObj = NetworkManager.Singleton.LocalClient.PlayerObject;

        FPController.FPController fpController = localPlayerNetObj.GetComponent<FPController.FPController>();
        fpController.LockMovement = true;
        StartCoroutine(TeleportPlayerToDeathTub());



        PlayerHealth[] playerHealths = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);

        playerObjects = new GameObject[playerHealths.Length];

        for (int i = 0; i < playerHealths.Length; i++)
        {
            playerObjects[i] = playerHealths[i].gameObject;
        }

        localPlayerNetObj.gameObject.GetComponent<MainCameraObjectReference>().mainCamera.SetActive(false);

        for(int i = 0;i < playerObjects.Length; i++)
        {
            if (playerObjects[i].GetComponent<PlayerHealth>().alive) {
                playerObjects[i].GetComponent<SpectatorCamera>().camera.SetActive(true);
                currentTarget = playerObjects[i];
                break;
            }
        }
        deathScreen.SetActive(true);


        var countDown = deathScreen.GetComponent<CountDown>();
        countDown.StartCountdown(20, OnRespawn);

    }

    private void OnRespawn()
    {
        deathScreen.SetActive(false);
        NetworkObject localPlayerNetObj = NetworkManager.Singleton.LocalClient.PlayerObject;
        currentTarget.GetComponent<SpectatorCamera>().camera.SetActive(false);
        localPlayerNetObj.gameObject.GetComponent<MainCameraObjectReference>().mainCamera.SetActive(true);
        FPController.FPController fpController = localPlayerNetObj.GetComponent<FPController.FPController>();
        fpController.LockMovement = false;
        localPlayerNetObj.gameObject.GetComponent<PlayerHealth>().Respawn();
        StartCoroutine(TeleportPlayersToSpawnRoom());
    }

    private IEnumerator TeleportPlayersToSpawnRoom()
    {
        Debug.Log("Called teleport players to spawn room");
        SpawnLiftSettings[] components = FindObjectsByType<SpawnLiftSettings>(FindObjectsSortMode.None);


        foreach (SpawnLiftSettings component in components)
        {
            if (component.InitialSpawn == true)
            {
                spawnLiftLocations[component.id] = component.transform.position;
            }
        }

        ulong playerIDUlong = NetworkManager.Singleton.LocalClientId;
        int playerIDInt = (int)playerIDUlong;

        Vector3 spawnPosition = spawnLiftLocations[playerIDInt];

        NetworkObject localPlayerNetObj = NetworkManager.Singleton.LocalClient.PlayerObject;
        if(localPlayerNetObj.TryGetComponent(out NetworkTransform netTransform))
        {
            netTransform.Teleport(spawnPosition, Quaternion.identity, transform.localScale);
            Debug.Log($"Teleporting player to {spawnPosition}");
        }


        yield return null;
    }

    private IEnumerator TeleportPlayerToDeathTub()
    {
        Debug.Log("Called teleport player to death tub");

        NetworkObject localPlayerNetObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        if (localPlayerNetObject.TryGetComponent(out NetworkTransform netTransform))
        {
            netTransform.Teleport(deathTubPosition, Quaternion.identity, transform.localScale);
        }

        yield return null;
    }

}
