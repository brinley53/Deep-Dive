/**
SpikePlatformLogic.cs
Description: File that handles damaging the player after landing on a spike platform
Creation date: 11/24/24
Authors: Connor Bennudriti
Other sources of code: None

Revisions
* None
Preconditions:
* Script must be attached to the Spike platform prefabs (all sizes)
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

public class SpikePlatformLogic : MonoBehaviour
{
    public int spikePlatformDamage = 10; // the amount of damage for the spike platforms to do to the player

    // When something enters the collider for the object this script is attached to
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // When the player lands on the spike platform
        if (collision.gameObject.CompareTag("Player"))
        {
            // If the last thing that damaged the player was NOT the container for this platform segment 
            if (this.transform.parent != collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource) {
                collision.gameObject.GetComponent<PlayerMovement>().TakeDamage(spikePlatformDamage); // player take damage
                Debug.Log("Player landed on spike platform and took 10 damage");
                collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource = this.transform.parent; // set the previous damage source to this platform segmen's container
            }
            else {
                Debug.Log("Player was already hit by this spike platform");
            }
        }
    }
}
