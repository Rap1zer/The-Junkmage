using UnityEngine;

public class Frost : StatusEffect
{
    // Frost reduces move speed, dash speed, bullet speed, and increases attack cooldown.
    private readonly StatModifier[] modifiers = 
    {
        new(StatType.MoveSpeed, -0.1f, ModifierType.PercentMul),
        new(StatType.DashSpeed, -0.1f, ModifierType.PercentMul),
        new(StatType.AttackCooldown, 0.1f, ModifierType.PercentAdd),
        new(StatType.BulletSpeed,  -0.05f, ModifierType.PercentMul),
    };
    
    public Frost(float duration) : base(duration) { }

    public override void OnApply()
    {
        base.OnApply();
        
        if (stats)
        {
            foreach (StatModifier modifier in modifiers)
            {
                stats.ApplyModifier(modifier);
            }
        }
    }

    public override void OnRemove()
    {
        base.OnRemove();
        
        if (stats)
        {
            foreach (StatModifier modifier in modifiers)
            {
                stats.RemoveModifier(modifier);
            }   
        }
    }
}