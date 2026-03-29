using UnityEngine;
using UnityEngine.InputSystem;

public class Medkit : MonoBehaviour
{

    public void HealSelf()
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
