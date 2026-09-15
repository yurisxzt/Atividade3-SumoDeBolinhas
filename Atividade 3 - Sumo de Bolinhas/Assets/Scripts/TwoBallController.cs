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
    // RESPAWN
    // =========================================================

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
    // INPUT
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
    // COMPATIBILIDADE COM O SUMÔ
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
        initialSpawnPoint =
            transform.position;

        if (startPoint == null)
        {
            GameObject startObject =
                GameObject.Find(
                    "StartPoint"
                );

            if (startObject == null)
            {
                startObject =
                    GameObject.Find(
                        "SpawnPoint"
                    );
            }

            if (startObject != null)
            {
                startPoint =
                    startObject.transform;
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

        rb.useGravity =
            true;

        rb.isKinematic =
            false;

        rb.linearDamping =
            0.8f;

        rb.angularDamping =
            0.2f;

        SetupInput();
    }

    // =========================================================
    // INPUT SETUP
    // =========================================================

    private void SetupInput()
    {
        if (
            playerInput == null ||
            playerInput.actions == null
        )
        {
            return;
        }

        InputActionMap playerMap =
            playerInput.actions
                .FindActionMap(
                    "Player",
                    false
                );

        if (playerMap == null)
        {
            Debug.LogError(
                "Action Map 'Player' não encontrado."
            );

            return;
        }

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

        if (moveAction == null)
        {
            Debug.LogError(
                "Ação 'Move' não encontrada."
            );
        }

        if (jumpAction == null)
        {
            Debug.LogError(
                "Ação 'Jump' não encontrada."
            );
        }
    }

    // =========================================================
    // ENABLE / DISABLE
    // =========================================================

    private void OnEnable()
    {
        moveAction?.Enable();

        jumpAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();

        jumpAction?.Disable();
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

    private void FixedUpdate()
    {
        MovePlayer();

        LimitSpeed();
    }

    // =========================================================
    // MOVIMENTO
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

    private void MovePlayer()
    {
        Vector3 direction =
            new Vector3(
                moveInput.x,
                0f,
                moveInput.y
            );

        if (
            direction.sqrMagnitude <
            0.01f
        )
        {
            return;
        }

        float speed =
            moveSpeed;

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
    // PULO
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

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }

        Vector3 velocity =
            rb.linearVelocity;

        velocity.y =
            0f;

        rb.linearVelocity =
            velocity;

        rb.AddForce(
            Vector3.up
            * jumpForce,
            ForceMode.Impulse
        );

        isGrounded =
            false;
    }

    private void CheckGround()
    {
        if (playerCollider == null)
        {
            isGrounded =
                false;

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
    // EVENTOS INPUT SYSTEM
    // =========================================================

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

    public void OnPush(
        InputAction.CallbackContext context
    )
    {
        // Não usado nesta atividade.
    }

    // =========================================================
    // COMPATIBILIDADE
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
    // CRIAR SAVE DO INÍCIO
    // =========================================================

    private SaveData CreateStartSave()
    {
        Vector3 startPosition =
            startPoint != null
                ? startPoint.position
                : initialSpawnPoint;

        return new SaveData
        {
            sceneName =
                SceneManager
                    .GetActiveScene()
                    .name,

            playerPosition =
                startPosition,

            checkpointPosition =
                Vector3.zero,

            checkpointPassed =
                false,

            hasReachedCheckpoint =
                false,

            activeCheckpointId =
                "",

            coins =
                0,

            collectedCoinIds =
                new List<string>()
        };
    }

    // =========================================================
    // ESTADO QUE PODE SER SALVO MANUALMENTE
    // =========================================================

    /*
     * Se já passou no checkpoint,
     * salva EXATAMENTE o estado do checkpoint.
     *
     * Se ainda não passou,
     * salva o início da fase.
     */

    public SaveData BuildResumeSaveData()
    {
        if (SaveManager.Instance != null)
        {
            SaveData autosave =
                SaveManager.Instance
                    .LoadFromSlot(
                        SaveManager.AutosaveSlot
                    );

            if (
                autosave != null &&
                autosave.sceneName ==
                SceneManager
                    .GetActiveScene()
                    .name &&
                autosave.checkpointPassed
            )
            {
                return autosave.Clone();
            }
        }

        return CreateStartSave();
    }

    // =========================================================
    // AUTOSAVE
    // =========================================================

    public void SaveCurrentProgress()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogWarning(
                "SaveManager não encontrado."
            );

            return;
        }

        SaveData save =
            BuildResumeSaveData();

        SaveManager.Instance
            .SaveToSlot(
                SaveManager.AutosaveSlot,
                save
            );

        Debug.Log(
            "Autosave atualizado."
        );
    }

    // =========================================================
    // SAVE MANUAL
    // =========================================================

    public void SaveManualProgress(
        int slot
    )
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        if (
            slot <
            SaveManager.FirstManualSlot ||
            slot >
            SaveManager.LastManualSlot
        )
        {
            Debug.LogWarning(
                "Slot manual inválido: "
                + slot
            );

            return;
        }

        SaveData save =
            BuildResumeSaveData();

        SaveManager.Instance
            .SaveToSlot(
                slot,
                save
            );

        Debug.Log(
            "Save manual realizado no slot "
            + slot
        );
    }

    // =========================================================
    // CHECKPOINT
    // =========================================================

    public void SaveCheckpointProgress(
        string checkpointId,
        Vector3 checkpointPosition
    )
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        int currentCoins =
            stats != null
                ? stats.Coins
                : 0;

        SaveData save =
            new SaveData
            {
                sceneName =
                    SceneManager
                        .GetActiveScene()
                        .name,

                playerPosition =
                    checkpointPosition,

                checkpointPosition =
                    checkpointPosition,

                checkpointPassed =
                    true,

                hasReachedCheckpoint =
                    true,

                activeCheckpointId =
                    checkpointId,

                coins =
                    currentCoins,

                collectedCoinIds =
                    new List<string>(
                        SaveManager.Instance
                            .CurrentCollectedCoins
                    )
            };

        SaveManager.Instance
            .SaveToSlot(
                SaveManager.AutosaveSlot,
                save
            );

        Debug.Log(
            "Checkpoint salvo. Moedas: "
            + currentCoins
        );
    }

    // =========================================================
    // LOAD
    // =========================================================

    public void LoadProgress(
        int slot = 0
    )
    {
        if (
            SaveRestoreManager.Instance !=
            null
        )
        {
            SaveRestoreManager.Instance
                .RequestLoadSlot(
                    slot
                );

            return;
        }

        Debug.LogWarning(
            "SaveRestoreManager não encontrado."
        );
    }

    // =========================================================
    // APLICAR SAVE
    // =========================================================

    public void ApplyLoadedData(
        SaveData data
    )
    {
        if (data == null)
        {
            return;
        }

        rb.linearVelocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;

        Vector3 spawnPosition;

        if (data.checkpointPassed)
        {
            spawnPosition =
                data.checkpointPosition;
        }
        else if (startPoint != null)
        {
            spawnPosition =
                startPoint.position;
        }
        else
        {
            spawnPosition =
                initialSpawnPoint;
        }

        rb.position =
            spawnPosition;

        transform.position =
            spawnPosition;

        Physics.SyncTransforms();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance
                .SetCurrentCollectedCoins(
                    data.collectedCoinIds
                );
        }

        if (stats != null)
        {
            stats.SetCoins(
                data.coins
            );
        }

        HUDController hud =
            FindFirstObjectByType<HUDController>();

        if (hud != null)
        {
            hud.SetCoins(
                data.coins
            );
        }

        RefreshCoins();
    }

    // =========================================================
    // RESPAWN
    // =========================================================

    public Vector3 GetRespawnPosition()
    {
        if (SaveManager.Instance != null)
        {
            SaveData autosave =
                SaveManager.Instance
                    .LoadFromSlot(
                        SaveManager.AutosaveSlot
                    );

            if (
                autosave != null &&
                autosave.checkpointPassed &&
                autosave.sceneName ==
                SceneManager
                    .GetActiveScene()
                    .name
            )
            {
                return autosave
                    .checkpointPosition;
            }
        }

        if (startPoint != null)
        {
            return startPoint.position;
        }

        return initialSpawnPoint;
    }

    // =========================================================
    // MORTE
    // =========================================================

    public void RespawnToCheckpointOrStart()
    {
        if (SaveManager.Instance != null)
        {
            SaveData autosave =
                SaveManager.Instance
                    .LoadFromSlot(
                        SaveManager.AutosaveSlot
                    );

            if (
                autosave != null &&
                autosave.sceneName ==
                SceneManager
                    .GetActiveScene()
                    .name &&
                autosave.checkpointPassed
            )
            {
                // Volta EXATAMENTE ao estado do checkpoint
                ApplyLoadedData(
                    autosave
                );

                return;
            }
        }

        // Não passou no checkpoint:
        // volta ao início com 0 moedas.
        SaveData startSave =
            CreateStartSave();

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance
                .ClearCollectedCoins();
        }

        ApplyLoadedData(
            startSave
        );
    }

    // =========================================================
    // REATIVAR / DESATIVAR MOEDAS
    // =========================================================

    private void RefreshCoins()
    {
        Coin[] coins =
            FindObjectsByType<Coin>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (
            Coin coin
            in coins
        )
        {
            if (coin != null)
            {
                coin.RefreshFromCurrentSaveState();
            }
        }
    }

    // =========================================================
    // SAIR
    // =========================================================

    private void OnApplicationQuit()
    {
        /*
         * Mantém um autosave válido.
         *
         * Se houver checkpoint, mantém o checkpoint.
         * Caso contrário, salva o início da fase.
         */

        SaveCurrentProgress();
    }

    // =========================================================
    // DEBUG
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