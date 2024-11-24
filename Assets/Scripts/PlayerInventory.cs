/*
    Script name: PlayerInventory
    Description: Updates Inventory UI with items the player has collected
    Inputs: Whatever Items the Player colliedes with. This script needs a text element for each item there is.
    Outputs: UI element that tracks items player has collected
    Sources of code: ChatGPT
    Authors: Kyle Moore
    Creation Date: 11/24/24
*/

using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class PlayerInventory : MonoBehaviour
{
    public int harpoonCount = 0;    //Counter for harpoons
    public int heartCount = 0;  //Counter for hearts

    //Reference to UI Text elements 
     public TMP_Text harpoonCountText;  
     public TMP_Text heartCountText;

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

    //Method to update the UI with the current counts
    private void UpdateUI()
    {
        //Update the UI elements with the current counts
        if (harpoonCountText != null)
            harpoonCountText.text = "Harpoons: " + harpoonCount;

        if (heartCountText != null)
            heartCountText.text = "Hearts: " + heartCount;
    }
}

