using UnityEngine;

[CreateAssetMenu(fileName = "ConnecterType", menuName = "Map/Connecter Type")]

public class ConnecterType : ScriptableObject
{
    public string displayName;
    public GameObject creationPrefab;
    public GameObject actualPrefab;
}
