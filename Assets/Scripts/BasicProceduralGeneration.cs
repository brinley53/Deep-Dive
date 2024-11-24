
/**
BasicProceduralGeneration.cs
Description: Script for procedurally generating the level layout
Creation date: 11/10/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation
**/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BasicProceduralGeneration : MonoBehaviour
{
    GameObject player; // the player object

    /* Platforms */
    GameObject platformPrefab; // the prefab object for a basic platform
    GameObject platformPrefab2;
    GameObject harpoonItem;
    GameObject heartItem;
    GameObject[] basicPlatformsArray;


    GameObject checkpointSectionPrefab; // the prefab for a checkpoint section

    public float platformMinYDistance = 1f;
    public float platformMaxYDistance = 10f;
    public int maxNumberPlatforms = 50; // number of platforms to spawn 

    // Array to store the spawned platforms
    GameObject[] platformsArray;
    Vector3 lowestPlatformPos;
    Vector3 checkpointSectionSpawnLocation;
    GameObject spawnedPlatformsContainer; // Create an empty gameobject to store the instantiated platforms so they can be refereneced after creation
    GameObject instantiatedPlatform; 
    GameObject platformTypeToInstantiate; // variable to hold the type of the next platofrm to spawn

    private int distanceBetweenFinalPlatformAndCheckpointSection = 15;
    private int distanceBetweenCheckpointSectionAndPlatformStart = 5;

    Vector3 getNextPlatformPos(Vector3 prevPlatformPos) {
        // Set next x position to a random x position on the screen that does not overlap with the previous platform's x position
        int nextXPos;
        while (true) {
            nextXPos = Random.Range(-10,10);
            if (nextXPos != prevPlatformPos.x) { break; }
        }
        // Ensure next platform spawns below the previous platform by a random distance in the given range
        int nextYPos = Random.Range((int)(prevPlatformPos.y-platformMinYDistance), (int)(prevPlatformPos.y-platformMaxYDistance));
        return new Vector3(nextXPos,nextYPos);
    }

    /// <summary> Function to get the type of the next platform (eg regular, spikes, etc) </summary>
    GameObject getNextPlatformType(Vector3 platformPos) {
        return basicPlatformsArray[Random.Range(0, basicPlatformsArray.Length)];
    }

  void spawnPlatformGroup(int numberPlatformsPerGroup, Vector3 startingPos) {
    // Create a new vector 3d to hold location for platform locations
    Vector3 spawnPos = startingPos;
    // Create the specified number of platforms
    for (int i = 0; i < numberPlatformsPerGroup; i++) {
        spawnPos = getNextPlatformPos(spawnPos);
        platformTypeToInstantiate = getNextPlatformType(spawnPos);

        // Create a new platform at the specified location
        instantiatedPlatform = Instantiate(platformTypeToInstantiate, spawnPos, Quaternion.identity); 
        instantiatedPlatform.transform.parent = spawnedPlatformsContainer.transform; // Assign parent for cleanliness
        platformsArray[i] = instantiatedPlatform; // Add the platform to the array

        // Chance-based item spawning
        spawnItemOnPlatform(instantiatedPlatform.transform.position);
    }
    lowestPlatformPos = platformsArray[numberPlatformsPerGroup - 1].transform.position;

    checkpointSectionSpawnLocation = new Vector3(0, lowestPlatformPos.y - distanceBetweenFinalPlatformAndCheckpointSection);
    // Instantiate a checkpoint section after the platform group
    Instantiate(checkpointSectionPrefab, checkpointSectionSpawnLocation, Quaternion.identity);
}

/// <summary>
/// Spawns an item (heart or harpoon) on the given platform position with a chance.
/// </summary>
/// <param name="platformPos">Position of the platform</param>
void spawnItemOnPlatform(Vector3 platformPos) {
    // Random chance for item generation (adjust probabilities as needed)
    float spawnChance = Random.value; // Generates a value between 0.0 and 1.0
    GameObject itemToSpawn = null;

    if (spawnChance <= 0.5f) { // 10% chance to spawn a harpoon item
        itemToSpawn = harpoonItem;
    } else if (spawnChance > 0.5f && spawnChance <= 0.2f) { // Additional 10% chance for a heart item
        itemToSpawn = heartItem;
        
    }

    // If an item is chosen, spawn it slightly above the platform
    if (itemToSpawn != null) {
        Vector3 itemSpawnPos = new Vector3(platformPos.x, platformPos.y + 1f, platformPos.z);
        Instantiate(itemToSpawn, itemSpawnPos, Quaternion.identity);
        Debug.Log("Spawned item: " + itemToSpawn.name + " at position: " + itemSpawnPos);
}

}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the player gameobject for its position
        player = GameObject.FindGameObjectWithTag("Player");

        // Get the platform prefab and get each image for each item
        platformPrefab = Resources.Load("prefabs/Platform") as GameObject; 
        platformPrefab2 = Resources.Load("prefabs/Platform2") as GameObject;
        harpoonItem = Resources.Load("prefabs/harpoon_item_0") as GameObject;
        heartItem = Resources.Load("prefabs/heart_item_0") as GameObject;
        basicPlatformsArray = new GameObject[] {
            platformPrefab,
            platformPrefab2
        };
        Debug.Log("Harpoon Item Loaded: " + (harpoonItem != null));
        Debug.Log("Heart Item Loaded: " + (heartItem != null));


        // Get the checkpoint section prefab
        checkpointSectionPrefab = Resources.Load("prefabs/CheckpointSection") as GameObject;
        // Create a new checkpoiont section at the start
        //Instantiate(checkpointSectionPrefab, new Vector3(0,0), Quaternion.identity);

        // Create a new empty gmaeobject to hold the spawned platforms
        spawnedPlatformsContainer = new GameObject("spawnedPlatformsContainer");
        // Give the platform array a length
        platformsArray = new GameObject[maxNumberPlatforms];
        // Spawn a group of platforms
        spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,-distanceBetweenCheckpointSectionAndPlatformStart));
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(player.transform.position.x);
        // Debug.Log(player.transform.position.y);
        // Debug.Log(platformsArray[10]);

        // If player is below the lowest platform spawned in the previous group
        if (player.transform.position.y <= checkpointSectionSpawnLocation.y) {
            // Debug.Log("Destroying all platforms");
            // Destroy platforms from prevoius group
            for (int i = 0; i < maxNumberPlatforms; i++) {
                Destroy(platformsArray[i]);
            }
            // Spawn a new group of platforms
            spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,checkpointSectionSpawnLocation.y-distanceBetweenCheckpointSectionAndPlatformStart));
        }
    }
}
