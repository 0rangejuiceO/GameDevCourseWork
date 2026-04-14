using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class DebugToggleGenerator : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Button powerOnButton = GameObject.Find("TogglePowerOn").GetComponent<Button>();
            powerOnButton.onClick.AddListener(() => TogglePower(true));

            Button powerOffButton = GameObject.Find("TogglePowerOff").GetComponent<Button>();
            powerOffButton.onClick.AddListener(() => TogglePower(false));

            Button breakButton = GameObject.Find("ToggleGeneratorStatusBroken").GetComponent<Button>();
            breakButton.onClick.AddListener(() => ToggleStatus(false));

            Button fixButton = GameObject.Find("ToggleGeneratorStatusFixed").GetComponent<Button>();
            fixButton.onClick.AddListener(() => ToggleStatus(true));
        }
    }

    public void TogglePower(bool isOn)
    {
        Generator.SetGeneratorPower(isOn);
    }

    public void ToggleStatus(bool status)
    {
        Generator.SetGeneratorBroken(status);
    }
}
