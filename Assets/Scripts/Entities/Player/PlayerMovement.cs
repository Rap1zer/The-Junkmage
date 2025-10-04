using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStats stats;
    private Vector2 moveInput;

    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    public void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f) isDashing = false;
        }

        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        RotateToMouse();
    }

    void FixedUpdate()
    {
        if (isDashing)
            rb.linearVelocity = dashDirection * stats.GetVal(StatType.DashSpeed);
        else
            rb.linearVelocity = moveInput * stats.GetVal(StatType.MoveSpeed);
    }

    void StartDash()
    {
        if (moveInput == Vector2.zero) return;

        isDashing = true;
        dashTimeLeft = stats.GetVal(StatType.DashDuration);
        dashCooldownTimer = stats.GetVal(StatType.DashCooldown);
        dashDirection = moveInput;
    }

    void RotateToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
