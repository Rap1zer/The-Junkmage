using JunkMage.Entities.Player;
using JunkMage.Player;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour, IPlayerItemConsumer
{
    private PlayerStats stats;
    private PlayerMovement movement;
    private PlayerCombat combat;
    private EntityEventDispatcher dispatcher;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();
        combat = GetComponent<PlayerCombat>();
        dispatcher = GetComponent<EntityEventDispatcher>();
    }

    void Update()
    {
        movement.HandleInput();
        combat.HandleInput();
    }

    public void ApplyStatModifier(StatModifier modifier)
    {
        stats.ApplyModifier(modifier);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        dispatcher.AddEffect(effect);
    }
}
