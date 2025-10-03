using UnityEngine;

public class EnemyStats : StatsBase
{
    [Header("Base Stats")]
    [SerializeField] private float baseMaxHealth = 10f;
    [SerializeField] private float baseMoveSpeed = 2f;
    [SerializeField] private float baseAttackDmg = 1f;
    [SerializeField] private float baseAttackCooldown = 1f;

    protected override float GetBaseStat(StatType stat)
    {
        return stat switch
        {
            StatType.MaxHealth => baseMaxHealth,
            StatType.MoveSpeed => baseMoveSpeed,
            StatType.AttackDmg => baseAttackDmg,
            StatType.AttackCooldown => baseAttackCooldown,
            _ => 0f
        };
    }
}
