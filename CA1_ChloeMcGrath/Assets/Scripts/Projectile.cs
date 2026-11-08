using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.AddForce(direction * force);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
        {
            Player p = gameObject.GetComponent<Player>();
            Vector2 pos = rb.transform.position;
            Destroy(this.gameObject);
        }
        else if(other.gameObject.tag == "Breakable") // Unseen Mechanic - Breakable Wall
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
