/**
CheckpointTrigger.cs
Description: Script for handling setting checkpoints
Creation date: 11/13/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation

Revisions
* 12/7/24 - Connor: added tracking of the number of checkpoints hit for difficultly scaling
Preconditions:
* Script must be attached to the GameManager object 
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
**/

using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{

    GameObject player; // the player object
    [HideInInspector] public int numCheckpointsHit; // int to track how many checkponts have been hit 
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // get the player gameobject for its position
        numCheckpointsHit = 0;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log("Landed on checkpoint:");
        player.GetComponent<FallDistanceTracker>().SetRespawnLocation(); // set the spawn location in the FallDistanceTracker script to the checkpoints location
        player.GetComponent<PlayerMovement>().numCheckpointsHit++; // increment the counter for the number of checkpoints hit in the PlayerMovemtn script
    }
}
