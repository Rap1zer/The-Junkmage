using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float dmg = 1f;

    private GameObject owner;
    private bool isPlayerBullet => owner.CompareTag("Player");

    public void Initilaise(float dmg, GameObject owner)
    {
        this.dmg = dmg;
        this.owner = owner;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Attack object if hit valid target
        if ((!isPlayerBullet && other.CompareTag("Player")) || (isPlayerBullet && other.CompareTag("Enemy")))
        {
            Attack(other.gameObject);
        }

        // Destroy the bullet after it collides
        if (isPlayerBullet && !other.CompareTag("Player") || !isPlayerBullet && !other.CompareTag("Enemy"))
        {
            Destroy(gameObject, 0.1f);
        }

    }
    
    private void Attack(GameObject target)
    {
        EntityEventDispatcher dispatcher = owner.GetComponent<EntityEventDispatcher>();
        if (dispatcher != null) dispatcher.DispatchDealDamage(dmg, target);

        target.GetComponent<IDamageable>().TakeDamage(dmg);
    }
}
