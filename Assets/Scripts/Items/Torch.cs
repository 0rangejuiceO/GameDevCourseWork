using UnityEngine;

public class Torch : MonoBehaviour
{
    private bool on = false;
    [SerializeField]private Light torchLight;

    public void ToggleTorch()
    {
        on = !on;
        torchLight.enabled = on;
    }
}
