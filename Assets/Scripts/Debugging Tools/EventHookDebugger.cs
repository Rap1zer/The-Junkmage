using JunkMage.Systems;
using UnityEngine;

public class EventHookDebugger : MonoBehaviour,
    IIncomingModifier, IAfterDamageHandler, IDealDamageHandler, IRoomClearedHandler, IMissedAttackHandler
{
    private void Awake()
    {
        var dispatcher = GetComponent<EntityEventDispatcher>();
        dispatcher.RegisterItemHandlers(this);
    }

    public float OnIncomingDamage(float dmg, GameObject attacker)
    {
        Debug.Log($"[HookDebug] Incoming damage modified hook fired on {name}: {dmg}");
        return dmg;
    }

    public void OnAfterDamageTaken(DamageInfo dmgInfo)
    {
        Debug.Log($"[HookDebug] AfterDamage hook fired: {dmgInfo.Dmg}");
    }

    public void OnDealDamage(DamageInfo dmgInfo)
    {
        Debug.Log($"[HookDebug] DealDamage hook fired: {dmgInfo.Dmg}");
    }

    public void OnRoomCleared()
    {
        Debug.Log("[HookDebug] RoomCleared fired");
    }

    public void OnMissedAttack()
    {
        Debug.Log("[HookDebug] MissedAttack fired");
    }
}