using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BasicProceduralGeneration : MonoBehaviour
{
    GameObject player; // the player object
    GameObject platformPrefab; // from the "Assets/Prefabs" folder, drag the "Platform" prefab to the location for this in the inspector
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
        return platformPrefab;
    }

    void spawnPlatformGroup(int numberPlatformsPerGroup, Vector3 startingPos) {
        // Create a new vector 3d to hold location for platform locations
        Vector3 spawnPos = startingPos;
        // Create the specifed number of platforms
        for (int i = 0; i < numberPlatformsPerGroup; i++) {
            spawnPos = getNextPlatformPos(spawnPos);
            platformTypeToInstantiate = getNextPlatformType(spawnPos);

            instantiatedPlatform = Instantiate(getNextPlatformPos, spawnPos, Quaternion.identity); // create a new platform at the specified location
            instantiatedPlatform.transform.parent = spawnedPlatformsContainer.transform; // set the spawned platofrm's parent to be the empty gameobejct created previously for cleanliness
            platformsArray[i] = instantiatedPlatform; // add the platform to the list keeping track of the platforms
        }
        lowestPlatformPos = platformsArray[numberPlatformsPerGroup-1].transform.position;

        checkpointSectionSpawnLocation = new Vector3(0, lowestPlatformPos.y-distanceBetweenFinalPlatformAndCheckpointSection);
        // Instantiate a checkpoint section after the platform group
        Instantiate(checkpointSectionPrefab, checkpointSectionSpawnLocation, Quaternion.identity);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the player gameobject for its position
        player = GameObject.FindGameObjectWithTag("Player");
        // Get the platform prefab
        platformPrefab = Resources.Load("prefabs/Platform") as GameObject;
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
