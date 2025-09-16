using UnityEngine;

public class SpikeController : MonoBehaviour, IEnemy
{
    public int RoomIndex { get; private set; }
    public int Health { get; set; } = 20;
    public int AttackDmg { get; set; } = 3;
    public float AttackCooldown { get; set; } = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
