using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int dmg = 1;
    private bool isPlayerBullet = false;

    public void SetDmg(int dmg, bool isPlayerBullet)
    {
        this.dmg = dmg;
        this.isPlayerBullet = isPlayerBullet;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPlayerBullet && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
        }
        else if (isPlayerBullet && other.CompareTag("Enemy"))
        {
            IEnemy enemy = other.GetComponent<IEnemy>();
            if (enemy != null) enemy.TakeDamage(1);
        }
    }
}
