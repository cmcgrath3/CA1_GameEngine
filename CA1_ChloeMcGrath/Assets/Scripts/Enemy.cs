using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float moveTime;
    [SerializeField] private float idleTime;
    [SerializeField] private float fireTime;
    [SerializeField] private GameObject projectile;

    private Rigidbody2D rb;
    private Animator animator;

    private float elapsedTime;
    private int direction = 1;
    private bool isMoving = false;

    private float fireCountdown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Movement
        if(isMoving && elapsedTime < moveTime)
        {
            Vector2 position = rb.transform.position;
            position.x += direction * Time.deltaTime;
            rb.transform.position = position;
        }
        else if(!isMoving && elapsedTime > idleTime)
        {
            isMoving = true;
            elapsedTime = 0;
            direction *= -1;
            animator.SetInteger("Direction", direction);
            animator.SetFloat("Move", direction); 
        }
        else if(isMoving && elapsedTime > moveTime)
        {
            isMoving = false;
            elapsedTime = 0;
            animator.SetFloat("Move", 0);
        }
        
        elapsedTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(rb.transform.position, new Vector2(direction, 0), 5f, LayerMask.GetMask("Player"));
        if(hit.collider != null)
        {
            if(hit.collider.GetComponent<Player>() != null)
            {
                Fire();
            }
        }
        fireCountdown += Time.fixedDeltaTime;
    }

    private void Fire()
    {
        if(fireCountdown > fireTime)
        {
            GameObject go = Instantiate(projectile, rb.transform.position, Quaternion.identity);
            Projectile pro = go.GetComponent<Projectile>();
            pro.Launch(new Vector2(direction, 0), 300);
            fireCountdown = 0;
        }
        
    }
}
