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

    private Rigidbody rb;

    private PlayerStats stats;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        //rb.useGravity = false;
        rb.linearDamping = 0.8f;
        rb.angularDamping = 0.2f;
       // rb.constraints = RigidbodyConstraints.FreezeRotation;

        stats = GetComponent<PlayerStats>();
    }

    private void FixedUpdate()
    {
        Vector2 movement = ReadMovementInput();
        Vector3 moveDirection = new Vector3(movement.x, 0f, movement.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            float speed = moveSpeed;

            if(stats != null)
            {
                speed *= stats.SpeedMultiplier;
            }

            rb.AddForce(moveDirection.normalized * speed, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
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
            if (IsKeyHeld(KeyCode.RightArrow)) horizontal += 1f;
            if (IsKeyHeld(KeyCode.LeftArrow)) horizontal -= 1f;

            float vertical = 0f;
            if (IsKeyHeld(KeyCode.UpArrow)) vertical += 1f;
            if (IsKeyHeld(KeyCode.DownArrow)) vertical -= 1f;

            return new Vector2(horizontal, vertical);
        }

        float horizontalWASD = 0f;
        if (IsKeyHeld(KeyCode.D)) horizontalWASD += 1f;
        if (IsKeyHeld(KeyCode.A)) horizontalWASD -= 1f;

        float verticalWASD = 0f;
        if (IsKeyHeld(KeyCode.W)) verticalWASD += 1f;
        if (IsKeyHeld(KeyCode.S)) verticalWASD -= 1f;

        return new Vector2(horizontalWASD, verticalWASD);
    }

    private bool IsKeyHeld(KeyCode key)
    {
        return IsKeyPressed(key, false);
    }

    private bool IsKeyPressed(KeyCode key, bool usePressedThisFrame)
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
            return false;

        var inputKey = ToInputSystemKey(key);
        var control = keyboard[inputKey];
        return usePressedThisFrame ? control.wasPressedThisFrame : control.isPressed;
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

    private void ApplyPush()
    {
        var others = FindObjectsOfType<TwoBallController>();
        if (others == null || others.Length < 2) return;

        TwoBallController enemy = null;
        foreach (var candidate in others)
        {
            if (candidate == null || candidate == this) continue;
            enemy = candidate;
            break;
        }

        if (enemy == null) return;

        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, enemy.transform.position);
        distance = Mathf.Max(distance, 0.2f);

        float forceMagnitude = Mathf.Clamp(basePushForce / distance, 300f, maxPushForce);

        if(stats != null)
        {
            forceMagnitude *= stats.ForceMultiplier;
        }
        float resistance = 1f;

        PlayerStats enemyStats =
        enemy.GetComponent<PlayerStats>();

        if(enemyStats != null)
        {
            resistance = enemyStats.ResistanceMultiplier;
        }

        enemy.rb?.AddForce(direction * (forceMagnitude / resistance), ForceMode.Impulse);
    }
    // No cenário devem ter moedas, que quando coletadas, aumentam a pontuação do jogador e também aumentam a força de empurrão da bolinha. A cada 5 moedas coletadas, a bolinha do jogador aumenta de tamanho, ficando mais lenta, mas mais resistente a ser empurrada para longe.
    public void CollectCoin(int value)
    {
        // Aqui você pode adicionar lógica adicional, como aumentar a força de empurrão da bolinha ou aumentar o tamanho da bolinha a cada 5 moedas coletadas.
        for (int i = 0; i < value / 5; i++)
        {
            // Aumenta o tamanho da bolinha e ajusta a força de empurrão
            transform.localScale += Vector3.one * 0.1f;
            basePushForce += 50f;
        }
    }
    public void Configure(float speed, float push, float maxPush)
    {
        moveSpeed = speed;
        basePushForce = push;
        maxPushForce = maxPush;
    }
}
