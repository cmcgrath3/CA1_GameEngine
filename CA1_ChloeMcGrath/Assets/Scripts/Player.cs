using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // Movement Variables
    private int direction = 1;
    public float speed;
    private Vector2 move;

    // Jump Variables
    [SerializeField] private float JumpHeight = 2.4f;
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movement
        move = InputSystem.actions["Move"].ReadValue<Vector2>();

        if(move.x != 0)
        {
            // Return -1 if move is less than 0
            direction = move.x < 0 ? 1 : -1;
        }

        // Jump
        if(InputSystem.actions["Jump"].IsPressed() && jumpCount <= 2)
        {
            InputSystem.actions["Jump"].Reset();
            rb.linearVelocity = new Vector2(0,0);
            rb.AddForce(new Vector2(0, Mathf.Sqrt(-2*Physics.gravity.y * JumpHeight)), ForceMode2D.Impulse);
            jumpCount++;
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rb.position;
        position.x += move.x * speed * Time.fixedDeltaTime;
        rb.position = position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            jumpCount = 0; // Reset jump count when the player touches the ground
        }
    }
}
