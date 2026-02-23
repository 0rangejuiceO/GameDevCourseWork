using System;
using UnityEngine;

public class MiniGameHandler : MonoBehaviour
{
    [Header("MiniGame Settings")]
    [SerializeField] private GameObjectFloatDictionary miniGames = new GameObjectFloatDictionary();
    [SerializeField] private GameObject defaultMiniGame;

    [Header("Global Refs")]
    [SerializeField] private FPController fpController;

    public void Interact()
    {
        fpController.LockMovement = true;
    }

    public void LeaveInteract()
    {
        fpController.LockMovement = false;
    }

    private GameObject SelectMiniGame()
    {
        float totalWeight = 0f;
        foreach (float weight in miniGames.Values)
        {
            totalWeight += weight;
        }

        float rand = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in floorNumProbability)
        {
            cumulative += entry.Value;

            if (rand <= cumulative)
            {
                Debug.Log($"Selected mini game {entry.Key.name}");
                return entry.Key;
            }
        }

        return defaultMiniGame;

    }


}
