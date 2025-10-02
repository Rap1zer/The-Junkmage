using UnityEngine;
using System.Collections.Generic;

public class EnemyStats : MonoBehaviour
{
    // Base values (serialized so you can set in Inspector)
    [Header("Base Stats")]
    [SerializeField] private float baseMaxHealth = 10f;
    [SerializeField] private float baseMoveSpeed = 2f;
    [SerializeField] private float AttackDmg = 1f;
    [SerializeField] private float AttackCooldown = 1f;

    // Active modifiers for each stat
    private Dictionary<StatType, List<StatModifier>> modifiers
        = new Dictionary<StatType, List<StatModifier>>();


    public void ApplyModifier(StatModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.Stat))
            modifiers[modifier.Stat] = new List<StatModifier>();

        modifiers[modifier.Stat].Add(modifier);

        // If it's health, clamp current
        // if (modifier.Stat == StatType.MaxHealth)
        // {
        //     CurrentHealth = Mathf.Clamp(CurrentHealth, 0, (int)GetVal(StatType.MaxHealth));
        // }
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
            StatType.AttackDmg => AttackDmg,
            StatType.AttackCooldown => AttackCooldown,
            _ => 0f
        };
    }
}
