using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SceneHandler : NetworkBehaviour
{
    public List<GameObject> players = new List<GameObject>();
    [SerializeField] GameObject MapSpawner;
    [SerializeField] NavMeshSurface surface;
    [SerializeField] FurnitureGenerator furnitureGenerator;
    [SerializeField] GameObject leavingScreen;

    public static event Action spawnEnemyEvent;
    public static event Action teleportPlayersEvent;
    public static event Action winLevelEvent;

    private int finalSeed;
    private bool finishedGeneration = false;

    private Vector3[] spawnLiftLocations = new Vector3[12];

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(DoMapGeneration());
        }
        else
        {
            requestSeedRPC();

        }
        SceneHandler.teleportPlayersEvent += TeleportPlayersRPC;
        SceneHandler.winLevelEvent += WinLevel;
    }

    public override void OnNetworkDespawn()
    {
        SceneHandler.teleportPlayersEvent -= TeleportPlayersRPC;
        SceneHandler.winLevelEvent -= WinLevel;
    }

    public void AddPlayer(GameObject newPlayer)
    {
        players.Add(newPlayer);
    }

    IEnumerator DoMapGeneration()
    {
        bool succeeded = false;
        bool doReplacement = false;
        int masterSeed;
        while (!succeeded)
        {
            masterSeed = (int)System.DateTime.Now.Ticks;
            //masterSeed = 420;
            var task = MapSpawner.GetComponent<MapGenerator>().BeginMapGeneration(masterSeed);

            yield return new WaitUntil(() => task.IsCompleted);

            succeeded = task.Result;
            if (succeeded)
            {
                finalSeed = masterSeed;
                finishedGeneration = true;
                Debug.Log($"Master seed: {masterSeed}");
                doReplacement = MapSpawner.GetComponent<MapGenerator>().doReplacement;
                if (doReplacement)
                {
                    CreateMapRPC(masterSeed);
                }
            }
        }

        //Debug.Log(doReplacement);

    }

    [Rpc(SendTo.NotOwner)]
    private void CreateMapRPC(int seed)
    {
        StartCoroutine(CreateMap(seed));
    }

    private IEnumerator CreateMap(int seed)
    {
        var task = MapSpawner.GetComponent<MapGenerator>().BeginMapGeneration(seed);

        yield return new WaitUntil(() => task.IsCompleted);

        bool succeeded = task.Result;

        if (succeeded)
        {
            Debug.Log($"Successfully generated map with seed: {seed}");
        }
        else
        {
            Debug.LogError($"Failed to generate map with seed: {seed}");
        }
    }

    public static void InvokeTeleportPlayers()
    {
        teleportPlayersEvent.Invoke();
    }

    private IEnumerator doLaterStuff()
    {
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

        foreach (GameObject player in players)
        {
            if (player.TryGetComponent(out NetworkObject netObj))
            {
                if (netObj.IsOwner)
                {
                    Debug.Log($"Teleporting player {player.name} with clientId {netObj.OwnerClientId}");
                    // Disable CharacterController if you have one to prevent physics fighting
                    var cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    if (player.TryGetComponent(out NetworkTransform netTransform))
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
                    break;
                }
            }
        }

        if (IsServer)
        {
            MapSpawner.GetComponent<MapGenerator>().OpenDoorsRPC();
        }
        


        furnitureGenerator.GenerateFurniture();

        yield return null;

        if (IsServer)
        {
            surface.BuildNavMesh();

            yield return null;

            spawnEnemyEvent.Invoke();
        }


    }

    private IEnumerator TeleportPlayersToSpawnRoom()
    {
        Debug.Log("Called teleport players to spawn room");
        SpawnLiftSettings[] components = FindObjectsByType<SpawnLiftSettings>(FindObjectsSortMode.None);

        foreach (SpawnLiftSettings component in components)
        {
            if (component.LobbySpawn == true)
            {
                spawnLiftLocations[component.id] = component.transform.position;
            }
        }

        ulong playerIDUlong = NetworkManager.Singleton.LocalClientId;
        int playerIDInt = (int)playerIDUlong;

        Vector3 spawnPosition = spawnLiftLocations[playerIDInt];

        foreach (GameObject player in players)
        {
            if (player.TryGetComponent(out NetworkObject netObj))
            {
                if (netObj.IsOwner)
                {
                    Debug.Log($"Teleporting player {player.name} with clientId {netObj.OwnerClientId}");
                    // Disable CharacterController if you have one to prevent physics fighting
                    var cc = player.GetComponent<CharacterController>();
                    if (cc != null) cc.enabled = false;

                    if (player.TryGetComponent(out NetworkTransform netTransform))
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
                    break;
                }
            }
        }
        yield return null;
    }

    public static void InvokeWinLevel()
    {
        winLevelEvent.Invoke();
    }

    public void WinLevel()
    {
        SendLeavingScreenRPC();
    }

    private void CallTeleport()
    {
        leavingScreen.SetActive(false);
        StartCoroutine(TeleportPlayersToSpawnRoom());
    }

    [Rpc(SendTo.Everyone,InvokePermission = RpcInvokePermission.Everyone)]
    private void SendLeavingScreenRPC()
    {
        leavingScreen.SetActive(true);
        var countDown = leavingScreen.GetComponent<CountDown>();
        countDown.StartCountdown(10, CallTeleport);
    }



    [Rpc(SendTo.Server)]
    private void requestSeedRPC(RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        int seed = finalSeed;
        if(!finishedGeneration)
        {
            seed= 0;
        }
        ReturnSeedRPC(seed, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ReturnSeedRPC(int seed, RpcParams rpcParams = default)
    {
        Debug.Log($"Received seed: {seed}");
        if (seed == 0)
        {
            Debug.LogWarning("Map generation not finished yet. Client will generate map with seed 0, which may lead to desyncs if the map generator doesn't handle it properly.");
        }
        else
        {
            StartCoroutine(CreateMap(seed));
        }
    }


    [Rpc(SendTo.Everyone)]
    private void TeleportPlayersRPC()
    {
        Debug.Log("told to teleport");
        StartCoroutine(doLaterStuff());
    }

}
