using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Unity.Netcode;

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

    private void Awake()
    {
        miniGameHandler = GameObject.Find("MiniGameHandler");
    }
    private void OnEnable()
    {
        interactAction.action.actionMap.Enable();
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();

        holdInteractAction.action.actionMap.Enable();
        holdInteractAction.action.performed += OnHoldInteract;
        holdInteractAction.action.Enable();

        pressItemAction.action.actionMap.Enable();
        pressItemAction.action.performed += OnPressItem;
        pressItemAction.action.Enable();
    }

    private void OnDisable()
    {
        
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();

        holdInteractAction.action.performed -= OnHoldInteract;
        holdInteractAction.action.Disable();

        pressItemAction.action.performed -= OnPressItem;
        holdInteractAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext  context)
    {
        Interact();
    }

    private void OnHoldInteract(InputAction.CallbackContext context)
    {
        HoldItem();
    }

    private void OnPressItem(InputAction.CallbackContext context)
    {
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
                hit.collider.gameObject.SetActive(false);
            }
            else if(hitTag == "GameMachine")
            {
                miniGameHandler.SetActive(true);

                miniGameHandler.GetComponent<MiniGameHandler>().StartMiniGame(hit.collider.gameObject);
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
                    if (hitTag == "NO")
                    {

                        if (currentItem.name.Contains("Key"))
                        {

                            if (hit.collider.gameObject.GetComponent<Door>().isLocked)
                            {
                                hit.collider.gameObject.GetComponent<Door>().isLocked = false;
                                hit.collider.gameObject.GetComponent<Door>().rb.isKinematic = false;
                                inventoryHandler.DropItem(currentItem, true);

                            }
                        }
                    }
                    else if(hitTag == "Generator")
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
                    
                    
                    if(hitTag == "Player")
                    {
                        
                    }
                    else
                    {
                        if (currentItem.name.Contains("Medkit"))
                        {
                            currentItem.GetComponent<Medkit>().HealSelf();
                            inventoryHandler.DropItem(currentItem, false);
                        }
                    }

                    if(hit.collider.isTrigger)
                    {
                        HoldInteractable holdInteractable = hit.collider.gameObject.GetComponent<HoldInteractable>();

                        if (holdInteractable != null)
                        {
                            holdInteractable.GetComponent<HoldInteractable>().Interact(currentItem.name);
                        }
                    }



                }

            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }


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
                        currentItem.GetComponent<Torch>().ToggleTorch();
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