using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class TwoBallController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;

    [Header("Action")]
    [SerializeField] private float basePushForce = 3f;
    [SerializeField] private float maxPushForce = 7f;
    [SerializeField] private float pushRange = 6f;
    [SerializeField] private float maxKnockbackSpeed = 8f;

    [Header("Push Cooldown")]
    [SerializeField] private float pushCooldown = 3f;

    private float pushCooldownTimer = 0f;

    private Rigidbody rb;
    private PlayerStats stats;

    private Vector2 moveInput;

    public float PushCooldownNormalized
    {
        get
        {
            if (pushCooldown <= 0f)
                return 1f;

            return 1f -
                (pushCooldownTimer / pushCooldown);
        }
    }

    public bool CanPush =>
        pushCooldownTimer <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb =
                gameObject.AddComponent<Rigidbody>();
        }

        rb.linearDamping = 0.8f;
        rb.angularDamping = 0.2f;

        stats =
            GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (pushCooldownTimer > 0f)
        {
            pushCooldownTimer -=
                Time.deltaTime;

            if (pushCooldownTimer < 0f)
            {
                pushCooldownTimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 moveDirection =
            new Vector3(
                moveInput.x,
                0f,
                moveInput.y
            );

        if (
            moveDirection.sqrMagnitude >
            0.01f
        )
        {
            float speed =
                moveSpeed;

            if (stats != null)
            {
                speed *=
                    stats.SpeedMultiplier;
            }

            rb.AddForce(
                moveDirection.normalized *
                speed,
                ForceMode.Acceleration
            );
        }
    }

    // ========================================
    // INPUT EVENTS
    // ========================================

    public void OnMove(
        InputAction.CallbackContext context)
    {
        moveInput =
            context.ReadValue<Vector2>();
    }

    public void OnPush(
        InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        ApplyPush();
    }

    // ========================================
    // EMPURRÃO
    // ========================================

    private void ApplyPush()
    {
        if (!CanPush)
            return;

        TwoBallController[] players =
            FindObjectsOfType<TwoBallController>();

        if (
            players == null ||
            players.Length < 2
        )
            return;

        TwoBallController enemy = null;

        foreach (
            TwoBallController candidate
            in players
        )
        {
            if (
                candidate != null &&
                candidate != this
            )
            {
                enemy = candidate;
                break;
            }
        }

        if (enemy == null)
            return;

        Rigidbody enemyRb =
            enemy.GetComponent<Rigidbody>();

        if (enemyRb == null)
            return;

        Vector3 offset =
            enemy.transform.position -
            transform.position;

        offset.y = 0f;

        float distance =
            offset.magnitude;

        if (distance < 0.01f)
            return;

        Vector3 direction =
            offset.normalized;

        float proximity =
            1f -
            Mathf.Clamp01(
                distance / pushRange
            );

        float pushStrength =
            Mathf.Lerp(
                basePushForce,
                maxPushForce,
                proximity
            );

        if (stats != null)
        {
            float forceMultiplier =
                Mathf.Clamp(
                    stats.ForceMultiplier,
                    1f,
                    1.5f
                );

            pushStrength *=
                forceMultiplier;
        }

        PlayerStats enemyStats =
            enemy.GetComponent<PlayerStats>();

        if (enemyStats != null)
        {
            float resistance =
                Mathf.Clamp(
                    enemyStats.ResistanceMultiplier,
                    1f,
                    2.5f
                );

            pushStrength /=
                resistance;
        }

        enemyRb.AddForce(
            direction * pushStrength,
            ForceMode.VelocityChange
        );

        Vector3 velocity =
            enemyRb.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        horizontalVelocity =
            Vector3.ClampMagnitude(
                horizontalVelocity,
                maxKnockbackSpeed
            );

        enemyRb.linearVelocity =
            new Vector3(
                horizontalVelocity.x,
                velocity.y,
                horizontalVelocity.z
            );

        pushCooldownTimer =
            pushCooldown;
    }

    public void Configure(
        float speed,
        float push,
        float maxPush)
    {
        moveSpeed =
            speed;

        basePushForce =
            Mathf.Clamp(
                push / 150f,
                2f,
                5f
            );

        maxPushForce =
            Mathf.Clamp(
                maxPush / 200f,
                5f,
                9f
            );
    }
}