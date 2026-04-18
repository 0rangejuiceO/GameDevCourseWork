using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.Netcode;
using System.Collections.Generic;

public class InteractChecker : NetworkBehaviour
{
    [SerializeField]private float rayDistance = 100f;
    [SerializeField]private InputActionReference interactAction;
    [SerializeField] private InputActionReference holdInteractAction;
    [SerializeField] private InputActionReference pressItemAction;
    [SerializeField] private float openDoorForce = 5f;
    [SerializeField] private InventoryHandler inventoryHandler;
    [SerializeField]private GameObject miniGameHandler;
    [SerializeField] private Camera playerCamera;
    [SerializeField]private LayerMask interactableLayers;

    private string interactMessage="";

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("InteractChecker spawned for local player.");

            miniGameHandler = GameObject.Find("MiniGameHandler");

            interactAction.action.performed -= OnInteract;
            interactAction.action.performed += OnInteract;

            holdInteractAction.action.performed -= OnHoldInteract;
            holdInteractAction.action.performed += OnHoldInteract;

            pressItemAction.action.performed -= OnPressItem;
            pressItemAction.action.performed += OnPressItem;

            interactAction.action.Enable();
            holdInteractAction.action.Enable();
            pressItemAction.action.Enable();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (!IsOwner) { return; }
        interactAction.action.performed -= OnInteract;
        holdInteractAction.action.performed -= OnHoldInteract;
        pressItemAction.action.performed -= OnPressItem;

        interactAction.action.Disable();
        holdInteractAction.action.Disable();
        pressItemAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext  context)
    {
        if (!IsOwner) { return; }
        Debug.Log("Interact called");
        Interact();
    }

    private void OnHoldInteract(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }
        HoldItem();
    }

    private void OnPressItem(InputAction.CallbackContext context)
    {
        if (!IsOwner) { return; }
        PressItem();
    }

    private void Interact()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance,interactableLayers))
        {
            string hitTag = hit.collider.gameObject.tag;
            string hitName = hit.collider.gameObject.name;
            Debug.Log($"Hit object tag: {hitTag}\nHit object name: {hitName}");


            if(hitTag == "Item")
            {
                inventoryHandler.AddItemToInventory(hit.collider.gameObject);
                //hit.collider.gameObject.SetActive(false);
            }
            else if( hitTag == "Generator")
            {
                hit.collider.gameObject.GetComponent<Generator>().FlipState();
            }
            else
            {
                if(hit.collider.isTrigger)
                {
                    Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                    if(interactable != null)
                    {
                        hit.collider.gameObject.GetComponent<Interactable>().Interact();
                    }
                    
                }
            }
        }
    }

    private void HoldItem()
    {
        Debug.Log("Hold Item Called");

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance,interactableLayers))
        {
            string hitTag = hit.collider.gameObject.tag;
            string hitName = hit.collider.gameObject.name;

            try
            {
                GameObject currentItem = inventoryHandler.GetCurrentItem();

                if (currentItem != null)
                {
                    Debug.Log($"{currentItem.name}");
                    if(hitTag == "Generator")
                    {
                        if (currentItem.name.Contains("Generator"))
                        {
                            if (Generator.GetGeneratorBroken())
                            {
                                Generator.SetGeneratorBroken(false);
                                inventoryHandler.DropItem(currentItem, true);
                            }
                        }
                    }
                    else if(hit.collider.isTrigger)
                    {
                        Debug.Log("Found IsTrigger");
                        HoldInteractable holdInteractable = hit.collider.gameObject.GetComponent<HoldInteractable>();

                        if (holdInteractable != null)
                        {
                            holdInteractable.GetComponent<HoldInteractable>().Interact(currentItem.name);
                        }
                    }
                    else
                    {
                        currentItem = inventoryHandler.GetCurrentItem();
                        if (currentItem.name.Contains("Medkit"))
                        {
                            currentItem.GetComponent<Medkit>().HealSelf();
                        }
                    }



                }
                return;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }


        }
        try
        {
            GameObject currentItem = inventoryHandler.GetCurrentItem();
            if (currentItem.name.Contains("Medkit"))
            {
                currentItem.GetComponent<Medkit>().HealSelf();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    private void PressItem()
    {
        Debug.Log("PressItem called");
        try
        {
            GameObject currentItem = inventoryHandler.GetCurrentItem();
            if(currentItem != null)
            {
                string itemName = currentItem.GetComponent<ItemName>().actualName;

                switch (itemName)
                {
                    case "Torch":
                        currentItem.GetComponent<Torch>().ToggleTorchRPC();
                        break;
                    case "Pistol":
                        currentItem.GetComponent<Pistol>().shoot();
                        break;
                    default:
                        Debug.Log("Current item cannot be used with press.");
                        break;

                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error in PressItem: " + e.Message);
            return;
        }
    }


    private void Update()
    {

        if(!IsOwner) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance, interactableLayers))
        {
            if (hit.collider.isTrigger)
            {
                //hit.collider.gameObject.GetComponent<Interactable>().Interact();
                
                Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();

                if (interactable != null)
                {
                    if (interactMessage != hit.collider.gameObject.GetComponent<Interactable>().interactionPrompt)
                    {
                        interactMessage = hit.collider.gameObject.GetComponent<Interactable>().interactionPrompt;
                        Debug.Log(interactMessage);
                    }
                }


            }
            else
            {
                if(interactMessage != "")
                {
                    interactMessage = "";
                }
            }
        }
        else
        {
            if(interactMessage != "")
            {
                interactMessage = "";
            }
        }


    }



}