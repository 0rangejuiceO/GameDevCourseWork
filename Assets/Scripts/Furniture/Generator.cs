using System;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool isOn;
    public bool isBroken = false;

    public static event Action<bool> OnGeneratorStateChanged;
    public static event Action<bool> OnGeneratorBrokenStatusChanged;

    void OnEnable()
    {
        Generator.OnGeneratorStateChanged += HandleGeneratorState;
    }

    void OnDisable()
    {
        Generator.OnGeneratorStateChanged -= HandleGeneratorState;
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

    public void TurnOn()
    {
        isOn = true;
        OnGeneratorStateChanged?.Invoke(isOn);
    }

    public void TurnOff()
    {
        isOn = false;
        OnGeneratorStateChanged?.Invoke(isOn);
    }

    public void FlipState()
    {
        if (isOn)
        {
            TurnOff();
            isOn = false;

        }
        else
        {
            if (!isBroken)
            {
                TurnOn();
                isOn = true;
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
            isOn = false;
        }
    }
}
