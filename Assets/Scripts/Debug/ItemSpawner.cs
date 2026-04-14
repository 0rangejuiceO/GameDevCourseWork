using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] private Item[] items;
    [SerializeField] private GameObject itemHolderPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Dropdown itemList;
    [SerializeField] private float spawnOffset;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            itemList = GameObject.Find("SpawnItemDropdown").GetComponent<TMP_Dropdown>();

            itemList.ClearOptions();

            List<string> itemNames = new List<string>();

            foreach (var item in items)
            {
                itemNames.Add(item.displayName);
            }

            itemList.AddOptions(itemNames);

            itemList.RefreshShownValue();

            Button spawnItemsButton = GameObject.Find("SpawnItemButton").GetComponent<Button>();
            spawnItemsButton.onClick.AddListener(spawnItem);

        }
    }

    public void spawnItem()
    {
        int index = itemList.value;

        Vector3 spawnLocation = player.position + (player.forward * spawnOffset);

        

        NetworkObject netObject = items[index].prefab.GetComponent<NetworkObject>();
        if (netObject != null)
        {
          
            SpawnNewItemRPC(index, spawnLocation);
        }
        else
        {
            var newItem = Instantiate(items[index].prefab, spawnLocation, Quaternion.identity);
        }

    }

    [Rpc(SendTo.Server)]
    private void SpawnNewItemRPC(int index, Vector3 location)
    {

        var newItem = Instantiate(items[index].prefab, location, Quaternion.identity);

        newItem.GetComponent<NetworkObject>().Spawn();

        Debug.Log($"Spawned item {newItem.name} on the server.");

    }




}
