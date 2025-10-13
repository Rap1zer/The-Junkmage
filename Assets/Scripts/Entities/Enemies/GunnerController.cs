using JunkMage.Entities.Enemies.Movement;
using UnityEngine;

namespace JunkMage.Entities.Enemies
{
    [RequireComponent(typeof(EnemyMovement))]
    public class GunnerController : EnemyBase
    {
        [Header("Shooting")]
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;

        private float AttackRange => Stats != null ? Stats.GetVal(Stat.AttackRange) : 8f;
        private float RetreatRange => Stats != null ? Stats.GetVal(Stat.RetreatRange) : 4f;
        private float DetectionRange => Stats != null ? Stats.GetVal(Stat.DetectionRange) : 20f;

        private float AttackEndMultiplier => Stats != null ? Stats.GetVal(Stat.AttackEndMultiplier) : 1.5f;
        private float RetreatEndMultiplier => Stats != null ? Stats.GetVal(Stat.RetreatEndMultiplier) : 1.4f;
        
        private MoveToTarget moveToBehaviour;
        private StrafeAroundTarget strafeBehavior;
        private Retreat retreatBehavior;

        protected override void Start()
        {
            base.Start();
            Stats = GetComponent<EnemyStats>();
            if (firePoint == null)
                firePoint = transform.Find("Fire Point");
            
            moveToBehaviour = new MoveToTarget();
            strafeBehavior = new StrafeAroundTarget();
            retreatBehavior = new Retreat();
        
            Stats.ApplyModifier(new StatModifier(Stat.AttackRange, Random.Range(-0.1f, 0.1f), ModifierType.PercentAdd));
            Stats.ApplyModifier(new StatModifier(Stat.RetreatRange, Random.Range(-0.1f, 0.1f), ModifierType.PercentAdd));
            Stats.ApplyModifier(new StatModifier(Stat.DetectionRange, Random.Range(-0.1f, 0.1f), ModifierType.PercentAdd));
        }

        protected override void LookForPlayer()
        {
            if (player == null) return;

            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist <= DetectionRange)
                CurrentState = EnemyState.Chasing;
        }

        protected override void DoChaseBehavior()
        {
            if (player == null) return;

            Vector2 toPlayer = player.transform.position - transform.position;
            float dist = toPlayer.magnitude;

            if (dist > DetectionRange)
            {
                CurrentState = EnemyState.Idle;
                return;
            }

            if (dist <= AttackRange)
            {
                CurrentState = EnemyState.Attacking;
                return;
            }
            
            Movement.SetBehavior(moveToBehaviour);
            Movement.Move(new MovementContext {Target = player.transform.position } );
            FacePlayer(toPlayer);
        }

        // Called at every frame when CurrentState = Attacking
        protected override void DoAttackBehavior()
        {
            if (player == null) return;

            Vector2 toPlayer = player.transform.position - transform.position;
            float dist = toPlayer.magnitude;
        
            // Stand still or strafe slightly while attacking
            Movement.SetBehavior(strafeBehavior);
            Movement.Move(new MovementContext {Target = player.transform.position } ); 

            // Smoothly face the player
            FacePlayer(toPlayer);

            if (dist < RetreatRange)
            {
                CurrentState = EnemyState.Fleeing;
                return;
            }

            if (dist > AttackRange * AttackEndMultiplier)
            {
                CurrentState = EnemyState.Chasing;
                return;
            }

            if (AttackCooled()) Attack();
        }

        protected override void DoFleeBehavior()
        {
            if (player == null) return;
            
            Movement.SetBehavior(retreatBehavior);
            Vector2 away = (transform.position - player.transform.position).normalized;
            Movement.Move(new MovementContext {Target = away } ); 

            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist > RetreatRange * RetreatEndMultiplier)
                CurrentState = EnemyState.Chasing;

            FacePlayer(-away);
            if (AttackCooled()) Attack();
        }

        private void FacePlayer(Vector2 toPlayer)
        {
            if (toPlayer == Vector2.zero) return;
            float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg - 90f;
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
        }

        protected override void Attack()
        {
            base.Attack();
            if (bulletPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            var b = bullet.GetComponent<Bullet>();
            if (b != null)
                b.Initilaise(AttackDmg, gameObject);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = -firePoint.up * Stats.GetVal(Stat.BulletSpeed);
        }
    
        private void OnDrawGizmosSelected()
        {
            // Attack Range (Orange) - When the enemy starts attacking
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.75f); // Orange
            Gizmos.DrawWireSphere(transform.position, AttackRange);

            // Retreat Range (Red) - When the enemy starts fleeing
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.75f); // Red
            Gizmos.DrawWireSphere(transform.position, RetreatRange);

            // Detection Range (Yellow) - When the enemy switches from Idle to Chasing
            Gizmos.color = new Color(1f, 1f, 0f, 0.5f); // Yellow (more transparent)
            Gizmos.DrawWireSphere(transform.position, DetectionRange);
        
            // Extended Retreat Range (Light Blue) - The threshold to stop fleeing
            // The fleeing logic uses 'retreatRange * RetreatEndMultiplier'
            Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.6f); // Light Blue
            Gizmos.DrawWireSphere(transform.position, RetreatRange * RetreatEndMultiplier);
        
            // Extended Attack Range (Green) - The threshold to stop attacking and start chasing again
            // The attacking logic uses 'attackRange * attackEndMultiplier'
            Gizmos.color = new Color(0f, 1f, 0f, 0.6f); // Green
            Gizmos.DrawWireSphere(transform.position, AttackRange * AttackEndMultiplier);
        }
    }
}
