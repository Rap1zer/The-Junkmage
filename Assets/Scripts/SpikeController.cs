using UnityEngine;

public class SpikeController : MonoBehaviour, IEnemy
{
    public int roomIndex;
    private PlayerController player;
    public int Health { get; set; } = 13;
    public int AttackDmg { get; set; } = 3;
    public float AttackCooldown { get; set; } = 0.5f;
    public float Speed { get; set; } = 5f;
    public EnemyState CurrentState { get; set; }

    private float lastAttackTime = -Mathf.Infinity;
    public bool playerInRange = false;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        CurrentState = EnemyState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState = roomIndex == player.inRoomIndex ? EnemyState.Chasing : EnemyState.Idle;

        if (AttackCooled() && playerInRange)
        {
            Attack();
            Debug.Log("I'm attacking!!!");
        }

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
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0) Die();
    }
    

}
