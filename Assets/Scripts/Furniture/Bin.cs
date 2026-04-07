using UnityEngine;

public class Bin : MonoBehaviour
{

    public void DestroyItem()
    {
        InventoryHandler.onDestroyItem();
    }
}
