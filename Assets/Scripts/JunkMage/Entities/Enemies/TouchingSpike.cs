using JunkMage.Entities.Enemies;
using UnityEngine;

public class TouchingSpike : MonoBehaviour
{
    private SpikeEnemy spikeEnemy;

    private void Awake()
    {
        spikeEnemy = gameObject.transform.parent.GetComponent<SpikeEnemy>();
    }
}
