using UnityEngine;

public class TouchingSpike : MonoBehaviour
{
    private SpikeController spikeController;

    private void Awake()
    {
        spikeController = gameObject.transform.parent.GetComponent<SpikeController>();
    }
}
