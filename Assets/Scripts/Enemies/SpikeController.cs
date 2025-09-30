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
        var damageable = player.GetComponent<IDamageable>();
        damageable?.TakeDamage(AttackDmg);
    }
}
