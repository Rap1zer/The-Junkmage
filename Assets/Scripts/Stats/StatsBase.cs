using UnityEngine;
using System.Collections.Generic;

public enum StatType
{
    MaxHealth,
    MoveSpeed,
    Acceleration,
    DashSpeed,
    DashDuration,
    DashCooldown,
    BulletDmg,
    CritChance,
    CritMultiplier,
    BulletSpeed,
    AttackDmg,
    AttackCooldown,
    Defence,
}

[System.Serializable]
public class StatEntry
{
    public StatType type;
    public float baseValue;
}

public abstract class StatsBase : MonoBehaviour
{
    [SerializeField] protected List<StatEntry> baseStatsList = new();
    protected Dictionary<StatType, float> baseStatsDict;

    protected Dictionary<StatType, List<StatModifier>> modifiers = new();

    protected virtual void Awake()
    {
        baseStatsDict = new Dictionary<StatType, float>(baseStatsList.Count);
        foreach (var entry in baseStatsList)
            baseStatsDict[entry.type] = entry.baseValue;
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

    protected virtual float GetBaseStat(StatType stat)
        => baseStatsDict != null && baseStatsDict.TryGetValue(stat, out float val) ? val : -1f;
}
