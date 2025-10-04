using UnityEngine;

[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    private PlayerStats stats;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
            Shoot();
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || InventoryManager.Instance.isInventoryOpen) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        int dmg = (int)(UnityEngine.Random.value < stats.GetVal(StatType.CritChance)
            ? (stats.GetVal(StatType.BulletDmg) * stats.GetVal(StatType.CritMultiplier))
            : stats.GetVal(StatType.BulletDmg));

        bullet.GetComponent<Bullet>().Initilaise(dmg, gameObject);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.linearVelocity = firePoint.up * stats.GetVal(StatType.BulletSpeed);
    }
}
