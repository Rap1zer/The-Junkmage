using JunkMage.Stats;
using JunkMage.Systems;
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
        bullet.transform.localScale = new Vector3(stats.GetVal(Stat.BulletSize), stats.GetVal(Stat.BulletSize), 1f);

        bool isCrit = Random.value < stats.GetVal(Stat.CritChance);
        float dmg = isCrit
            ? (stats.GetVal(Stat.AttackDmg) * stats.GetVal(Stat.CritMultiplier))
            : stats.GetVal(Stat.AttackDmg);

        DamageInfo dmgInfo = new DamageInfo
        {
            Dmg = dmg,
            IsCrit = isCrit,
            Attacker = gameObject
        };

        bullet.GetComponent<Bullet>().Initialise(dmgInfo);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
            bulletRb.linearVelocity = firePoint.up * stats.GetVal(Stat.BulletSpeed);
    }
}
