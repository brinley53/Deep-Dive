
/**
ShopCollision.cs
Description: Script for handling the player opening the shop
Creation date: 10/26/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation

Revisions
* None
Preconditions:
* Script must be attached to the GameManager object shop collider object in the checkpoint prefab
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

public class ShopCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("Entered shop trigger");
    }
}
