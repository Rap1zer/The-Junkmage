using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(EnemyMovement))]
public class GunnerController : EnemyBase
{
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Ranges")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private float retreatRange = 4f;
    [SerializeField] private float detectionRange = 14f;
    
    [Header("Buffers")]
    [SerializeField] private float attackEndMultiplier = 1.5f; // Switch from attacking to chasing
    [SerializeField] private float retreatExitMultiplier = 1.4f; // Switch from retreating to chasing

    private EnemyMovement movement;
    private GunnerStats stats;

    protected override void Start()
    {
        base.Start();
        movement = GetComponent<EnemyMovement>();
        stats = GetComponent<GunnerStats>();
        if (firePoint == null)
            firePoint = transform.Find("Fire Point");
        
        attackRange = attackRange * Random.Range(0.9f, 1.1f);
        retreatRange = retreatRange * Random.Range(0.9f, 1.1f);
        detectionRange = detectionRange * Random.Range(0.9f, 1.1f);
    }

    protected override void LookForPlayer()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist <= detectionRange)
            CurrentState = EnemyState.Chasing;
    }

    protected override void DoChaseBehavior()
    {
        if (player == null) return;

        Vector2 toPlayer = player.transform.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > detectionRange)
        {
            CurrentState = EnemyState.Idle;
            return;
        }

        if (dist <= attackRange)
        {
            CurrentState = EnemyState.Attacking;
            return;
        }

        movement.MoveTo(player.transform.position, 1.5f);
        FacePlayer(toPlayer);
    }

    // Called at every frame when CurrentState = Attacking
    protected override void DoAttackBehavior()
    {
        if (player == null) return;

        Vector2 toPlayer = player.transform.position - transform.position;
        float dist = toPlayer.magnitude;
        
        // Stand still or strafe slightly while attacking
        movement.StrafeAround(player.transform.position);

        // Smoothly face the player
        FacePlayer(toPlayer);

        if (dist < retreatRange)
        {
            CurrentState = EnemyState.Fleeing;
            return;
        }

        if (dist > attackRange * attackEndMultiplier)
        {
            CurrentState = EnemyState.Chasing;
            return;
        }

        if (AttackCooled()) Attack();
    }

    protected override void DoFleeBehavior()
    {
        if (player == null) return;

        Vector2 away = (transform.position - player.transform.position).normalized;
        movement.Retreat(away);

        float dist = Vector2.Distance(transform.position, player.transform.position);
        if (dist > retreatRange * retreatExitMultiplier)
            CurrentState = EnemyState.Chasing;

        FacePlayer(-away);
        if (AttackCooled()) Attack();
    }

    private void FacePlayer(Vector2 toPlayer)
    {
        if (toPlayer == Vector2.zero) return;
        float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg - 90f;
        float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 10f);
        transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
    }

    public override void Attack()
    {
        base.Attack();
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var b = bullet.GetComponent<Bullet>();
        if (b != null)
            b.Initilaise(AttackDmg, gameObject);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = -firePoint.up * stats.GetVal(StatType.BulletSpeed);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Attack Range (Orange) - When the enemy starts attacking
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.75f); // Orange
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Retreat Range (Red) - When the enemy starts fleeing
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.75f); // Red
        Gizmos.DrawWireSphere(transform.position, retreatRange);

        // Detection Range (Yellow) - When the enemy switches from Idle to Chasing
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f); // Yellow (more transparent)
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Extended Retreat Range (Light Blue) - The threshold to stop fleeing
        // The fleeing logic uses 'retreatRange * retreatExitMultiplier'
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.6f); // Light Blue
        Gizmos.DrawWireSphere(transform.position, retreatRange * retreatExitMultiplier);
        
        // Extended Attack Range (Green) - The threshold to stop attacking and start chasing again
        // The attacking logic uses 'attackRange * attackEndMultiplier'
        Gizmos.color = new Color(0f, 1f, 0f, 0.6f); // Green
        Gizmos.DrawWireSphere(transform.position, attackRange * attackEndMultiplier);
    }
}
