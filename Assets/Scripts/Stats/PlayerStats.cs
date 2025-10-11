using UnityEngine;

public class PlayerStats : StatsBase
{
    [Header("Player Base Stats")]
    [Tooltip("Assign a StatSheet asset containing the player's default base stats.")]
    [SerializeField] private StatSheet playerStatSheet;

    // legacy serialized list for scenes that haven't migrated yet (kept hidden in inspector)
    [SerializeField, HideInInspector] private System.Collections.Generic.List<StatEntry> playerBaseStats;

    protected override void Awake()
    {
        // Prefer the assigned sheet
        if (playerStatSheet != null)
            baseStatsSheet = playerStatSheet;
        else
            baseStatsList = playerBaseStats; // legacy fallback

        base.Awake();
    }
}