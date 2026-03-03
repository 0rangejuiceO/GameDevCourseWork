using UnityEngine;
using UnityEngine.InputSystem;
using FPController;

public class DebugMenuHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference openDebugMenuAction;
    [SerializeField] private GameObject debugMenuObject;
    [SerializeField]private FPController.FPController playerController;

    private bool menuIsOpen = false;

    private void OnEnable()
    {
        openDebugMenuAction.action.actionMap.Enable();
        openDebugMenuAction.action.performed += OnInteract;
        openDebugMenuAction.action.Enable();
    }

    private void OnDisable()
    {

        openDebugMenuAction.action.performed -= OnInteract;
        openDebugMenuAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        OpenMenu();
    }

    private void OpenMenu()
    {
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
