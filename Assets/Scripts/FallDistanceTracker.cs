/*
FallDistanceTracker.cs
Description: Script for tracking how far the player has fallen in a Unity game.
Creation date: 11/10/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials
Revisions:
- [Date] [Author] [Description of revision]

Preconditions:
- The GameObject this script is attached to must have a Transform component.
- The 'distanceText' must be assigned a valid TextMeshProUGUI component in the Unity Editor.

Postconditions:
- The UI will display the current fall distance of the player.
- The maximum fall distance will be tracked for potential respawn logic.

Acceptable Input:
- 'distanceText' should be a valid TextMeshProUGUI component.

Unacceptable Input:
- Null or unassigned 'distanceText' will result in a null reference error.

Error and Exception Conditions:
- NullReferenceException if 'distanceText' is not assigned.

Side Effects:
- Updates the UI text to reflect the player's fall distance.

Invariants:
- 'currentFallDistance' is always non-negative.
- 'maxFallDistance' is always greater than or equal to 'currentFallDistance'.

Known Faults:
- None documented.

*/

using UnityEngine;
using TMPro;

// Class: FallDistanceTracker
// Description: Manages the tracking and display of the player's fall distance.
public class FallDistanceTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText; // UI element for displaying fall distance
    private float startY; // Initial Y position of the player
    float currentHeight; // Current Y position of the player
    public float currentFallDistance; // Current fall distance from the start position
    
    public float maxFallDistance; // Maximum fall distance recorded
    [HideInInspector] public Vector3 respawnLocation; // Location to respawn the player

    // Method: SetRespawnLocation
    // Description: Sets the respawn location based on the maximum fall distance.
    public void SetRespawnLocation() {
        respawnLocation = new Vector3(-5, -currentFallDistance); // Set respawn location to the max fall distance
    }

    // Method: Start
    // Description: Initializes the starting position and fall distance tracking.
    void Start()
    {
        startY = transform.position.y; // Record the starting Y position
        maxFallDistance = 0f; // Initialize max fall distance
        respawnLocation = new Vector3(0,0); // Initialize respawn location
        currentFallDistance = 0.0f; // Initialize current fall distance
    }

    // Method: Update
    // Description: Updates the current fall distance and UI text each frame.
    void Update()
    {
        currentHeight = transform.position.y; // Get the current Y position
        currentFallDistance = startY - currentHeight; // Calculate the fall distance

        // Only track distance when falling
        if (currentHeight < startY)
        {
            maxFallDistance = Mathf.Max(maxFallDistance, currentFallDistance); // Update max fall distance
        }

        // Update UI text with the current fall distance
        distanceText.text = $"Depth: {currentFallDistance:F1}m"; // Display fall distance in meters
    }
}