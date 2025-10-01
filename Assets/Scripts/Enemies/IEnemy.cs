public enum EnemyState
{
    Idle,
    Attacking,
    Dead
}

public interface IEnemy
{
    public float Health { get; }
    public int AttackDmg { get; set; }
    public float AttackCooldown { get; set; }
    public float Speed { get; set; }
    EnemyState CurrentState { get; }
    void Attack();
    void Die();
}
