using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private int inventorySize = 4;

    private GameObject[] currentInventory;
    private int itemsInInventory;
    private int currentSlot = 0;
    [SerializeField] private float spawnOffset;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform inventoryUIStart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInventory = new GameObject[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            currentInventory[i] = null;
        }
    }

    public void AddItemToInventory(GameObject newItem)
    {
        if (currentInventory[currentSlot] == null)
        {
            currentInventory[currentSlot] = newItem;
        }
        else
        {
            DropItem(currentInventory[currentSlot]);
            currentInventory[currentSlot] = newItem;
        }
    }

    private void DropItem(GameObject itemToDrop)
    {
        Vector3 spawnLocation = transform.position + (transform.forward * spawnOffset);

        var newItem = Instantiate(itemToDrop, spawnLocation, Quaternion.identity);
    }

    private void createInventoryUI()
    {
        for(int i = 0; i < inventorySize; i++)
        {
            Vector3 location = new Vector3(inventoryUIStart.transform.position.x + 125,inventoryUIStart.transform.position.y, inventoryUIStart.transform.position.z);
            var newSlot = Instantiate(inventorySlotPrefab, location, Quaternion.identity, inventoryUIStart);
        }
    }

}
