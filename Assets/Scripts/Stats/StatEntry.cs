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
    MaxHealth = 0,
    MoveSpeed = 10,
    Acceleration = 20,
    DashSpeed = 30,
    DashDuration = 40,
    DashCooldown = 50,
    BulletSpeed = 60,
    BulletSize = 70,
    CritChance = 80,
    CritMultiplier = 90,
    AttackDmg = 100,
    AttackCooldown = 110,
    Defence = 120,
    StrafeSpeed = 130,
    StrafeDuration = 140,
    AttackRange = 150,
    RetreatRange = 160,
    DetectionRange = 170,
    AttackEndMultiplier = 180,
    RetreatEndMultiplier = 190
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