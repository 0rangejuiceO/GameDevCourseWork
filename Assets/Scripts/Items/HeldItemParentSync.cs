using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class HeldItemParentSync : NetworkBehaviour
{
    [SerializeField] private Vector3 heldLocalPos = new Vector3(0.65f, -0.5f, 1.1f);
    [SerializeField] private Quaternion heldLocalRot = Quaternion.identity;
    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        if (parentNetworkObject != null)
        {

            // 2. Only the Authority (Server) can call Teleport to sync it
            if (IsServer && TryGetComponent(out NetworkTransform nt))
            {
                nt.Teleport(heldLocalPos, heldLocalRot, transform.localScale);
            }

            // 3. Force Physics off
            if (TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false; // Set this to false! Your log showed it was still True.
            }
        }
        else
        {

            if (TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.useGravity = true; // Set this to false! Your log showed it was still True.
            }
        }
    }
}
