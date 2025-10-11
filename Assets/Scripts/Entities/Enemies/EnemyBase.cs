using System.Collections;
using JunkMage.Entities.Enemies.Movement;
using UnityEngine;

namespace JunkMage.Entities.Enemies
{
    public enum EnemyState
    {
        Idle,        // Wandering / standing still
        Chasing,     // Moving toward the player
        Attacking,   // Shooting / melee
        Fleeing,     // Running away (low health)
        Investigating // Player was seen or something happened
    }

    [RequireComponent(typeof(EnemyStats), typeof(EntityEventDispatcher), typeof(EnemyMovement))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        [Header("Enemy Info")]
        public int roomIndex;
        protected Room spawnRoom;
        protected GameObject player;
        protected PlayerMovement playerMovement;
    
        [Header("State")]
        [SerializeField] private float stateChangeCooldown = 0.3f;
        private float lastStateChangeTime;

        public EnemyStats Stats { get; protected set; } // reference to the new EnemyStats component

        public float Health { get; protected set; }

        protected Rigidbody2D rb;
        protected float lastAttackTime = -Mathf.Infinity;
        protected float lastDamagedTime = -Mathf.Infinity;

        private EnemyState currentState;
        protected EnemyMovement Movement { get; private set; }
        
        private Wander wander;
        
        public EnemyState CurrentState
        {
            get => currentState;
            protected set
            {
                if (Time.time - lastStateChangeTime < stateChangeCooldown) return;
                if (value == currentState) return;
            
                currentState = value;
                lastStateChangeTime = Time.time;
            }
        }
        protected float AttackDmg => Stats.HasStat(Stat.AttackDmg) ? Stats.GetVal(Stat.AttackDmg) : 1f;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            Stats = GetComponent<EnemyStats>();
            Movement = GetComponent<EnemyMovement>();
            lastStateChangeTime = -Mathf.Infinity;
            wander = new Wander();
        }

        protected virtual void Start()
        {
            player = GameObject.FindWithTag("Player");
            playerMovement = player.GetComponent<PlayerMovement>();
            spawnRoom = RoomManager.Instance.rooms[roomIndex];

            Health = Stats.GetVal(Stat.MaxHealth); // initialize health from playerStats
            CurrentState = EnemyState.Idle;

            RoomManager.Instance.OnPlayerEnterRoom += HandlePlayerEnterRoom;
        }

        protected virtual void Update()
        {
            switch (CurrentState)
            {
                case EnemyState.Idle:
                    DoIdleBehavior();
                    LookForPlayer();
                    break;
                case EnemyState.Chasing:
                    DoChaseBehavior();
                    break;
                case EnemyState.Attacking:
                    DoAttackBehavior();
                    break;
                case EnemyState.Fleeing:
                    DoFleeBehavior();
                    break;
                case EnemyState.Investigating:
                    DoInvestigateBehavior();
                    break;
            }
        }

        void OnDisable()
        {
            if (RoomManager.Instance != null)
                RoomManager.Instance.OnPlayerEnterRoom -= HandlePlayerEnterRoom;
        }

        private void HandlePlayerEnterRoom(int playerRoomIndex)
        {
            if (playerRoomIndex == roomIndex)
            {
                StartCoroutine(DelayedAttackState(Random.Range(0.2f, 0.4f)));
            }
            else
            {
                CurrentState = EnemyState.Idle;
            }
        }

        private IEnumerator DelayedAttackState(float delay)
        {
            yield return new WaitForSeconds(delay);
            CurrentState = EnemyState.Attacking;
        }

        protected bool AttackCooled()
        {
            return Time.time >= lastAttackTime + Stats.GetVal(Stat.AttackCooldown);
        }

        // Must be implemented in subclasses
        public virtual void Attack()
        {
            lastAttackTime = Time.time;
        }
        protected abstract void DoAttackBehavior();

        // Preallocate this once in your class
        private Collider2D[] avoidanceHits = new Collider2D[10]; // max 10 nearby enemies

        protected virtual void DoIdleBehavior()
        {
            Movement.SetBehavior(wander);
            Movement.Move(new MovementContext());
        }

        protected virtual void LookForPlayer() { }
        protected virtual void DoChaseBehavior() { }
        protected virtual void DoFleeBehavior() { }
        protected virtual void DoInvestigateBehavior() { }
        
        public virtual void TakeDamage(float dmg, GameObject attacker = null)
        {
            lastDamagedTime = Time.time;
            Health -= dmg;
            if (Health <= 0) Die();
        }

        public virtual void Die()
        {
            spawnRoom.EnemyCount--;
            Destroy(gameObject);
        }
    }
}