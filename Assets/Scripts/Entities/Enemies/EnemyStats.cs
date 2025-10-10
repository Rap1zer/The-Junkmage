using UnityEngine;

public class EnemyStats : StatsBase
{
    [Header("Universal Base Stats")]
    [SerializeField] protected float baseMaxHealth = 10f;
    [SerializeField] protected float baseMoveSpeed = 2f;
    [SerializeField] protected float baseAcceleration = 10f;
    [SerializeField] protected float baseAttackDmg = 1f;
    [SerializeField] protected float baseAttackCooldown = 1f;

    protected override float GetBaseStat(StatType stat)
    {
        return stat switch
        {
            StatType.MaxHealth => baseMaxHealth,
            StatType.MoveSpeed => baseMoveSpeed,
            StatType.Acceleration => baseAcceleration,
            StatType.AttackDmg => baseAttackDmg,
            StatType.AttackCooldown => baseAttackCooldown,
            _ => -1f
        };
    }
}
