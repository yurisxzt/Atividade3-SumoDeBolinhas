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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        rb.linearDamping = 0.8f;
        rb.angularDamping = 0.2f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        Vector2 movement = ReadMovementInput();
        Vector3 moveDirection = new Vector3(movement.x, 0f, movement.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);
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
        enemy.rb?.AddForce(direction * forceMagnitude, ForceMode.Impulse);
    }
}
