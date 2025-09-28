using UnityEngine;

public class SpikeController : MonoBehaviour, IEnemy
{
    public int roomIndex;
    private Room spawnRoom;
    private PlayerController player;
    private GameManager gameManager;
    public int Health { get; set; } = 12;
    public int AttackDmg { get; set; } = 5;
    public float AttackCooldown { get; set; } = 0.5f;
    public float Speed { get; set; } = 5f;
    public EnemyState CurrentState { get; set; }

    private float lastAttackTime = -Mathf.Infinity;
    public bool playerInRange = false;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        CurrentState = EnemyState.Idle;
        spawnRoom = gameManager.rooms[roomIndex];
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState = roomIndex == player.inRoomIndex ? EnemyState.Attacking : EnemyState.Idle;

        if (AttackCooled() && playerInRange) Attack();

        // Blindly beeline towards the player if Attacking
        if (CurrentState == EnemyState.Attacking)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * Speed;
        }
    }

    private bool AttackCooled()
    {
        return Time.time >= lastAttackTime + AttackCooldown;
    }

    public void Attack()
    {
        player.TakeDamage(AttackDmg);
        lastAttackTime = Time.time;
    }

    public void Die()
    {
        spawnRoom.EnemyCount--;
        Destroy(gameObject);
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0) Die();
    }
    

}
