using System;
using UnityEngine;

public class Scope : MonoBehaviour, IOnHit, IOnMiss, IItem
{
    public ItemData ItemData { get; private set; }
    private PlayerController Player { get; set; }
    int critChanceStack = 0;
    int maxCritChanceStack = 30;

    public void Initialise (ItemData itemData, PlayerController player)
    {
        ItemData = itemData;
        Player = player;
    }

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