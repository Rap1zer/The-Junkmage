using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StatEntry
{
    public Stat type;
    public float baseValue;

    public StatEntry(Stat type, float baseValue)
    {
        this.type = type;
        this.baseValue = baseValue;
    }
}

public enum Stat
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