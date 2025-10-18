using JunkMage.Stats;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerStats stats;
    private Vector2 moveInput;
    private SpriteRenderer sprite;

    public bool IsDashing { get; private set; } = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;
    public float DashCooldownRemaining => dashCooldownTimer;

    private int originalLayer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void HandleInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        if (IsDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f) EndDash();
        }

        if (dashCooldownTimer > 0f) dashCooldownTimer -= Time.deltaTime;

        RotateToMouse();
    }

    void FixedUpdate()
    {
        if (IsDashing)
            rb.linearVelocity = dashDirection * stats.GetVal(Stat.DashSpeed);
        else
            rb.linearVelocity = moveInput * stats.GetVal(Stat.MoveSpeed);
    }
    
    void StartDash()
    {
        if (moveInput == Vector2.zero) return;

        IsDashing = true;
        dashTimeLeft = stats.GetVal(Stat.DashDuration);
        dashCooldownTimer = stats.GetVal(Stat.DashCooldown);
        dashDirection = moveInput;

        // Make player semi-transparent
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = 0.5f; // 50% transparent
            sprite.color = c;
        }
        
        // Switch to PlayerDash layer (ignore collisions with certain layers)
        originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("PlayerDash");
    }

    void EndDash()
    {
        IsDashing = false;

        // Restore layer
        gameObject.layer = originalLayer;

        // Restore opacity
        if (sprite != null)
        {
            Color c = sprite.color;
            c.a = 1f;
            sprite.color = c;
        }
    }

    void RotateToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
