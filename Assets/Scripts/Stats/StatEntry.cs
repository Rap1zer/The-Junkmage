using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StatEntry
{
    public StatType type;
    public float baseValue;

    public StatEntry(StatType type, float baseValue)
    {
        this.type = type;
        this.baseValue = baseValue;
    }
}

public enum StatType
{
    MaxHealth,
    MoveSpeed,
    Acceleration,
    DashSpeed,
    DashDuration,
    DashCooldown,
    BulletSpeed,
    CritChance,
    CritMultiplier,
    AttackDmg,
    AttackCooldown,
    Defence,
}