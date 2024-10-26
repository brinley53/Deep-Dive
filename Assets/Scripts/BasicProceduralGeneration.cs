using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BasicProceduralGeneration : MonoBehaviour
{
    public GameObject platformPrefab; // from the "Assets/Prefabs" folder, drag the "Platform" prefab to the location for this in the inspector
    GameObject player;

    public float platformMinYDistance = 1f;
    public float platformMaxYDistance = 10f;
    public int maxNumberPlatforms = 50; // number of platforms to spawn 

    // Array to store the spawned platforms
    GameObject[] platformsArray;// = new GameObject[maxNumberPlatforms]; 
    Vector3 lowestPlatformPos;
    GameObject spawnedPlatformsContainer;
    // Create an empty gameobject to store the instantiated platforms so they can be refereneced after creation
    GameObject instantiatedPlatform;


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

    void spawnPlatformGroup(int numberPlatformsPerGroup, Vector3 startingPos) {
        // Create a new vector 3d to hold location for platform locations
        Vector3 spawnPos = startingPos;//new Vector3(0,0); 
        // Create the specifed number of platforms
        for (int i = 0; i < numberPlatformsPerGroup; i++) {
            spawnPos = getNextPlatformPos(spawnPos);

            instantiatedPlatform = Instantiate(platformPrefab, spawnPos, Quaternion.identity); // create a new platform at the specified location
            instantiatedPlatform.transform.parent = spawnedPlatformsContainer.transform; // set the spawned platofrm's parent to be the empty gameobejct created previously for cleanliness
            platformsArray[i] = instantiatedPlatform; // add the platform to the list keeping track of the platforms
        }
        lowestPlatformPos = platformsArray[numberPlatformsPerGroup-1].transform.position;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the player gameobject for its position
        player = GameObject.FindGameObjectWithTag("Player");
        // Create a new empty gmaeobject to hold the spawned platforms
        spawnedPlatformsContainer = new GameObject("spawnedPlatformsContainer");
        // Give the platform array a length
        platformsArray = new GameObject[maxNumberPlatforms];
        // Spawn a group of platforms
        spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,0));
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(player.transform.position.x);
        // Debug.Log(player.transform.position.y);
        // Debug.Log(platformsArray[10]);

        // If player is below the lowest platform spawned in the previous group
        if (player.transform.position.y <= lowestPlatformPos.y) {
            // Debug.Log("Destroying all platforms");
            // Destroy platforms from prevoius group
            for (int i = 0; i < maxNumberPlatforms; i++) {
                Destroy(platformsArray[i]);
            }
            // Spawn a new group of platforms
            spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,lowestPlatformPos.y));
        }
    }
}
