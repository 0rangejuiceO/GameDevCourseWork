using FPController;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryHandler : NetworkBehaviour
{
    [SerializeField] private int inventorySize = 4;

    [SerializeField] private NetworkObject playerNetworkObject;
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

    private static Dictionary<ulong, GameObject> playerHeldItems = new Dictionary<ulong, GameObject>();

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            itemSlotAction.action.actionMap.Enable();
            itemSlotAction.action.performed += SetActiveSlot;
            itemSlotAction.action.Enable();
            dropItemAction.action.actionMap.Enable();
            dropItemAction.action.performed += OnDropItem;
            dropItemAction.action.Enable();
            InventoryHandler.DropItemEvent += DropItem;
            InventoryHandler.DestroyItemEvent += DestroyItem;

            inventoryUIStart = GameObject.Find("InventorySlotStartPoint").GetComponent<Transform>();

            currentInventory = new GameObject[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                currentInventory[i] = null;
            }

            inventorySlots = new GameObject[inventorySize];

            createInventoryUI();
        }
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


    public void AddItemToInventory(GameObject newItem)
    {
        NetworkObject netObj = newItem.GetComponent<NetworkObject>();
        if (netObj != null)
        {

            TellServerItemPickedUpRPC(newItem, playerNetworkObject);
        }
        else
        {

            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;


            newItem.transform.parent = camera;
            newItem.transform.localPosition = cameraOffset;
            newItem.transform.localRotation = Quaternion.identity;

        }


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
        NetworkObject netObj = itemToDrop.GetComponent<NetworkObject>();

        if (netObj != null)
        {
            TellServerDropItemRPC(netObj,playerNetworkObject);
        }
        else
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

        NetworkObject netObj = itemToDestroy.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            TellServerDestroyItemRPC(netObj);
        }
        else
        {
            Destroy(itemToDestroy);
        }


        
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


    [Rpc(SendTo.Server)]
    private void TellServerItemPickedUpRPC(NetworkObjectReference netObjRef, NetworkObjectReference playerObjRef, RpcParams rpcParams = default)
    {

        if (!IsServer) { return; }
        
        netObjRef.TryGet(out NetworkObject netObj);
        playerObjRef.TryGet(out NetworkObject playerObj);

        ulong OwnerClientId = rpcParams.Receive.SenderClientId;

        playerHeldItems[OwnerClientId] = netObj.gameObject;

        Debug.Log("Attempting to parent item to player on the server.");
        netObj.TrySetParent(playerObj,false);

        Rigidbody rb = netObj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        netObj.transform.localPosition = cameraOffset;
        netObj.transform.localRotation = Quaternion.identity;
        ulong clientId = rpcParams.Receive.SenderClientId;

        FollowCamera followCamera = netObj.GetComponent<FollowCamera>();

        followCamera.camera = camera;
        followCamera.isFollowing = true;

        netObj.ChangeOwnership(clientId);

        TellClientsRBStatusRPC(netObjRef,playerObj,clientId);

    }

    [Rpc(SendTo.Everyone)]
    private void TellClientsRBStatusRPC(NetworkObjectReference netObjRef,NetworkObjectReference playerObjectRef, ulong ID=999UL,bool follow=true,RpcParams rpcParams = default)
    {

        if (IsServer) return;

        if (netObjRef.TryGet(out NetworkObject netObj))
        {

            if (!netObj.gameObject.activeSelf)
            {
                netObj.gameObject.SetActive(true);
            }
        }

        playerObjectRef.TryGet(out NetworkObject playerObj);

        ulong clientId = NetworkManager.Singleton.LocalClientId;

        FollowCamera followCamera = netObj.GetComponent<FollowCamera>();
        if (follow)
        {
            if (clientId == ID)
            {

                followCamera.camera = camera;

            }
            else
            {
                followCamera.camera = playerObj.GetComponent<InventoryHandler>().camera;
            }
        }

        followCamera.isFollowing = follow;
    }

    [Rpc(SendTo.Server)]
    private void TellServerDropItemRPC(NetworkObjectReference netObjRef,NetworkObjectReference playerObjRef, RpcParams rpcParams = default)
    {

        netObjRef.TryGet(out NetworkObject netObj);
        playerObjRef.TryGet(out NetworkObject playerObj);
        if (netObj != null)
        {
            TellClientsRBStatusRPC(netObjRef, playerObj, 999UL, false);

            netObj.TryRemoveParent(true);

            InventoryHandler inventoryHandler = playerObj.GetComponent<InventoryHandler>();
            Vector3 spawnLocation = inventoryHandler.camera.position + (inventoryHandler.camera.forward * spawnOffset);
            Quaternion rotation = Quaternion.LookRotation(inventoryHandler.camera.forward);

            netObj.transform.rotation = rotation;
            netObj.transform.position = spawnLocation;

            Rigidbody rb = netObj.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;


            FollowCamera followCamera = netObj.GetComponent<FollowCamera>();

            followCamera.isFollowing = false;


            netObj.RemoveOwnership();

        }
    }

    [Rpc(SendTo.Server)]
    private void TellServerDestroyItemRPC(NetworkObjectReference netObjRef)
    {
        if (!IsServer) return;

        if (netObjRef.TryGet(out NetworkObject netObj))
        {
            netObj.Despawn();
        }
    }

}
