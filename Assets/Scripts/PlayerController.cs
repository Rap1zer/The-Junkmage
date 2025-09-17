using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public int inRoomIndex = 0;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isDashing = false;
    private float dashTimeLeft = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;   // assign your ball prefab here
    public float bulletSpeed = 14f;
    public Transform firePoint;       // empty child object at gun/barrel tip

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // --- Handle movement input ---
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // --- Handle dash input ---
        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        // --- Dash timing ---
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
            {
                isDashing = false;
            }
        }

        // --- Cooldown timer ---
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // --- Rotate player to face mouse ---
        RotateToMouse();

        // --- Shooting with left mouse button ---
        if (Input.GetMouseButtonDown(0)) Shoot();
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    void StartDash()
    {
        if (moveInput == Vector2.zero) return; // can't dash without direction

        isDashing = true;
        dashTimeLeft = dashDuration;
        dashCooldownTimer = dashCooldown;

        dashDirection = moveInput; // dash in current movement direction
    }

    void RotateToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // -90 if your sprite faces up
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        Debug.Log("Shooting!");
        if (bulletPrefab == null || firePoint == null) return;

        // Spawn bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().SetDmg(1, true);

        // Apply velocity
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = firePoint.up * bulletSpeed; 
        }
    }
}