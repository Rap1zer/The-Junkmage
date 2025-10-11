using JunkMage.Entities.Enemies;
using UnityEngine;

namespace JunkMage.Entities.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private EnemyStats stats;
    
        private float StrafeSpeed => stats != null ? stats.GetVal(StatType.StrafeSpeed) : 5f;
        private float StrafeDuration => stats != null ? stats.GetVal(StatType.StrafeDuration) : 2f;
    
        private float strafeEndTime;
        private Vector2 strafeDir;
        private bool isStrafing;

        private float MoveSpeed => stats != null ? stats.GetVal(StatType.MoveSpeed) : 5f;
        private float Acceleration => stats != null? stats.GetVal(StatType.Acceleration) : 1f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stats = gameObject.GetComponent<EnemyStats>();
        }

        private void FixedUpdate()
        {
            if (isStrafing && Time.time > strafeEndTime)
                isStrafing = false;
        }

        public void MoveTo(Vector2 target, float arriveThreshold = 1f)
        {
            Vector2 dir = (target - rb.position);
            if (dir.magnitude < arriveThreshold)
            {
                StopImmediate();
                return;
            }

            Vector2 desired = dir.normalized * MoveSpeed;
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desired, Acceleration * Time.deltaTime);
        }

        public void StrafeAround(Vector2 center)
        {
            if (isStrafing) return;

            Vector2 dirToCenter = (center - rb.position).normalized;
            Vector2 perpendicular = new Vector2(-dirToCenter.y, dirToCenter.x) * (Random.value > 0.5f ? 1 : -1);
            strafeDir = perpendicular;
            strafeEndTime = Time.time + StrafeDuration;
            isStrafing = true;

            rb.linearVelocity = strafeDir * StrafeSpeed;
        }

        public void Retreat(Vector2 away)
        {
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, away * MoveSpeed, Acceleration * Time.deltaTime);
        }

        public void StopImmediate()
        {
            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, Acceleration * Time.deltaTime);
        }
    }
}
