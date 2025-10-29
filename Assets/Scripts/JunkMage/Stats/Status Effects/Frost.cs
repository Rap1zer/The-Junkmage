using UnityEngine;

public class Frost : StatusEffect
{
    // Frost reduces move speed, dash speed, bullet speed, and increases attack cooldown.
    private readonly StatModifier[] modifiers = 
    {
        new(Stat.MoveSpeed, -0.1f, ModifierType.PercentMul),
        new(Stat.DashSpeed, -0.1f, ModifierType.PercentMul),
        new(Stat.AttackCooldown, 0.1f, ModifierType.PercentAdd),
        new(Stat.BulletSpeed,  -0.1f, ModifierType.PercentMul),
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