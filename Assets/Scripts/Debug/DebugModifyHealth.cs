using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class DebugModifyHealth : NetworkBehaviour
{
    [SerializeField] TMP_InputField healthInputField;
    [SerializeField] PlayerHealth playerHealth;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            healthInputField = GameObject.Find("ModifyHealthInput").GetComponent<TMP_InputField>();

            Button modifyHealthButton = GameObject.Find("ModifyHealthButton").GetComponent<Button>();
            modifyHealthButton.onClick.AddListener(ModifyHealth);

        }
    }

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
