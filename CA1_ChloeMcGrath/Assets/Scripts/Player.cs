using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private int direction = 1;
    public int speed;

    private Vector2 move;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Player Movement
        move = InputSystem.actions["Move"].ReadValue<Vector2>();
        if(move.x != 0)
        {
            direction = move.x < 0 ? 1 : -1;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rb.position;
        position.x += move.x * speed * Time.fixedDeltaTime;
        rb.position = position;
    }
}
