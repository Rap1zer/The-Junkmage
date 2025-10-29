using System;
using JunkMage.Stats;
using UnityEngine;

namespace JunkMage.Entities.Player
{
    public class PlayerMana : MonoBehaviour
    {
        private float currentMana;
        public float CurrentMana
        {
            get => currentMana;
            set => currentMana = Mathf.Clamp(value, 0, stats.GetVal(Stat.MaxMana));
        }

        private float regenBuffer = 0f;
        private readonly float incrementThreshold = 0.01f;

        public event Action OnRegenMana;

        private PlayerController player;
        private PlayerStats stats => player.Stats;

        void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        void Start()
        {
            CurrentMana = stats.GetVal(Stat.MaxMana);
        }
    
        void Update()
        {
            float regenRate = stats.GetVal(Stat.ManaRegenRate);
            regenBuffer += regenRate * Time.deltaTime;
        
            if (regenBuffer >= incrementThreshold)
            {
                CurrentMana += regenBuffer;
                regenBuffer = 0;
                OnRegenMana?.Invoke();
            }
        }
    }
}
