using UnityEngine;

public class Scope : ItemBase, IOnHit, IOnMiss
{
    int critChanceStack = 0;
    int maxCritChanceStack = 30;

    public override void OnHit()
    {
        critChanceStack += 3;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        int increase = Mathf.Min(3, maxCritChanceStack - critChanceStack);
        Player.critChance += increase;
    }

    public override void OnMiss()
    {
        Player.critChance = Mathf.Max(0, Player.critChance - critChanceStack);
        critChanceStack = 0;
    }
}
