using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : StatsBase
{
    [Header("Player Base Stats")]
    [SerializeField] private List<StatEntry> playerBaseStats;

    protected override void Awake()
    {
        baseStatsList = playerBaseStats;
        base.Awake();
    }
}