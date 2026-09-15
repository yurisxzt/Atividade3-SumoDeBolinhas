using System.Collections.Generic;
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

    [Header("Respawn")]
    [SerializeField]
    private Transform startPoint;

    private Vector3 initialSpawnPoint;

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
        initialSpawnPoint = transform.position;

        if (startPoint == null)
        {
            startPoint = GameObject.Find("StartPoint")?.transform;
            if (startPoint == null)
            {
                startPoint = GameObject.Find("SpawnPoint")?.transform;
            }
        }

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
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("SaveManager não encontrado.");
            return;
        }

        SaveData existingSave = SaveManager.Instance.LoadFromSlot(0);

        HUDController hud = FindFirstObjectByType<HUDController>();
        SaveData save = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = transform.position,
            checkpointPosition = existingSave != null && existingSave.checkpointPassed ? existingSave.checkpointPosition : transform.position,
            checkpointPassed = existingSave != null && existingSave.checkpointPassed,
            activeCheckpointId = existingSave != null ? existingSave.activeCheckpointId : "",
            coins = hud != null ? hud.Coins : 0,
            collectedCoinIds = new List<string>(SaveManager.Instance.CurrentCollectedCoins)
        };

        SaveManager.Instance.SaveToSlot(0, save);

        if (saveSlot != 0)
        {
            SaveManager.Instance.SaveToSlot(saveSlot, save);
        }

        Debug.Log("Progresso salvo no autosave e slot " + saveSlot);
    }

    public void SaveCheckpointProgress(string checkpointId, Vector3 checkpointPosition)
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        HUDController hud = FindFirstObjectByType<HUDController>();
        SaveData save = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            playerPosition = checkpointPosition,
            checkpointPosition = checkpointPosition,
            checkpointPassed = true,
            activeCheckpointId = checkpointId,
            coins = hud != null ? hud.Coins : 0,
            collectedCoinIds = new List<string>(SaveManager.Instance.CurrentCollectedCoins)
        };

        SaveManager.Instance.SaveToSlot(0, save);
    }

    // =========================================================
    // LOAD
    // =========================================================

    public void LoadProgress(int slot = -1)
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        int slotToUse = slot < 0 ? saveSlot : slot;
        SaveData data = SaveManager.Instance.LoadFromSlot(slotToUse);

        if (data == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(data.sceneName) && data.sceneName != SceneManager.GetActiveScene().name)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ForceSceneChange(data.sceneName);
            }

            return;
        }

        ApplyLoadedData(data);
    }

    public void ApplyLoadedData(SaveData data)
    {
        if (data == null)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 spawnPosition = data.checkpointPassed ? data.checkpointPosition : data.playerPosition;
        rb.position = spawnPosition;
        transform.position = spawnPosition;

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SetCurrentCollectedCoins(data.collectedCoinIds);
        }

        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            hud.SetCoins(data.coins);
        }

        PlayerStats playerStats = GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.SetCoins(data.coins);
        }

        foreach (Coin coin in FindObjectsByType<Coin>(FindObjectsSortMode.None))
        {
            if (coin == null)
            {
                continue;
            }

            bool coinAlreadyCollected = SaveManager.Instance != null && SaveManager.Instance.IsCoinCollected(coin.CoinId);
            coin.gameObject.SetActive(!coinAlreadyCollected);
        }
    }

    public Vector3 GetRespawnPosition()
    {
        if (SaveManager.Instance != null)
        {
            SaveData data = SaveManager.Instance.LoadFromSlot(0);
            if (data != null && data.checkpointPassed && data.sceneName == SceneManager.GetActiveScene().name)
            {
                return data.checkpointPosition;
            }
        }

        if (startPoint != null)
        {
            return startPoint.position;
        }

        return initialSpawnPoint;
    }

    public void RespawnToCheckpointOrStart()
    {
        Vector3 respawnPosition = GetRespawnPosition();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = respawnPosition;
        }

        transform.position = respawnPosition;
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