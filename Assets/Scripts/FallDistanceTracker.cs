
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
    float maxFallDistance; // float to track the lowest the player has gone
    // bool isFalling;

    void Start()
    {
        startY = transform.position.y; // get the starting location of the player
        maxFallDistance = 0f;
    }

    void Update()
    {
        float currentHeight = transform.position.y; // set the current height to the location of the player on every frame
        float currentFallDistance = startY - currentHeight; // get how far down the player currently is

        // Only track distance when falling
        if (currentHeight < startY)
        {
            // isFalling = true;
            maxFallDistance = Mathf.Max(maxFallDistance, currentFallDistance); // set the distance to display to be the maximum of the lowest the player has gone and the current depth
        }

        // Update UI text
        distanceText.text = $"Fall Distance: {maxFallDistance:F1}m";
    }
} 