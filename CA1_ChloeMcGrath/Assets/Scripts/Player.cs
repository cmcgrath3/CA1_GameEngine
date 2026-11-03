using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float JumpHeight = 2.4f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private UIManager ui;
    
    private int direction = 1;
    public float speed;
    private Vector2 move;
    private int jumpCount = 0;

    private Rigidbody2D rb;
    private Animator animator;

    private bool hasKey = false;
    private int score = 0;
    private int lives = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Move
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
        // Reset jump count when the player touches the ground
        if(collision.gameObject.tag == "Ground")
        {
            jumpCount = 0;
        }
        else if(collision.gameObject.tag == "EnemyProjectile")
        {
            rb.linearVelocity = Vector2.zero;
            PlayerHit();
        }
        // Delete door from scene when the player collects the key
        if(collision.gameObject.tag == "Door" && hasKey == true)
        {
            Destroy(collision.gameObject); 
        }
    }

    public void AddKey()
    {
        hasKey = true;
    }

    public void CollectItem()
    {
        // Increase the score
        score++;

        // Call the SetScore function in the UIManager script
        ui.SetScore(score);
    }

    private void PlayerHit()
    {
        lives--;
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    }
}
