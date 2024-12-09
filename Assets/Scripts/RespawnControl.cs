/*
RespawnControl.cs
Description: This script handles the respawn logic for the player character in a Unity game.
Programmer: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Date Created: 11/13/2024
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials
Revisions:
- Added required comments (12/8/2024, Gianni-Louisa)
- Fixed buggy respawning (11/23/2024, cbennudr)
- Added Player death, player damage, player attributes, enemy attack (11/19/2024, Gianni-Louisa)
- Added respawn functionality (11/13/2024, cbennudr)


Preconditions:
- The GameObject this script is attached to must have a tag "Player" assigned to the player character.
- The player GameObject must have a component 'FallDistanceTracker' that provides the respawn location.

Postconditions:
- The player will be respawned at the last checkpoint location when triggered.

Acceptable Input:
- The player GameObject must be tagged as "Player".
- The 'FallDistanceTracker' component must be present on the player GameObject.

Unacceptable Input:
- Missing or incorrectly tagged player GameObject will result in a null reference error.
- Missing 'FallDistanceTracker' component will result in incorrect respawn location.

Error and Exception Conditions:
- NullReferenceException if the player GameObject or 'FallDistanceTracker' component is not found.

Side Effects:
- Changes the player's position to the respawn location.

Invariants:
- The respawn location is always a valid Vector3 position.

Known Faults:
- None documented.

*/

using UnityEngine;

// Class: RespawnControl
// Description: Manages the respawn logic for the player character.
public class RespawnControl : MonoBehaviour
{
    GameObject player; // Reference to the player GameObject
    Vector3 respawnPlatformLocation; // Location to respawn the player at

    // Method: RespawnPlayerAtPlatform
    // Description: Moves the player to the respawn platform location.
    public void RespawnPlayerAtPlatform() {
        player.transform.position = respawnPlatformLocation; // Set player's position to the respawn location
    }

    // Method: Start
    // Description: Initializes the player reference and sets up initial state.
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Find the player GameObject by tag
    }

    // Method: Update
    // Description: Continuously checks for new checkpoints and handles respawn logic.
    void Update()
    {
        GameObject checkpointTrigger = GameObject.Find("CheckpointTrigger"); // Find the checkpoint trigger GameObject
        if (checkpointTrigger != null) { // Check if the checkpoint trigger exists
            respawnPlatformLocation = player.GetComponent<FallDistanceTracker>().respawnLocation; // Get the respawn location from the player's FallDistanceTracker component
            
            // If "R" is pressed, respawn the player (for debugging purposes)
            if (Input.GetKeyDown(KeyCode.R)) // Check if the "R" key is pressed
            {
                RespawnPlayerAtPlatform(); // Respawn the player at the platform location
                // Debugging code for lives and game over logic (commented out)
                // if (player.GetComponent<PlayerMovement>().lives > 0) {
                //     player.GetComponent<PlayerMovement>().lives--;
                //     Debug.Log($"Respawn at {respawnPlatformLocation}. Lives remaining: {player.GetComponent<PlayerMovement>().lives}");
                //     RespawnPlayerAtPlatform(respawnPlatformLocation);
                // } else {
                //     Debug.Log("Game Over: No lives remaining.");
                //     // Implement game over logic here, e.g., stop the game or show a game over screen
                // }
            }
        }
    }
}