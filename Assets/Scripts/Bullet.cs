/* 
Bullet.cs
Description: File that controls the harpoon bullets when they get shot
Authors: Ben Renner, Brinley Hull, Connor Bennudriti, Gianni Louisa, Kyle Moore
Creation date: 11/23/24
Other sources of code: Muddy Wolf (youtube)
*/


using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 12f; // Speed of the harpoons
    private float timeAlive = 3f; // Time the bullet stays defined if it hasn't hit anything
    private Rigidbody2D rb; // The rigidbody that controls the actual harpoon object
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Creates the rigidbody by getting the harpoon spawned from the game
        Destroy(gameObject, timeAlive); // Destroys the harpoon after the time specified to stay alive
    }

    // Update is called once per frame
    [System.Obsolete]
    void FixedUpdate()
    {
        rb.velocity = transform.right * speed; // Shoot the harpoon forward 
    }
}
