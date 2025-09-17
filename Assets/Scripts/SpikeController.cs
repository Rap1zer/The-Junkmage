using UnityEngine;

public class SpikeController : MonoBehaviour, IEnemy
{
    public int roomIndex;
    private Room spawnRoom;
    private PlayerController player;
    private GameManager gameManager;
    public int Health { get; set; } = 13;
    public int AttackDmg { get; set; } = 3;
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
        CurrentState = roomIndex == player.inRoomIndex ? EnemyState.Chasing : EnemyState.Idle;

        if (AttackCooled() && playerInRange) Attack();

        // Blindly beeline towards the player if chasing
        if (CurrentState == EnemyState.Chasing)
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
        lastAttackTime = Time.time;
    }

    public void Die()
    {
        Debug.Log("Spike ded");
        spawnRoom.EnemyCount--;
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0) Die();
    }
    

}
