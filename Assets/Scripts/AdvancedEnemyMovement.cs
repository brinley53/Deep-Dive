/*
AdvancedEnemyMovement.cs
Description: File that works with the more advanced enemy's movement
Creation date: 11/29/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials
*/

using System;
using System.Collections;
using UnityEngine;

public class AdvancedEnemyMovement : MonoBehaviour
{
    public float moveSpeed = 1f; // Movement speed of the enemy
    public Transform groundCheck; // Reference to ground check
    public LayerMask groundLayer; // Layers considered as ground
    public float groundCheckWidth = 5f; // Ground check box width
    public float groundCheckHeight = 0.2f; // Ground check box height
    public int health; // Enemy's health
    public int damage; // Damage the enemy can inflict

    private Rigidbody2D rb; // Enemy's Rigidbody2D
    private Vector2 movement; // Movement direction
    private bool isGrounded; // Is the enemy on the ground
    private bool facingRight = true; // Direction facing
    private SpriteRenderer spriteRenderer; // Enemy's SpriteRenderer
    private Collider2D cllider; // Reference to the enemy's collider

    private float startX; // Initialize the starting position x of the enemy

    private bool chasePlayer = false;
    public Transform player;

    public Animator animator;

    private bool attacking;
    private Collision2D playerc;

    public AudioSource audioSource; //audio manager?
    public AudioClip enemyHit;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>(); // Reference Rigidbody2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference SpriteRenderer
        cllider = GetComponent<Collider2D>(); // Reference Collider2D
        startX = rb.position.x;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>(); // Get the Animator component
        damage = 25;
        health = 200;
        attacking = false;
    }

    void Update()
    {
        if ((rb.position.x > startX + 10 && facingRight || rb.position.x < startX - 10 && !facingRight) && !chasePlayer) {
            Flip();
        }

        if (chasePlayer) {
            if (player.position.x > rb.position.x + 2 && !facingRight || player.position.x < rb.position.x - 2 && facingRight) {
                Flip();
            }
            if (!isGrounded) {
                if (player.position.y > rb.position.y + 1) {
                    movement.y = 1;
                } else if (player.position.y < rb.position.y - 1) {
                    movement.y = -1;
                } else {
                    movement.y = 0;
                }
                moveSpeed = 2f;
            } else {
                movement.y = 0;
                moveSpeed = 2f;
            }
            if (Math.Abs(rb.position.x-player.position.x) < 4 && Math.Abs(rb.position.y-player.position.y) < 4) {
                animator.SetBool("Attack", true);
            } else {
                animator.SetBool("Attack", false);
            }
        } else {
            moveSpeed = 1f;
            movement.y = 0;
        }

        if (attacking) {
            playerc.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage);
        }

        movement.x = facingRight ? 1 : -1; // Movement in the direction determined by whether the sprite is facing right or left
        
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer); // Determine if the object is grounded
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed); // Apply movement
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            chasePlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            chasePlayer = false;
        }
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
            playerc = collision;
        } else if (collision.gameObject.CompareTag("Bullet")) { // If enemy is hit by a bullet
            TakeDamage(50); //Make the enemy lose health
            //audioSource.PlayOneShot(enemyHit); //yeah
            Destroy(collision.gameObject); // Destroy the bullet
        } else {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
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
