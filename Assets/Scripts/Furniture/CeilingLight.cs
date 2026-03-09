using UnityEngine;

public class CeilingLight : MonoBehaviour
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

    void HandleGeneratorState(bool state)
    {
        lightSource.enabled = state;
    }
}