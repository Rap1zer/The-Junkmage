using System;
using System.Collections.Generic;
using JunkMage.Systems;
using UnityEngine;

/// <summary>
/// Central event dispatcher per entity. Both StatusEffects and Items register handlers here.
/// This keeps dispatch fast (only call registered handlers) and allows deterministic ordering.
/// </summary>
public class EntityEventDispatcher : MonoBehaviour
{
    // Keep effect list for lifecycle and RemoveAllOfType semantics
    private readonly List<StatusEffect> effects = new List<StatusEffect>();

    // Per-hook registries (preserve registration order)
    private readonly List<Func<float, GameObject, float>> incomingModifiers = new();
    private readonly List<Action<DamageInfo>> afterDamageHandlers = new();
    private readonly List<Action<DamageInfo>> dealDamageHandlers = new();
    private readonly List<Action> roomClearedHandlers = new();
    private readonly List<Action<float>> tickHandlers = new();
    private readonly List<Action> missedAttackHandlers = new();

    // Mapping registrant (effect or item) -> the delegates we added for it,
    // so we can remove them exactly on unregister.
    private readonly Dictionary<object, RegisteredHandlers> registrations = new Dictionary<object, RegisteredHandlers>();

    private float tickTimer = 0f;
    [SerializeField] private float tickInterval = 0.5f;

    private void Update()
    {
        float dt = Time.deltaTime;
        tickTimer += dt;
        
        // Only process tick handlers if an interval passed
        if (tickTimer >= tickInterval)
        {
            // We might be slightly above tickInterval; carry the remainder forward
            tickTimer -= tickInterval;

            for (int i = 0; i < tickHandlers.Count; ++i)
                tickHandlers[i](tickInterval);
        }

        // Now check for expired effects and remove them
        for (int i = effects.Count - 1; i >= 0; --i)
        {
            var e = effects[i];
            if (e.IsExpired)
            {
                // RemoveEffect will unregister handlers and call OnRemove
                RemoveEffect(e);
            }
        }
    }

    // -------------------------
    // Effect lifecycle (public)
    // -------------------------
    public void AddEffect(StatusEffect effect)
    {
        if (effect == null) return;
        effect.SetOwner(this);
        effects.Add(effect);
        RegisterHandlers(effect, isItem: false);
        effect.OnApply();
    }

    public void RemoveEffect(StatusEffect effect)
    {
        if (effect == null) return;
        if (effects.Remove(effect))
        {
            UnregisterHandlers(effect);
            effect.OnRemove();
        }
    }

    public void RemoveAllOfType<T>() where T : StatusEffect
    {
        for (int i = effects.Count - 1; i >= 0; --i)
        {
            if (effects[i] is T se)
            {
                // RemoveEffect will handle unregistering and OnRemove
                RemoveEffect(se);
            }
        }
    }

    // -------------------------
    // Item registration helpers
    // -------------------------
    // Items should call this to register their hook methods when initialised (or equipped).
    public void RegisterItemHandlers(object item)
    {
        if (item == null) return;
        RegisterHandlers(item, isItem: true);
    }

    public void UnregisterItemHandlers(object item)
    {
        if (item == null) return;
        UnregisterHandlers(item);
    }

    // -------------------------
    // Dispatchers (public)
    // -------------------------
    // Just before damage is applied to this entity
    public float DispatchIncomingDamage(float dmg, GameObject attacker = null)
    {
        float modified = dmg;
        // Iterate over a snapshot to be safe if handlers register/unregister during iteration
        var handlers = incomingModifiers.ToArray();
        for (int i = 0; i < handlers.Length; ++i)
            modified = handlers[i](modified, attacker);
        return Mathf.Max(0, modified);
    }

    // After damage has been subtracted from this entity's health
    public void DispatchAfterDamageTaken(DamageInfo dmgInfo)
    {
        var handlers = afterDamageHandlers.ToArray();
        for (int i = 0; i < handlers.Length; ++i)
            handlers[i](dmgInfo);
    }

    // Called on the attacker entity after it deals damage to a target
    public void DispatchDealDamage(DamageInfo dmgInfo)
    {
        var handlers = dealDamageHandlers.ToArray();
        for (int i = 0; i < handlers.Length; ++i)
            handlers[i](dmgInfo);
    }

    // Called when the entity clears a room (or equivalent event)
    public void DispatchRoomCleared()
    {
        var handlers = roomClearedHandlers.ToArray();
        for (int i = 0; i < handlers.Length; ++i)
            handlers[i]();
    }
    
