using UnityEngine;

namespace JunkMage.Entities.Enemies.Movement
{
    public class MoveToTarget : IMovementBehavior
    {
        public void UpdateMovement(Rigidbody2D rb, EnemyStats stats, MovementContext ctx)
        {
            Vector2 target = ctx.Target ?? rb.position;
            float moveSpeed = stats.GetVal(Stat.MoveSpeed);
            float acceleration = stats.GetVal(Stat.Acceleration);
            
            Vector2 dir = target - rb.position;
            if (dir.magnitude < 0.1f) return;

            Vector2 desired = dir.normalized * moveSpeed;
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desired, acceleration * Time.deltaTime);
        }

        public void Stop(Rigidbody2D rb, EnemyStats stats)
        {
            float acceleration = stats.GetVal(Stat.Acceleration);
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, acceleration * Time.deltaTime);
        }
    }
}