using UnityEngine;

public class PlayerStats : StatsBase
{
    [Header("Base Stats")]
    [SerializeField] private float baseMaxHealth = 20f;
    [SerializeField] private float baseMoveSpeed = 5f;
    [SerializeField] private float baseDashSpeed = 12f;
    [SerializeField] private float baseDashDuration = 0.2f;
    [SerializeField] private float baseDashCooldown = 1f;
    [SerializeField] private float baseBulletDmg = 1f;
    [SerializeField] private float baseCritChance = 0.1f;
    [SerializeField] private float baseCritMultiplier = 2f;
    [SerializeField] private float baseBulletSpeed = 14f;
    [SerializeField] private int baseDefence = 0;

    protected override float GetBaseStat(StatType stat)
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
            StatType.Defence => baseDefence,
            _ => 0f
        };
    }
}
