using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Map/Enemy")]

public class Enemy : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public GameObject physicsShadow;
}
