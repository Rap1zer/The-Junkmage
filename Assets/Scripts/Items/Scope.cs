using System.Collections.Generic;
using UnityEngine;

public class Scope : ItemBase, IDealDamageHandler, IMissedAttackHandler
{
    float critChanceStack = 0f;
    float maxCritChanceStack = 0.4f;
    private static float stack = 0.1f;
    
    private List<StatModifier> modifiers = new();

    public void OnDealDamage(float dmg, GameObject target = null)
    {
        critChanceStack += stack;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        float increase = Mathf.Min(stack, maxCritChanceStack - critChanceStack);
        
        StatModifier newModifier = new StatModifier(Stat.CritChance, increase, ModifierType.Flat);
        modifiers.Add(newModifier);
        
        playerStats.ApplyModifier(newModifier);
        
        Debug.Log($"Increased crit chance by {increase}");
    }

    public void OnMissedAttack()
    {
        foreach (var modifier in modifiers)
        {
            playerStats.RemoveModifier(modifier);
        }
        modifiers.Clear();
        
        critChanceStack = 0;
        Debug.Log("Crit chance stack reset");
    }
}
