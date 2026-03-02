using UnityEngine;
using UnityEngine.InputSystem;


public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private int inventorySize = 4;

    private GameObject[] currentInventory;
    private int itemsInInventory;
    private int previousSlot = 0;
    private int currentSlot = 0;
    [SerializeField] private float spawnOffset;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform inventoryUIStart;
    private GameObject[] inventorySlots;
    [SerializeField]private InputActionReference itemSlotAction;
    [SerializeField] private InputActionReference dropItemAction;


    private void OnEnable()
    {
        itemSlotAction.action.actionMap.Enable();
        itemSlotAction.action.performed += SetActiveSlot;
        itemSlotAction.action.Enable();
        dropItemAction.action.actionMap.Enable();
        dropItemAction.action.performed += OnDropItem;
        dropItemAction.action.Enable();
    }

    private void OnDisable()
    {

        itemSlotAction.action.performed -= SetActiveSlot;
        itemSlotAction.action.Disable();
        dropItemAction.action.performed -= OnDropItem;
        dropItemAction.action.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInventory = new GameObject[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            currentInventory[i] = null;
        }

        inventorySlots = new GameObject[inventorySize];

        createInventoryUI();
    }

    public void AddItemToInventory(GameObject newItem)
    {
        if (currentInventory[currentSlot] != null)
        {
            DropItem(currentInventory[currentSlot]);
        }
        currentInventory[currentSlot] = newItem;
        UpdateInventorySlot(newItem);
    }

    private void OnDropItem(InputAction.CallbackContext context)
    {
        DropItem(currentInventory[currentSlot]);
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName("");
        currentInventory[currentSlot] = null;
    }

    private void DropItem(GameObject itemToDrop)
    {
        Vector3 spawnLocation = transform.position + (transform.forward * spawnOffset);

        var newItem = Instantiate(itemToDrop, spawnLocation, Quaternion.identity);
        newItem.SetActive(true);
        newItem.name=itemToDrop.name;
        Destroy(itemToDrop);
    }

    private void createInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            Vector3 location = new Vector3(inventoryUIStart.transform.position.x + (i* 125), inventoryUIStart.transform.position.y, inventoryUIStart.transform.position.z);
            var newSlot = Instantiate(inventorySlotPrefab, location, Quaternion.identity, inventoryUIStart);
            RectTransform rt = newSlot.GetComponent<RectTransform>();

            Vector2 pos = rt.anchoredPosition;
            pos.x = location.x;
            rt.anchoredPosition = pos;


            newSlot.GetComponent<InventorySlot>().SetNum(i + 1);
            newSlot.GetComponent<InventorySlot>().SetItemName("");
            inventorySlots[i] = newSlot;
        }
    
    }

    private void SetActiveSlot(InputAction.CallbackContext context)
    {
        
        float value = context.ReadValue<float>();
        previousSlot = currentSlot;
        currentSlot = Mathf.RoundToInt(value)-1;
        Debug.Log($"Current Slot {currentSlot+1}");
    }


    private void UpdateInventorySlot(GameObject newItem)
    {
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName(newItem.name);
    }

}
