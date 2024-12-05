/**
MagmaPlatformLogic.cs
Description: File that handles damaging the player after landing on a magma platform
Creation date: 11/24/24
Authors: Connor Bennudriti
Other sources of code: 
**/



using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MagmaPlatformLogic : MonoBehaviour
{
    public int magmaPlatform_TotalDamage = 20; // the total amount of damage the magma will deal
    public int magmaPlatform_NumberOfDamageBursts = 2; // the number of bursts of damage
    public float magmaPlatform_TimeBetweenBursts = 1.0f; // how long between the bursts of damage

    private int singleDamageBurstDamage; // int to store how much each burst should do
    private float timeOfPreviousDamageBurst; // float to track when the last burst was 

    void Start() {
        singleDamageBurstDamage = Mathf.RoundToInt(magmaPlatform_TotalDamage/magmaPlatform_NumberOfDamageBursts); // get how much damage each burst should do
        timeOfPreviousDamageBurst = Time.fixedTime; // get the starting time
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // if platform collided with the player
        {
            Debug.Log($"Player landed on magma platform will take {magmaPlatform_TotalDamage} over {magmaPlatform_NumberOfDamageBursts} bursts ({singleDamageBurstDamage} each)");
            StartCoroutine(MagmaDamageBurst(collision)); // start damaging
            collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource = this.transform.parent; // set the previous damage source to this platform segmen's container
        }
    }

    // Coroutine to damage player over time 
    private IEnumerator MagmaDamageBurst(Collision2D playerObjCollider) {
        // For the number of damage bursts
        for (int i=0; i < magmaPlatform_NumberOfDamageBursts; i++) {
            Debug.Log($"Damage burst {i+1}/{magmaPlatform_NumberOfDamageBursts}");

            playerObjCollider.gameObject.GetComponent<PlayerMovement>().TakeDamage(singleDamageBurstDamage, true); // apply damge to the player, ignoring I-frames
            yield return new WaitForSeconds(magmaPlatform_TimeBetweenBursts); // wait

        }
    }
}
