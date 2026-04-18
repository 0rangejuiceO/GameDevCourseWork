using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections.Generic;

public class OnSpawn : NetworkBehaviour
{
    [SerializeField] private GameObject[] gameObjectsToCloseIfNotOwner;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private GameObject playerMesh;

    private Vector3[] spawnLiftLocations = new Vector3[12];

    public override void OnNetworkSpawn()
    {
        // 1. Handle Visuals/UI for everyone
        if (!IsOwner)
        {
            DisableRemotePlayerVisuals();
        }

        // 2. Handle Logic specifically for the Owner
        if (IsOwner)
        {
            SetupLocalPlayer();
            TeleportAsOwner();
            playerMesh.SetActive(false);
        }

        SceneHandler sceneHandler = GameObject.Find("SceneHandler").GetComponent<SceneHandler>();

        sceneHandler.AddPlayer(gameObject);


    }

    private void TeleportAsOwner()
    {
        // Because the NetworkTransform is Owner Authoritative, 
        // the Owner is the ONLY one allowed to call Teleport.

        SpawnLiftSettings[] components = FindObjectsByType<SpawnLiftSettings>(FindObjectsSortMode.None);

        foreach (SpawnLiftSettings component in components)
        {
            if(component.LobbySpawn == true)
            {
                spawnLiftLocations[component.id] = component.transform.position;
            }
        }

        ulong playerIDUlong = NetworkManager.Singleton.LocalClientId;
        int playerIDInt = (int)playerIDUlong;

        Vector3 spawnPosition = spawnLiftLocations[playerIDInt];

        var cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        if (TryGetComponent(out NetworkTransform netTransform))
        {
            // This will now work because IsOwner is true
            netTransform.Teleport(spawnPosition, Quaternion.identity, transform.localScale);
            Debug.Log($"Owner teleported successfully to {spawnPosition}");
        }
        else
        {
            transform.position = spawnPosition;
            transform.rotation = Quaternion.identity;
        }

        if (cc != null) cc.enabled = true;
    }

    private void SetupLocalPlayer()
    {
        cameraHolder.SetActive(true);
        playerCamera.enabled = true;
        if (audioListener != null) audioListener.enabled = true;

        var ui = GameObject.Find("NetcodeStuff");
        if (ui != null) ui.SetActive(false);

        var uiCam = GameObject.Find("StartCamera");
        if (uiCam != null && uiCam.TryGetComponent<Camera>(out var cam))
            cam.enabled = false;
    }

    private void DisableRemotePlayerVisuals()
    {
        playerCamera.enabled = false;
        if (audioListener != null) audioListener.enabled = false;

        foreach (GameObject obj in gameObjectsToCloseIfNotOwner)
        {
            if (obj != null) obj.SetActive(false);
        }
    }
}