using UnityEngine;
using UnityEngine.InputSystem;
using FPController;
using Unity.Netcode;

public class DebugMenuHandler : NetworkBehaviour
{
    [SerializeField] private InputActionReference openDebugMenuAction;
    [SerializeField] private GameObject debugMenuObject;
    [SerializeField]private FPController.FPController playerController;

    private bool menuIsOpen = true;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log("attempting to find debug menu object");
            debugMenuObject = GameObject.Find("DebugMenu");


            openDebugMenuAction.action.actionMap.Enable();
            openDebugMenuAction.action.performed += OnInteract;
            openDebugMenuAction.action.Enable();

        }

    }


    private void OnDisable()
    {
        if (!IsOwner) {return; }
        openDebugMenuAction.action.performed -= OnInteract;
        openDebugMenuAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if(!IsOwner) {return;}
        OpenMenu();
    }

    private void OpenMenu()
    {
        Debug.Log("menu called");
        float xScale = debugMenuObject.GetComponent<RectTransform>().localScale.x;
        if (xScale == 0)
        {
            debugMenuObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            menuIsOpen = true;
            Debug.Log("Debug Menu Opened");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            playerController.LockMovement = true;
            return;
        }

        if (menuIsOpen)
        {
            debugMenuObject.SetActive(false);
            menuIsOpen = false;
            Debug.Log("Debug Menu Closed");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerController.LockMovement = false;
        }
        else
        {
            debugMenuObject.SetActive(true);
            menuIsOpen=true;
            Debug.Log("Debug Menu Opened");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible =true;
            playerController.LockMovement = true;
        }

    }
}
