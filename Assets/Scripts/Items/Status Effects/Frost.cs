using UnityEngine;

public class Frost : StatusEffect
{
    private StatModifier[] modifiers =
    {
        new StatModifier(StatType.MoveSpeed, -0.03f, ModifierType.PercentMul),
        new StatModifier(StatType.DashSpeed, -0.03f, ModifierType.PercentMul),
        new StatModifier(StatType.AttackCooldown, 0.05f, ModifierType.PercentMul)
    };
    public Frost(float duration) : base(duration) { }

    public override void OnApply()
    {
        base.OnApply();
        StatsBase stats = owner.GetComponent<StatsBase>();
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
        StatsBase stats = owner.GetComponent<StatsBase>();
        if (stats)
        {
            foreach (StatModifier modifier in modifiers)
            {
                stats.RemoveModifier(modifier);
            }   
        }
    }
}