using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    private PlayerStats stats;

    public float CurrentHealth { get; private set; }

    public event System.Action OnDeath;
    public event System.Action<float> OnHealthChanged;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        CurrentHealth = (int)stats.GetVal(StatType.MaxHealth);
    }

    public void TakeDamage(float dmg, GameObject attacker = null)
    {
        EntityEventDispatcher dispatcher = attacker?.GetComponent<EntityEventDispatcher>();

        if (dispatcher != null)
        {
            dmg = dispatcher.DispatchIncomingDamage(dmg, attacker);
        }

        CurrentHealth -= dmg;

        if (dispatcher != null)
            dispatcher.DispatchAfterDamageTaken(dmg, attacker);

        if (CurrentHealth <= 0)
        {
            Die();
            OnDeath?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, (int)stats.GetVal(StatType.MaxHealth));
        OnHealthChanged?.Invoke(CurrentHealth);
    }

    private void Die()
    {
        Debug.Log("Player died");
        // TODO: trigger events
    }
}
