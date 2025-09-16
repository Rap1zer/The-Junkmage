using UnityEngine;

public class SpikeController : MonoBehaviour, IEnemy
{
    public int roomIndex;
    private PlayerController player;
    public int Health { get; set; } = 20;
    public int AttackDmg { get; set; } = 3;
    public float AttackCooldown { get; set; } = 0.5f;
    private float lastAttackTime = -Mathf.Infinity;
    public bool playerInRange = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (AttackCooled() && playerInRange)
        {
            Attack();
            Debug.Log("I'm attacking!!!");
        }
    }

    private bool AttackCooled()
    {
        return Time.time >= lastAttackTime + AttackCooldown;
    }

    public void Attack()
    {
        lastAttackTime = Time.time;
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
