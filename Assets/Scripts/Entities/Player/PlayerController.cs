using JunkMage.Entities.Player;
using JunkMage.Stats;
using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerCombat))]
[RequireComponent(typeof(PlayerHealth))]
public class PlayerController : MonoBehaviour
{
    public PlayerStats Stats { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerCombat Combat { get; private set; }
    public PlayerMana Mana { get; private set; }
    
    private EntityEventDispatcher dispatcher;

    void Awake()
    {
        Stats = GetComponent<PlayerStats>();
        Movement = GetComponent<PlayerMovement>();
        Combat = GetComponent<PlayerCombat>();
        Mana = GetComponent<PlayerMana>();
        dispatcher = GetComponent<EntityEventDispatcher>();
    }

    void Update()
    {
        Movement.HandleInput();
        Combat.HandleInput();
    }

    public void ApplyStatModifier(StatModifier modifier)
    {
        Stats.ApplyModifier(modifier);
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        dispatcher.AddEffect(effect);
    }
}
