using System;
using UnityEngine;

public class WoodenShield : ItemBase, IIncomingModifier
{
    private int blockAmount = 1;
    private float blockChance = 0.4f;

    public override float OnIncomingDamage(float dmg, GameObject attacker)
    {
        int newDmg = (int)(UnityEngine.Random.value < blockChance
            ? Mathf.Max(0,  dmg - blockAmount) : dmg);
        return newDmg;
    }
}
