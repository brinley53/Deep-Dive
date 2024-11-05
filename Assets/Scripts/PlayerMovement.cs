using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f; // Speed of the player
    public float jumpForce = 7f; // Force applied when the player jumps
    public Transform groundCheck; // Transform to check if the player is grounded
    public float groundCheckRadius = 1f; // Radius of the ground check area
    public LayerMask groundLayer; // Layer mask to identify what is considered ground
    public float groundCheckWidth = 1f; // Width of the ground check box
    public float groundCheckHeight = 0.2f; // Height of the ground check box

    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private Vector2 movement; // Vector to store movement input
    private bool isGrounded; // Boolean to check if the player is on the ground

    void Start() // Unity method called once when the script is enabled
    {
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component attached to the player
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent the Rigidbody2D from rotating

        // Log the ground layer mask value for debugging
        Debug.Log("Ground Layer Mask: " + groundLayer.value);
    }

    void Update() // Unity method called once per frame
    {
        movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input for movement

        bool wasGrounded = isGrounded; // Store the previous grounded state

        // Check three points: center and both edges
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer);

        // Log if the grounded state has changed
        if (isGrounded != wasGrounded)
        {
            Debug.Log($"Grounded state changed to: {isGrounded}"); // Log the grounded state
        }

        if (Input.GetButtonDown("Jump")) // Check if the jump button is pressed
        {
            Debug.Log("Jump button pressed"); // Log jump button press
            if (isGrounded) // Check if the player is grounded
            {
                Debug.Log("Applying jump force"); // Log jump force application
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply upward force to jump
            }
            else
            {
                Debug.Log("Can't jump - not grounded"); // Log if jump is not possible
            }
        }
    }

    void FixedUpdate() // Unity method called at a fixed interval
    {
        // Set the player's horizontal velocity based on input and move speed
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);

        float maxFallSpeed = -20f; // Maximum speed the player can fall
        if (rb.linearVelocity.y < maxFallSpeed) // Check if the player is falling too fast
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed); // Limit the fall speed
        }
    }

    void OnDrawGizmos() // Unity method to draw gizmos in the editor
    {
        if (groundCheck != null) // Check if groundCheck is assigned
        {
            // Draw main ground check box
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f));
            
            // Draw edge check boxes
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector3(0.2f, groundCheckHeight, 0f));
            Gizmos.DrawWireCube(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector3(0.2f, groundCheckHeight, 0f));
        }
    }
}
