/**
BasicEnemyMovement.cs
Description: File to control basic back and forth enemy movement
Creation date: 11/17/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Blackthornprod (youtube)
**/
using System;
using System.Threading;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Movement speed of the player in units per second
    public Transform groundCheck; // Reference to an empty GameObject that marks where to check for ground
    public LayerMask groundLayer; // Layer mask to specify what layers should be considered as ground
    public float groundCheckWidth = 1f; // Width of the main ground check box
    public float groundCheckHeight = 0.2f; // Height of the ground check box

    private Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    private Vector2 movement; // Stores the current movement input (-1 to 1)
    private bool isGrounded; // Tracks whether the player is currently touching the ground

    public Animator animator; // Add an animator for animating the player character

    private bool facingRight = false;

    private float oldX = 1f;

    void Start() // Called once when the script is first enabled
    {
        rb = GetComponent<Rigidbody2D>(); // Get and store reference to the Rigidbody2D component
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent the player from rotating
    }

    void Update() // Called every frame
    {
        if (isGrounded == false) {
            transform.Rotate(new UnityEngine.Vector3(0,180,0)); // Rotate the object 180 degrees
            facingRight = !facingRight;
        }
        movement.x = facingRight ? 1 : -1;
        
        bool wasGrounded = isGrounded; // Store the previous grounded state for comparison
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer);
        
        // animator.SetFloat("Horizontal", Math.Abs(movement.x * moveSpeed)); // Set the animator's x to reference in animator 
    }

    void FixedUpdate() // Called at a fixed time interval (better for physics calculations)
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y); // Apply horizontal movement
        
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
