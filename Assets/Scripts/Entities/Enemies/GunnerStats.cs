using UnityEngine;

public class GunnerStats : EnemyStats
{
    [Header("Specific Stats")]
    [SerializeField] private float baseBulletSpeed = 10f;

    protected override float GetBaseStat(StatType stat)
    {
        return stat switch
        {
            StatType.BulletSpeed => baseBulletSpeed,
            _ => base.GetBaseStat(stat)
        };
    }
}