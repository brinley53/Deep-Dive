/*
    Script name: PlayerInventory
    Description: Updates Inventory UI with items the player has collected. Includes oxygen logic
    Inputs: Whatever Items the Player colliedes with. This script needs a text element for each item there is.
    Outputs: UI element that tracks items player has collected
    Sources of code: ChatGPT, discussions.unity.com
    Authors: Kyle Moore, Gianni Louisa, Connor Bennudriti, Ben Renner, Brinley Hull
    Creation Date: 11/24/24
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerInventory : MonoBehaviour
{
    public int harpoonCount = 0;    //Counter for harpoons
    public int heartCount = 0;  //Counter for hearts

    public int oxygen = 100; // Keep track of oxygen

    private int nextUpdate = 1; // time tracker to deplete oxygen

    public PlayerMovement player; // Reference playermovement

    //Reference to UI Text elements 
     public TMP_Text harpoonCountText;  
     public TMP_Text heartCountText;

     public TMP_Text oxygenLevel;

    void Start()
    {
        //Initialize the UI with the current item counts
        UpdateUI();
    }

    //Method to add items to the inventory
    public void AddItem(string itemType)
    {
        if (itemType == "Harpoon")
        {
            harpoonCount++;  //Increment the harpoon counter
            Debug.Log("Harpoon count: " + harpoonCount);
        }
        else if (itemType == "Heart")
        {
            heartCount++;  //Increment the heart counter
            Debug.Log("Heart count: " + heartCount);
        }
        UpdateUI();  //Update the UI with new counts
    }

    void Update() {

        // Depletes oxygen every second, code helped by Unity discussion board Topper_Harley
        if(Time.time >= nextUpdate){
    		// Change the next update (current second+1)
    		nextUpdate=Mathf.FloorToInt(Time.time)+1;
    		depleteOxygen(); // deplete the oxygen
            if (oxygen <= 0) {
                player.Die();
            }
    	}
    }

    private void depleteOxygen() { // Slowly deplete player's oxygen by one
        oxygen -= 1;
    }

    public void RefillOxygen() { // Refill player's oxygen to full
        oxygen = 100;
    }

    //Method to update the UI with the current counts
    private void UpdateUI()
    {
        //Update the UI elements with the current counts
        if (harpoonCountText != null)
            harpoonCountText.text = "Harpoons: " + harpoonCount;

        if (heartCountText != null)
            heartCountText.text = "Hearts: " + heartCount;

        if (oxygenLevel != null)
            oxygenLevel.text = "Oxygen: " + oxygen;
    }
}

