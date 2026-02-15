using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    [SerializeField] private int lineLength = 5;
    [Header("Red Door")]
    public GameObject forwardDoor;
    [Header("Green Door")] 
    public GameObject backDoor; 
    [Header("Blue Door")] 
    public GameObject leftDoor; 
    [Header("Yellow Door")] 
    public GameObject rightDoor;
    [Header("DoorsToOpen")]
    public List<string> doorsToOpen = new List<string>();

    private void OnDrawGizmosSelected()
    {
        Vector3[] directions = new Vector3[] { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        Color[] colours = new Color[] { Color.red, Color.green, Color.blue, Color.yellow };

        

        for(int i =0; i< 4; i++)
        {
            Vector3 to = directions[i] * lineLength + transform.position;
            Gizmos.color = colours[i];
            Gizmos.DrawLine(transform.position, to);
        }

    }

    public void OpenDoors()
    {
        foreach(string door in doorsToOpen)
        {
            switch (door) 
            { 
                case "Red": forwardDoor.SetActive(false); break; 
                case "Green": backDoor.SetActive(false); break; 
                case "Blue": leftDoor.SetActive(false); break; 
                case "Yellow": rightDoor.SetActive(false); break; 
            }
        }
    }
}
