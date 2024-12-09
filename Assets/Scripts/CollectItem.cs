/*
    Script name: CollectItem
    Description: Item disappears when player touches it
    Inputs: Item Object just needs collision detection and check event trigger box and each item needs this script attached
    Also each item prefab needs the correct tag for the player to recognize it
    Outputs:
    Sources of code: None
    Authors: Kyle Moore, Gianni Louisa, Ben Renner, Connor Bennudriti, Brinley Hull
    Creation Date: 11/9/24
    Revisions
        Commits on Dec 4, 2024
        Brinley: bubbles and oxygen death
        Commits on Nov 24, 2024
        Connor: Moved start of platform spawning downwards so platforms dont spawn on the ship
        Kyle: added very basic inventory that updates when player collects each item and TMP dependencies
        Kyle: colision works again but does not add item to inventory ui yet

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
*/

using UnityEngine;
public class ItemCollector : MonoBehaviour
{

    public GameObject harpoonItem;  //Declare harpoon item
    public GameObject heartItem;    //Declare heart item
    public PlayerInventory playerInventory;  //Reference to PlayerInventory

    void Start()
    {
        //Load the prefabs from the Resources folder
        playerInventory = GetComponent<PlayerInventory>();
        harpoonItem = Resources.Load("prefabs/harpoon_item_0") as GameObject;
        heartItem = Resources.Load("prefabs/heart_item_0") as GameObject;

        //Check if they were loaded correctly
        if (harpoonItem == null)
        {
            Debug.LogError("HarpoonItem prefab not found!");
        }
        if (heartItem == null)
        {
            Debug.LogError("HeartItem prefab not found!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the collided object is a harpoon item
        if (other.gameObject.CompareTag("HarpoonItem"))
        {
            playerInventory.AddItem("Harpoon"); //Calls the inventory Script to update the UI correctly
            Destroy(other.gameObject); //Destroy the collected item
            // Debug.Log("Harpoon collected!"); //for debuging purposes log
        }
        // heck if the collided object is a heart item
        else if (other.gameObject.CompareTag("HeartItem"))
        {
            playerInventory.AddItem("Heart"); //Calls the inventory Script to update the UI correctly
            Destroy(other.gameObject);  //Destroy the collected item
            // Debug.Log("Heart collected!"); //for debuging purposes log
        } else if (other.gameObject.CompareTag("Bubble")) { // If the player collides with the bubbles
            // Refill oxygen level
            playerInventory.RefillOxygen();
            Destroy(other.gameObject); // Destroy the collected item
        }
    }
}
