using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField]private int maxHealth = 100;
    private int health;

    private void Awake()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();

        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);
        StartCoroutine(SmoothHealthChange());

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

}