using System.Collections.Generic;
using UnityEngine;

public class Scope : ItemBase, IDealDamageHandler, IMissedAttackHandler
{
    float critChanceStack = 0f;
    float maxCritChanceStack = 50f;
    private static float stack = 10f;
    
    private PlayerStats stats;
    private List<StatModifier> modifiers = new();

    protected override void Awake()
    {
        base.Awake();
        stats = player.GetComponent<PlayerStats>();
    }

    public void OnDealDamage(float dmg, GameObject target = null)
    {
        critChanceStack += stack;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        float increase = Mathf.Min(stack, maxCritChanceStack - critChanceStack);
        
        StatModifier newModifier = new StatModifier(StatType.CritChance, increase, ModifierType.Flat);
        modifiers.Add(newModifier);
        
        stats.ApplyModifier(newModifier);
        
        Debug.Log($"Increased crit chance by {increase}");
    }

    public void OnMissedAttack()
    {
        foreach (var modifier in modifiers)
        {
            stats.RemoveModifier(modifier);
        }
        modifiers.Clear();
        
        critChanceStack = 0;
        Debug.Log("Crit chance stack reset");
    }
}
