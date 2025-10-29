using UnityEngine;

public class Banana : ItemBase
{
    private StatModifier maxHealthModifier = new(Stat.MaxHealth, 2f, ModifierType.Flat);
    
    protected override void OnEquip()
    {
        playerStats.ApplyModifier(maxHealthModifier);
    }

    protected override void OnUnequip()
    {
        playerStats.RemoveModifier(maxHealthModifier);
    }
}
