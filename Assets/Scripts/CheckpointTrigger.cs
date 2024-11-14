/**
CheckpointTrigger.cs
Description: Script for handling setting checkpoints
Creation date: 11/13/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation
**/

using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{

    GameObject player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // get the player gameobject for its position
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log("Landed on checkpoint:");
        player.GetComponent<FallDistanceTracker>().SetRespawnLocation();
    }
}
