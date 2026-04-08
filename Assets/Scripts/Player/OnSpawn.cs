using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class OnSpawn : NetworkBehaviour
{
    [SerializeField] private GameObject[] gameObjectsToCloseIfNotOwner;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private GameObject cameraHolder;

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
        }

        SceneHandler sceneHandler = GameObject.Find("SceneHandler").GetComponent<SceneHandler>();

        sceneHandler.AddPlayer(gameObject);


    }

    private void TeleportAsOwner()
    {
        // Because the NetworkTransform is Owner Authoritative, 
        // the Owner is the ONLY one allowed to call Teleport.

        GameObject spawnObj = GameObject.Find("PlayerSpawnPoint");
        if (spawnObj != null)
        {
            // Disable CharacterController if you have one to prevent physics fighting
            var cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            if (TryGetComponent(out NetworkTransform netTransform))
            {
                // This will now work because IsOwner is true
                netTransform.Teleport(spawnObj.transform.position, spawnObj.transform.rotation, transform.localScale);
                Debug.Log($"Owner teleported successfully to {spawnObj.transform.position}");
            }
            else
            {
                transform.position = spawnObj.transform.position;
                transform.rotation = spawnObj.transform.rotation;
            }

            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogError("Teleport failed: PlayerSpawnPoint not found in scene!");
        }
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