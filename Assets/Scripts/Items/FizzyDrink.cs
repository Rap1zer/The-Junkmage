using System.Collections.Generic;
using UnityEngine;

public class FizzyDrink : ItemBase
{
    private StatModifier modifier;

    protected override void Awake()
    {
        base.Awake();
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        modifier = new StatModifier(StatType.AttackCooldown, -0.25f, ModifierType.PercentMul);
    }

    protected override void OnEquip()
    {
        base.OnEquip();
        playerStats.ApplyModifier(modifier);
    }

    protected override void OnUnequip()
    {
        base.OnUnequip();
        playerStats.RemoveModifier(modifier);
    }
}
