using System;
using JunkMage.Player;
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

        public void TakeDamage(float dmg, GameObject attacker = null)
        {
            if (movement.IsDashing) return;
        
            if (dispatcher != null)
            {
                dmg = Math.Max(dmg - stats.GetVal(Stat.Defence), 0);
                dmg = dispatcher.DispatchIncomingDamage(dmg, attacker);
            }

            CurrentHealth -= dmg;

            if (dispatcher != null)
                dispatcher.DispatchAfterDamageTaken(dmg, attacker);
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
