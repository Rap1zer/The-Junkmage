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
    BulletSpeed,
    AttackDmg,
    AttackCooldown,
    Defence
}

public abstract class StatsBase : MonoBehaviour
{
    // Active modifiers for each stat
    protected Dictionary<StatType, List<StatModifier>> modifiers
        = new Dictionary<StatType, List<StatModifier>>();

    public void ApplyModifier(StatModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.Stat))
            modifiers[modifier.Stat] = new List<StatModifier>();

        modifiers[modifier.Stat].Add(modifier);

        // Example: Clamp health if needed (optional to override in subclasses)
        // if (modifier.Stat == StatType.MaxHealth)
        // {
        //     CurrentHealth = Mathf.Clamp(CurrentHealth, 0, (int)GetVal(StatType.MaxHealth));
        // }
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.TryGetValue(modifier.Stat, out List<StatModifier> list))
        {
            list.Remove(modifier);

            if (list.Count == 0)
            {
                modifiers.Remove(modifier.Stat);
            }
        }
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
                        flat += m.Amount;
                        break;
                    case ModifierType.PercentAdd:
                        percentAdd += m.Amount;
                        break;
                    case ModifierType.PercentMul:
                        percentMul *= (1 + m.Amount);
                        break;
                }
            }
        }

        return (baseValue + flat) * (1 + percentAdd) * percentMul;
    }

    // Subclasses must provide their base stat values
    protected abstract float GetBaseStat(StatType stat);
}
