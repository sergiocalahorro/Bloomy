using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains all health management logic
/// </summary>
public class HealthComponent : MonoBehaviour
{
    private int health;
    [SerializeField, Range(1, 100)]
    private int maxHealth;
    private bool isDead;
    [SerializeField]
    private bool isInvincible;
    [SerializeField]
    private bool shouldDestroyOnZeroHealth = true;

    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        isDead = false;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    /// <summary>
    /// Heal a given amount of health
    /// <param name="healingAmount">Amount of health to add to the current health</param>
    /// </summary>
    public void Heal(int healingAmount)
    {
        if (isDead)
        {
            return;
        }

        health = Mathf.Clamp(health + healingAmount, 0, maxHealth);
    }

    /// <summary>
    /// Take a given amount of damage
    /// <param name="damageAmount">Amount of health to substract from current health</param>
    /// </summary>
    public void TakeDamage(int damageAmount)
    {
        if (isInvincible)
        {
            return;
        }

        health = Mathf.Clamp(health - damageAmount, 0, maxHealth);

        if (health == 0)
        {
            isDead = true;

            if (shouldDestroyOnZeroHealth)
            {
                IDestroyable destroyable = GetComponent<IDestroyable>();
                if (destroyable != null)
                {
                    destroyable.HandleDestruction();
                }
            }
        }
    }
}