    // Called when the entity misses an attack
    public void DispatchMissedAttack()
    {
        var handlers = missedAttackHandlers.ToArray();
        for (int i = 0; i < handlers.Length; ++i)
            handlers[i]();
    }

    // -------------------------
    // Internal registration helpers
    // -------------------------
    // This inspects the object and registers delegates for the hooks it supports.
    private void RegisterHandlers(object obj, bool isItem)
    {
        if (obj == null) return;
        if (registrations.ContainsKey(obj)) return; // already registered

        var reg = new RegisteredHandlers();

        // If object is a StatusEffect, use that strongly-typed instance (so we can also add to effects list)
        if (obj is StatusEffect se)
        {
            // Register only the interfaces the effect implements
            if (se is IIncomingModifier im)
            {
                reg.incoming = new Func<float, GameObject, float>(im.OnIncomingDamage);
                incomingModifiers.Add(reg.incoming);
            }

            if (se is IAfterDamageHandler after)
            {
                reg.afterDamage = new Action<DamageInfo>(after.OnAfterDamageTaken);
                afterDamageHandlers.Add(reg.afterDamage);
            }

            if (se is IDealDamageHandler deal)
            {
                reg.dealDamage = new Action<DamageInfo>(deal.OnDealDamage);
                dealDamageHandlers.Add(reg.dealDamage);
            }

            if (se is IRoomClearedHandler rc)
            {
                reg.roomCleared = new Action(rc.OnRoomCleared);
                roomClearedHandlers.Add(reg.roomCleared);
            }

            if (se is ITickHandler th)
            {
                reg.tick = new Action<float>(th.Tick);
                tickHandlers.Add(reg.tick);
            }

            if (se is IMissedAttackHandler miss)
            {
                reg.missedAttack = new Action(miss.OnMissedAttack);
                missedAttackHandlers.Add(reg.missedAttack);
            }

            // ensure lifecycle tracking for non-item effects
            if (!isItem && !effects.Contains(se))
                effects.Add(se);
        }
        else
        {
            // Generic object (likely an ItemBase). Prefer items to implement the same interfaces.
            if (obj is IIncomingModifier im)
            {
                reg.incoming = new Func<float, GameObject, float>(im.OnIncomingDamage);
                incomingModifiers.Add(reg.incoming);
            }

            if (obj is IAfterDamageHandler after)
            {
                reg.afterDamage = new Action<DamageInfo>(after.OnAfterDamageTaken);
                afterDamageHandlers.Add(reg.afterDamage);
            }

            if (obj is IDealDamageHandler deal)
            {
                reg.dealDamage = new Action<DamageInfo>(deal.OnDealDamage);
                dealDamageHandlers.Add(reg.dealDamage);
            }

            if (obj is IRoomClearedHandler rc)
            {
                reg.roomCleared = new Action(rc.OnRoomCleared);
                roomClearedHandlers.Add(reg.roomCleared);
            }

            if (obj is ITickHandler th)
            {
                reg.tick = new Action<float>(th.Tick);
                tickHandlers.Add(reg.tick);
            }
            
            if (obj is IMissedAttackHandler miss)
            {
                reg.missedAttack = new Action(miss.OnMissedAttack);
                missedAttackHandlers.Add(reg.missedAttack);
            }
        }

        // store registration (even if empty — makes Unregister safe)
        registrations[obj] = reg;
    }

    private void UnregisterHandlers(object obj)
    {
        if (obj == null) return;
        if (!registrations.TryGetValue(obj, out var reg)) return;

        if (reg.incoming != null) incomingModifiers.Remove(reg.incoming);
        if (reg.afterDamage != null) afterDamageHandlers.Remove(reg.afterDamage);
        if (reg.dealDamage != null) dealDamageHandlers.Remove(reg.dealDamage);
        if (reg.roomCleared != null) roomClearedHandlers.Remove(reg.roomCleared);
        if (reg.tick != null) tickHandlers.Remove(reg.tick);

        registrations.Remove(obj);
    }


    // -------------------------
    // Private types
    // -------------------------
    // Store exact delegates added for a registrant so we can remove them precisely.
    private class RegisteredHandlers
    {
        public Func<float, GameObject, float> incoming;
        public Action<DamageInfo> afterDamage;
        public Action<DamageInfo> dealDamage;
        public Action roomCleared;
        public Action<float> tick;
        public Action missedAttack;
    }
}
