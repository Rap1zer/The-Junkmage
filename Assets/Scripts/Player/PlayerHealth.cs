using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    private PlayerStats stats;
    public int CurrentHealth { get; private set; }

    public event System.Action OnDeath;
    public event System.Action<int> OnHealthChanged;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        CurrentHealth = (int)stats.GetVal(StatType.MaxHealth);
    }

    public void TakeDamage(int dmg)
    {
        ModifyHealth(-dmg);
    }

    public void Heal(int amount)
    {
        ModifyHealth(amount);
    }

    private void ModifyHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, (int)stats.GetVal(StatType.MaxHealth));
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
    }
}
