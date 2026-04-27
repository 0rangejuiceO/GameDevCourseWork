using System;
using Unity.Netcode;
using UnityEngine;

public class PowerHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        Generator.OnGeneratorStateChanged += HandleGeneratorPower;
        Generator.OnGeneratorBrokenStatusChanged += HandleGeneratorState;
    }

    void OnDisable()
    {
        Generator.OnGeneratorStateChanged -= HandleGeneratorPower;
        Generator.OnGeneratorBrokenStatusChanged -= HandleGeneratorState;

    }

    private void HandleGeneratorState(bool state, bool fromNetwork)
    {
        if (!fromNetwork)
        {
            TellEveyoneBrokenStatusRPC(state);
        }
       
    }
    private void HandleGeneratorPower(bool power, bool fromNetwork)
    {
        if (!fromNetwork)
        {
            TellEveryonePowerRPC(power);
        }
    }

    [Rpc(SendTo.NotMe, InvokePermission = RpcInvokePermission.Everyone)]
    private void TellEveryonePowerRPC(bool power)
    {
        Generator.SetGeneratorPower(power,true);
    }

    [Rpc(SendTo.NotMe, InvokePermission = RpcInvokePermission.Everyone)]
    private void TellEveyoneBrokenStatusRPC(bool status)
    {
        Generator.SetGeneratorBroken(status,true);
    }



}

