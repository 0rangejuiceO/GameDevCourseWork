using System;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool isOn;
    public static bool isBroken = false;

    public static event Action<bool> OnGeneratorStateChanged;
    public static event Action<bool> OnGeneratorBrokenStatusChanged;

    void OnEnable()
    {
        Generator.OnGeneratorStateChanged += HandleGeneratorPower;
        Generator.OnGeneratorBrokenStatusChanged += HandleGeneratorState;
    }

    void OnDisable()
    {
        Generator.OnGeneratorStateChanged -= HandleGeneratorPower;
        Generator.OnGeneratorBrokenStatusChanged -= HandleGeneratorState;
    }

    void HandleGeneratorState(bool state)
    {
        isBroken = state;
        if (isBroken)
        {
            TurnOff();
            isOn = false;
        }

    }

    private void HandleGeneratorPower(bool power)
    {
        isOn = power;
    }


    public void TurnOn()
    {
        
        OnGeneratorStateChanged?.Invoke(true);
    }

    public void TurnOff()
    {

        OnGeneratorStateChanged?.Invoke(false);
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

    private int powerFailChance = 5000; // 1 in 5000 chance per game tick

    private void Update()
    {
        if (isOn)
        {
            return;
        }

        int num = UnityEngine.Random.Range(1, powerFailChance);
        if (num == 1)
        {
            TurnOff();
        }
    }

    public static void SetGeneratorPower(bool state)
    {
        OnGeneratorStateChanged?.Invoke(state);
    }

    public static void SetGeneratorBroken(bool state)
    {
        OnGeneratorBrokenStatusChanged?.Invoke(state);
    }

    public static bool GetGeneratorBroken()
    {
        return isBroken;
    }

}
