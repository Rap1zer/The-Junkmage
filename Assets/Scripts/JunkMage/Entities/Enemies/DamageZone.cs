using JunkMage.Entities.Enemies;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public MortarEnemy Owner { get; set; }
    
    public AnimationCurve scaleCurve;
    
    private float timer = 0f;
    private readonly float duration = 2f;
    
    private Vector3 startScale = new Vector3(1f, 1f, 1f);
    private Vector3 targetScale = new Vector3(3f, 3f, 1f);

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, scaleCurve.Evaluate(t));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Owner.OnHit();
        }
    }
}
