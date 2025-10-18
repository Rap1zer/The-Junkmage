using UnityEngine;
using System.Collections.Generic;
using JunkMage.Stats;

public abstract class StatsBase : MonoBehaviour
{
    [Header("Use a StatSheet ScriptableObject")]
    [Tooltip("Assign a StatSheet ScriptableObject to use shared base stats.")]
    protected StatSheet baseStatsSheet;

    // Modifiers per stat (unchanged)
    protected Dictionary<Stat, List<StatModifier>> modifiers = new();

    public virtual void ApplyModifier(StatModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.Stat))
            modifiers[modifier.Stat] = new List<StatModifier>();

        modifiers[modifier.Stat].Add(modifier);
    }

    public void RemoveModifier(StatModifier modifier)
    {
        if (modifiers.TryGetValue(modifier.Stat, out var list))
        {
            list.Remove(modifier);
            if (list.Count == 0)
                modifiers.Remove(modifier.Stat);
        }
    }

    public float GetVal(Stat stat)
    {
        float baseValue = GetBaseStat(stat);

        float flat = 0f, percentAdd = 0f, percentMul = 1f;
        if (modifiers.TryGetValue(stat, out var list))
        {
            foreach (var m in list)
            {
                switch (m.Type)
                {
                    case ModifierType.Flat: flat += m.Amount; break;
                    case ModifierType.PercentAdd: percentAdd += m.Amount; break;
                    case ModifierType.PercentMul: percentMul *= (1 + m.Amount); break;
                }
            }
        }

        return (baseValue + flat) * (1 + percentAdd) * percentMul;
    }

    /// <summary>
    /// Gets the base stat value. Prefers the assigned StatSheet.
    /// Falls back to the StatDefinition defaultValue.
    /// If nothing found, returns 0f.
    /// </summary>
    private float GetBaseStat(Stat stat)
    {
        if (baseStatsSheet != null)
        {
            float val = baseStatsSheet.GetBaseValue(stat);
            if (!Mathf.Approximately(val, -1f))
                return val;
        }

        // Fallback: default value from StatDefinition
        var def = StatDefinitionDatabase.Instance?.GetDefinition(stat);
        if (def != null)
            return def.defaultValue;

        // Last fallback
        Debug.LogWarning($"[StatsBase] No base value found for stat {stat}. Returning 0f.");
        return 0f;
    }
}
