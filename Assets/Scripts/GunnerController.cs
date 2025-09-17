using UnityEngine;

public class GunnerController : MonoBehaviour, IEnemy
{
    public int roomIndex;
    private Room spawnRoom;
    private PlayerController player;
    private GameManager gameManager;
    public int Health { get; set; } = 6;
    public int AttackDmg { get; set; } = 2;
    public float AttackCooldown { get; set; } = 1.7f;
    public float Speed { get; set; } = 2f;
    public EnemyState CurrentState { get; set; }

    private float lastAttackTime = -Mathf.Infinity;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 9f;
    private Transform firePoint;

    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        firePoint = transform.Find("Fire Point");
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        CurrentState = EnemyState.Idle;
        spawnRoom = gameManager.rooms[roomIndex];
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState = roomIndex == player.inRoomIndex ? EnemyState.Attacking : EnemyState.Idle;

        if (CurrentState == EnemyState.Attacking)
        {
            // Inch away from the player
            Vector2 direction = -(player.transform.position - transform.position).normalized;
            rb.linearVelocity = direction * Speed;

            // Rotate to face player
            direction = (transform.position - player.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // -90 if your sprite faces up
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            if (AttackCooled()) Attack();
        }
    }

    private bool AttackCooled()
    {
        return Time.time >= lastAttackTime + AttackCooldown;
    }

    public void Attack()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().SetDmg(1, false);

        // Apply velocity
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = firePoint.up * bulletSpeed;
        }
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
