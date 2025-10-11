using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : StatsBase
{
    [Header("Enemy Base Stats")]
    [SerializeField] private List<StatEntry> enemyBaseStats;

    protected override void Awake()
    {
        baseStatsList = enemyBaseStats;
        base.Awake();
    }
}