using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to follow")]
    public Transform target;
    private Vector3 offset = new Vector3(0, 0, -10f);

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 targetPosition = target.position + offset;
        transform.position = targetPosition;
    }
}
