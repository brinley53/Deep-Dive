using UnityEngine;

public class AssignRB2DMaterial : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Material slipperyMaterial = Resources.Load("slippery", typeof(Material)) as Material;
        // Rigidbody2D rb2d = GetComponent<Rigidbody2D>();
        Renderer rb2d = GetComponent<Renderer>();
        rb2d.material = slipperyMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
