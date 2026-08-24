using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpForce = 8f;
    Rigidbody2D rb;
    bool grounded;

    void Awake() { rb = GetComponent<Rigidbody2D>(); }
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(h * speed, rb.linearVelocity.y);
        if (Input.GetButtonDown("Jump") && grounded) { rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); }
        if (Input.GetKeyDown(KeyCode.Escape)) { /* open pause menu via UI manager */ }
    }

    void OnCollisionEnter2D(Collision2D c) { grounded = true; }
    void OnCollisionExit2D(Collision2D c) { grounded = false; }
}
