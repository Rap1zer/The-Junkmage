public enum EnemyState
{
    Idle,
    Attacking,
    Dead
}

public interface IEnemy
{
    EnemyStats Stats { get; }
    EnemyState CurrentState { get; }
    void Attack();
    void Die();
}
