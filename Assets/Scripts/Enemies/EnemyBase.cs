using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IEnemy, IDamageable
{
    [Header("Enemy Info")]
    public int roomIndex;
    protected Room spawnRoom;
    protected PlayerController player;
    protected GameManager gameManager;

    [Header("Stats")]
    public int maxHealth = 10;
    public int AttackDmg { get; set; } = 1;
    public float AttackCooldown { get; set; } = 1f;
    public float Speed { get; set; } = 2f;

    public float Health { get; protected set; }
    public EnemyState CurrentState { get; set; }

    protected Rigidbody2D rb;
    protected float lastAttackTime = -Mathf.Infinity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        spawnRoom = RoomManager.Instance.rooms[roomIndex];

        Health = maxHealth;
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
        return Time.time >= lastAttackTime + AttackCooldown;
    }

    // Must be implemented in subclasses
    public abstract void Attack();
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
}
