/*
PlayerMovement.cs
Description: This script manages the player's movement, jumping, and interactions in a 2D game.
Programmer: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Date Created: 10/26/2024
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials

Revisions:
- Added required comments (12/8/2024, Gianni-Louisa)
- Working Player Audio with enemy audio commented out (12/8/2024, KyleMoore12)
- Added sound (12/8/2024, TrixTheWolf)
- Fix Scoreboard (12/8/2024, KyleMoore12)
- 1 high score (12/8/2024, KyleMoore12)
- Health and oxygen bars (12/7/2024, KyleMoore12)
- Screen wrapping (12/7/2024, KyleMoore12)
- Fixed item collection fr (12/7/2024, KyleMoore12)
- Fixed elements not getting deleted after passing checkpoint (12/7/2024, cbennudr)
- Added damaging platform frequency scaling dependent on depth (12/7/2024, cbennudr)
- Added background music and fixed some errors with player object not being referenced (12/7/2024, cbennudr)
- Added lose and pause screen (12/6/2024, Gianni-Louisa)
- Health and oxygen bars (12/5/2024, brinley53)
- Bubbles and oxygen death (12/4/2024, brinley53)
- Comments (11/24/2024, brinley53)
- Added spike and magma platforms (11/24/2024, cbennudr)
- Player attack (11/24/2024, brinley53)
- Added I-frames (11/23/2024, cbennudr)
- Added Player death, player damage, player attributes, enemy attack (11/19/2024, Gianni-Louisa)
- Janky enemy movement (11/17/2024, brinley53)
- Added so player can fall through platforms if they press down arrow (11/15/2024, Gianni-Louisa)
- Added respawn functionality (11/13/2024, cbennudr)
- Comments for the script (11/10/2024, Gianni-Louisa)
- Rest of animation :D (11/9/2024, brinley53)
- Little dude walks and flips now :) (11/8/2024, brinley53)
- Added commenting (11/5/2024, Gianni-Louisa)
- Added fall distance tracker and made jump checking more efficient (11/5/2024, Gianni-Louisa)
- Added limiter for fall speed (10/31/2024, Gianni-Louisa)
- Basic procedural generation is now infinite (10/26/2024, cbennudr)
- Implementing basic procedural generation - platform spawning done (10/26/2024, cbennudr)


Preconditions:
- The GameObject this script is attached to must have Rigidbody2D, SpriteRenderer, and Animator components.
- The 'groundCheck' Transform must be assigned in the Unity Editor.
- The 'groundLayer' must be set to the appropriate layer(s) considered as ground.

Postconditions:
- The player can move, jump, shoot, and interact with the environment.
- The player's health and lives are managed and updated in the UI.

Acceptable Input:
- 'groundCheck' should be a valid Transform.
- 'groundLayer' should be a valid LayerMask.
- Input values for movement and actions should be within expected ranges.

Unacceptable Input:
- Null or unassigned 'groundCheck' or 'groundLayer' will result in incorrect behavior.

Error and Exception Conditions:
- None explicitly handled in this script.

Side Effects:
- Changes the player's position, health, and UI elements.
- Plays audio clips for various actions.

Invariants:
- The player's health is always between 0 and maxHealth.
- The player's movement is constrained to the x-axis.

Known Faults:
- None documented.

*/

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Class: PlayerMovement
// Description: Manages the player's movement, jumping, and interactions.
public class PlayerMovement : MonoBehaviour
{
    public FallDistanceTracker distanceScript; // Reference to the fall distance tracker
    public float finalScore; // Final score based on fall distance
    public float moveSpeed = 4f; // Movement speed of the player in units per second
    public float jumpForce = 7f; // Force applied upward when jumping
    public int strength = 10; // Player's strength attribute

    public int maxHealth = 100; // Player's max health
    public int health; // Player's current health
    public int lives = 3; // Player's lives (hearts) attribute
    public Transform groundCheck; // Reference to an empty GameObject that marks where to check for ground
    public float groundCheckRadius = 1f; // Radius used for the ground check circle (not currently used since we switched to box)
    public LayerMask groundLayer; // Layer mask to specify what layers should be considered as ground
    public float groundCheckWidth = 1f; // Width of the main ground check box
    public float groundCheckHeight = 0.2f; // Height of the ground check box

