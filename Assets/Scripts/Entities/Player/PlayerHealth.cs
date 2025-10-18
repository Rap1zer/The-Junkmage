using System;
using JunkMage.Player;
using JunkMage.Systems;
using UnityEngine;

namespace JunkMage.Entities.Player
{
    [RequireComponent(typeof(PlayerStats))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private PlayerStats stats;
        private PlayerMovement movement;
        private EntityEventDispatcher dispatcher;
        
        public event Action OnSetCurrentHealth;

        private float currentHealth;
        public float CurrentHealth
        {
            get => currentHealth;
            private set
            {
                currentHealth = value;

                OnSetCurrentHealth?.Invoke();
                
                if (CurrentHealth <= 0)
                {
                    Die();
                    OnDeath?.Invoke();
                }
            }
        
        }

        public event System.Action OnDeath;
        public event System.Action<float> OnHealthChanged;

        void Awake()
        {
            stats = GetComponent<PlayerStats>();
            dispatcher = GetComponent<EntityEventDispatcher>();
            movement = GetComponent<PlayerMovement>();
        }

        void Start()
        {
            CurrentHealth = (int)stats.GetVal(Stat.MaxHealth);
        }

        public void TakeDamage(DamageInfo dmgInfo)
        {
            if (movement.IsDashing) return;
        
            if (dispatcher != null)
            {
                dmgInfo.Dmg = Math.Max(dmgInfo.Dmg - stats.GetVal(Stat.Defence), 0);
                dmgInfo.Dmg = dispatcher.DispatchIncomingDamage(dmgInfo.Dmg, dmgInfo.Attacker);
            }

            CurrentHealth -= dmgInfo.Dmg;

            if (dispatcher != null)
                dispatcher.DispatchAfterDamageTaken(dmgInfo);
        }

        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, (int)stats.GetVal(Stat.MaxHealth));
            OnHealthChanged?.Invoke(CurrentHealth);
        }

        private void Die()
        {
            //Debug.Log("Player died");
            // TODO: trigger events
        }
    }
}
