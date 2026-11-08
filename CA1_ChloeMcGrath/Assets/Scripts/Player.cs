using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float JumpHeight = 2.4f;
    [SerializeField] private GameObject projectile;
    [SerializeField] private UIManager ui;
    [SerializeField] private AudioClip jumpAudio;
    [SerializeField] private AudioClip collectAudio;
    
    private int direction = 1;
    public float speed;
    private Vector2 move;

    private int jumpCount = 0;
    private bool wallJump = false;
    private int wallJumpDirection = 5;

    private Rigidbody2D rb;
    private Animator animator;

    private int score = 0;
    private bool hasKey = false;
    private int xp = 0;
    public int level = 1;

    private int lives = 3;
    private Vector2 lastCheckpoint;
    private float invincibilityTime = 3.0f;
    private float invincibilityCount = 0f;
    public bool isInvin = false;

    private AudioSource audio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        lastCheckpoint = rb.transform.position;
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
        if(InputSystem.actions["Jump"].IsPressed() && jumpCount <= 2 && !wallJump)
        {
            InputSystem.actions["Jump"].Reset();
            rb.linearVelocity = new Vector2(0,0);
            rb.AddForce(new Vector2(0, Mathf.Sqrt(-2*Physics.gravity.y * JumpHeight)), ForceMode2D.Impulse);
            jumpCount++;

            audio.PlayOneShot(jumpAudio);
        }
        else if(InputSystem.actions["Jump"].IsPressed() && wallJump) // Unseen Mechanic - Wall Jump
        {
            InputSystem.actions["Jump"].Reset();

            // Flip the direction the player is facing and the x direction of the jump
            direction *= -1;
            wallJumpDirection *= -1;
            
            // Referenced from Jump mechanic taught in class - x direction changed to move player horizontally
            rb.linearVelocity = new Vector2(0,0);
            rb.AddForce(new Vector2(wallJumpDirection, Mathf.Sqrt(-2*Physics.gravity.y * JumpHeight)), ForceMode2D.Impulse);
            jumpCount++;

            audio.PlayOneShot(jumpAudio);
        }

        animator.SetFloat("Move", move.x);
        
        // Attack
        if(InputSystem.actions["Attack"].IsPressed())
        {
            InputSystem.actions["Attack"].Reset();

            // Draw a new Projectile
            GameObject sb = Instantiate(projectile, rb.position + new Vector2(0,1.5f), Quaternion.identity);
            Projectile pro = sb.GetComponent<Projectile>();
            pro.Launch(new Vector2(direction, 0), 300);
        }

        // Remove invincibility after a certain period of time
        if(isInvin)
        {
            invincibilityCount += Time.deltaTime;
            if(invincibilityCount > invincibilityTime)
            {
                isInvin = false;
                invincibilityCount = 0;
                this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
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
            jumpCount = 0;
            wallJump = false;
        }
        else if(collision.gameObject.tag == "Wall") // Unseen Mechanic - Wall Jump
        {
            jumpCount = 0;
            wallJump = true;
        }
        else if(collision.gameObject.tag == "EnemyProjectile" && !isInvin)
        {
            rb.linearVelocity = Vector2.zero;
            PlayerHit();
        }
        else if(collision.gameObject.tag == "EnemyProjectile" && isInvin)
        {
            rb.linearVelocity = Vector2.zero;
        }
        else if(collision.gameObject.tag == "Door" && hasKey) // Unseen Mechanic - Unlockable Door
        {
            Destroy(collision.gameObject); 
        }
        else if(collision.gameObject.tag == "OutsideLevel") // Unseen Mechanic - Check if outside level
        {
            UpdateLives();
            rb.transform.position = lastCheckpoint;
        }
        else if(collision.gameObject.tag == "Finish") // Win Condition - Player reaches outside area
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void AddKey()
    {
        hasKey = true;
        audio.PlayOneShot(collectAudio);
    }

    public void CollectItem()
    {
        score++;
        ui.SetScore(score);
        audio.PlayOneShot(collectAudio);
    }

    private void PlayerHit()
    {
        UpdateLives();

        rb.transform.position = lastCheckpoint;
        isInvin = true;
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
    }

    public void UpdateLives()
    {
        lives--;
        ui.SetLives(lives);

        if(lives == 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void UpdateLevel() // Unseen Mechanic - Levelling Up
    {
        // Increase XP
        xp++;
        
        // Increase the player's level & boost speed when they defeat a certain number of enemies
        if(xp == 2)
        {
            // Set the level
            level++;
            ui.SetLevel(level);
            
            speed *= 2;
        }
    }
}
