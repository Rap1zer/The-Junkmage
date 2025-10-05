using UnityEngine;

[RequireComponent(typeof(EnemyStats), typeof(EntityEventDispatcher))]
public abstract class EnemyBase : MonoBehaviour, IEnemy, IDamageable
{
    [Header("Enemy Info")]
    public int roomIndex;
    protected Room spawnRoom;
    protected PlayerController player;
    protected GameManager gameManager;

    public EnemyStats Stats { get; protected set; } // reference to the new EnemyStats component

    public float Health { get; protected set; }
    public EnemyState CurrentState { get; set; }

    protected Rigidbody2D rb;
    protected float lastAttackTime = -Mathf.Infinity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Stats = GetComponent<EnemyStats>();
    }

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnRoom = RoomManager.Instance.rooms[roomIndex];

        Health = Stats.GetVal(StatType.MaxHealth); // initialize health from playerStats
        CurrentState = EnemyState.Idle;

        RoomManager.Instance.OnPlayerEnterRoom += HandlePlayerEnterRoom;
    }

    protected virtual void Update()
    {
        if (CurrentState == EnemyState.Attacking)
            DoAttackBehavior();
    }

    void OnDisable()
    {
        if (RoomManager.Instance != null)
            RoomManager.Instance.OnPlayerEnterRoom -= HandlePlayerEnterRoom;
    }

    private void HandlePlayerEnterRoom(int playerRoomIndex)
    {
        CurrentState = (playerRoomIndex == roomIndex)
            ? EnemyState.Attacking
            : EnemyState.Idle;
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

    public virtual void TakeDamage(float dmg, GameObject attacker = null)
    {
        Health -= dmg;
        if (Health <= 0) Die();
    }

    public virtual void Die()
    {
        spawnRoom.EnemyCount--;
        Destroy(gameObject);
    }

    // Optional convenience properties
    protected float Speed => Stats.GetVal(StatType.MoveSpeed);
    protected float AttackDmg => Stats.GetVal(StatType.AttackDmg);
}
