using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float dmg = 1f;
    private bool isPlayerBullet = false;

    public void SetDmg(float dmg, bool isPlayerBullet)
    {
        this.dmg = dmg;
        this.isPlayerBullet = isPlayerBullet;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((!isPlayerBullet && other.CompareTag("Player")) || (isPlayerBullet && other.CompareTag("Enemy")))
        {
            Attack(other.gameObject);
            Destroy(this, 0.1f);
        }
    }
    
    private void Attack(GameObject target)
    {
        StatusEffectManager statusManager = target.GetComponent<StatusEffectManager>();
        if (statusManager != null) dmg = statusManager.DispatchIncomingDamage(dmg);

        target.GetComponent<IDamageable>().TakeDamage(dmg);
    }
}
