using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Movement speed of the enemy
    public Transform groundCheck; // Reference to ground check
    public LayerMask groundLayer; // Layers considered as ground
    public float groundCheckWidth = 1f; // Ground check box width
    public float groundCheckHeight = 0.2f; // Ground check box height
    public int health = 100; // Enemy's health
    public int damage = 20; // Damage the enemy can inflict

    private Rigidbody2D rb; // Enemy's Rigidbody2D
    private Vector2 movement; // Movement direction
    private bool isGrounded; // Is the enemy on the ground
    private bool facingRight = false; // Direction facing
    private SpriteRenderer spriteRenderer; // Enemy's SpriteRenderer
    private Collider2D collider2D; // Reference to the enemy's collider

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Reference Rigidbody2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference SpriteRenderer
        collider2D = GetComponent<Collider2D>(); // Reference Collider2D

        // Start the test coroutine to reduce health every 2 seconds
        // StartCoroutine(TestHealthReduction());
    }

    void Update()
    {
        if (!isGrounded)
        {
            Flip();
        }
        movement.x = facingRight ? 1 : -1;

        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer);

    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y); // Apply movement
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(20);
        } else if (collision.gameObject.CompareTag("Bullet")) { // If enemy is hit by a bullet
            TakeDamage(damage); //Make the enemy lose health
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
        else
        {
            UpdateColorBasedOnHealth();
        }
    }

    private void UpdateColorBasedOnHealth()
    {
        float redIntensity = 1f - (health / 100f); // Calculate red intensity
        spriteRenderer.color = new Color(1f, 1f - redIntensity, 1f - redIntensity); // Set color
    }

    private IEnumerator HandleDeath()
    {
        spriteRenderer.color = Color.red; // Turn red
        float floatDuration = 2f; // Float for 2 seconds
        float floatSpeed = 2f; // Speed of floating up

        collider2D.enabled = false; // Disable the collider to prevent interactions
        rb.linearVelocity = Vector2.zero; // Stop movement
        rb.isKinematic = true; // Disable physics interactions

        Vector2 originalPosition = transform.position;
        float elapsedTime = 0f;

        // Rotate and float upwards
        transform.Rotate(0, 0, 90);
        while (elapsedTime < floatDuration)
        {
            transform.position = new Vector2(originalPosition.x, originalPosition.y + (floatSpeed * elapsedTime));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Destroy the enemy after floating
        Destroy(gameObject);
    }

    private void Flip()
    {
        transform.Rotate(new Vector3(0, 180, 0)); // Flip 180 degrees
        facingRight = !facingRight; // Toggle direction
    }

}
