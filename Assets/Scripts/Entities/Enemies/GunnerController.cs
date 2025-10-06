using UnityEngine;

public class GunnerController : EnemyBase
{
    [Header("Gunner Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 9f;
    private Transform firePoint;

    protected override void Start()
    {
        base.Start();
        firePoint = transform.Find("Fire Point");
    }

    protected override void DoAttackBehavior()
    {
        // Move backwards from player
        Vector2 direction = -(player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * Speed;

        // Rotate to face player
        Vector2 lookDir = (transform.position - player.transform.position).normalized;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (AttackCooled()&& !playerMovement.IsDashing) Attack();
    }

    public override void Attack()
    {
        if (bulletPrefab == null || firePoint == null) return;
        base.Attack();

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().Initilaise(AttackDmg, gameObject);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.linearVelocity = firePoint.up * bulletSpeed;
    }
}
