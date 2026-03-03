using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DebugModifyHealth : MonoBehaviour
{
    [SerializeField] TMP_InputField healthInputField;
    [SerializeField] PlayerHealth playerHealth;

    public void ModifyHealth()
    {
        int health = 0;
        try
        {
            health = int.Parse(healthInputField.text);
        }
        catch { 
            Debug.LogError("Invalid input for health. Please enter a valid integer.");
                return;
        }

        playerHealth.TakeDamage(health);
    }
}
