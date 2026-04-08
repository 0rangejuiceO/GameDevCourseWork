using UnityEngine;
using UnityEngine.InputSystem;
using FPController;
using System;

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

    public static event Action DropItemEvent;
    public static event Action DestroyItemEvent;

    private void OnEnable()
    {
        itemSlotAction.action.actionMap.Enable();
        itemSlotAction.action.performed += SetActiveSlot;
        itemSlotAction.action.Enable();
        dropItemAction.action.actionMap.Enable();
        dropItemAction.action.performed += OnDropItem;
        dropItemAction.action.Enable();
        InventoryHandler.DropItemEvent += DropItem;
        InventoryHandler.DestroyItemEvent += DestroyItem;

    }

    private void OnDisable()
    {

        itemSlotAction.action.performed -= SetActiveSlot;
        itemSlotAction.action.Disable();
        dropItemAction.action.performed -= OnDropItem;
        dropItemAction.action.Disable();
        InventoryHandler.DropItemEvent -= DropItem;
        InventoryHandler.DestroyItemEvent -= DestroyItem;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        inventoryUIStart = GameObject.Find("InventorySlotStartPoint").GetComponent<Transform>();

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
        newItem.transform.parent = camera;
        newItem.transform.localPosition = cameraOffset;
        newItem.transform.localRotation = Quaternion.identity;

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;



        if (currentInventory[currentSlot] != null)
        {
            DropItem(currentInventory[currentSlot]);
        }
        currentInventory[currentSlot] = newItem;
        UpdateInventorySlot(newItem);
    }

    public static void onDropItem()
    { 
        Debug.Log("DropItemEvent Invoked");
        DropItemEvent?.Invoke();
    }

    private void OnDropItem(InputAction.CallbackContext context)
    {
        DropItem(currentInventory[currentSlot]);
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName("");
        currentInventory[currentSlot] = null;
    }

    public void DropItem()
    {
        DropItem(currentInventory[currentSlot]);
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName("");
        currentInventory[currentSlot] = null;
    }

    private void DropItem(GameObject itemToDrop)
    {
        Vector3 spawnLocation = camera.position + (camera.forward * spawnOffset);
        Quaternion rotation = Quaternion.LookRotation(camera.forward);
        itemToDrop.transform.parent = null;
        itemToDrop.transform.rotation = rotation;
        itemToDrop.transform.position = spawnLocation;
        Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

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

    public static void onDestroyItem()
    {
        Debug.Log("DestroyItemEvent Invoked");
        DestroyItemEvent?.Invoke();
    }

    private void DestroyItem()
    {
        GameObject itemToDestroy = currentInventory[currentSlot];
        inventorySlots[currentSlot].GetComponent<InventorySlot>().SetItemName("");
        currentInventory[currentSlot] = null;
        Destroy(itemToDestroy);
    }

    public GameObject GetCurrentItem()
    {
        return currentInventory[currentSlot];
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
