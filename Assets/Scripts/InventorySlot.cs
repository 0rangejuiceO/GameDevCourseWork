using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text numTxt;
    [SerializeField] private TMP_Text nameTxt;

    public void SetNum(int num)
    {
        numTxt.text = $"{num}";
    }

    public void SetItemName(string itemName)
    {
        nameTxt.text = itemName;
    }
}
