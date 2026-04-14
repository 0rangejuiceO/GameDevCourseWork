using System.Collections.Generic;
using UnityEngine;
using System;

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
    [SerializeField] private GameObject doorPrefab;

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

    public GameObject[] GetDoorsToReplace()
    {
        GameObject[] doorObjectsToOpen = new GameObject[doorsToOpen.Count];
        for(int i= 0; i<doorsToOpen.Count;i++)
        {
            switch (doorsToOpen[i])
            {
                case "Red": doorObjectsToOpen[i] = forwardDoor; break;
                case "Green": doorObjectsToOpen[i] = backDoor; break;
                case "Blue": doorObjectsToOpen[i] = leftDoor; break;
                case "Yellow": doorObjectsToOpen[i] = rightDoor; break;
            }
        }

        return doorObjectsToOpen;
    }

    public GameObject GetDoorPrefab()
    {
        return doorPrefab;
    }

    public void OpenDoors()
    {
        foreach(string door in doorsToOpen)
        {
            switch (door) 
            { 
                case "Red": tellDoorItsOpen(forwardDoor); break; 
                case "Green": tellDoorItsOpen(backDoor); break; 
                case "Blue": tellDoorItsOpen(leftDoor); break; 
                case "Yellow": tellDoorItsOpen(rightDoor); break; 
            }
        }
    }

    private void tellDoorItsOpen(GameObject door)
    {
        try
        {
            Vector3 position = door.transform.position;
            Quaternion rotation = door.transform.rotation;

            var newDoor = Instantiate(doorPrefab, position, rotation,transform);

            newDoor.GetComponentInChildren<Door>().canOpen = true;

            door.SetActive(false);
            //Debug.Log("set canOpen to true");
        }
        catch (Exception e)
        {
            return;
        }
    }
}
