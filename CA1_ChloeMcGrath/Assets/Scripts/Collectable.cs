using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private Player player;

    private float floatTime = 1;
    private float timePassed = 0;
    private int direction = 1;
    private float height = .3f;

    void Update()
    {
        // Make collectable float
        Vector2 position = transform.position;
        position.y += height * Time.deltaTime * direction;
        transform.position = position;

        timePassed += Time.deltaTime;

        // Change float direction
        if(timePassed > floatTime)
        {
            direction *= -1;
            timePassed = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && this.gameObject.tag == "Key") // Unseen Mechanic - Key for Door - referenced from collectable code taught in class
        {
            player.AddKey();
            Destroy(this.gameObject);
        }
        
        if(collision.gameObject.tag == "Player" && this.gameObject.tag == "Collectable")
        {
            player.CollectItem();
            Destroy(this.gameObject);
        }
    }
}
