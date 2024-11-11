/**
PlayerFlip.cs
Description: Flips the player character objects
Creation date: 11/8/2024
Authors: Brinley Hull, Ben Renner, Connor Bennudriti, Gianni Louisa, Kyle Moore
Other sources of code: YouTube Unity tutorials
**/

using System.Numerics;
using UnityEngine; // Import UnityEngine to use vector math

public class PlayerFlip : MonoBehaviour
{
    public float horizontalInput; // Initialize horizontalInput variable
    public bool facingRight = true; // Initialize the facing Right boolean

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // Set the horizontal input to the horizontal axis of the input; negative for left, positive for right
        
        SetDirection(); // Set the direction
    }

    private void SetDirection() {
        if (horizontalInput < 0 && facingRight || horizontalInput > 0 && !facingRight) { // If the horizontal input is left and we're facing right or the horizontal input is right and we're not facing right
            facingRight = !facingRight; // Set the facing right boolean to the opposite side
            transform.Rotate(new UnityEngine.Vector3(0,180,0)); // Rotate the object 180 degrees
        }
    }
}