    public float iFrameDuration = 2.0f; // The duration of the I-frames after the player gets hit
    float timeOfLastHit = 0.0f; // Time when the player was last hit

    private Rigidbody2D rb; // Reference to the player's Rigidbody2D component
    private Vector2 movement; // Stores the current movement input (-1 to 1)
    private bool isGrounded; // Tracks whether the player is currently touching the ground

    public Animator animator; // Animator for animating the player character

    private bool isFallingThrough = false; // Whether the player is currently falling through platforms
    private float fallThroughDuration = 0.6f; // Duration to remain in NoCollision layer after releasing Ctrl
    private float fallThroughTimer = 0f; // Timer for falling through platforms

    public Image[] hearts; // Array to hold heart images
    public UIBar healthBar; // Reference to the health bar slider
    public Text attributeText; // Reference to the text displaying attributes

    public AudioSource audioSource; // Audio manager
    public AudioClip jumpSound; // Sound for the jump
    public AudioClip playerHit; // Sound for the player getting hurt
    public AudioClip playerDeath; // Sound for the player dying
    public AudioClip playerShoot; // Sound for player shooting 

    private SpriteRenderer spriteRenderer; // Reference to the player's SpriteRenderer component

    [HideInInspector] public Transform previousDamageSource; // Previous source of damage

    // Gun variables
    [SerializeField] private GameObject bullet; // Harpoon objects
    [SerializeField] private Transform firingPoint; // The point where the harpoon shoots from
    private float fireRate = 0.4f; // Fire rate of harpoon gun (larger number is slower)

    private float fireTimer; // Timer to make the gun wait before being able to shoot again

    public UIManager uim; // Reference to the UI manager

    [HideInInspector] public int numCheckpointsHit; // Number of checkpoints hit

