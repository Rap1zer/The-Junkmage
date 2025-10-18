using UnityEngine;
using System.Collections.Generic;

public class GunnerStats : EnemyStats
{
    [Header("Gunner Specific Stats")]
    [SerializeField] private List<StatEntry> gunnerExtraStats;
}