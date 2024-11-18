/**
PlayerMovement.cs
Description: File that works with the player's movement and jumping and player speed and jump force.
Creation date: 11/6/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums
**/
using System;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f; // Movement speed of the player in units per second
    public float jumpForce = 7f; // Force applied upward when jumping
    public Transform groundCheck; // Reference to an empty GameObject that marks where to check for ground
    public float groundCheckRadius = 1f; // Radius used for the ground check circle (not currently used since we switched to box)
    public LayerMask groundLayer; // Layer mask to specify what layers should be considered as ground
    public float groundCheckWidth = 1f; // Width of the main ground check box
    public float groundCheckHeight = 0.2f; // Height of the ground check box

    private Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    private Vector2 movement; // Stores the current movement input (-1 to 1)
    private bool isGrounded; // Tracks whether the player is currently touching the ground

    public Animator animator; // Add an animator for animating the player character

    private bool isFallingThrough = false;
    private float fallThroughDuration = 0.6f; // Duration to remain in NoCollision layer after releasing Ctrl
    private float fallThroughTimer = 0f;

    void Start() // Called once when the script is first enabled
    {
        rb = GetComponent<Rigidbody2D>(); // Get and store reference to the Rigidbody2D component
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent the player from rotating
        // Debug.Log("Ground Layer Mask: " + groundLayer.value); // Output the ground layer mask value for debugging purposes
    }

    void Update() // Called every frame
    {
        movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input (-1 for left, 1 for right, 0 for no input)
        
        bool wasGrounded = isGrounded; // Store the previous grounded state for comparison
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer);
        if (isGrounded != wasGrounded) // Log when the grounded state changes
        {
            // Debug.Log($"Grounded state changed to: {isGrounded}");
        }
        if (Input.GetButtonDown("Jump") && isGrounded) // Check for jump being held down 
        {
            animator.SetBool("Jump", true); // Set the jump condition for the animator to true to set off the jump animation
        }
        else if (Input.GetButtonUp("Jump")) // Check for jump input
        {
            // Debug.Log("Jump button pressed"); // Log that jump button was pressed
            if (isGrounded) // Only allow jumping if the player is grounded
            {
                // Debug.Log("Applying jump force"); // Log that jump force is being applied
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply the upward force for jumping
            }
            else
            {
                // Debug.Log("Can't jump - not grounded"); // Log if jump attempt failed due to not being grounded
            }
        } else {
            animator.SetBool("Jump", false); // Initialize the jump condition for the jump animation to false
        }

        movement.y = rb.linearVelocity.y; // Get the movement speed in the vertical axis
        animator.SetFloat("Horizontal", Math.Abs(movement.x * moveSpeed)); // Set the animator's x to reference in animator 
        animator.SetFloat("Vertical", movement.y); // Set the animator's y to reference in animator
    
        // Check if the "Ctrl" key is pressed
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            animator.SetBool("Jump", true); // Set the jump condition for the animator to true to set off the crouch animation
            int noCollisionLayer = LayerMask.NameToLayer("NoCollision");
            if (noCollisionLayer != -1) // Ensure the layer exists
            {
                if (gameObject.layer != noCollisionLayer) // Only change if not already set
                {
                    gameObject.layer = noCollisionLayer; // Change to the "NoCollision" layer
                    isFallingThrough = true;
                    fallThroughTimer = fallThroughDuration; // Reset the timer
                    Debug.Log("Player layer set to NoCollision");
                }
            }
            else
            {
                Debug.LogError("NoCollision layer not found. Please ensure it is created in Unity.");
            }
        }
        else if (isFallingThrough)
        {
            fallThroughTimer -= Time.deltaTime;
            if (fallThroughTimer <= 0f)
            {
                int defaultLayer = LayerMask.NameToLayer("Default");
                if (gameObject.layer != defaultLayer)
                {
                    gameObject.layer = defaultLayer;
                    isFallingThrough = false;
                    Debug.Log("Player layer set to Default");
                }
            }
        }
    }

    void FixedUpdate() // Called at a fixed time interval (better for physics calculations)
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y); // Apply horizontal movement while preserving vertical velocity
        float maxFallSpeed = -20f; // Maximum speed the player can fall
        if (rb.linearVelocity.y < maxFallSpeed) // Check if player is falling faster than the maximum fall speed
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed); // Clamp the fall speed to the maximum
        }
    }

    void OnDrawGizmos() // Called in the editor to draw debug visuals
    {
        if (groundCheck != null) // Only draw if we have a ground check point set
        {
            Gizmos.color = Color.red; // Draw the main ground check box in red
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f));
            Gizmos.color = Color.yellow; // Draw the right edge check box in yellow
            Gizmos.DrawWireCube(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector3(0.2f, groundCheckHeight, 0f));// Draw the left edge check box in yellow
            Gizmos.DrawWireCube(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector3(0.2f, groundCheckHeight, 0f)); // Draw the left edge check box in yellow
        }
    }
}
