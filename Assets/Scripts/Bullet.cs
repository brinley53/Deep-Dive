/* 
Author:
Creation date: 11/23/24
Other sources of code: Muddy Wolf (youtube)
*/

using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private float speed = 12f; // Speed of the harpoons
    private float timeAlive = 3f; // Time the bullet stays defined if it hasn't hit anything

    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, timeAlive);
    }

    // Update is called once per frame
    [System.Obsolete]
    void FixedUpdate()
    {
        rb.velocity = transform.right * speed;
    }
}
