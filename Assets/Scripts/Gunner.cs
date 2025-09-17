using UnityEngine;

public class Gunner : MonoBehaviour, IEnemy
{
    public int roomIndex;
    private Room spawnRoom;
    private PlayerController player;
    private GameManager gameManager;
    public int Health { get; set; } = 8;
    public int AttackDmg { get; set; } = 2;
    public float AttackCooldown { get; set; } = 1.7f;
    public float Speed { get; set; } = 2f;
    public EnemyState CurrentState { get; set; }

    private float lastAttackTime = -Mathf.Infinity;

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

        if (AttackCooled()) Attack();

        // Blindly beeline towards the player if Attacking
        if (CurrentState == EnemyState.Attacking)
        {
            Vector2 direction = -(player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * Speed;

            // Rotate to face player
            direction = (transform.position - player.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // -90 if your sprite faces up
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
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
        Debug.Log("Gunner ded");
        spawnRoom.EnemyCount--;
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0) Die();
    }
    

}
