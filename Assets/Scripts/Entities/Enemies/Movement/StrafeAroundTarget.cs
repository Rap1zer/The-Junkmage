using UnityEngine;

namespace JunkMage.Entities.Enemies.Movement
{
    public class StrafeAroundTarget : IMovementBehavior
    {
        private bool isStrafing;
        private Vector2 strafeDir;
        private float strafeEndTime;

        public void UpdateMovement(Rigidbody2D rb, EnemyStats stats, MovementContext ctx)
        {
            Vector2 center = ctx.Target ?? rb.position;
            float strafeSpeed = stats.HasStat(Stat.StrafeSpeed) ? stats.GetVal(Stat.StrafeSpeed) : 4f;
            float strafeDuration = stats.HasStat(Stat.StrafeDuration) ? stats.GetVal(Stat.StrafeDuration) : 2f;

            if (!isStrafing)
            {
                Vector2 dirToCenter = (center - rb.position).normalized;
                Vector2 perpendicular = new Vector2(-dirToCenter.y, dirToCenter.x) * (Random.value > 0.5f ? 1 : -1);

                strafeDir = perpendicular;
                strafeEndTime = Time.time + strafeDuration;
                isStrafing = true;

                rb.linearVelocity = strafeDir * strafeSpeed;
            }

            if (isStrafing && Time.time > strafeEndTime)
                isStrafing = false;
        }

        public void Stop(Rigidbody2D rb, EnemyStats stats)
        {
            rb.linearVelocity = Vector2.zero;
            isStrafing = false;
        }
    }
}