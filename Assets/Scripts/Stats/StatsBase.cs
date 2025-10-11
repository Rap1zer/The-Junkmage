using UnityEngine;
using System.Collections.Generic;

public abstract class StatsBase : MonoBehaviour
{
    [Header("Prefer using a StatSheet ScriptableObject")]
    [Tooltip("Assign a StatSheet ScriptableObject to use shared base stats. If none is assigned, the legacy baseStatsList will be used.")]
    protected StatSheet baseStatsSheet;

    // Legacy support for scenes that still use lists (you can remove later)
    [SerializeField, HideInInspector] protected List<StatEntry> baseStatsList = new();

    // Modifiers per stat (unchanged)
    protected Dictionary<StatType, List<StatModifier>> modifiers = new();

    protected virtual void Awake()
    {
        // Nothing needed on Awake for ScriptableObject approach.
        // We keep the legacy list around for backward compatibility.
    }

    public void ApplyModifier(StatModifier modifier)
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

    public float GetVal(StatType stat)
    {
        float baseValue = GetBaseStat(stat);
        if (Mathf.Approximately(baseValue, -1f))
            return 0f;

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
    /// Gets the base stat value. Prefers the assigned StatSheet. Falls back to the legacy list.
    /// Returns -1f if stat not found.
    /// </summary>
    protected virtual float GetBaseStat(StatType stat)
    {
        if (baseStatsSheet != null)
            return baseStatsSheet.GetBaseValue(stat);

        if (baseStatsList != null)
        {
            foreach (var e in baseStatsList)
                if (e.type == stat)
                    return e.baseValue;
        }

        return -1f;
    }
}
