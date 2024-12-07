
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

    GameObject harpoonItem;
    GameObject heartItem;
    GameObject bubble;

    GameObject lionfish; // the basic enemy object

    GameObject shark; //the shark enemy object initialization

    GameObject kraken; // the kraken enemy object initialization

    private int numEnemies = 5; // integer to hold the number of enemies in a group at a time
    
    // Regular platform prefab locations
    string[] regularPlatformSizes = {
        "prefabs/Regular XXXS",
        "prefabs/Regular XXS",
        "prefabs/Regular XS",
        "prefabs/Regular S",
        "prefabs/Regular M",
        "prefabs/Regular L",
        "prefabs/Regular XL",
        "prefabs/Regular XXL",
        "prefabs/Regular XXXL"
    };

    GameObject regularPlatform; // Regular platform object

    // Spike
    // GameObject spikePlatformLeftPrefab;
    // GameObject spikePlatformMiddlePrefab;
    // GameObject spikePlatformRightPrefab;
    //Magma Platform prefab locations
    string[] spikePlatformSizes = {
        "prefabs/Spike Platform XS",
        "prefabs/Spike Platform S",
        "prefabs/Spike Platform M",
        "prefabs/Spike Platform L"
    };
    GameObject spikePlatform; // Magma Platform object

    //Magma Platform prefab locations
    string[] magmaPlatformSizes = {
        "prefabs/Magma Platform XS",
        "prefabs/Magma Platform S",
        "prefabs/Magma Platform M",
        "prefabs/Magma Platform L"
    };
    GameObject magmaPlatform; // Magma Platform object

    string[] platformTypesArray = {
        "normal",
        "spike",
        "magma"
    };

    // Array to store platform type probabilities
    // Vector3[] platformTypeProbabilityScaling = {
    //     new Vector3(1.0, 0.0, 0.0),
    //     new Vector3(0.9, 0.1, 0.0),
    //     new Vector3(0.8, 0.1, 0.1),
    //     new Vector3(0.7, 0.2, 0.1),
    //     new Vector3(0.55, 0.225, 0.225),
    //     new Vector3(0.5, 0.25, 0.25),
    //     new Vector3(0.4, 0.25, 0.35),
    //     new Vector3(0.35, 0.30, 0.35),
    // };
    Vector3[] platformTypeProbabilityScaling = {
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(0.9f, 1.0f, 0.0f),
        new Vector3(0.8f, 0.9f, 1.0f),
        new Vector3(0.7f, 0.9f, 1.0f),
        new Vector3(0.55f, 0.775f, 1.0f),
        new Vector3(0.5f, 0.75f, 1.0f),
        new Vector3(0.4f, 0.65f, 1.0f),
        new Vector3(0.35f, 0.65f, 1.0f),
    };
    Vector3 curPlatformTypeProbabilities;

    GameObject spawnedPlatformGroupContainer;

    GameObject checkpointSectionPrefab; // the prefab for a checkpoint section


    public float platformMinYDistance = 1f;
    public float platformMaxYDistance = 10f;
    public int maxNumberPlatforms = 50; // number of platforms to spawn 

    // Array to store the spawned platforms
    GameObject[] platformsArray;
    Vector3 lowestPlatformPos;
    Vector3 checkpointSectionSpawnLocation;
    GameObject platformSection;

    public int maxPlatformSize = 20; // the maximum size of a platform

    private int distanceBetweenFinalPlatformAndCheckpointSection = 15;
    private int distanceBetweenCheckpointSectionAndPlatformStart = 5;

    private int numCheckpointsHit;

    Vector3 getNextPlatformPos(Vector3 prevPlatformPos) {
        // Set next x position to a random x position on the screen that does not overlap with the previous platform's x position
        int nextXPos;
        while (true) {
            nextXPos = Random.Range(-20,20);
            if (nextXPos != prevPlatformPos.x) { break; }
        }
        // Ensure next platform spawns below the previous platform by a random distance in the given range
        int nextYPos = Random.Range((int)(prevPlatformPos.y-platformMinYDistance), (int)(prevPlatformPos.y-platformMaxYDistance));
        return new Vector3(nextXPos,nextYPos);
    }

    // Function to get which platform type to spawn with damaging platform types frequency scaled by distance
    string getPlatformTypeToSpawn() {
        // Use number of checkpoints hit to calculate type of platform to spawn
        float ran = (Random.Range(0.0f,1.0f));
        if      ((ran>=0.0)                            && (ran<=curPlatformTypeProbabilities.x))     {return "normal";}
        else if ((ran>curPlatformTypeProbabilities.x) && (ran<=curPlatformTypeProbabilities.y))      {return "spike";}
        else if ((ran>curPlatformTypeProbabilities.y) && (ran<=curPlatformTypeProbabilities.z))      {return "magma";}
        else {Debug.Log($"Issue with platform type scaling - random # {ran} not in range"); return "normal";}

        // platformTypesArray[Random.Range(0, platformTypesArray.Length)];
    }

    // Function to piece together the left/middle/right portions of a platform to form a whole
    GameObject createPlatform(Vector3 platformSpawnPos) {

        // Get the type of platform to spawn (normal, spike, magma)
        string platformTypeToSpawn = getPlatformTypeToSpawn();

        // Get the length of the platform
        int platformLengthToSpawn = Random.Range(1, maxPlatformSize);

        // Create container object for the individual platform
        GameObject platformContainer = new GameObject("platformContainer"); 
        
        // If spawning a single unit platform
        if (platformTypeToSpawn == "normal") {
            regularPlatform = Resources.Load(regularPlatformSizes[Random.Range(0, regularPlatformSizes.Length)]) as GameObject;
            platformSection = Instantiate(regularPlatform, platformSpawnPos, Quaternion.identity);
        } 
        else if (platformTypeToSpawn == "magma") {
            magmaPlatform = Resources.Load(magmaPlatformSizes[Random.Range(0, magmaPlatformSizes.Length)]) as GameObject;
            platformSection = Instantiate(magmaPlatform, platformSpawnPos, Quaternion.identity);
        } 
        else {
            spikePlatform = Resources.Load(spikePlatformSizes[Random.Range(0, spikePlatformSizes.Length)]) as GameObject;
            platformSection = Instantiate(spikePlatform, platformSpawnPos, Quaternion.identity);
            // if (platformLengthToSpawn == 1) {
            //     platformSection = Instantiate(spikePlatformMiddlePrefab, platformSpawnPos, Quaternion.identity);
            //     platformSection.transform.parent = platformContainer.transform; // add the platform to the container
            // }
            // else {
            //     Vector3 nextPlatformSegmentLoc = platformSpawnPos; // the location to spawn the next platform segment
            //     // For each segment needed to make up the whole platform 
            //     for (int i=0; i < platformLengthToSpawn; i++) {
            //         // Spawn a spike middle platform
            //         platformSection = Instantiate(spikePlatformMiddlePrefab, nextPlatformSegmentLoc, Quaternion.identity);
            //         nextPlatformSegmentLoc = new Vector3(nextPlatformSegmentLoc.x + (float)0.99, nextPlatformSegmentLoc.y); // set the next platform segment to be to the right of the previous one
            //         platformSection.transform.parent = platformContainer.transform; // add the platform to the container
            //     }
            // }
        
        }
        return platformContainer;
    }

    GameObject spawnPlatformGroup(int numberPlatformsPerGroup, Vector3 startingPos) {
        // Create a new vector 3d to hold location for platform locations
        Vector3 spawnPos = startingPos;
        // Create container for the platform group
        GameObject spawnedPlatformsContainer = new GameObject("spawnedPlatformsContainer");
        numEnemies = 5; // Reset the number of enemies needed in this group
        // Create the specifed number of platforms
        for (int i = 0; i < numberPlatformsPerGroup; i++) {
            spawnPos = getNextPlatformPos(spawnPos);
            GameObject platformsContainer = createPlatform(spawnPos);

            platformsContainer.transform.parent = spawnedPlatformsContainer.transform; // set the spawned platofrm's parent to be the empty gameobejct created previously for cleanliness
            platformsArray[i] = platformsContainer; // add the platform to the list keeping track of the platforms


            // Chance-based item spawning
            spawnItemOnPlatform(spawnPos);
            spawnBubble(spawnPos);

            int enemySpawnChance = Random.Range(0, numberPlatformsPerGroup); // Randomize enemy spawning so that it doesn't spawn on just the first platforms
            if (numEnemies > 0 && enemySpawnChance%3 == 0) { // If there are still needing to be enemies spawned
                numEnemies--; // Decrease the amount of enemies needing to be spawning
                spawnEnemy(spawnPos); // Spawn an enemy
            }
        }
        lowestPlatformPos = spawnPos;
        

        checkpointSectionSpawnLocation = new Vector3(0, lowestPlatformPos.y-distanceBetweenFinalPlatformAndCheckpointSection);
        // Instantiate a checkpoint section after the platform group
        Instantiate(checkpointSectionPrefab, checkpointSectionSpawnLocation, Quaternion.identity);

        return spawnedPlatformsContainer;
    }

    void spawnBubble(Vector3 platformPos) {
        float spawnChance = Random.value;

        if (spawnChance > 0.8f) {
            Vector3 pos = new Vector3(Random.Range(-15, 16), platformPos.y + Random.Range(0, 5), platformPos.z);
            Instantiate(bubble, pos, Quaternion.identity);
        }
    }

    void spawnItemOnPlatform(Vector3 platformPos) {
        // Random chance for item generation (adjust probabilities as needed)
        float spawnChance = Random.value; // Generates a value between 0.0 and 1.0
        GameObject itemToSpawn = null;

        if (spawnChance <= 0.15f) { //adjust chance for harpoon item to spwan
            itemToSpawn = harpoonItem;
        } else if (spawnChance <= 0.2f) { // adjust chance for heart item to spawn
            itemToSpawn = heartItem;
        } 
        // If an item is chosen, spawn it slightly above the platform
        if (itemToSpawn != null) {
            Vector3 itemSpawnPos = new Vector3(platformPos.x, platformPos.y + 1f, platformPos.z);
            Instantiate(itemToSpawn, itemSpawnPos, Quaternion.identity);
            // Debug.Log("Spawned item: " + itemToSpawn.name + " at position: " + itemSpawnPos); //logs position of item spaw in case prefab is not loaded correctly
        }
    }

    void spawnEnemy(Vector3 platformPos) { // Function to spawn an enemy object
        // Random chance for enemy generation
        float spawnChance = Random.value; // Generates a value between 0.0 and 1.0
        GameObject enemyToSpawn = null; // Initialize the type of enemy to be spawned variable
        Vector3 enemySpawnPos = new Vector3(platformPos.x, platformPos.y + 2f, platformPos.z); // Create the position of the enemy . on platform

        if (spawnChance <= 0.65f) { //adjust chance for lionfish to spawn
            enemyToSpawn = lionfish; // Set the enemy type to be spawned as a lionfish
        } else if (spawnChance > 0.65f && spawnChance <= 0.95f) { // adjust chance for shark to spawn
            enemyToSpawn = shark; // Set the enemy type to be spawned as a lionfish
            enemySpawnPos = new Vector3(Random.Range(-15, 15), platformPos.y + Random.Range(1, 8), platformPos.z); // Create the position of the enemy . random
        } else { //spawn chance for kraken to spawn
            enemyToSpawn = kraken; // Set the enemy type to be spawned as a lionfish
            enemySpawnPos = new Vector3(Random.Range(-15, 15), platformPos.y + Random.Range(1, 8), platformPos.z); // Create the position of the enemy . random
        }

        // Spawn the enemy slightly above the platform if the object is not null
        if (enemyToSpawn != null) {
            Instantiate(enemyToSpawn, enemySpawnPos, Quaternion.identity); // Add the enemy to the game
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the player gameobject for its position
        player = GameObject.FindGameObjectWithTag("Player");

        // Get fall distance 
        numCheckpointsHit = player.GetComponent<PlayerMovement>().numCheckpointsHit;
        curPlatformTypeProbabilities = platformTypeProbabilityScaling[0];

        // // Get the spike platform prefabs
        // spikePlatformLeftPrefab = Resources.Load("prefabs/spike_platform_prefab_left") as GameObject; 
        // spikePlatformMiddlePrefab = Resources.Load("prefabs/spike_platform_prefab_mid") as GameObject;
        // spikePlatformRightPrefab = Resources.Load("prefabs/spike_platform_prefab_right") as GameObject;

        // Load items
        harpoonItem = Resources.Load("prefabs/harpoon_item_0") as GameObject;
        heartItem = Resources.Load("prefabs/heart_item_0") as GameObject;
        bubble = Resources.Load("prefabs/Bubble_0") as GameObject;
        lionfish = Resources.Load("prefabs/lionfish") as GameObject; // Load the basic enemy prefab
        shark = Resources.Load("prefabs/Shark") as GameObject; // Load the shark enemy prefab
        kraken = Resources.Load("prefabs/Kraken_0") as GameObject; // Load the kraken enemy prefab
        
        // Debug.Log("Harpoon Item Loaded: " + (harpoonItem != null));
        // Debug.Log("Heart Item Loaded: " + (heartItem != null));



        // Get the checkpoint section prefab
        checkpointSectionPrefab = Resources.Load("prefabs/CheckpointSection") as GameObject;

        // Give the platform array a length
        platformsArray = new GameObject[maxNumberPlatforms];
        // Spawn a group of platforms
        spawnedPlatformGroupContainer = spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,-distanceBetweenCheckpointSectionAndPlatformStart-10));
        
    }

    // Update is called once per frame
    void Update()
    {

        // Get current fall distance of player
        numCheckpointsHit = player.GetComponent<PlayerMovement>().numCheckpointsHit;
        curPlatformTypeProbabilities = platformTypeProbabilityScaling[Mathf.Min(numCheckpointsHit,platformTypeProbabilityScaling.Length -1)];
        // Debug.Log(curPlatformTypeProbabilities);

        // If player is below the lowest platform spawned in the previous group
        if (player.transform.position.y <= checkpointSectionSpawnLocation.y) {
            Destroy(spawnedPlatformGroupContainer); // destory platforms to free memory for next platform group
            // Spawn a new group of platforms
           spawnedPlatformGroupContainer = spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,checkpointSectionSpawnLocation.y-distanceBetweenCheckpointSectionAndPlatformStart));
        }
    }
}
