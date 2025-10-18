using JunkMage.Systems;
using UnityEngine;

public interface IIncomingModifier
{
    // Return modified incoming damage
    float OnIncomingDamage(float damage, GameObject attacker);
}

public interface IAfterDamageHandler
{
    void OnAfterDamageTaken(DamageInfo dmgInfo);
}

public interface IDealDamageHandler
{
    void OnDealDamage(DamageInfo dmgInfo);
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
