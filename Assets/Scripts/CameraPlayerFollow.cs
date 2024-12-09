/**
CameraPlayerFollow.cs
Description: Script for procedurally generating the level layout
Creation date: 10/26/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: None

Revisions
* None
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

public class playerFollow : MonoBehaviour
{
    public GameObject player; // drag the player object from the hierarchy over to the the spot for this in the inspector

    // Update is called once per frame
    void Update()
    {
        // on every frame, set the camera's y position to be the player's y position
        transform.position = new Vector3(0, player.transform.position.y, -10);
    }
}
