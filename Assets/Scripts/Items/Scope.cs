using UnityEngine;

public class Scope : ItemBase, IOnHit, IOnMiss
{
    int critChanceStack = 0;
    int maxCritChanceStack = 30;

    IPlayerItemConsumer target;

    public override void Initialise(ItemData itemData)
    {
        base.Initialise(itemData);
        target = player.GetComponent<IPlayerItemConsumer>();
    }

    public override void OnHit()
    {
        critChanceStack += 3;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        int increase = Mathf.Min(3, maxCritChanceStack - critChanceStack);
        target.ApplyStatModifier(new StatModifier(StatType.CritChance, increase, ModifierType.Flat));
    }

    public override void OnMiss()
    {
        target.ApplyStatModifier(new StatModifier(StatType.CritChance, -critChanceStack, ModifierType.Flat));
        critChanceStack = 0;
    }
}
