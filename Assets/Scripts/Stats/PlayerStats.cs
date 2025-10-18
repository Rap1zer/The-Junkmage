using System;
using JunkMage.Stats;
using UnityEngine;

namespace JunkMage.Stats
{
    public class PlayerStats : StatsBase
    {
        [Header("Player Base Stats")]
        [Tooltip("Assign a StatSheet asset containing the player's default base stats.")]
        [SerializeField] private StatSheet playerStatSheet;

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