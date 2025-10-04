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

    public void OnAfterDamageTaken(float dmg, GameObject attacker)
    {
        Debug.Log($"[HookDebug] AfterDamage hook fired: {dmg}");
    }

    public void OnDealDamage(float dmg, GameObject target)
    {
        Debug.Log($"[HookDebug] DealDamage hook fired: {dmg}");
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