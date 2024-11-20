/**
RespawnControl.cs
Description: Script for handling player respwawn
Creation date: 11/13/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation
**/
using UnityEngine;

public class RespawnControl : MonoBehaviour
{

    GameObject player; // the player object
    Vector3 respawnPlatformLocation; // location to respawn player at

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // Get the player gameobject for its position

    }

    void Update()
    {
        GameObject checkpointTrigger = GameObject.Find("CheckpointTrigger"); // repeatedly call since new checkpoints spawn in
        if (checkpointTrigger != null) {
            respawnPlatformLocation = player.GetComponent<FallDistanceTracker>().respawnLocation;
            // Debug.Log(respawnPlatformLocation);
            
            // If "R" is pressed 
            if (Input.GetKeyDown(KeyCode.R)) // DEBUGGING ONLY - will be replaced with on player death
            {
                if (player.GetComponent<PlayerMovement>().lives > 0) {
                    player.GetComponent<PlayerMovement>().lives--;
                    Debug.Log($"Respawn at {respawnPlatformLocation}. Lives remaining: {player.GetComponent<PlayerMovement>().lives}");
                    player.transform.position = respawnPlatformLocation;
                } else {
                    Debug.Log("Game Over: No lives remaining.");
                    // Implement game over logic here, e.g., stop the game or show a game over screen
                }
            }
        }
    }
}
