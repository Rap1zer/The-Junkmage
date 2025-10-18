using System;
using JunkMage.Player;
using UnityEngine;

namespace JunkMage.Player
{
    public class PlayerStats : StatsBase
    {
        [Header("Player Base Stats")]
        [Tooltip("Assign a StatSheet asset containing the player's default base stats.")]
        [SerializeField] private StatSheet playerStatSheet;

        // legacy serialized list for scenes that haven't migrated yet (kept hidden in inspector)
        [SerializeField, HideInInspector] private System.Collections.Generic.List<StatEntry> playerBaseStats;

        public event Action OnSetMaxHealth;
        
        void Awake()
        {
            // Prefer the assigned sheet
            if (playerStatSheet != null)
                baseStatsSheet = playerStatSheet;
        }

        public override void ApplyModifier(StatModifier modifier)
        {
            if (modifier.Stat == Stat.MaxHealth) OnSetMaxHealth?.Invoke();
            base.ApplyModifier(modifier);
        }
    }
}