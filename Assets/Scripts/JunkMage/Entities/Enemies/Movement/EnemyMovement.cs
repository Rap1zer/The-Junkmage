using UnityEngine;

namespace JunkMage.Entities.Enemies.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyMovement : MonoBehaviour
    {
        private Rigidbody2D rb;
        private EnemyStats stats;

        public IMovementBehavior CurrentBehavior { get; private set; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            stats = GetComponent<EnemyStats>();
        }

        private void FixedUpdate()
        {
            // Could optionally call CurrentBehavior.UpdateMovement automatically
        }

        public void SetBehavior(IMovementBehavior behavior)
        {
            CurrentBehavior = behavior;
        }

        public void Move(MovementContext ctx = new())
        {
            CurrentBehavior?.UpdateMovement(rb, stats, ctx);
        }

        public void Stop()
        {
            CurrentBehavior?.Stop(rb, stats);
        }
    }
}