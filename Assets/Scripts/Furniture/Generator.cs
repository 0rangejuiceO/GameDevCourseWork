using System;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool isOn;
    public bool isBroken = false;

    public static event Action<bool,bool> OnGeneratorStateChanged;
    public static event Action<bool,bool> OnGeneratorBrokenStatusChanged;


    private PowerHandler powerHandler;

    void OnEnable()
    {
        Generator.OnGeneratorStateChanged += HandleGeneratorPower;
        Generator.OnGeneratorBrokenStatusChanged += HandleGeneratorState;

        powerHandler = FindFirstObjectByType<PowerHandler>();

    }

    void OnDisable()
    {
        Generator.OnGeneratorStateChanged -= HandleGeneratorPower;
        Generator.OnGeneratorBrokenStatusChanged -= HandleGeneratorState;
    }

    private void HandleGeneratorState(bool state,bool fromNetwork)
    {
        isBroken = state;
        if (isBroken)
        {
            TurnOff();
        }

    }

    private void HandleGeneratorPower(bool power,bool fromNetwork)
    {
        isOn = power;
    }


    public void TurnOn()
    {
        
        OnGeneratorStateChanged?.Invoke(true,false);
    }

    public void TurnOff()
    {

        OnGeneratorStateChanged?.Invoke(false,false);
    }

    public void FlipState()
    {
        if (isOn)
        {
            TurnOff();

        }
        else
        {
            if (!isBroken)
            {
                TurnOn();
            }

        }
    }

    public void FixGenerator()
    {
        OnGeneratorBrokenStatusChanged(!isBroken, false);
    }

    private int powerFailChance = 20000000; // 1 in x chance per game tick this runs separately for each player so more players higher chance of lights turning off

    private void Update()
    {
        if (!isOn)
        {
            return;
        }

        int num = UnityEngine.Random.Range(1, powerFailChance);
        if (num == 1)
        {
            Debug.Log("Lights randomly turned off");
            TurnOff();
        }
    }

    public static void SetGeneratorPower(bool state, bool networkSend = false)
    {
        OnGeneratorStateChanged?.Invoke(state,networkSend);
    }

    public static void SetGeneratorBroken(bool state, bool networkSend = false)
    {
        OnGeneratorBrokenStatusChanged?.Invoke(state,networkSend);
    }

    public bool GetGeneratorBroken()
    {
        return isBroken;
    }

}
