using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Item[] items;
    [SerializeField] private Transform player;
    [SerializeField] private TMP_Dropdown itemList;
    [SerializeField] private float spawnOffset;

    private void Awake()
    {
        itemList.ClearOptions();

        List<string> itemNames = new List<string> ();

        foreach (var item in items)
        {
            itemNames.Add(item.displayName);
        }

        itemList.AddOptions(itemNames);

        itemList.RefreshShownValue();
    }

    public void spawnItem()
    {
        int index = itemList.value;

        Vector3 spawnLocation = player.position + (player.forward * spawnOffset);

        var newItem = Instantiate(items[index].prefab, spawnLocation, Quaternion.identity);

    }



}
