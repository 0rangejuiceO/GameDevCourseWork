using UnityEngine;

public class NetworkItem : MonoBehaviour
{
    public GameObject rbObject;
    public GameObject heldObject;

    public void SetItem(GameObject rbObj, GameObject heldObj)
    {
        rbObject = rbObj;
        heldObject = heldObj;
    }
}
