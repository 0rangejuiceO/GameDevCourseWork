using UnityEngine;

public class Lamp : MonoBehaviour
{
    public Light lightSource;

    void OnEnable()
    {
        Generator.OnGeneratorStateChanged += HandleGeneratorState;
    }

    void OnDisable()
    {
        Generator.OnGeneratorStateChanged -= HandleGeneratorState;
    }

    void HandleGeneratorState(bool state, bool fromNetwork)
    {
        lightSource.enabled = state;
    }

    public void ToggleLamp()
    {
        lightSource.enabled = !lightSource.enabled;
    }

}
