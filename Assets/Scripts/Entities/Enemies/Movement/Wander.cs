using UnityEngine;

namespace JunkMage.Entities.Enemies.Movement
{
    public class Wander: IMovementBehavior
    {
        Vector2 wanderTarget;
        float wanderRadius = 6f;
        float wanderSpeedMultiplier = 0.5f;
        float wanderTimer = 0f;
        float wanderInterval = 6f;
        
        public void UpdateMovement(Rigidbody2D rb, EnemyStats stats, MovementContext ctx)
        {
            float speed = stats.HasStat(Stat.MoveSpeed) ? stats.GetVal(Stat.MoveSpeed) : 4f;
            speed = speed * 0.6f;
            
            Transform transform = rb.transform;
            GameObject gameObject = rb.gameObject;
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0f || Vector2.Distance(transform.position, wanderTarget) <= 0.1f)
            {
                PickNewWanderTarget(transform);
                wanderTimer = AddVariation(wanderInterval, 0.3f);
            }

            // Calculate direction toward target
            Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;

            // --- Avoid nearby enemies ---
            Collider2D[] hits = new Collider2D[10]; // preallocate or reuse array
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Enemy"));
            contactFilter.useTriggers = false;
            int hitCount = Physics2D.OverlapCircle(transform.position, 1.5f, contactFilter, hits);
            
            Vector2 avoidance = Vector2.zero;
            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].gameObject == gameObject) continue;
                avoidance += ((Vector2)transform.position - (Vector2)hits[i].transform.position).normalized;
            }
            if (avoidance != Vector2.zero)
                direction = (direction + avoidance.normalized).normalized;

            // Move
            rb.linearVelocity = direction * (speed * wanderSpeedMultiplier);

            // --- Smooth rotation ---
            if (direction != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.deltaTime * 5f); // smooth factor
                transform.rotation = Quaternion.Euler(0f, 0f, smoothAngle);
            }
        }

        public void Stop(Rigidbody2D rb, EnemyStats stats)
        {
            throw new System.NotImplementedException();
        }
        
        private void PickNewWanderTarget(Transform transform)
        {
            LayerMask wallLayer = LayerMask.GetMask("Wall");

            for (int i = 0; i < 10; i++) // try 10 times max
            {
                float r = Mathf.Sqrt(Random.value) * wanderRadius; // bias toward edges
                Vector2 randomOffset = Random.insideUnitCircle.normalized * r;
                Vector2 candidate = (Vector2)transform.position + randomOffset;

                // Cast a ray toward candidate
                Vector2 direction = candidate - (Vector2)transform.position;
                float distance = direction.magnitude;
                direction.Normalize();

                // If ray does NOT hit a wall, the candidate is valid
                if (!Physics2D.Raycast(transform.position, direction, distance, wallLayer))
                {
                    wanderTarget = candidate;
                    return;
                }
            }

            // fallback: stay in place if no valid target found
            wanderTarget = transform.position;
        }
        
        private float AddVariation(float val, float percent) => val + Random.Range(-val * percent, val * percent);
    }
}