    // Method: Awake
    // Description: Ensures the player object is not destroyed on scene load.
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // Prevent the player object from being destroyed on scene load
    }

    // Method: Start
    // Description: Initializes components and sets up initial state.
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        numCheckpointsHit = 0; // Initialize checkpoints hit
        health = maxHealth; // Set health to max health
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        animator = GetComponent<Animator>(); // Get the Animator component
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent the player from rotating
        UpdateUI(); // Update the UI elements
    }

    // Method: Update
    // Description: Handles input and updates player state every frame.
    void Update()
    {
        Wrapping(); // Handle screen wrapping
        movement.x = Input.GetAxisRaw("Horizontal"); // Get horizontal input (-1 for left, 1 for right, 0 for no input)
        
        bool wasGrounded = isGrounded; // Store the previous grounded state for comparison
        isGrounded = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer) ||
                     Physics2D.OverlapBox(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector2(0.2f, groundCheckHeight), 0f, groundLayer);

        // If not grounded, reset the previous damage source
        if (!isGrounded) {
            previousDamageSource = null;
        }

        // Log when the grounded state changes
        if (isGrounded != wasGrounded) {
            // Debug.Log($"Grounded state changed to: {isGrounded}");
        }

        // Handle jump input
        if (Input.GetButtonDown("Jump") && isGrounded) {
            animator.SetBool("Jump", true); // Trigger jump animation
        } else if (Input.GetButtonUp("Jump")) {
            if (isGrounded) {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply jump force
                audioSource.PlayOneShot(jumpSound); // Play jump sound
            }
        } else {
            animator.SetBool("Jump", false); // Reset jump animation
        }

        movement.y = rb.linearVelocity.y; // Get the movement speed in the vertical axis
        animator.SetFloat("Horizontal", Math.Abs(movement.x * moveSpeed)); // Set animator's horizontal speed
        animator.SetFloat("Vertical", movement.y); // Set animator's vertical speed
    
        // Handle fall through platforms
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            animator.SetBool("Jump", true); // Trigger crouch animation
            int noCollisionLayer = LayerMask.NameToLayer("NoCollision");
            if (noCollisionLayer != -1) {
                if (gameObject.layer != noCollisionLayer) {
                    gameObject.layer = noCollisionLayer; // Change to "NoCollision" layer
                    isFallingThrough = true;
                    fallThroughTimer = fallThroughDuration; // Reset the timer
                    Debug.Log("Player layer set to NoCollision");
                }
            } else {
                Debug.LogError("NoCollision layer not found. Please ensure it is created in Unity.");
            }
        } else if (isFallingThrough) {
            fallThroughTimer -= Time.deltaTime;
            if (fallThroughTimer <= 0f) {
                int defaultLayer = LayerMask.NameToLayer("Default");
                if (gameObject.layer != defaultLayer) {
                    gameObject.layer = defaultLayer;
                    isFallingThrough = false;
                    Debug.Log("Player layer set to Default");
                }
            }
        }

        UpdateColorBasedOnHealth(); // Update player color based on health

        // Handle shooting input
        if (Input.GetKey(KeyCode.F) && fireTimer <= 0f) {
            animator.SetBool("Shoot", true); // Trigger shoot animation
            Shoot(); // Shoot the bullet
            fireTimer = fireRate; // Reset the timer
        } else {
            fireTimer -= Time.deltaTime; // Decrease the timer
        }

        if (Input.GetKeyUp(KeyCode.F)) {
            animator.SetBool("Shoot", false); // Reset shoot animation
        }

        // Update final score based on fall distance
        if(finalScore < distanceScript.maxFallDistance) {
            finalScore = distanceScript.maxFallDistance;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y); // Apply horizontal movement
        float maxFallSpeed = -20f; // Maximum speed the player can fall
        if (rb.linearVelocity.y < maxFallSpeed) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed); // Clamp fall speed
        }
    }

    private void Shoot()
    {
        Instantiate(bullet, firingPoint.position, firingPoint.rotation); // Instantiate the bullet
        audioSource.PlayOneShot(playerShoot); // Play shooting sound
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null) {
            Gizmos.color = Color.red; // Set Gizmo color to red
            Gizmos.DrawWireCube(groundCheck.position, new Vector3(groundCheckWidth, groundCheckHeight, 0f)); // Draw main ground check box
            Gizmos.color = Color.yellow; // Set Gizmo color to yellow
            Gizmos.DrawWireCube(groundCheck.position + new Vector3(groundCheckWidth/2, 0, 0), new Vector3(0.2f, groundCheckHeight, 0f)); // Draw right edge check box
            Gizmos.DrawWireCube(groundCheck.position + new Vector3(-groundCheckWidth/2, 0, 0), new Vector3(0.2f, groundCheckHeight, 0f)); // Draw left edge check box
        }
    }

    // Method: HighScoreUpdate
    // Description: Updates the high score if the current score is higher.
    public void HighScoreUpdate()
    {
        Debug.Log("In High Score Function");
        if (PlayerPrefs.HasKey("SavedHighScore")) {
            Debug.Log("First if");
            if(finalScore > PlayerPrefs.GetFloat("SavedHighScore")) {
                Debug.Log("in second if");
                PlayerPrefs.SetFloat("SavedHighScore", finalScore); // Set new high score
            }
        } else {
            PlayerPrefs.SetFloat("SavedHighScore", finalScore); // Set high score if none exists
        }
    }

    // Method: Die
    // Description: Handles player death, life decrement, and game over logic.
    public void Die()
    {
        lives--; // Decrement lives
        Debug.Log($"Player died. Lives remaining: {lives}");
        
        if (lives > 0) {
            StartCoroutine(RespawnPlayer()); // Respawn player if lives remain
        } else {
            HighScoreUpdate(); // Update high score
            Debug.Log("Saving Score");
            PlayerPrefs.Save(); // Save player preferences
            Debug.Log(finalScore); // Log final score
            Debug.Log(PlayerPrefs.GetFloat("SavedHighScore"));

            Debug.Log("Game Over: No lives remaining.");
            Time.timeScale = 0f; // Pause the game
            if (uim != null) {
                Debug.Log("Showing lose menu via UIManager...");
                uim.ToggleLoseMenu(); // Show lose menu
            } else {
                Debug.LogError("UIManager reference not set in PlayerMovement! Please assign it in the Unity Inspector.");
            }
        }
    }

    // Method: TakeDamage
    // Description: Reduces player health and handles invincibility frames.
    public void TakeDamage(int damage, bool ignoreIFrames=false)
    {
        if (((timeOfLastHit + iFrameDuration) <= Time.fixedTime) || (ignoreIFrames)) {
            timeOfLastHit = Time.fixedTime; // Update last hit time
            Debug.Log("Player hit");
            audioSource.PlayOneShot(playerHit); // Play hit sound

            health -= damage; // Reduce health
            Debug.Log($"Player health: {health}");

            if (health <= 0) {
                Debug.Log("Player health is zero or less. Calling Die().");
                audioSource.PlayOneShot(playerDeath); // Play death sound
                Die(); // Call Die method
            }
            UpdateUI(); // Update UI elements
        } else {
            // Debug.Log("Player hit but is still invincible from I-frames");
        }
    }

    // Method: UpdateColorBasedOnHealth
    // Description: Updates the player's color based on current health.
    private void UpdateColorBasedOnHealth()
    {
        if (health <= 20) {
            StartCoroutine(FlashRed()); // Flash red if health is low
        } else {
            float redIntensity = 1f - (health / 100f); // Calculate red intensity
            spriteRenderer.color = new Color(1f, 1f - redIntensity, 1f - redIntensity); // Set sprite color
        }
    }

    // Method: FlashRed
    // Description: Coroutine to flash the player red when health is low.
    private IEnumerator FlashRed()
    {
        while (health <= 20) {
            spriteRenderer.color = new Color(1f, 0.5f, 0.5f); // Set color to red
            yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
            spriteRenderer.color = Color.white; // Reset color to white
            yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
        }
    }

    // Method: RespawnPlayer
    // Description: Coroutine to handle player respawn after death.
    private IEnumerator RespawnPlayer()
    {
        spriteRenderer.color = Color.red; // Change color to red
        animator.enabled = false; // Stop animations
        float floatDuration = 2f; // Duration to float upward
        float floatSpeed = 2f; // Speed of floating upward

        float elapsedTime = 0f; // Initialize elapsed time
        Vector2 originalPosition = transform.position; // Store original position

        transform.Rotate(0, 0, 90); // Rotate the player 90 degrees counterclockwise

        while (elapsedTime < floatDuration) {
            transform.position = new Vector2(originalPosition.x, originalPosition.y + (floatSpeed * elapsedTime)); // Move upwards
            elapsedTime += Time.unscaledDeltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        Time.timeScale = 0f; // Pause the game
        yield return new WaitForSecondsRealtime(1f); // Wait for 1 second in real time
        Time.timeScale = 1f; // Resume the game

        transform.Rotate(0, 0, -90); // Reset rotation
        health = maxHealth; // Reset health to full

        spriteRenderer.color = Color.white; // Reset color to white
        animator.enabled = true; // Re-enable animations
        transform.position = GetComponent<FallDistanceTracker>().respawnLocation; // Set position to respawn location
        UpdateUI(); // Update UI elements
        Debug.Log($"Respawned. Lives remaining: {lives}");
    }

    // Method: Wrapping
    // Description: Handles screen wrapping for the player character.
    private void Wrapping()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position); // Get screen position of player

        float rightSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x; // Get right side of screen
        float leftSideOfScreenInWorld = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x; // Get left side of screen

        if(screenPos.x <= 0 && rb.linearVelocity.x < 0) {
            transform.position = new Vector2(rightSideOfScreenInWorld, transform.position.y); // Wrap to right side
        } else if(screenPos.x >= Screen.width && rb.linearVelocity.x > 0) {
            transform.position = new Vector2(leftSideOfScreenInWorld, transform.position.y); // Wrap to left side
        }
    }

    // Method: UpdateUI
    // Description: Updates the UI elements for health, lives, and attributes.
    private void UpdateUI()
    {
        for (int i = 0; i < hearts.Length; i++) {
            hearts[i].enabled = i < lives; // Update hearts based on lives
        }

        healthBar.SetHealth(health); // Update health bar

        attributeText.text = $"Strength: {strength}\nHealth: {health}\nLives: {lives}"; // Update attribute text
    }
}