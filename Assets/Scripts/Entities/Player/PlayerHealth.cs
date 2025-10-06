using System;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    private PlayerStats stats;
    private PlayerMovement movement;
    private EntityEventDispatcher dispatcher;

    private float currentHealth;
    private float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            if (CurrentHealth <= 0)
            {
                Die();
                OnDeath?.Invoke();
            }
        }
        
    }

    public event System.Action OnDeath;
    public event System.Action<float> OnHealthChanged;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        CurrentHealth = (int)stats.GetVal(StatType.MaxHealth);
        dispatcher = GetComponent<EntityEventDispatcher>();
        movement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(float dmg, GameObject attacker = null)
    {
        if (movement.IsDashing) return;
        
        if (dispatcher != null)
        {
            dmg = Math.Max(dmg - stats.GetVal(StatType.Defence), 0);
            dmg = dispatcher.DispatchIncomingDamage(dmg, attacker);
        }

        CurrentHealth -= dmg;

        if (dispatcher != null)
            dispatcher.DispatchAfterDamageTaken(dmg, attacker);
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, (int)stats.GetVal(StatType.MaxHealth));
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Die()
    {
        //Debug.Log("Player died");
        // TODO: trigger events
    }
}
