using UnityEngine;

public interface IIncomingModifier
{
    // Return modified incoming damage
    float OnIncomingDamage(float damage, GameObject attacker);
}

public interface IAfterDamageHandler
{
    void OnAfterDamageTaken(float damage, GameObject attacker);
}

public interface IDealDamageHandler
{
    void OnDealDamage(float damage, GameObject target);
}

public interface IRoomClearedHandler
{
    void OnRoomCleared();
}

public interface ITickHandler
{
    void Tick(float dt);
}

public interface IMissedAttackHandler
{
    void OnMissedAttack();
}
