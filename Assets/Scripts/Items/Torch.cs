using Unity.Netcode;
using UnityEngine;

public class Torch : NetworkBehaviour
{
    private bool on = false;
    [SerializeField]private Light torchLight;

    public void ToggleTorch()
    {
        on = !on;
        torchLight.enabled = on;
    }

    [Rpc(SendTo.Everyone)]
    public void ToggleTorchRPC()
    {
        ToggleTorch();
    }
}
