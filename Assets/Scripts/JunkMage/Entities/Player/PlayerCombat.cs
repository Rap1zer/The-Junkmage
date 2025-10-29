using JunkMage.Entities;
using JunkMage.Entities.Player;
using JunkMage.Stats;
using JunkMage.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PlayerStats))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    private PlayerStats stats;
    private PlayerMana mana;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        mana = GetComponent<PlayerMana>();
    }

    public void HandleInput()
    {
        float manaCost = stats.GetVal(Stat.AttackManaCost);
        if (Input.GetMouseButtonDown(0) && mana.CurrentMana >= manaCost)
        {
            mana.CurrentMana -= manaCost;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || InventoryManager.Instance.IsInventoryOpen) return;

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
