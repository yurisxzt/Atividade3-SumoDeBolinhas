using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class TwoBallController : MonoBehaviour
{
    public enum ControlScheme
    {
        Wasd,
        Arrows
    }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private ControlScheme controlScheme = ControlScheme.Wasd;

    [Header("Action")]
    [SerializeField] private float basePushForce = 800f;
    [SerializeField] private float maxPushForce = 1800f;
    [SerializeField] private KeyCode actionKey = KeyCode.Space;

    [Header("Push Cooldown")]
    [SerializeField] private float pushCooldown = 3f;

    private float pushCooldownTimer = 0f;

    private Rigidbody rb;
    private PlayerStats stats;

    // Retorna de 0 a 1 para controlar a barra da interface.
    // 0 = acabou de usar o empurrão
    // 1 = empurrão pronto
    public float PushCooldownNormalized
    {
        get
        {
            if (pushCooldown <= 0f)
                return 1f;

            return 1f - (pushCooldownTimer / pushCooldown);
        }
    }

    // Informa se o jogador pode empurrar.
    public bool CanPush => pushCooldownTimer <= 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.linearDamping = 0.8f;
        rb.angularDamping = 0.2f;

        stats = GetComponent<PlayerStats>();
    }

    private void FixedUpdate()
    {
        Vector2 movement = ReadMovementInput();

        Vector3 moveDirection =
            new Vector3(movement.x, 0f, movement.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            float speed = moveSpeed;

            if (stats != null)
            {
                speed *= stats.SpeedMultiplier;
            }

            rb.AddForce(
                moveDirection.normalized * speed,
                ForceMode.Acceleration
            );
        }
    }

    private void Update()
    {
        // Conta o tempo do cooldown
        if (pushCooldownTimer > 0f)
        {
            pushCooldownTimer -= Time.deltaTime;

            if (pushCooldownTimer < 0f)
            {
                pushCooldownTimer = 0f;
            }
        }

        // Verifica se o jogador apertou o botão de ação
        if (IsKeyPressed(actionKey, true))
        {
            ApplyPush();
        }
    }

    private Vector2 ReadMovementInput()
    {
        if (controlScheme == ControlScheme.Arrows)
        {
            float horizontal = 0f;

            if (IsKeyHeld(KeyCode.RightArrow))
                horizontal += 1f;

            if (IsKeyHeld(KeyCode.LeftArrow))
                horizontal -= 1f;

            float vertical = 0f;

            if (IsKeyHeld(KeyCode.UpArrow))
                vertical += 1f;

            if (IsKeyHeld(KeyCode.DownArrow))
                vertical -= 1f;

            return new Vector2(horizontal, vertical);
        }

        float horizontalWASD = 0f;

        if (IsKeyHeld(KeyCode.D))
            horizontalWASD += 1f;

        if (IsKeyHeld(KeyCode.A))
            horizontalWASD -= 1f;

        float verticalWASD = 0f;

        if (IsKeyHeld(KeyCode.W))
            verticalWASD += 1f;

        if (IsKeyHeld(KeyCode.S))
            verticalWASD -= 1f;

        return new Vector2(
            horizontalWASD,
            verticalWASD
        );
    }

    private bool IsKeyHeld(KeyCode key)
    {
        return IsKeyPressed(key, false);
    }

    private bool IsKeyPressed(
        KeyCode key,
        bool usePressedThisFrame)
    {
        var keyboard = Keyboard.current;

        if (keyboard == null)
            return false;

        var inputKey = ToInputSystemKey(key);

        var control = keyboard[inputKey];

        return usePressedThisFrame
            ? control.wasPressedThisFrame
            : control.isPressed;
    }

    private static Key ToInputSystemKey(KeyCode key)
    {
        return key switch
        {
            KeyCode.W => Key.W,
            KeyCode.A => Key.A,
            KeyCode.S => Key.S,
            KeyCode.D => Key.D,

            KeyCode.UpArrow => Key.UpArrow,
            KeyCode.DownArrow => Key.DownArrow,
            KeyCode.LeftArrow => Key.LeftArrow,
            KeyCode.RightArrow => Key.RightArrow,

            KeyCode.Space => Key.Space,

            KeyCode.LeftShift => Key.LeftShift,
            KeyCode.RightShift => Key.RightShift,

            _ => Key.None
        };
    }

    // =========================================================
    // EMPURRÃO
    // =========================================================

    private void ApplyPush()
    {
        // Se ainda estiver no cooldown, não faz nada.
        if (!CanPush)
            return;

        // Procura todas as outras bolinhas existentes na cena.
        var others = FindObjectsOfType<TwoBallController>();

        if (others == null || others.Length < 2)
            return;

        TwoBallController enemy = null;

        foreach (var candidate in others)
        {
            if (candidate == null || candidate == this)
                continue;

            enemy = candidate;
            break;
        }

        if (enemy == null)
            return;

        // Direção da minha bolinha para a bolinha inimiga.
        Vector3 direction =
            (enemy.transform.position - transform.position).normalized;

        // Distância entre as duas bolinhas.
        float distance =
            Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

        // Evita divisão por zero.
        distance = Mathf.Max(distance, 0.2f);

        // Quanto menor a distância,
        // maior será a força.
        float forceMagnitude =
            Mathf.Clamp(
                basePushForce / distance,
                300f,
                maxPushForce
            );

        // Bônus das moedas do jogador.
        if (stats != null)
        {
            forceMagnitude *= stats.ForceMultiplier;
        }

        float resistance = 1f;

        // Resistência da bolinha inimiga.
        PlayerStats enemyStats =
            enemy.GetComponent<PlayerStats>();

        if (enemyStats != null)
        {
            resistance =
                enemyStats.ResistanceMultiplier;
        }

        // Aplica a força na bolinha inimiga.
        enemy.rb?.AddForce(
            direction *
            (forceMagnitude / resistance),
            ForceMode.Impulse
        );

        // Começa o cooldown de 3 segundos.
        pushCooldownTimer = pushCooldown;
    }

    // =========================================================
    // CONFIGURAÇÃO DA BOLINHA
    // =========================================================

    public void Configure(
        float speed,
        float push,
        float maxPush)
    {
        moveSpeed = speed;
        basePushForce = push;
        maxPushForce = maxPush;
    }
}
