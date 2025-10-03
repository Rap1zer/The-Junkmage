using UnityEngine;

public class Snowflake : ItemBase, IDealDamageHandler
{
    public void OnDealDamage(float dmg, GameObject target = null)
    {
        if (target == null) return;

        // Get the target's dispatcher
        var dispatcher = target.GetComponent<EntityEventDispatcher>();
        if (dispatcher == null) return;

        // Add Frost
        for (int i = 0; i < 4; ++i)
        {
            var frost = new Frost(5f);
            dispatcher.AddEffect(frost);
        }
    }
}