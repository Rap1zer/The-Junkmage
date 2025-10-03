using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float dmg;
    private GameObject owner;
    private bool IsPlayerBullet => owner.CompareTag("Player");

    public void Initilaise(float dmg, GameObject owner)
    {
        this.dmg = dmg;
        this.owner = owner;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitEntity = (!IsPlayerBullet && other.CompareTag("Player")) ||
                             (IsPlayerBullet && other.CompareTag("Enemy"));
        bool hitValidObj = !((IsPlayerBullet && other.CompareTag("Player")) ||
                             (!IsPlayerBullet && other.CompareTag("Enemy"))) &&
                           !other.CompareTag("Bullet");
        
        if (hitEntity) Attack(other.gameObject);

        // Destroy the bullet after it collides
        if (hitValidObj)
        {
            if (!hitEntity) owner.GetComponent<EntityEventDispatcher>().DispatchMissedAttack();
            Destroy(gameObject);
        }

    }
    
    private void Attack(GameObject target)
    {
        EntityEventDispatcher dispatcher = owner.GetComponent<EntityEventDispatcher>();
        dispatcher?.DispatchDealDamage(dmg, target);

        target.GetComponent<IDamageable>()?.TakeDamage(dmg);
    }
}
