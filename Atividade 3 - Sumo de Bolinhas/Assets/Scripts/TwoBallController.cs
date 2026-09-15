using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class TwoBallController : MonoBehaviour
{
    // =========================================================
    // MOVIMENTO
    // =========================================================

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 12f;

    [SerializeField]
    private float maxMoveSpeed = 8f;

    // =========================================================
    // PULO
    // =========================================================

    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 7f;

    [SerializeField]
    private float groundCheckExtraDistance = 0.15f;

    // =========================================================
    // SAVE
    // =========================================================

    [Header("Save")]
    [SerializeField]
    private int saveSlot = 0;

    // =========================================================
    // COMPONENTES
    // =========================================================

    private Rigidbody rb;

    private PlayerInput playerInput;

    private PlayerStats stats;

    private Collider playerCollider;

    // =========================================================
    // INPUT ACTIONS
    // =========================================================

    private InputAction moveAction;

    private InputAction jumpAction;

    private Vector2 moveInput;

    // =========================================================
    // ESTADO
    // =========================================================

    private bool isGrounded;

    public bool IsGrounded =>
        isGrounded;

    // =========================================================
    // COMPATIBILIDADE COM SCRIPTS ANTIGOS DO SUMÔ
    // =========================================================

    public bool CanPush =>
        true;

    public float PushCooldownNormalized =>
        1f;

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        rb =
            GetComponent<Rigidbody>();

        playerInput =
            GetComponent<PlayerInput>();

        stats =
            GetComponent<PlayerStats>();

        playerCollider =
            GetComponent<Collider>();

        // Física da bolinha
        rb.useGravity = true;

        rb.isKinematic = false;

        rb.linearDamping = 0.8f;

        rb.angularDamping = 0.2f;

        // =====================================================
        // INPUT SYSTEM
        // =====================================================

        if (
            playerInput != null &&
            playerInput.actions != null
        )
        {
            // Usa especificamente o Action Map "Player"
            InputActionMap playerMap =
                playerInput.actions.FindActionMap(
                    "Player",
                    false
                );

            if (playerMap != null)
            {
                playerMap.Enable();

                moveAction =
                    playerMap.FindAction(
                        "Move",
                        false
                    );

                jumpAction =
                    playerMap.FindAction(
                        "Jump",
                        false
                    );
            }
        }

        // =====================================================
        // DEBUG
        // =====================================================

        if (moveAction == null)
        {
            Debug.LogError(
                "ERRO: ação 'Move' não encontrada no Action Map 'Player'."
            );
        }

        if (jumpAction == null)
        {
            Debug.LogError(
                "ERRO: ação 'Jump' não encontrada no Action Map 'Player'."
            );
        }
    }

    // =========================================================
    // ON ENABLE
    // =========================================================

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
        }

        if (jumpAction != null)
        {
            jumpAction.Enable();
        }
    }

    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        ReadMovement();

        CheckGround();

        ReadJump();
    }

    // =========================================================
    // FIXED UPDATE
    // =========================================================

    private void FixedUpdate()
    {
        MovePlayer();

        LimitSpeed();
    }

    // =========================================================
    // INPUT - MOVIMENTO
    // =========================================================

    private void ReadMovement()
    {
        if (moveAction == null)
        {
            moveInput =
                Vector2.zero;

            return;
        }

        moveInput =
            moveAction.ReadValue<Vector2>();
    }

    // =========================================================
    // MOVIMENTO
    // =========================================================

    private void MovePlayer()
    {
        Vector3 direction =
            new Vector3(
                moveInput.x,
                0f,
                moveInput.y
            );

        if (
            direction.sqrMagnitude
            < 0.01f
        )
        {
            return;
        }

        float speed =
            moveSpeed;

        // Continua compatível com PlayerStats
        if (stats != null)
        {
            speed *=
                stats.SpeedMultiplier;
        }

        rb.AddForce(
            direction.normalized
            * speed,
            ForceMode.Acceleration
        );
    }

    // =========================================================
    // LIMITAR VELOCIDADE
    // =========================================================

    private void LimitSpeed()
    {
        Vector3 velocity =
            rb.linearVelocity;

        Vector3 horizontalVelocity =
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        horizontalVelocity =
            Vector3.ClampMagnitude(
                horizontalVelocity,
                maxMoveSpeed
            );

        rb.linearVelocity =
            new Vector3(
                horizontalVelocity.x,
                velocity.y,
                horizontalVelocity.z
            );
    }

    // =========================================================
    // INPUT - PULO
    // =========================================================

    private void ReadJump()
    {
        if (jumpAction == null)
        {
            return;
        }

        if (
            jumpAction.WasPressedThisFrame()
        )
        {
            Jump();
        }
    }

    // =========================================================
    // PULO
    // =========================================================

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }

        Vector3 velocity =
            rb.linearVelocity;

        velocity.y = 0f;

        rb.linearVelocity =
            velocity;

        rb.AddForce(
            Vector3.up
            * jumpForce,
            ForceMode.Impulse
        );

        isGrounded = false;
    }

    // =========================================================
    // DETECTAR CHÃO
    // =========================================================

    private void CheckGround()
    {
        if (playerCollider == null)
        {
            isGrounded = false;

            return;
        }

        float distance =
            playerCollider.bounds.extents.y
            + groundCheckExtraDistance;

        isGrounded =
            Physics.Raycast(
                transform.position,
                Vector3.down,
                distance,
                ~0,
                QueryTriggerInteraction.Ignore
            );
    }

    // =========================================================
    // EVENTOS DO INPUT SYSTEM
    // =========================================================

    /*
     * Esses métodos continuam existindo caso
     * algum objeto antigo ainda esteja conectado
     * via Invoke Unity Events.
     */

    public void OnMove(
        InputAction.CallbackContext context
    )
    {
        moveInput =
            context.ReadValue<Vector2>();
    }

    public void OnJump(
        InputAction.CallbackContext context
    )
    {
        if (!context.performed)
        {
            return;
        }

        Jump();
    }

    // =========================================================
    // PUSH ANTIGO
    // =========================================================

    public void OnPush(
        InputAction.CallbackContext context
    )
    {
        // Não utilizado nesta atividade.
    }

    // =========================================================
    // CONFIGURAÇÃO ANTIGA
    // =========================================================

    public void Configure(
        float speed,
        float push,
        float maxPush
    )
    {
        moveSpeed =
            speed;
    }

    // =========================================================
    // SAVE
    // =========================================================

    public void SaveCurrentProgress()
    {
        if (
            SaveManager.Instance == null
        )
        {
            Debug.LogWarning(
                "SaveManager não encontrado."
            );

            return;
        }

        SaveData save =
            new SaveData();

        save.sceneName =
            SceneManager
                .GetActiveScene()
                .name;

        save.playerPosition =
            transform.position;

        HUDController hud =
            FindFirstObjectByType<HUDController>();

        if (hud != null)
        {
            save.coins =
                hud.Coins;
        }

        SaveManager.Instance.SaveToSlot(
            saveSlot,
            save
        );

        Debug.Log(
            "Progresso salvo no slot "
            + saveSlot
        );
    }

    // =========================================================
    // LOAD
    // =========================================================

    public void LoadProgress(
        int slot = -1
    )
    {
        if (
            SaveManager.Instance == null
        )
        {
            return;
        }

        int slotToUse =
            slot < 0
                ? saveSlot
                : slot;

        SaveData data =
            SaveManager.Instance.LoadFromSlot(
                slotToUse
            );

        if (data == null)
        {
            return;
        }

        // Se estiver em outra cena
        if (
            !string.IsNullOrEmpty(
                data.sceneName
            )
            &&
            data.sceneName
            != SceneManager
                .GetActiveScene()
                .name
        )
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance
                    .ForceSceneChange(
                        data.sceneName
                    );
            }

            return;
        }

        // Para a física antes de mover
        rb.linearVelocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        rb.position =
            data.playerPosition;

        HUDController hud =
            FindFirstObjectByType<HUDController>();

        if (hud != null)
        {
            hud.SetCoins(
                data.coins
            );
        }
    }

    // =========================================================
    // SAIR DO JOGO
    // =========================================================

    private void OnApplicationQuit()
    {
        SaveCurrentProgress();
    }

    // =========================================================
    // DEBUG DO CHÃO
    // =========================================================

    private void OnDrawGizmosSelected()
    {
        Collider col =
            GetComponent<Collider>();

        if (col == null)
        {
            return;
        }

        float distance =
            col.bounds.extents.y
            + groundCheckExtraDistance;

        Gizmos.DrawLine(
            transform.position,
            transform.position
            + Vector3.down
            * distance
        );
    }
}