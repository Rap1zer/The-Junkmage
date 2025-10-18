using JunkMage.Systems;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float dmg;
    private GameObject owner;
    private bool isPLayerBullet;
    private bool isCrit;

    public void Initialise(DamageInfo dmgInfo)
    {
        this.dmg = dmgInfo.Dmg;
        this.isCrit = dmgInfo.IsCrit;
        this.owner = dmgInfo.Attacker;
        isPLayerBullet = owner != null && owner.CompareTag("Player");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        bool hitEntity = (!isPLayerBullet && other.CompareTag("Player") && !other.GetComponent<PlayerMovement>().IsDashing) ||
                             (isPLayerBullet && other.CompareTag("Enemy"));
        bool hitValidObj = !((isPLayerBullet && other.CompareTag("Player") && !other.GetComponent<PlayerMovement>().IsDashing) ||
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
        
        DamageInfo dmgInfo = new DamageInfo
        {
            Dmg = dmg,
            IsCrit = isCrit,
            Attacker = ownerDestoyed ? null : owner,
            Target = target
        };
        
        if (!ownerDestoyed)
        {
            EntityEventDispatcher dispatcher = owner.GetComponent<EntityEventDispatcher>();
            
            dispatcher?.DispatchDealDamage(dmgInfo);
        }

        target?.GetComponent<IDamageable>()?.TakeDamage(dmgInfo);
    }
}
