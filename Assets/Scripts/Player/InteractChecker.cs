using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InteractChecker : MonoBehaviour
{
    [SerializeField]private float rayDistance = 100f;
    [SerializeField]private InputActionReference interactAction;
    [SerializeField] private float openDoorForce = 5f;
    [SerializeField] private InventoryHandler inventoryHandler;
    [SerializeField]private GameObject miniGameHandler;
    [SerializeField] private Camera playerCamera;

    private void OnEnable()
    {
        interactAction.action.actionMap.Enable();
        interactAction.action.performed += OnInteract;
        interactAction.action.Enable();
    }

    private void OnDisable()
    {
        
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext  context)
    {
        Interact();
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
        }
    }


}