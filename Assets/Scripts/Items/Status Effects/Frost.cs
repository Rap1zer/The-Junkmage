using UnityEngine;

public class Frost : StatusEffect
{
    public Frost(float duration) : base(duration)
    {
        Duration = duration;
    }

    public override void OnApply()
    {
        base.OnApply();
        Debug.Log("Frost applied");
    }
    
}