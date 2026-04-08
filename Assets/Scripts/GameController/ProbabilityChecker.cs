using UnityEngine;

public class ProbabilityChecker : MonoBehaviour
{
    [SerializeField]
    private IntFloatDictionary floorNumProbability = new IntFloatDictionary()
    {
        {1, 0.07f},
        {2, 0.18f},
        {3, 0.60f},
        {4, 0.12f},
        {5,0.03f }
    };

    public int totalTrials = 1000;
    public int[] results = new int[]
    {
        0, // Count for 1 floor
        0, // Count for 2 floors
        0, // Count for 3 floors
        0, // Count for 4 floors
        0  // Count for 5 floors
    }; // To store counts for 1 to 5 floors

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < results.Length; i++)
        {
            results[i] = 0; // Initialize counts to zero
        }

        for (int i = 0; i < totalTrials; i++)
        {
            int result = SetNumOfFloors();
            Debug.Log(result);
            results[result]++;
        }
    }

    private int SetNumOfFloors()
    {
        float totalWeight = 0f;
        foreach (float weight in floorNumProbability.Values)
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
                Debug.Log($"Selected number of floors: {entry.Key}");
                return (entry.Key -1);
            }
        }

        return 4;
    }
}
