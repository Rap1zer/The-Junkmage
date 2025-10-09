public enum EnemyState
{
    Idle,        // Wandering / standing still
    Chasing,     // Moving toward the player
    Attacking,   // Shooting / melee
    Fleeing,     // Running away (low health)
    Investigating // Player was seen or something happened
}


public interface IEnemy
{
    EnemyStats Stats { get; }
    EnemyState CurrentState { get; }
    void Attack();
    void Die();
}
