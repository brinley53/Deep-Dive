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
    public int magmaPlatform_TotalDamage = 20; 
    public int magmaPlatform_NumberOfDamageBursts = 2;
    public float magmaPlatform_TimeBetweenBursts = 1.0f;

    private int singleDamageBurstDamage;
    private float timeOfPreviousDamageBurst;

    void Start() {
        singleDamageBurstDamage = Mathf.RoundToInt(magmaPlatform_TotalDamage/magmaPlatform_NumberOfDamageBursts);
        timeOfPreviousDamageBurst = Time.fixedTime;

        Debug.Log($"Total damage = {magmaPlatform_TotalDamage}");
        Debug.Log($"Num bursts = {magmaPlatform_NumberOfDamageBursts}");
        Debug.Log($"Single burst damage = {singleDamageBurstDamage}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (this.transform.parent != collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource) {
                Debug.Log($"Player landed on magma platform will take {magmaPlatform_TotalDamage} over {magmaPlatform_NumberOfDamageBursts} bursts ({singleDamageBurstDamage} each)");
                StartCoroutine(MagmaDamageBurst(collision));
                collision.gameObject.GetComponent<PlayerMovement>().previousDamageSource = this.transform.parent;
            }
            else {
                Debug.Log("Already hit by this magma platform");
            }
        }
    }

    private IEnumerator MagmaDamageBurst(Collision2D playerObjCollider) {
        for (int i=0; i < magmaPlatform_NumberOfDamageBursts; i++) {
            Debug.Log($"Damage burst {i+1}/{magmaPlatform_NumberOfDamageBursts}");

            playerObjCollider.gameObject.GetComponent<PlayerMovement>().TakeDamage(singleDamageBurstDamage, true);
            yield return new WaitForSeconds(magmaPlatform_TimeBetweenBursts);

        }
    }
}
