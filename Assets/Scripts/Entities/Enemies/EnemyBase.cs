using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyStats), typeof(EntityEventDispatcher))]
public abstract class EnemyBase : MonoBehaviour, IEnemy, IDamageable
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

    protected float cooldownTime;
    
    protected Vector2 wanderTarget;
    protected float wanderRadius = 6f;
    protected float wanderSpeedMultiplier = 0.5f;
    protected float wanderTimer = 0f;
    protected float wanderInterval = 6f; // seconds before picking a new target
    
    // Optional convenience properties
    protected float Speed => Stats.GetVal(StatType.MoveSpeed);
    protected float AttackDmg => Stats.GetVal(StatType.AttackDmg);

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Stats = GetComponent<EnemyStats>();
        cooldownTime = AddVariation(Stats.GetVal(StatType.AttackCooldown), 0.2f);
        lastStateChangeTime = -Mathf.Infinity;
    }

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        spawnRoom = RoomManager.Instance.rooms[roomIndex];

        Health = Stats.GetVal(StatType.MaxHealth); // initialize health from playerStats
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
        return Time.time >= lastAttackTime + Stats.GetVal(StatType.AttackCooldown);
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
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f || Vector2.Distance(transform.position, wanderTarget) <= 0.1f)
        {
            PickNewWanderTarget();
            wanderTimer = AddVariation(wanderInterval, 0.3f);
        }

        // Calculate direction toward target
        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;

        // --- Avoid nearby enemies ---
        Collider2D[] hits = new Collider2D[10]; // preallocate or reuse array
        int hitCount = Physics2D.OverlapCircleNonAlloc(transform.position, 1f, hits, LayerMask.GetMask("Enemy"));
        Vector2 avoidance = Vector2.zero;
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].gameObject == gameObject) continue;
            avoidance += ((Vector2)transform.position - (Vector2)hits[i].transform.position).normalized;
        }
        if (avoidance != Vector2.zero)
            direction = (direction + avoidance.normalized).normalized;

        // Move
        rb.linearVelocity = direction * (Speed * wanderSpeedMultiplier);

        // --- Smooth rotation ---
        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 5f); // smooth factor
            transform.rotation = Quaternion.Euler(0f, 0f, smoothAngle);
        }
    }



    private void PickNewWanderTarget()
    {
        LayerMask wallLayer = LayerMask.GetMask("Wall");

        for (int i = 0; i < 10; i++) // try 10 times max
        {
            float r = Mathf.Sqrt(Random.value) * wanderRadius; // bias toward edges
            Vector2 randomOffset = Random.insideUnitCircle.normalized * r;
            Vector2 candidate = (Vector2)transform.position + randomOffset;

            // Cast a ray toward candidate
            Vector2 direction = candidate - (Vector2)transform.position;
            float distance = direction.magnitude;
            direction.Normalize();

            // If ray does NOT hit a wall, the candidate is valid
            if (!Physics2D.Raycast(transform.position, direction, distance, wallLayer))
            {
                wanderTarget = candidate;
                return;
            }
        }

        // fallback: stay in place if no valid target found
        wanderTarget = transform.position;
    }

    protected virtual void LookForPlayer() { }
    protected virtual void DoChaseBehavior() { }
    protected virtual void DoFleeBehavior() { }
    protected virtual void DoInvestigateBehavior() { }

    private float AddVariation(float val, float percent) => val + Random.Range(-val * percent, val * percent);

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
