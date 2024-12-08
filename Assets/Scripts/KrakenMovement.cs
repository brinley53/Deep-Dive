/*
KrakenMovement.cs
Description: File that works with the more advanced enemy's movement
Creation date: 12/3/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials
*/

using System;
using System.Collections;
using UnityEngine;

public class KrakenMovement : MonoBehaviour
{
    public float moveSpeed = 1f; // Movement speed of the enemy
    public LayerMask groundLayer; // Layers considered as ground
    public int health; // Enemy's health
    public int damage; // Damage the enemy can inflict

    private Rigidbody2D rb; // Enemy's Rigidbody2D
    private Vector2 movement; // Movement direction
    private bool facingRight = true; // Direction facing
    private bool goingDown = true; // Which direction enemy is moving
    private SpriteRenderer spriteRenderer; // Enemy's SpriteRenderer
    private Collider2D cllider; // Reference to the enemy's collider

    private int playerTriggerCount = 0;
    private int bulletTriggerCount = 0;

    private float startY; // Initialize the starting position y of the enemy

    private bool chasePlayer = false;
    public Transform player;

    private bool attacking;
    private Collider2D playerc;

    public AudioClip enemyHit;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Reference Rigidbody2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference SpriteRenderer
        cllider = GetComponent<Collider2D>(); // Reference Collider2D
        startY = rb.position.y;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        damage = 99;
        health = 500;
        attacking = false;
    }

    void Update()
    {
        if ((rb.position.y > startY + 10 && !goingDown || rb.position.y < startY - 10 && goingDown) && !chasePlayer) {
            goingDown = !goingDown; // Toggle direction
        }

        if (chasePlayer) {
            if (player.position.x > rb.position.x && !facingRight || player.position.x < rb.position.x && facingRight) {
                facingRight = !facingRight; // Toggle direction
            }
            if (player.position.y > rb.position.y && goingDown || player.position.y < rb.position.y && !goingDown) {
                goingDown = !goingDown; //Toggle direction
            }
            movement.x = facingRight ? 1 : -1; // Movement in the direction determined by whether the sprite is facing right or left
        } else {
            movement.x = 0;
        }

        if (attacking) {
            playerc.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage);
        }

        movement.y = goingDown ? -1 : 1;
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed); // Apply movement
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            chasePlayer = true;
            playerTriggerCount += 1;
            if (playerTriggerCount == 2) {
                attacking = true;
                playerc = collision;
            }
        } else if (collision.gameObject.CompareTag("Bullet")) {
            bulletTriggerCount += 1;
            if (bulletTriggerCount == 2) {
                TakeDamage(50); //Make the enemy lose health
                audioSource.PlayOneShot(enemyHit);
                Destroy(collision.gameObject); // Destroy the bullet
                bulletTriggerCount = 0;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) {
            playerTriggerCount -= 1;
            if (playerTriggerCount == 0) {
                chasePlayer = false;
            }
        } else if (collision.gameObject.CompareTag("Bullet")) {
            bulletTriggerCount = 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // if (collision.gameObject.CompareTag("Player"))
        // {
        //     collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage);
        // } else if (collision.gameObject.CompareTag("Bullet")) { // If enemy is hit by a bullet
        //     TakeDamage(50); //Make the enemy lose health
        //     Destroy(collision.gameObject); // Destroy the bullet
        // }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
        // else
        // {
        //     UpdateColorBasedOnHealth();
        // }
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

}
