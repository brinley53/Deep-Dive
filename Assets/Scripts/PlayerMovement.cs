using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f; // Speed of the player
    public float jumpForce = 7f; // Adjusted force for jumping
    public Transform groundCheck; // Position to check if grounded
    public float groundCheckRadius = 0.2f; // Radius of the ground check
    public LayerMask groundLayer; // Layer of the ground hey

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        // Add this debug line to check which layer we're looking for
        Debug.Log("Ground Layer Mask: " + groundLayer.value);
    }

    void Update()
    {
        // Get input for horizontal movement
        movement.x = Input.GetAxisRaw("Horizontal");

        // Store the previous grounded state
        bool wasGrounded = isGrounded;
        
        // Check if the player is grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Debug information
        // Debug.Log($"Ground Check Position: {groundCheck.position}, Radius: {groundCheckRadius}, Layer: {groundLayer.value}");
        
        if (isGrounded != wasGrounded)
        {
            Debug.Log($"Grounded state changed to: {isGrounded}");
        }

        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump button pressed");
            if (isGrounded)
            {
                Debug.Log("Applying jump force");
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.Log("Can't jump - not grounded");
            }
        }
    }

    void FixedUpdate()
    {
        // Move the player left and right
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    void OnDrawGizmos()
    {
        // Add this to visualize the ground check radius in the Scene view
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
