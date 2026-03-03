using UnityEngine;
using UnityEngine.InputSystem;
using FPController;

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
    [SerializeField] private Transform camera;
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField]private FPController.FPController playerController;


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
        var realNewItem = Instantiate(newItem, new Vector3(0,100,0), Quaternion.identity);
        realNewItem.SetActive(false);
        Rigidbody rb = realNewItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        realNewItem.transform.parent = camera;
        realNewItem.transform.localPosition = cameraOffset;
        realNewItem.transform.localRotation = Quaternion.identity;


        if (currentInventory[currentSlot] != null)
        {
            DropItem(currentInventory[currentSlot]);
        }
        currentInventory[currentSlot] = realNewItem;
        UpdateInventorySlot(realNewItem);
    }

    private void OnDropItem(InputAction.CallbackContext context)
    {
        DropItem(currentInventory[currentSlot]);
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName("");
        currentInventory[currentSlot] = null;
    }

    private void DropItem(GameObject itemToDrop)
    {
        Vector3 spawnLocation = camera.position + (camera.forward * spawnOffset);
        Quaternion rotation = Quaternion.LookRotation(camera.forward);
        var newItem = Instantiate(itemToDrop, spawnLocation, rotation);
        newItem.SetActive(true);
        newItem.name=itemToDrop.name;
        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        Destroy(itemToDrop);
    }

    public void DropItem(GameObject itemToDrop, bool destoryItem)
    {

        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName("");
        currentInventory[currentSlot] = null;
        if (destoryItem)
        {
            Destroy(itemToDrop);
        }
        else
        {
            DropItem(itemToDrop);
        }
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
        if(playerController.LockMovement)
        {
            return;
        }


        float value = context.ReadValue<float>();
        int newSlot = Mathf.RoundToInt(value)-1;

        if(newSlot >= inventorySlots.Length)
        {
            return;
        }

        previousSlot = currentSlot;
        currentSlot = newSlot;
        Debug.Log($"Current Slot {currentSlot+1}");

        if (currentInventory[previousSlot] != null)
        {
            currentInventory[previousSlot].SetActive(false);
        }

        if (currentInventory[currentSlot] != null)
        {
            currentInventory[currentSlot].SetActive(true);
        }

    }


    private void UpdateInventorySlot(GameObject newItem)
    {
        string itemName = newItem.GetComponent<ItemName>().actualName;
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName(itemName);
    }

}
