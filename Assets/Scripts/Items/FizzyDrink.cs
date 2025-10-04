using System.Collections.Generic;
using UnityEngine;

public class FizzyDrink : ItemBase
{
    private PlayerStats stats;

    private StatModifier modifier;

    protected override void Awake()
    {
        base.Awake();
        stats = GameObject.Find("Player").GetComponent<PlayerStats>();
        modifier = new StatModifier(StatType.AttackCooldown, -0.25f, ModifierType.PercentMul);
    }

    protected override void OnEquip()
    {
        base.OnEquip();
        stats.ApplyModifier(modifier);
    }

    protected override void OnUnequip()
    {
        base.OnUnequip();
        stats.RemoveModifier(modifier);
    }
}
