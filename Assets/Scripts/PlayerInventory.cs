using UnityEngine;
using UnityEngine.UI;  // Don't forget to include this for Text
using System.Collections.Generic;  // Required for Dictionary


public class PlayerInventory : MonoBehaviour
{
    public Text inventoryText; // Reference to the UI Text
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    // Function to update the inventory UI
    void UpdateInventoryUI()
    {
        string inventoryDisplay = "Inventory:\n";
        foreach (var item in inventory)
        {
            inventoryDisplay += item.Key + ": " + item.Value + "\n";
        }
        inventoryText.text = inventoryDisplay;
    }

    // Function to add items to the inventory
    public void AddItem(string itemName)
    {
        if (inventory.ContainsKey(itemName))
        {
            inventory[itemName]++;
        }
        else
        {
            inventory[itemName] = 1;
        }

        UpdateInventoryUI();  // Update UI after adding an item
    }

    // Call this when an item is collected
    void OnItemCollected(string itemName)
    {
        AddItem(itemName);
    }
}
