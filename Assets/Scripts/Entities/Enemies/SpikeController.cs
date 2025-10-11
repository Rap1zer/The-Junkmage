using JunkMage.Entities.Enemies;
using UnityEngine;

namespace JunkMage.Entities.Enemies
{
    public class SpikeController : EnemyBase
    {
        public bool playerInRange = false;

        protected override void DoAttackBehavior()
        {
            // Chase player
            Vector2 direction = (player.transform.position - transform.position).normalized;
            Debug.Log(Stats.HasStat(Stat.MoveSpeed));
            rb.linearVelocity = direction * Stats.GetVal(Stat.MoveSpeed);

            if (AttackCooled() && playerInRange && !playerMovement.IsDashing)
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
}
