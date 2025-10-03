using System.Collections.Generic;
using UnityEngine;

public class Scope : ItemBase, IDealDamageHandler, IMissedAttackHandler
{
    int critChanceStack = 0;
    int maxCritChanceStack = 30;
    
    private PlayerStats stats;
    private List<StatModifier> modifiers = new();

    protected override void Awake()
    {
        base.Awake();
        stats = player.GetComponent<PlayerStats>();
    }

    public void OnDealDamage(float dmg, GameObject target = null)
    {
        critChanceStack += 3;
        critChanceStack = Mathf.Min(critChanceStack, maxCritChanceStack);

        int increase = Mathf.Min(3, maxCritChanceStack - critChanceStack);
        
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
