using UnityEngine;

public class BlazingBoots : ItemBase
{
    private StatModifier speedModifier = new(StatType.MoveSpeed, 3f, ModifierType.Flat);
    
    protected override void OnEquip()
    {
        playerStats.ApplyModifier(speedModifier);
    }

    protected override void OnUnequip()
    {
        playerStats.RemoveModifier(speedModifier);
    }
}
