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