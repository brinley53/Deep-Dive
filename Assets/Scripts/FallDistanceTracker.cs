
/*
FallDistanceTracker.cs
Description: Script for tracking how far the player has fallen
Creation date: 11/10/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation
**/
using UnityEngine;
using TMPro;

public class FallDistanceTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText; // object for the displayed text
    private float startY; // starting location of the player
    float currentHeight;
    float currentFallDistance;
    
    [HideInInspector] public float maxFallDistance; // float to track the lowest the player has gone
    [HideInInspector] public Vector3 respawnLocation; // location to respawn player at

    public void SetRespawnLocation() {
        respawnLocation = new Vector3(-5, -currentFallDistance); // set the respawn location to be the max fall distance
    }

    void Start()
    {
        startY = transform.position.y; // get the starting location of the player
        maxFallDistance = 0f;
        respawnLocation = new Vector3(0,0);
        currentFallDistance = 0.0f;
    }

    void Update()
    {
        currentHeight = transform.position.y; // set the current height to the location of the player on every frame
        currentFallDistance = startY - currentHeight; // get how far down the player currently is

        // Only track distance when falling
        if (currentHeight < startY)
        {
            maxFallDistance = Mathf.Max(maxFallDistance, currentFallDistance); // set the distance to display to be the maximum of the lowest the player has gone and the current depth
        }

        // Update UI text
        // distanceText.text = $"Fall Distance: {maxFallDistance:F1}m";
        distanceText.text = $"Depth: {currentFallDistance:F1}m";
    }
} 