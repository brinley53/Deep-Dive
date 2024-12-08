/*
PlayerFlip.cs
Description: This script handles the flipping of the player character based on horizontal input in a Unity game.
Programmer: Brinley Hull, Ben Renner, Connor Bennudriti, Gianni Louisa, Kyle Moore
Date Created: 11/8/2024
Other sources of code: ChatGPT, Unity Documentation, Unity Forums, Youtube tutorials

Revisions:
- [Date] [Author] [Description of revision]

Preconditions:
- The GameObject this script is attached to must have a Transform component.
- The input axis "Horizontal" must be configured in Unity's Input Manager.

Postconditions:
- The player character will flip its orientation based on the direction of movement.

Acceptable Input:
- Horizontal input values ranging from -1 (left) to 1 (right).

Unacceptable Input:
- Input values outside the range of -1 to 1 are not expected and may result in undefined behavior.

Error and Exception Conditions:
- None explicitly handled in this script.

Side Effects:
- Rotates the player character's transform by 180 degrees on the Y-axis.

Invariants:
- The 'facingRight' boolean accurately reflects the player's current facing direction.

Known Faults:
- None documented.

*/

using UnityEngine; // Import UnityEngine to use vector math

// Class: PlayerFlip
// Description: Manages the flipping of the player character based on input.
public class PlayerFlip : MonoBehaviour
{
    public float horizontalInput; // Stores the horizontal input value
    public bool facingRight = true; // Indicates if the player is facing right

    // Method: Update
    // Description: Called once per frame to update the player's direction based on input.
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal"); // Get the horizontal input value
        SetDirection(); // Determine and set the player's facing direction
    }

    // Method: SetDirection
    // Description: Flips the player's orientation if the input direction changes.
    private void SetDirection()
    {
        // Check if the player needs to flip direction
        if (horizontalInput < 0 && facingRight || horizontalInput > 0 && !facingRight)
        {
            facingRight = !facingRight; // Toggle the facing direction
            transform.Rotate(new Vector3(0, 180, 0)); // Rotate the player 180 degrees on the Y-axis
        }
    }
}