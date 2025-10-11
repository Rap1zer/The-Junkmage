using UnityEngine;

namespace JunkMage.Entities.Enemies.Movement
{
    public class Retreat : IMovementBehavior
    {
        public void UpdateMovement(Rigidbody2D rb, EnemyStats stats, MovementContext ctx)
        {
            Vector2 away = ctx.Target ?? rb.position;
            float moveSpeed = stats.HasStat(Stat.MoveSpeed) ? stats.GetVal(Stat.MoveSpeed) : 4f;
            float acceleration = stats.HasStat(Stat.Acceleration) ? stats.GetVal(Stat.Acceleration) : 1f;

            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, away * moveSpeed, acceleration * Time.deltaTime);
        }

        public void Stop(Rigidbody2D rb, EnemyStats stats)
        {
            float acceleration = stats.GetVal(Stat.Acceleration);
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, acceleration * Time.deltaTime);
        }
    }
}