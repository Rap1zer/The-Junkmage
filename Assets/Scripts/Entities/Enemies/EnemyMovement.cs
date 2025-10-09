using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour, IMovementController
{
    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float acceleration = 10f;

    [Header("Strafing")]
    public float strafeSpeed = 2f;
    public float strafeRadius = 1.5f;
    public float strafeDuration = 1.2f;

    private Rigidbody2D rb;
    private float strafeEndTime;
    private Vector2 strafeDir;
    private bool isStrafing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isStrafing && Time.time > strafeEndTime)
            isStrafing = false;
    }

    public void MoveTo(Vector2 target, float arriveThreshold = 1f)
    {
        Vector2 dir = (target - rb.position);
        if (dir.magnitude < arriveThreshold)
        {
            StopImmediate();
            return;
        }

        Vector2 desired = dir.normalized * moveSpeed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desired, acceleration * Time.deltaTime);
    }

    public void StrafeAround(Vector2 center)
    {
        if (isStrafing) return;

        Vector2 dirToCenter = (center - rb.position).normalized;
        Vector2 perpendicular = new Vector2(-dirToCenter.y, dirToCenter.x) * (Random.value > 0.5f ? 1 : -1);
        strafeDir = perpendicular;
        strafeEndTime = Time.time + strafeDuration;
        isStrafing = true;

        rb.linearVelocity = strafeDir * strafeSpeed;
    }

    public void Retreat(Vector2 away)
    {
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, away * moveSpeed, acceleration * Time.deltaTime);
    }

    public void StopImmediate()
    {
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, Vector2.zero, acceleration * Time.deltaTime);
    }

    // Interface placeholders (simplified version)
    public void RequestStrafe(Vector2 toPlayer) => StrafeAround(rb.position - toPlayer);
    public void ImmediateStrafe(Vector2 toPlayer) => StrafeAround(rb.position - toPlayer);
    public void UpdateStrafe(Vector2 toPlayer) { }
    public void SetHoldSuppressed(bool suppressed) { }
    public bool IsAtPosition(Vector2 pos, float threshold) => Vector2.Distance(rb.position, pos) <= threshold;
    public bool IsHolding() => false;
}
