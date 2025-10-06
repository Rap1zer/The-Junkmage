using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float dmg;
    private GameObject owner;
    private bool isPLayerBullet;

    public void Initilaise(float dmg, GameObject owner)
    {
        this.dmg = dmg;
        this.owner = owner;
        isPLayerBullet = owner != null && owner.CompareTag("Player");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitEntity = (!isPLayerBullet && other.CompareTag("Player")) ||
                             (isPLayerBullet && other.CompareTag("Enemy"));
        bool hitValidObj = !((isPLayerBullet && other.CompareTag("Player")) ||
                             (!isPLayerBullet && other.CompareTag("Enemy"))) &&
                           !other.CompareTag("Bullet");
        
        if (hitEntity) Attack(other.gameObject);

        // Destroy the bullet after it collides
        if (hitValidObj)
        {
            if (!hitEntity && owner != null) owner.GetComponent<EntityEventDispatcher>().DispatchMissedAttack();
            Destroy(gameObject);
        }

    }
    
    private void Attack(GameObject target)
    {
        bool ownerDestoyed = owner == null;
        if (!ownerDestoyed)
        {
            EntityEventDispatcher dispatcher = owner.GetComponent<EntityEventDispatcher>();
            dispatcher?.DispatchDealDamage(dmg, target);
        }

        target?.GetComponent<IDamageable>()?.TakeDamage(dmg, ownerDestoyed ? null : owner);
    }
}
