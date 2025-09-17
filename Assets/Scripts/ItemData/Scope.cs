using System;
using Unity.VisualScripting;
using UnityEngine;

public class Scope : IOnHit, IItemBehaviour, IOnMiss
{
    public ItemData ScopeData { get; private set; }
    private PlayerController Player { get; set; }
    public GameObject ScopeObj { get; set; }
    int critChanceStack = 0;
    int maxCritChanceStack = 30;

    public Scope(ItemData scopeData, PlayerController player, GameObject scopeObj)
    {
        ScopeData = scopeData;
        Player = player;
        ScopeObj = scopeObj;
    }

    public virtual void OnEquip()
    {

    }

    public virtual void OnUnequip() { }

    public void OnHit()
    {
        critChanceStack += 3;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        int increase = Mathf.Min(3, maxCritChanceStack - critChanceStack);
        Player.critChance += increase;
    }

    public void OnMiss() {
        Player.critChance = Math.Max(0, Player.critChance - critChanceStack);
        critChanceStack = 0;
    }
}
