public interface IEnemy
{
    public int Health { get; set; }
    public int AttackDmg { get; set; }
    public float AttackCooldown { get; set; }
    void Attack();
    void TakeDamage(int damage);
    void Die();
}
