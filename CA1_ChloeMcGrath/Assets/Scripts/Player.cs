using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private int direction = 1;
    public float speed;
    private Vector2 move;

    [SerializeField] private float JumpHeight = 2.4f;
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private Animator animator;

    private bool hasKey = false;
    private int objectsCollected = 0;

    [SerializeField] private GameObject projectile;

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
            direction = move.x < 0 ? -1 : 1;
            animator.SetInteger("Direction", direction);
        }

        // Jump
        if(InputSystem.actions["Jump"].IsPressed() && jumpCount <= 2)
        {
            InputSystem.actions["Jump"].Reset();
            rb.linearVelocity = new Vector2(0,0);
            rb.AddForce(new Vector2(0, Mathf.Sqrt(-2*Physics.gravity.y * JumpHeight)), ForceMode2D.Impulse);
            jumpCount++;
        }

        animator.SetFloat("Move", move.x);

        // Attack
        if(InputSystem.actions["Attack"].IsPressed())
        {
            InputSystem.actions["Attack"].Reset();
            GameObject sb = Instantiate(projectile, rb.position + new Vector2(0,1.5f), Quaternion.identity);
            Projectile pro = sb.GetComponent<Projectile>();
            pro.Launch(new Vector2(direction, 0), 300);
        }
    }

    void FixedUpdate()
    {
        animator.SetInteger("Direction", direction);
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

        if(collision.gameObject.tag == "Door" && hasKey == true)
        {
            Destroy(collision.gameObject);
        }
    }

    public void AddKey()
    {
        hasKey = true;
    }

    public void CollectObject()
    {
        objectsCollected += 1;
    }
}
