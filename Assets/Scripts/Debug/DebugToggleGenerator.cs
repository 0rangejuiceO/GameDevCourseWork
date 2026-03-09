using UnityEngine;


public class DebugToggleGenerator : MonoBehaviour
{

    public void TogglePower(bool isOn)
    {
        Generator.SetGeneratorPower(isOn);
    }

    public void ToggleStatus(bool status)
    {
        Generator.SetGeneratorBroken(status);
    }
}
