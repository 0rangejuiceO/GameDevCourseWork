using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InteractChecker : MonoBehaviour
{
    [SerializeField]private float rayDistance = 100f;
    [SerializeField]private InputActionReference interactAction;
    [SerializeField] private InputActionReference holdInteractAction;
    [SerializeField] private float openDoorForce = 5f;
    [SerializeField] private InventoryHandler inventoryHandler;
    [SerializeField]private GameObject miniGameHandler;
    [SerializeField] private Camera playerCamera;

    private void OnEnable()
    {
        interactAction.action.actionMap.Enable();
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();

        holdInteractAction.action.actionMap.Enable();
        holdInteractAction.action.performed += OnHoldInteract;
        holdInteractAction.action.Enable();
    }

    private void OnDisable()
    {
        
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();

        holdInteractAction.action.performed -= OnHoldInteract;
        holdInteractAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext  context)
    {
        Interact();
    }

    private void OnHoldInteract(InputAction.CallbackContext context)
    {
        HoldInteract();
    }

    private void Interact()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            string hitTag = hit.collider.gameObject.tag;
            string hitName = hit.collider.gameObject.name;
            Debug.Log($"Hit object tag: {hitTag}\nHit object name: {hitName}");

            if(hitTag == "Door")
            {
                try
                {
                    hit.collider.gameObject.GetComponent<Door>().addForce(ray.direction,openDoorForce);
                }
                catch(Exception e)
                {
                    return;
                }
            }
            else if(hitTag == "Item")
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
        }
    }

    private void HoldInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            string hitTag = hit.collider.gameObject.tag;
            string hitName = hit.collider.gameObject.name;

            try
            {
                GameObject currentItem = inventoryHandler.GetCurrentItem();

                if (currentItem != null)
                {
                    Debug.Log($"{currentItem.name}");
                    if (hitTag == "Door")
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



                }

            }
            catch (Exception e)
            {
                return;
            }


        }
    }




}