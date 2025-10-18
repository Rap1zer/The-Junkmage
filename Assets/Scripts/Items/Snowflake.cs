using JunkMage.Systems;
using UnityEngine;

public class Snowflake : ItemBase, IDealDamageHandler
{
    public void OnDealDamage(DamageInfo dmgInfo)
    {
        if (dmgInfo.Target == null) return;

        // Get the target's dispatcher
        var dispatcher = dmgInfo.Target.GetComponent<EntityEventDispatcher>();
        if (dispatcher == null) return;

        // Add Frost
        for (int i = 0; i < 2; ++i)
        {
            var frost = new Frost(5f);
            dispatcher.AddEffect(frost);
        }
    }
}