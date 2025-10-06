using UnityEngine;

public class SpikeController : EnemyBase
{
    public bool playerInRange = false;

    protected override void DoAttackBehavior()
    {
        // Chase player
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * Speed;

        if (AttackCooled() && playerInRange)
            Attack();
    }

    public override void Attack()
    {
        base.Attack();
        var damageable = player.GetComponent<IDamageable>();
        damageable?.TakeDamage(AttackDmg, gameObject);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
