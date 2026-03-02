using UnityEngine;
using UnityEngine.InputSystem;

public class DebugMenuHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference openDebugMenuAction;
    [SerializeField] private GameObject debugMenuObject;

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
        }
        else
        {
            debugMenuObject.SetActive(true);
            menuIsOpen=true;
            Debug.Log("Debug Menu Opened");
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible =true;
        }
    }
}
