/*
EnemyMovement.cs
Description: File that works with the basic enemy's movement
Creation date: 11/17/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials
*/

using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Movement speed of the enemy
    public Transform groundCheck; // Reference to ground check
    public LayerMask groundLayer; // Layers considered as ground
    public float groundCheckWidth = 1f; // Ground check box width
    public float groundCheckHeight = 0.2f; // Ground check box height
    public int health = 50; // Enemy's health
    public int damage = 50; // Damage the enemy can inflict

    private Rigidbody2D rb; // Enemy's Rigidbody2D
    private Vector2 movement; // Movement direction
    private bool isGrounded; // Is the enemy on the ground
    private bool facingRight = false; // Direction facing
    private SpriteRenderer spriteRenderer; // Enemy's SpriteRenderer
    private Collider2D cllider; // Reference to the enemy's collider

    private GameObject player;
    private bool attacking;
    public AudioClip enemyHit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Reference Rigidbody2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference SpriteRenderer
        cllider = GetComponent<Collider2D>(); // Reference Collider2D
        attacking = false;

        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (!isGrounded && rb.linearVelocity.y == 0) // When the enemy reaches the edge of a platform (it senses there is no more ground in front of it)
        {
            Flip(); // It flips the sprite
        }

        if (rb.linearVelocity.y != 0) { // If the enemy is falling
            movement.x = 0; // Make its left/right movement null to stop jitter
        } else { // If the enemy is on solid ground
            movement.x = facingRight ? 1 : -1; // Movement in the direction determined by whether the sprite is facing right or left
        }
        
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer); // Determine if the object is grounded

        if (attacking) {
            player.gameObject.GetComponent<PlayerMovement>().TakeDamage(5);
        }
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
            attacking = true;
            // player = collision;
        } else if (collision.gameObject.CompareTag("Bullet")) { // If enemy is hit by a bullet
            TakeDamage(damage); //Make the enemy lose health
            audioSource.PlayOneShot(enemyHit);
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            attacking = false;
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

        cllider.enabled = false; // Disable the collider to prevent interactions
        rb.linearVelocity = Vector2.zero; // Stop movement
        rb.isKinematic = true; // Disable physics interactions

        Vector2 originalPosition = transform.position;
        float elapsedTime = 0f;

        // Rotate and float upwards
        transform.Rotate(0, 0, 180);
        while (elapsedTime < floatDuration)
        {
            transform.position = new Vector2(originalPosition.x, originalPosition.y + (floatSpeed * elapsedTime));
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Destroy the enemy after floating
        Destroy(gameObject);
    }

    private void Flip() // Function to flip the enemy sprite
    {
        transform.Rotate(new Vector3(0, 180, 0)); // Flip 180 degrees
        facingRight = !facingRight; // Toggle direction
    }

}
