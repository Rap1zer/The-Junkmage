using UnityEngine;

/// <summary>
/// Base class for all stateful/behavioral effects (buffs/debuffs).
/// Duration: seconds. duration <= 0 means infinite until explicitly removed.
/// Effects are applied to an object that has a StatusEffectManager (the "target").
/// </summary>
public abstract class StatusEffect
{
    public readonly string Id;          // optional id/name for debugging / removal
    public float Duration;             // seconds; <= 0 => infinite
    public bool IsExpired => Duration > 0f && elapsed >= Duration;
    public bool IsApplied { get; private set; }

    protected float elapsed = 0f;
    protected StatusEffectManager owner; // the manager that owns this effect

    public StatusEffect(string id, float duration)
    {
        Id = id;
        Duration = duration;
    }

    internal void SetOwner(StatusEffectManager mgr)
    {
        owner = mgr;
    }

    /// <summary>Called once when effect is added to the manager.</summary>
    public virtual void OnApply() { IsApplied = true; }

    /// <summary>Called once when effect is removed or expires.</summary>
    public virtual void OnRemove() { IsApplied = false; }

    /// <summary>Called every frame from the manager (deltaTime).</summary>
    public virtual void Tick(float deltaTime)
    {
        if (Duration > 0f) elapsed += deltaTime;
    }

    /// <summary>
    /// Hook to allow an effect to modify incoming damage before it is applied.
    /// Return the modified damage. Attacker may be null.
    /// Effects are called in the order they were added.
    /// </summary>
    public virtual float OnIncomingDamage(float damage, GameObject attacker = null) => damage;

    /// <summary>
    /// Hook called after damage is applied (useful for thorns, lifesteal, etc).
    /// </summary>
    public virtual void OnAfterDamageTaken(float damageTaken, GameObject attacker = null) { }

    /// <summary>
    /// Hook called when this object deals damage to another entity.
    /// </summary>
    public virtual void OnDealDamage(float damageDealt, GameObject target = null) { }

    /// <summary>
    /// Hook called when the owning object clears a room (or when an external system calls it).
    /// </summary>
    public virtual void OnRoomCleared() { }

    /// <summary>
    /// Convenience: ask owner to remove this effect.
    /// </summary>
    protected void RemoveSelf() => owner?.RemoveEffect(this);
}
