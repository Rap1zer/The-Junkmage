using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int dmg = 1;
    private bool isPlayerBullet = false;

    public void SetDmg(int dmg, bool isPlayerBullet)
    {
        this.dmg = dmg;
        this.isPlayerBullet = isPlayerBullet;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPlayerBullet && other.CompareTag("Player"))
        {
            var player = other.GetComponent<IDamageable>();
            player?.TakeDamage(dmg);

        }
        else if (isPlayerBullet && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<IDamageable>();
            enemy?.TakeDamage(dmg);
        }

        // Destroy the bullet after it hits something
        if (isPlayerBullet && !other.CompareTag("Player") || !isPlayerBullet && !other.CompareTag("Enemy"))
        {
            Destroy(gameObject, 0.1f);
        }
        
    }
}
