/*
EnemyMovement.cs
Description: This script controls the basic movement and behavior of an enemy character in a 2D game.
Creation date: 11/17/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials
Revisions:
- Added required comments (12/8/2024, Gianni-Louisa)
- Working Player Audio with enemy audio commented out (12/8/2024, KyleMoore12)
- Added sound (12/8/2024, TrixTheWolf)
- Added background music and fixed some errors with player object not being referenced (12/7/2024, cbennudr)
- Fix lose health on collide (but still kinda buggy) (12/5/2024, brinley53)
- Health and oxygen bars (12/5/2024, brinley53)
- Get rid of small enemy flip issue with platforms (12/4/2024, brinley53)
- Regular platforms fixed & small layer issues (12/4/2024, brinley53)
- A bunch of kraken stuff (12/3/2024, brinley53)
- Enemy procedural generation & shark image (11/28/2024, brinley53)
- Fixed the pp (sorry) (11/24/2024, brinley53)
- Player attack (11/24/2024, brinley53)
- Fixed buggy respawning (11/23/2024, cbennudr)
- Added enemy damage, enemy death, and enemy attributes (damage and health) (11/21/2024, Gianni-Louisa)
- Added Player death, player damage, player attributes, enemy attack (11/19/2024, Gianni-Louisa)
- Janky enemy movement (11/17/2024, brinley53)

Preconditions:
- The GameObject this script is attached to must have a Rigidbody2D and a SpriteRenderer component.
- The 'groundCheck' Transform must be assigned in the Unity Editor.
- The 'groundLayer' must be set to the appropriate layer(s) considered as ground.

Postconditions:
- The enemy will move back and forth on a platform, flip direction at edges, and take damage from bullets.
- The enemy will attack the player when in contact.

Acceptable Input:
- 'groundCheck' should be a valid Transform.
- 'groundLayer' should be a valid LayerMask.

Unacceptable Input:
- Null or unassigned 'groundCheck' or 'groundLayer' will result in incorrect behavior.

Error and Exception Conditions:
- None explicitly handled in this script.

Side Effects:
- Changes the color of the enemy sprite based on health.
- Plays audio when the enemy is hit (commented out).

Invariants:
- The enemy's health is always between 0 and its initial value.
- The enemy's movement is constrained to the x-axis.

Known Faults:
- None documented.

*/

using System.Collections;
using UnityEngine;

// Class: EnemyMovement
// Description: Manages the movement and interactions of an enemy character.
public class EnemyMovement : MonoBehaviour
{
    // Public variables for movement, health, and damage
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public Transform groundCheck; // Transform used to check if the enemy is on the ground
    public LayerMask groundLayer; // LayerMask to identify ground
    public float groundCheckWidth = 1f; // Width of the ground check area
    public float groundCheckHeight = 0.2f; // Height of the ground check area
    public int health = 50; // Initial health of the enemy
    public int damage = 50; // Damage dealt to the player

    // Private variables for internal state
    private Rigidbody2D rb; // Rigidbody2D component for physics
    private Vector2 movement; // Current movement direction
    private bool isGrounded; // Whether the enemy is on the ground
    private bool facingRight = false; // Direction the enemy is facing
    private SpriteRenderer spriteRenderer; // SpriteRenderer component for visual representation
    private Collider2D cllider; // Collider2D component for collision detection

    private GameObject player; // Reference to the player GameObject
    private bool attacking; // Whether the enemy is currently attacking
    public AudioSource audioSource; // AudioSource component for playing sounds
    public AudioClip enemyHit; // AudioClip for when the enemy is hit

    // Method: Start
    // Description: Initializes components and sets up initial state.
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent the enemy from rotating
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        cllider = GetComponent<Collider2D>(); // Get the Collider2D component
        attacking = false; // Initialize attacking state

        player = GameObject.FindGameObjectWithTag("Player"); // Find the player GameObject by tag
    }

    // Method: Update
    // Description: Handles movement logic and attack behavior.
    void Update()
    {
        // Check if the enemy is grounded and not moving vertically
        if (!isGrounded && rb.linearVelocity.y == 0)
        {
            Flip(); // Flip the enemy's direction
        }

        // If the enemy is falling, stop horizontal movement
        if (rb.linearVelocity.y != 0)
        {
            movement.x = 0; // Stop horizontal movement
        }
        else
        {
            movement.x = facingRight ? 1 : -1; // Move in the direction the enemy is facing
        }

        // Check if the enemy is on the ground
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer);

        // If attacking, deal damage to the player
        if (attacking)
        {
            player.gameObject.GetComponent<PlayerMovement>().TakeDamage(5);
        }
    }

    // Method: FixedUpdate
    // Description: Applies movement to the enemy.
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y); // Apply horizontal movement
    }

    // Method: OnDrawGizmos
    // Description: Draws a visual representation of the ground check area in the editor.
    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red; // Set Gizmo color to red
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f)); // Draw the ground check area
        }
    }

    // Method: OnCollisionEnter2D
    // Description: Handles collision events with other objects.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attacking = true; // Start attacking the player
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(damage); // Take damage from the bullet
            //audioSource.PlayOneShot(enemyHit); // Play hit sound (commented out)
            Destroy(collision.gameObject); // Destroy the bullet
        }
    }

    // Method: OnCollisionExit2D
    // Description: Handles when the enemy stops colliding with the player.
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attacking = false; // Stop attacking the player
        }
    }

    // Method: TakeDamage
    // Description: Reduces the enemy's health and handles death.
    public void TakeDamage(int damage)
    {
        health -= damage; // Subtract damage from health
        if (health <= 0)
        {
            StartCoroutine(HandleDeath()); // Start death handling coroutine
        }
        else
        {
            UpdateColorBasedOnHealth(); // Update the enemy's color based on health
        }
    }

    // Method: UpdateColorBasedOnHealth
    // Description: Updates the enemy's color based on its current health.
    private void UpdateColorBasedOnHealth()
    {
        float redIntensity = 1f - (health / 100f); // Calculate red intensity based on health
        spriteRenderer.color = new Color(1f, 1f - redIntensity, 1f - redIntensity); // Set the sprite color
    }

    // Method: HandleDeath
    // Description: Handles the enemy's death animation and destruction.
    private IEnumerator HandleDeath()
    {
        spriteRenderer.color = Color.red; // Change color to red
        float floatDuration = 2f; // Duration of floating effect
        float floatSpeed = 2f; // Speed of floating upwards

        cllider.enabled = false; // Disable collider
        rb.linearVelocity = Vector2.zero; // Stop movement
        rb.isKinematic = true; // Disable physics interactions

        Vector2 originalPosition = transform.position; // Store original position
        float elapsedTime = 0f; // Initialize elapsed time

        // Rotate and float upwards
        transform.Rotate(0, 0, 180); // Rotate 180 degrees
        while (elapsedTime < floatDuration)
        {
            transform.position = new Vector2(originalPosition.x, originalPosition.y + (floatSpeed * elapsedTime)); // Move upwards
            elapsedTime += Time.unscaledDeltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Destroy the enemy after floating
        Destroy(gameObject); // Destroy the enemy GameObject
    }

    // Method: Flip
    // Description: Flips the enemy's sprite and direction.
    private void Flip()
    {
        transform.Rotate(new Vector3(0, 180, 0)); // Rotate the sprite 180 degrees
        facingRight = !facingRight; // Toggle the facing direction
    }
}