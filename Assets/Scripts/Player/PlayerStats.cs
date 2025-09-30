using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    MaxHealth,
    MoveSpeed,
    DashSpeed,
    DashDuration,
    DashCooldown,
    BulletDmg,
    CritChance,
    CritMultiplier,
    BulletSpeed
}

public class PlayerStats : MonoBehaviour, IDamageable
{
    // Base values (serialized so you can set in Inspector)
    [Header("Base Stats")]
    [SerializeField] private int baseMaxHealth = 20;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseDashSpeed = 12f;
    [SerializeField] private float baseDashDuration = 0.2f;
    [SerializeField] private float baseDashCooldown = 1f;
    [SerializeField] private int baseBulletDmg = 1;
    [SerializeField] private float baseCritChance = 0.1f;
    [SerializeField] private int baseCritMultiplier = 2;
    [SerializeField] private float baseBulletSpeed = 14f;

    // Active modifiers for each stat
    private Dictionary<StatType, List<StatModifier>> modifiers 
        = new Dictionary<StatType, List<StatModifier>>();

    // Current health
    public int CurrentHealth { get; private set; }

    void Awake()
    {
        CurrentHealth = (int)GetVal(StatType.MaxHealth);
    }

    public void ApplyModifier(StatModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.Stat))
            modifiers[modifier.Stat] = new List<StatModifier>();

        modifiers[modifier.Stat].Add(modifier);

        // If it's health, clamp current
        if (modifier.Stat == StatType.MaxHealth)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, (int)GetVal(StatType.MaxHealth));
        }
    }

    public void TakeDamage(int dmg)
    {
        CurrentHealth -= dmg;
        if (CurrentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, (int)GetVal(StatType.MaxHealth));
    }

    private void Die()
    {
        Debug.Log("Player died");
        // TODO: trigger events
    }

    // Compute final stat value from base + modifiers
    public float GetVal(StatType stat)
    {
        float baseValue = GetBaseStat(stat);
        float flat = 0f;
        float percentAdd = 0f;
        float percentMul = 1f;

        if (modifiers.TryGetValue(stat, out var statModifiers))
        {
            foreach (var m in statModifiers)
            {
                switch (m.Type)
                {
                    case ModifierType.Flat:
                        flat += m.Value;
                        break;
                    case ModifierType.PercentAdd:
                        percentAdd += m.Value;
                        break;
                    case ModifierType.PercentMul:
                        percentMul *= (1 + m.Value);
                        break;
                }
            }
        }

        return (baseValue + flat) * (1 + percentAdd) * percentMul;
    }

    private float GetBaseStat(StatType stat)
    {
        return stat switch
        {
            StatType.MaxHealth => baseMaxHealth,
            StatType.MoveSpeed => baseMoveSpeed,
            StatType.DashSpeed => baseDashSpeed,
            StatType.DashDuration => baseDashDuration,
            StatType.DashCooldown => baseDashCooldown,
            StatType.BulletDmg => baseBulletDmg,
            StatType.CritChance => baseCritChance,
            StatType.CritMultiplier => baseCritMultiplier,
            StatType.BulletSpeed => baseBulletSpeed,
            _ => 0f
        };
    }
}
