/*
KrakenMovement.cs
Description: Script for handling the movment of the kraken
Creation date: 12/3/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials

Revisions
* 11/24/24 - Brinley: added receiving damage from the player
* 12/3/24 - Brinley: improved kraken tracking 
* 12/3/24 - Brinley: added damaging player on collision
* 12/8/24 - Ben : added sound effects
Preconditions:
* Script must be attached to the kraken prefab
Postconditions:
* None
Error and Exception conditions:
* None
Side effects:
* None
Invariants:
* None
Known Faults:
* None
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

    private int playerTriggerCount = 0; // initialize a variable to keep track of how close the player is to the object
    private int bulletTriggerCount = 0; // initailzie a variable to keep track of how close the bullet is to the ojbect

    private float startY; // Initialize the starting position y of the enemy

    private bool chasePlayer = false; // bool for if kraken is chasing the player
    public Transform player; // the player transform

    private bool attacking; // bool for if kraken is attacking the player
    private Collider2D playerc; // the player collider

    public AudioSource audioSource; //audio manager?
    public AudioClip enemyHit;

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // audio source for sound effects
        rb = GetComponent<Rigidbody2D>(); // Reference Rigidbody2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference SpriteRenderer
        cllider = GetComponent<Collider2D>(); // Reference Collider2D
        startY = rb.position.y; // get starting y position
        player = GameObject.FindGameObjectWithTag("Player").transform; // get the player transform
        damage = 99; // set the damage the enemy can inflict
        health = 500; // set the health of the enemy
        attacking = false; // set the attacking boolean to false as he does not start attacking the player
    }

    void Update()
    {
        // if the kraken is past his patrol points and is not chasing the player
        if ((rb.position.y > startY + 10 && !goingDown || rb.position.y < startY - 10 && goingDown) && !chasePlayer) {
            goingDown = !goingDown; // Toggle direction
        }

        // If chasing the player
        if (chasePlayer) {
            // Chase player on x-axis
            if (player.position.x > rb.position.x && !facingRight || player.position.x < rb.position.x && facingRight) {
                facingRight = !facingRight; // Toggle direction
            }
            // Chase player y-axis
            if (player.position.y > rb.position.y && goingDown || player.position.y < rb.position.y && !goingDown) {
                goingDown = !goingDown; //Toggle direction
            }
            movement.x = facingRight ? 1 : -1; // Movement in the direction determined by whether the sprite is facing right or left
        } else {
            movement.x = 0; // otherwise only go up and down
        }
        // If attacking the player
        if (attacking) {
            // Player take damage
            playerc.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage);
        }

        movement.y = goingDown ? -1 : 1; // set movement in y direction to whichever way the kraken is facing
        
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed); // Apply movement
    }

    // WHen something enters the kraken's collider
    void OnTriggerEnter2D(Collider2D collision) {
        // If its the player
        if (collision.gameObject.CompareTag("Player")) {
            chasePlayer = true; // start chasing the player
            playerTriggerCount += 1; // increment counter
            // If second time player triggerd the trigger, start attacking
            if (playerTriggerCount == 2) { // if the player is closest to the kraken
                attacking = true; //the kraken is attacking
                playerc = collision; // set the collider to the player
            }
        } 
        // If collided with a harpoon
        else if (collision.gameObject.CompareTag("Bullet")) {
            bulletTriggerCount += 1; // increment counter
            // If second trigger, bullet is close to the kraken and it takes damage
            if (bulletTriggerCount == 2) {
                TakeDamage(50); //Make the enemy lose health
                //audioSource.PlayOneShot(enemyHit);
                Destroy(collision.gameObject); // Destroy the bullet
                bulletTriggerCount = 0;
            }
        }
    }
    // When object leaves trigger
    void OnTriggerExit2D(Collider2D collision) {
        // If object was the player
        if (collision.gameObject.CompareTag("Player")) {
            playerTriggerCount -= 1; // decrement counter 
            // If the player leaves the vicinity
            if (playerTriggerCount == 0) {
                chasePlayer = false; //stop chasing the player
            }
        } 
        // If bullet left trigger
        else if (collision.gameObject.CompareTag("Bullet")) {
            bulletTriggerCount = 0; // reset bullet trigger count
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f));
    }

    // Function for taking damage
    public void TakeDamage(int damage)
    {
        health -= damage; // decrement heatlh
        // Handle death if needed
        if (health <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    // Function to change the color of the kraken to reflect its remaining health
    private void UpdateColorBasedOnHealth()
    {
        float redIntensity = 1f - (health / 100f); // Calculate red intensity
        spriteRenderer.color = new Color(1f, 1f - redIntensity, 1f - redIntensity); // Set color
    }

    // IEnumerator function for handlnig the kraken's death
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
        // Start floating the kraken upwards
        while (elapsedTime < floatDuration)
        {
            transform.position = new Vector2(originalPosition.x, originalPosition.y + (floatSpeed * elapsedTime)); // float up
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Destroy the enemy after floating
        Destroy(gameObject);
    }

}
