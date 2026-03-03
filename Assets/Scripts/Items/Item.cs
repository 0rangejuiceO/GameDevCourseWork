using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Map/Item")]

public class Item : ScriptableObject
{
    public string displayName;
    public GameObject prefab;
}
