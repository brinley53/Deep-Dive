/*
    Script name: CollectItem
    Description: Item dissapears when player touches it
    Inputs: Object just needs collision detection and check event trigger box and each item needs this script attached
    Outputs:
    Sources of code: None
    Authors: Kyle Moore
    Creation Date: 11/9/24
*/
using UnityEngine;
public class DisappearOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object colliding is tagged as "Player"
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false); // Set the game object to inactive, making it disappear
            // Destroy(gameObject);
        }
    }
}