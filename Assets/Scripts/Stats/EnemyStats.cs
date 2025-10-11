using UnityEngine;

public class EnemyStats : StatsBase
{
    [Header("Enemy Base Stats")]
    [Tooltip("Assign a StatSheet asset containing the enemy's base stats.")]
    [SerializeField] private StatSheet enemyStatSheet;

    // legacy serialized list for scenes that haven't migrated yet (kept hidden in inspector)
    [SerializeField, HideInInspector] private System.Collections.Generic.List<StatEntry> enemyBaseStats;

    protected override void Awake()
    {
        if (enemyStatSheet != null)
            baseStatsSheet = enemyStatSheet;
        else
            baseStatsList = enemyBaseStats; // legacy fallback

        base.Awake();
    }
}