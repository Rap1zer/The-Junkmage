using UnityEngine;

public class Snowflake : ItemBase, IDealDamageHandler
{
    public void OnDealDamage(float dmg, GameObject target = null)
    {
        if (target == null) return;

        // Get the target's dispatcher
        var dispatcher = target.GetComponent<EntityEventDispatcher>();
        if (dispatcher == null) return;

        // Add a Frost effect (assuming you have a FrostStatusEffect)
        var frost = new Frost(3f);
        dispatcher.AddEffect(frost);
    }
}