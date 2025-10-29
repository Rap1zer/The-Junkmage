using JunkMage.Entities.Enemies;
using JunkMage.Entities.Enemies.Movement;
using JunkMage.Systems;
using UnityEngine;

namespace JunkMage.Entities.Enemies
{
    public class SpikeEnemy : EnemyBase
    {
        public bool playerInRange = false;
        
        private MoveToTarget moveToTarget;

        protected override void Awake()
        {
            base.Awake();
            moveToTarget = new MoveToTarget();
        }

        protected override void DoAttackBehavior()
        {
            // Chase player
            Movement.SetBehavior(moveToTarget);
            Movement.Move(new MovementContext {Target = player.transform.position});

            if (AttackCooled() && playerInRange && !playerMovement.IsDashing)
                Attack();
        }

        protected override void Attack()
        {
            base.Attack();

            DamageInfo dmgInfo = new DamageInfo
            {
                Dmg = AttackDmg,
                Attacker = gameObject,
                Target = player
            };
            
            playerHealth?.TakeDamage(dmgInfo);
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
