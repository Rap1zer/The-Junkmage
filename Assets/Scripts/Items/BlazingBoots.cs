using UnityEngine;

public class BlazingBoots : ItemBase
{
    private StatModifier speedModifier = new(Stat.MoveSpeed, 2f, ModifierType.Flat);
    
    protected override void OnEquip()
    {
        playerStats.ApplyModifier(speedModifier);
    }

    protected override void OnUnequip()
    {
        playerStats.RemoveModifier(speedModifier);
    }
}
