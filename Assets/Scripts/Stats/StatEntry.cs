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
    StrafeSpeed,
    StrafeDuration,
    StrafeRadius,
    AttackRange,
    RetreatRange,
    DetectionRange,
    AttackEndMultiplier,
    RetreatEndMultiplier
}

// public enum CharacterStatType
// {
//     MaxHealth = 0,
//     MoveSpeed = 1,
//     Acceleration = 2,
//     DashSpeed = 3,
//     DashDuration = 4,
//     DashCooldown = 5,
//     BulletSpeed = 6,
//     CritChance = 7,
//     CritMultiplier = 8,
//     AttackDmg = 9,
//     AttackCooldown = 10,
//     Defence = 11,
// }
//
// public enum EnemyAIStatType
// {
//     StrafeSpeed = 0,
//     StrafeDuration = 1,
//     StrafeRadius = 2,
//     AttackRange = 3,
//     RetreatRange = 4,
//     DetectionRange = 5,
//     AttackEndMultiplier = 6,
//     RetreatEndMultiplier = 7
// }