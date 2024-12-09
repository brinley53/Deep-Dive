
/**
BasicProceduralGeneration.cs
Description: Script for procedurally generating the level layout
Creation date: 10/26/2024
Authors: Gianni Louisa, Brinley Hull, Ben Renner, Connor Bennudriti, Kyle Moore
Other sources of code: Unity Documentation

Revisions
* 10/30/24 - Connor: added checkpoints
* 11/8/24 - Connor: added platform prefab spawning
* 11/24/24 - Kyle: added item and harpoon spawning
* 11/24/24 - Connor: added other platform types
* 11/28/24 - Brinley: add enemy spawning
* 12/1/24 - Brinley: added shark enemies
* 12/4/24 - Brinley: added kraken enemies
* 12/7/24 - Connor: added difficultly scaling for the procedural generation of damagin platofrms and enemy spawn chance 
Preconditions:
* Script must be attached to the GameManager object 
Postconditions:
* None
Error and Exception conditions:
* None
Side effects:
* None
Invariants:
* None
Known Faults:
* None

**/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BasicProceduralGeneration : MonoBehaviour
{
    GameObject player; // the player object

    GameObject harpoonItem; // harpoon (projectile) pickup item
    GameObject heartItem; //heart pickup
    GameObject bubble; // bubble pickup item

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

    // Spike Platform prefab locations
    string[] spikePlatformSizes = {
        "prefabs/Spike Platform XS",
        "prefabs/Spike Platform S",
        "prefabs/Spike Platform M",
        "prefabs/Spike Platform L"
    };
    GameObject spikePlatform; // Magma Platform object

    // Magma Platform prefab locations
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
    Vector3 curPlatformTypeProbabilities; // variable to store the current platform type probabilties for the checkpoint

    // Array to store enemy spawn proabibility based on checkponits hit
    float[] curEnemySpawnProbabilityScaling = {
        0.3f,
        0.35f,
        0.4f,
        0.45f,
        0.5f,
        0.55f,
        0.6f,
        0.65f,
        0.7f,
        0.75f,
    };
    float curEnemySpawnProbability; // variable to store the current enemy spawn probabilties for the checkpoint

    GameObject spawnedPlatformGroupContainer; // a container object to store the spawned platforms for cleanliness in the hierarchy

    GameObject checkpointSectionPrefab; // the prefab for a checkpoint section


    public float platformMinYDistance = 1f; // minimum y distance between platforms
    public float platformMaxYDistance = 10f; // max y distance between platforms
    public int maxNumberPlatforms = 50; // number of platforms to spawn 

    // Array to store the spawned platforms
    GameObject[] platformsArray;
    Vector3 lowestPlatformPos; // var to track where the lowest platform in a group has spawned
    Vector3 checkpointSectionSpawnLocation; // var to track where the last checkpoint was spawned
    GameObject platformSection; // the platform section that is being spawned in 

    public int maxPlatformSize = 20; // the maximum size of a platform

    private int distanceBetweenFinalPlatformAndCheckpointSection = 15; // distance between the last platform in a group and the checkpoint at the end of the spawn group
    private int distanceBetweenCheckpointSectionAndPlatformStart = 5; // distance between the checkpoint and where platforms can begin to spawn

    private int numCheckpointsHit; // var to track how many checkpoints the player has hit

    // Function to get the position of the next platform
    Vector3 getNextPlatformPos(Vector3 prevPlatformPos) {
        // Set next x position to a random x position on the screen that does not overlap with the previous platform's x position
        int nextXPos;
        while (true) {
            nextXPos = Random.Range(-20,20);
            if (nextXPos != prevPlatformPos.x) { break; }
        }
        // Ensure next platform spawns below the previous platform by a random distance in the given range
        int nextYPos = Random.Range((int)(prevPlatformPos.y-platformMinYDistance), (int)(prevPlatformPos.y-platformMaxYDistance));
        // Return the position wher to spawn the next platform
        return new Vector3(nextXPos,nextYPos);
    }

    // Function to get which platform type to spawn with damaging platform types frequency scaled by distance
    string getPlatformTypeToSpawn() {
        // Get a random float between 0 and 1
        float ran = (Random.Range(0.0f,1.0f));
        // Check where the random float is in the probability range for the current number of checkpoints hit and return the appropriate platform type
        if      ((ran>=0.0)                            && (ran<=curPlatformTypeProbabilities.x))     {return "normal";}
        else if ((ran>curPlatformTypeProbabilities.x) && (ran<=curPlatformTypeProbabilities.y))      {return "spike";}
        else if ((ran>curPlatformTypeProbabilities.y) && (ran<=curPlatformTypeProbabilities.z))      {return "magma";}
        else {Debug.Log($"Issue with platform type scaling - random # {ran} not in range"); return "normal";}

    }

    // Function to piece together the left/middle/right portions of a platform to form a whole
    GameObject createPlatform(Vector3 platformSpawnPos) {

        // Get the type of platform to spawn (normal, spike, magma)
        string platformTypeToSpawn = getPlatformTypeToSpawn();

        // Get the length of the platform
        int platformLengthToSpawn = Random.Range(1, maxPlatformSize);

        // // Create container object for the individual platform
        // GameObject platformContainer = new GameObject("platformContainer"); 
        
        // If spawning a regular platofrm
        if (platformTypeToSpawn == "normal") {
            regularPlatform = Resources.Load(regularPlatformSizes[Random.Range(0, regularPlatformSizes.Length)]) as GameObject; // load a random sized regular platofmr and instantiate it
            platformSection = Instantiate(regularPlatform, platformSpawnPos, Quaternion.identity);
        } 
        // If spawning a magma platofrm
        else if (platformTypeToSpawn == "magma") {
            magmaPlatform = Resources.Load(magmaPlatformSizes[Random.Range(0, magmaPlatformSizes.Length)]) as GameObject; // load a random sized magma platofmr and instantiate it
            platformSection = Instantiate(magmaPlatform, platformSpawnPos, Quaternion.identity);
        } 
        // If spawning a spike platofrm
        else {
            spikePlatform = Resources.Load(spikePlatformSizes[Random.Range(0, spikePlatformSizes.Length)]) as GameObject; // load a random sized spike platofmr and instantiate it
            platformSection = Instantiate(spikePlatform, platformSpawnPos, Quaternion.identity);
        }
        // Return the spawned platofrm 
        return platformSection;
    }

    // Function to spawn a grup of platofrms
    GameObject spawnPlatformGroup(int numberPlatformsPerGroup, Vector3 startingPos) {
        // Create a new vector 3d to hold location for platform locations
        Vector3 spawnPos = startingPos;
        // Create container for the platform group
        GameObject spawnedPlatformsContainer = new GameObject("spawnedPlatformsContainer");

        // Create the specifed number of platforms
        for (int i = 0; i < numberPlatformsPerGroup; i++) {
            spawnPos = getNextPlatformPos(spawnPos); // get next position to spawn platofrm in
            GameObject platformSection = createPlatform(spawnPos); // create the platform at the location

            platformSection.transform.parent = spawnedPlatformsContainer.transform; // set the spawned platofrm's parent to be the empty gameobejct created previously for cleanliness
            platformsArray[i] = platformSection; // add the platform to the list keeping track of the platforms


            GameObject spawnedItem = spawnItemOnPlatform(spawnPos); // randomly spawn an item
            spawnedItem.transform.parent = spawnedPlatformsContainer.transform; // add the spawned item to the container for the group

            GameObject spawnedBubble = spawnBubble(spawnPos); // randomly spawn a bubble
            spawnedBubble.transform.parent = spawnedPlatformsContainer.transform; // add the spawned item to the container for the group

            // Get a random number 
            float ran = Random.Range(0.0f,1.0f);
            // If the random number is within a range specifed by the current enemy spawn probability 
            if (ran <= curEnemySpawnProbability) {
                GameObject spawnedEnemy = spawnEnemy(spawnPos); // Spawn an enemy
                spawnedEnemy.transform.parent = spawnedPlatformsContainer.transform; // add the spawned enemy to the container for the group
            }

        }
        lowestPlatformPos = spawnPos; // store the lowest platform in the group
        

        // Get the location to spawn the next checkpoint at
        checkpointSectionSpawnLocation = new Vector3(0, lowestPlatformPos.y-distanceBetweenFinalPlatformAndCheckpointSection);
        // Instantiate a checkpoint section after the platform group
        GameObject cpSection = Instantiate(checkpointSectionPrefab, checkpointSectionSpawnLocation, Quaternion.identity);

        // Return the container 
        return spawnedPlatformsContainer;
    }

    // Function to randomly spawn a bubble item
    GameObject spawnBubble(Vector3 platformPos) {
        float spawnChance = Random.value; // get randonm number between 0 and 1

        // If the random number is greater than .8, choose a random spawn position and spawn the bubble there
        if (spawnChance > 0.8f) {
            Vector3 pos = new Vector3(Random.Range(-15, 16), platformPos.y + Random.Range(0, 5), platformPos.z); //set position of the bubble
            return Instantiate(bubble, pos, Quaternion.identity); //instantiate the bubble in the game
        } 
        // Return an empty bubble if not
        else {
            return new GameObject("emptyBubble");
        }
    }

    // Function to spawn an item on a platofrm
    GameObject spawnItemOnPlatform(Vector3 platformPos) {
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
            GameObject item = Instantiate(itemToSpawn, itemSpawnPos, Quaternion.identity);
            // Debug.Log("Spawned item: " + itemToSpawn.name + " at position: " + itemSpawnPos); //logs position of item spaw in case prefab is not loaded correctly
            return item;
        } else {
            return new GameObject("emptyItem");
        }
    }

    // Function to spawn an enemey
    GameObject spawnEnemy(Vector3 platformPos) { // Function to spawn an enemy object
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
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, enemySpawnPos, Quaternion.identity); // Add the enemy to the game
            return spawnedEnemy;
        } else {
            return new GameObject("emptyEnemy");
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
        curEnemySpawnProbability = curEnemySpawnProbabilityScaling[0];

        // Load items
        harpoonItem = Resources.Load("prefabs/harpoon_item_0") as GameObject; // Load the item prefab
        heartItem = Resources.Load("prefabs/heart_item_0") as GameObject; // Load the item prefab
        bubble = Resources.Load("prefabs/Bubble_0") as GameObject; // Load the item prefab
        lionfish = Resources.Load("prefabs/lionfish") as GameObject; // Load the basic enemy prefab
        shark = Resources.Load("prefabs/Shark") as GameObject; // Load the shark enemy prefab
        kraken = Resources.Load("prefabs/Kraken_0") as GameObject; // Load the kraken enemy prefab

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
        curEnemySpawnProbability = curEnemySpawnProbabilityScaling[Mathf.Min(numCheckpointsHit,curEnemySpawnProbabilityScaling.Length -1)];
        // Debug.Log(curPlatformTypeProbabilities);

        // If player is below the lowest platform spawned in the previous group
        if (player.transform.position.y <= checkpointSectionSpawnLocation.y) {
            Destroy(spawnedPlatformGroupContainer); // destory platforms to free memory for next platform group
            // Spawn a new group of platforms
            spawnedPlatformGroupContainer = spawnPlatformGroup(maxNumberPlatforms, new Vector3(0,checkpointSectionSpawnLocation.y-distanceBetweenCheckpointSectionAndPlatformStart));
        }
    }
}
