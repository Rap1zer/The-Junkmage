using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add this to the player (or any entity) to manage and dispatch status effects.
/// Other systems (PlayerStats, PlayerHealth, EnemyBase) should call the manager hooks
/// at the appropriate times (incoming damage, deal damage, room cleared).
/// </summary>
[RequireComponent(typeof(MonoBehaviour))] // placeholder; remove if not needed
public class StatusEffectManager : MonoBehaviour
{
    private readonly List<StatusEffect> effects = new List<StatusEffect>();

    void Update()
    {
        float dt = Time.deltaTime;
        for (int i = effects.Count - 1; i >= 0; --i)
        {
            var e = effects[i];
            e.Tick(dt);
            if (e.IsExpired)
            {
                e.OnRemove();
                effects.RemoveAt(i);
            }
        }
    }

    public void AddEffect(StatusEffect effect)
    {
        if (effect == null) return;
        effect.SetOwner(this);
        effects.Add(effect);
        effect.OnApply();
    }

    public void RemoveEffect(StatusEffect effect)
    {
        if (effect == null) return;
        if (effects.Remove(effect))
        {
            effect.OnRemove();
        }
    }

    public void RemoveAllOfType<T>() where T : StatusEffect
    {
        for (int i = effects.Count - 1; i >= 0; --i)
        {
            if (effects[i] is T se)
            {
                se.OnRemove();
                effects.RemoveAt(i);
            }
        }
    }

    // These dispatcher methods are used by the owning object to pass events into effects:

    public float DispatchIncomingDamage(float dmg, GameObject attacker = null)
    {
        float modified = dmg;
        // iterate in order added (you may want reverse order depending on design)
        foreach (var e in effects)
            modified = e.OnIncomingDamage(modified, attacker);
        return Mathf.Max(0, modified);
    }

    public void DispatchAfterDamageTaken(float dmg, GameObject attacker = null)
    {
        foreach (var e in effects)
            e.OnAfterDamageTaken(dmg, attacker);
    }

    public void DispatchDealDamage(float dmg, GameObject target = null)
    {
        foreach (var e in effects)
            e.OnDealDamage(dmg, target);
    }

    public void DispatchRoomCleared()
    {
        foreach (var e in effects)
            e.OnRoomCleared();
    }
}
