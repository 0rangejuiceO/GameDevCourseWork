using UnityEngine;

public class StairHandler : MonoBehaviour
{
    [SerializeField] private GameObject topCeiling;
    [SerializeField] private GameObject bottomFloor;
    [SerializeField] private GameObject[] topCeilingFalse;

    public void isTopCeiling()
    {
        topCeiling.SetActive(true); 
        foreach (GameObject ceiling in topCeilingFalse) 
        { 
            ceiling.SetActive(false); 
        }
    }

    public void isBottomFloor()
    {
        bottomFloor.SetActive(true);
    }
}
