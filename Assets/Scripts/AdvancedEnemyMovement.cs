/*
AdvancedEnemyMovement.cs
Description: File that works with the more advanced enemy's movement
Creation date: 11/29/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials

Revisions
* Commits on Dec 8, 2024:
Kyle: Working Player Audio with enemy audio commented out
Ben: added sound
Commits on Dec 5, 2024:
Brinley: fix lose health on collide, health and oxygen bars,
Commits on Dec 4, 2024
Brinley: regular platforms fixed & small layer issues
Commits on Dec 3, 2024:
Brinley: a bunch of kraken stuff
Commits on Dec 1, 2024
Brinley: shark procedural generation & some bugs

Preconditions:
* Script must be attached to the GameManager objects. 
*Game must have an object tagged "Player". 
* Object must have an animator and an attack animation boolean
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

    private bool chasePlayer = false; // intialize a variable to determine whether the enemy is chasing the player
    public Transform player; // initialize a variable to hold the reference to the player object

    public Animator animator; // intialize the variable to reference the object's animator

    private bool attacking; // initailize a variable to determine whether the enemy is attacking the player
    private Collision2D playerc; // initialize a variable to hold the reference to the player object's collider

    public AudioSource audioSource; //audio manager?
    public AudioClip enemyHit; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>(); // Reference Rigidbody2D
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent rotation
        spriteRenderer = GetComponent<SpriteRenderer>(); // Reference SpriteRenderer
        cllider = GetComponent<Collider2D>(); // Reference Collider2D
        startX = rb.position.x; // set the starting x of the object to reference later
        player = GameObject.FindGameObjectWithTag("Player").transform; // Get the player character
        animator = GetComponent<Animator>(); // Get the Animator component
        damage = 25; //set the damage that the enemy can do
        health = 200; //set the amount of health the enemy has
        attacking = false; // set the attacking player variable to false since we're not near the player 
    }

    void Update()
    {
        if ((rb.position.x > startX + 10 && facingRight || rb.position.x < startX - 10 && !facingRight) && !chasePlayer) { // if the enemy is not chasing the player and is too far away from its starting position/patrol area
            Flip(); // flip the enemy and start swimming in the other direction
        }

        if (chasePlayer) { // if the enemy is chasing the player
            if (player.position.x > rb.position.x + 2 && !facingRight || player.position.x < rb.position.x - 2 && facingRight) { // check if the player is to the left or right of the enemy and go that way
                Flip();
            }
            if (!isGrounded) { // if the enemy is not on the ground
                if (player.position.y > rb.position.y + 1) { // if the player is above the enemy
                    movement.y = 1; // go up
                } else if (player.position.y < rb.position.y - 1) { // if the player is below the enemy
                    movement.y = -1; // go down
                } else { 
                    movement.y = 0; //otherwise don't move in the y direction
                }
                moveSpeed = 2f; // set move speed
            } else {
                movement.y = 0; // don't move iny direction
                moveSpeed = 2f; // set move speed
            }
            if (Math.Abs(rb.position.x-player.position.x) < 4 && Math.Abs(rb.position.y-player.position.y) < 4) { // if the player is in range of the enemy
                animator.SetBool("Attack", true); // set the animation to the enemy to attack
            } else {
                animator.SetBool("Attack", false); // otherwise set the animation boolean attack to false
            }
        } else {
            moveSpeed = 1f; // set move speed to slow
            movement.y = 0; // don't move in y direction
        }

        if (attacking) { // if the enemy is attacking
            playerc.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage); //hurt the player
        }

        movement.x = facingRight ? 1 : -1; // Movement in the direction determined by whether the sprite is facing right or left
        
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer); // Determine if the object is grounded
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, movement.y * moveSpeed); // Apply movement
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) { // if a player is in the enemy's chase zone
            chasePlayer = true; // chase the player
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Player")) { // if a player is out of the chase zone
            chasePlayer = false; // stop chasing the player
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
        { // if the collision is with the player
            attacking = true; // set the attack variable to true
            playerc = collision; // set the collision object to the player
        } else if (collision.gameObject.CompareTag("Bullet")) { // If enemy is hit by a bullet
            TakeDamage(50); //Make the enemy lose health
            //audioSource.PlayOneShot(enemyHit); //yeah
            Destroy(collision.gameObject); // Destroy the bullet
        } else { 
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>()); // ignore any collision that is not the player
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player")) { // if the player leaves the collision
            attacking = false; // enemy is not attacking anymore
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage; // decrease health of enemy
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
