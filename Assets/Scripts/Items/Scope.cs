using UnityEngine;

public class Scope : ItemBase
{
    private IPlayerItemConsumer playerConsumer;
    int critChanceStack = 0;
    int maxCritChanceStack = 30;

    protected override void Awake()
    {
        base.Awake();
        playerConsumer = player.GetComponent<IPlayerItemConsumer>();
    }

    public void OnHit()
    {
        critChanceStack += 3;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        int increase = Mathf.Min(3, maxCritChanceStack - critChanceStack);
        playerConsumer.ApplyStatModifier(new StatModifier(StatType.CritChance, increase, ModifierType.Flat));
    }

    public void OnMiss()
    {
        playerConsumer.ApplyStatModifier(new StatModifier(StatType.CritChance, -critChanceStack, ModifierType.Flat));
        critChanceStack = 0;
    }
}
