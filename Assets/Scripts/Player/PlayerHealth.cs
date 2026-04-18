using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField]private int maxHealth = 100;
    private int health;

    public bool alive = true;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();

            health = maxHealth;
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        StartCoroutine(SmoothHealthChange());
        if (health <= 0) {
            TellAllDiedRPC(false);
            DeathHandler.OnPlayerDied(gameObject);
        }

    }

    IEnumerator SmoothHealthChange()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        float startValue = healthBar.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthBar.value = Mathf.Lerp(startValue, health, elapsed / duration);
            yield return null;
        }

        healthBar.value = health;
    }

    public void HealTarget()
    {
        Debug.Log("Running Heal Target");
        RequestDamageRPC(-50);

        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        InventoryHandler inventoryHandler = playerObject.GetComponentInParent<InventoryHandler>();
        inventoryHandler.DropItem(gameObject, true);
    }

    public void Respawn()
    {
        TellAllDiedRPC(true);

        health = Mathf.RoundToInt(maxHealth / 4f);
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    [Rpc(SendTo.Everyone)]
    private void TellAllDiedRPC(bool state)
    {
        alive = state;
    }


    [Rpc(SendTo.Server)]
    public void RequestDamageRPC(int damage)
    {
        Debug.Log("Damage Requested");

        TakeDamageRPC(damage, new RpcParams
        {
            Send = new RpcSendParams
            {
                Target = RpcTarget.Single(OwnerClientId, RpcTargetUse.Temp)
            }
        });
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void TakeDamageRPC(int damage, RpcParams rpcParams = default)
    {
        TakeDamage(damage);
    }

}