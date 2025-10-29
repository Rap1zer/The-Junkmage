using UnityEngine;

namespace JunkMage.Entities.Enemies.Movement
{
    public struct MovementContext
    {
        public Vector2? Target { get; set; }
    }
    
    public interface IMovementBehavior
    {
        void UpdateMovement(Rigidbody2D rb, EnemyStats stats, MovementContext ctx);
        void Stop(Rigidbody2D rb, EnemyStats stats);
    }
}