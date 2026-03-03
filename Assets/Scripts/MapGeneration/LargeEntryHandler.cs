using UnityEngine;

public class LargeEntryHandler : MonoBehaviour
{
    public Transform scaler;
    [SerializeField] private GameObject exitDoorFrameLeft;
    [SerializeField] private GameObject exitDoorFrameRight;
    [SerializeField] private GameObject entryDoorFrameLeft;
    [SerializeField] private GameObject entryDoorFrameRight;
    [SerializeField] private GameObject exitDoorFrameTop;
    [SerializeField] private GameObject entryDoorFrameTop;
    [SerializeField] private Vector2 doorSize = new Vector2(3, 3);

    public void setDoorFrameScale(bool isEntry, Vector3 doorCenter, Vector3 direction)
    {
        // Convert door center to room-local coordinates
        Vector3 localDoorPos = transform.InverseTransformPoint(doorCenter);

        // X is the room's total horizontal span (scaler.localScale.x * 5)
        float roomWidth = scaler.localScale.x * 5f;
        float doorHalfWidth = doorSize.x / 2f;

        GameObject leftFrame = isEntry ? entryDoorFrameLeft : exitDoorFrameLeft;
        GameObject rightFrame = isEntry ? entryDoorFrameRight : exitDoorFrameRight;
        GameObject topFrame = isEntry ? entryDoorFrameTop : exitDoorFrameTop;

        // 'd' is the horizontal offset of the door from the room center
        float d = localDoorPos.x;
        float e = localDoorPos.z;

        if (d > 0) // Door is shifted to the right
        {
            // 1. Scale the LEFT filler to bridge the gap from the left wall to the door
            float leftWidth = (roomWidth / 2f) + d - doorHalfWidth;
            leftFrame.transform.localScale = new Vector3(leftWidth, leftFrame.transform.localScale.y, leftFrame.transform.localScale.z);

            // 2. Position the LEFT filler (center point between left edge and door edge)
            float leftX = (-roomWidth / 2f + (d - doorHalfWidth)) / 2f;
            leftFrame.transform.localPosition = new Vector3(leftX, leftFrame.transform.localPosition.y, e);

            // 3. Keep RIGHT frame standard size and position it next to the door
            rightFrame.transform.localPosition = new Vector3(d + doorHalfWidth + (rightFrame.transform.localScale.x / 2f), rightFrame.transform.localPosition.y, e);
        }
        else // Door is shifted to the left (d < 0)
        {
            // 1. Scale the RIGHT filler
            float rightWidth = (roomWidth / 2f) - d - doorHalfWidth;
            rightFrame.transform.localScale = new Vector3(rightWidth, rightFrame.transform.localScale.y, rightFrame.transform.localScale.z);

            // 2. Position the RIGHT filler
            float rightX = (roomWidth / 2f + (d + doorHalfWidth)) / 2f;
            rightFrame.transform.localPosition = new Vector3(rightX, rightFrame.transform.localPosition.y, e);

            // 3. Keep LEFT frame standard size
            leftFrame.transform.localPosition = new Vector3(d - doorHalfWidth - (leftFrame.transform.localScale.x / 2f), leftFrame.transform.localPosition.y, e);
        }

        topFrame.transform.localPosition = new Vector3(d, topFrame.transform.localPosition.y, e);
    }

}
