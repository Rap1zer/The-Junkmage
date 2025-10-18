using JunkMage.Stats;
using UnityEngine;

public class EnemyStats : StatsBase
{
    [Header("Enemy Base Stats")]
    [Tooltip("Assign a StatSheet asset containing the enemy's base stats.")]
    [SerializeField] private StatSheet enemyStatSheet;

    void Awake()
    {
        if (enemyStatSheet != null)
            baseStatsSheet = enemyStatSheet;
    }
}