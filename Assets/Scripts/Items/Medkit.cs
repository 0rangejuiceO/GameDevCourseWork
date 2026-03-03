using UnityEngine;
using UnityEngine.InputSystem;

public class Medkit : MonoBehaviour
{
    [SerializeField]private InputActionReference openDebugMenuAction;

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
        HealSelf();
    }

    private void HealSelf()
    {
        PlayerHealth playerHealth = GetComponentInParent<PlayerHealth>();
        InventoryHandler inventoryHandler = GetComponentInParent<InventoryHandler>();
        if (playerHealth != null && inventoryHandler != null)
        {
            playerHealth.TakeDamage(-50);
            inventoryHandler.DropItem(gameObject, true);
        }
        else
        {
            Debug.LogError("PlayerHealth or InventoryHandler component not found in parent objects.");
        }
    }
}
