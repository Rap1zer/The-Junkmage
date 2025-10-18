using System.Collections;
using JunkMage.Environment;
using JunkMage.Entities.Enemies.Movement;
using Systems;
using UnityEngine;

namespace JunkMage.Entities.Enemies
{
    public enum EnemyState
    {
        Idle,          // Wandering / standing still
        Chasing,       // Moving toward the player
        Attacking,     // Shooting / melee
        Fleeing,       // Running away (low health)
    }

    [RequireComponent(typeof(EnemyStats), typeof(EntityEventDispatcher), typeof(EnemyMovement))]
    public abstract class EnemyBase : MonoBehaviour, IDamageable
    {
        #region === Fields ===

        [Header("Enemy Info")]
        public int roomIndex;

        protected Room spawnRoom;
        protected GameObject player;
        protected PlayerMovement playerMovement;
        protected IDamageable playerHealth;

        [Header("State")]
        [SerializeField] private float stateChangeCooldown = 0.3f;
        [SerializeField] protected float idleToAttackingBufferStart = 0.2f;
        [SerializeField] protected float idleToAttackingBufferEnd = 0.4f;
        private float lastStateChangeTime;

        private EnemyState currentState;
        private Wander wander;

        private float lastAttackTime = -Mathf.Infinity;
        #endregion

        #region === Components & Stats ===

        public EnemyStats Stats { get; protected set; }
        protected EnemyMovement Movement { get; private set; }

        protected float Health { get; set; }

        protected bool PlayerInRoom { get; private set;  } = false;

        protected float AttackDmg => Stats.HasStat(Stat.AttackDmg) ? Stats.GetVal(Stat.AttackDmg) : 1f;
        protected float AttackCooldown => Stats.HasStat(Stat.AttackCooldown) ? Stats.GetVal(Stat.AttackCooldown) : 1f;

        #endregion

        #region === Properties ===

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
        #endregion

        #region === Unity Lifecycle ===

        protected virtual void Awake()
        {
            Stats = GetComponent<EnemyStats>();
            Movement = GetComponent<EnemyMovement>();
            wander = new Wander();
            lastStateChangeTime = -Mathf.Infinity;
        }

        protected virtual void Start()
        {
            player = GameObject.FindWithTag("Player");
            playerMovement = player.GetComponent<PlayerMovement>();
            playerHealth = player.GetComponent<IDamageable>();
            spawnRoom = RoomManager.Instance.rooms[roomIndex];

            Health = Stats.GetVal(Stat.MaxHealth);
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
            }
        }

        protected virtual void OnDisable()
        {
            if (RoomManager.Instance != null)
                RoomManager.Instance.OnPlayerEnterRoom -= HandlePlayerEnterRoom;
        }

        #endregion

        #region === State Handling ===

        private void HandlePlayerEnterRoom(int playerRoomIndex)
        {
            if (playerRoomIndex == roomIndex)
            {
                StartCoroutine(DelayedAttackState(
                    Random.Range(idleToAttackingBufferStart, idleToAttackingBufferEnd)
                    ));
                PlayerInRoom = true;
            }
            else
                CurrentState = EnemyState.Idle;
        }

        private IEnumerator DelayedAttackState(float delay)
        {
            yield return new WaitForSeconds(delay);
            CurrentState = EnemyState.Attacking;
        }

        protected bool AttackCooled(float extraCooldown = 0f) =>
            Time.time >= lastAttackTime + AttackCooldown +  extraCooldown;

        #endregion

        #region === Combat ===

        public virtual void TakeDamage(float dmg, GameObject attacker = null)
        {
            Health -= dmg;
            if (Health <= 0)
                Die();
        }

        public virtual void Die()
        {
            spawnRoom.EnemyCount--;
            Destroy(gameObject);
        }

        protected virtual void Attack()
        {
            lastAttackTime = Time.time;
        }

        #endregion

        #region === AI Behaviors ===

        private void DoIdleBehavior()
        {
            Movement.SetBehavior(wander);
            Movement.Move(new MovementContext());
        }

        protected virtual void LookForPlayer() { }

        protected virtual void DoChaseBehavior() { }

        protected abstract void DoAttackBehavior();

        protected virtual void DoFleeBehavior() { }
        #endregion
    }
}
