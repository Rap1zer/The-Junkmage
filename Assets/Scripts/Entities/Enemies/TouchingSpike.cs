using UnityEngine;

public class TouchingSpike : MonoBehaviour
{
    private SpikeController spikeController;

    private void Awake()
    {
        spikeController = gameObject.transform.parent.GetComponent<SpikeController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spikeController.playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            spikeController.playerInRange = false;
        }
    }
}
