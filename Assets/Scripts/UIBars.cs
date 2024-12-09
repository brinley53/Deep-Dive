/*
UIBars.cs
Description: script for handling the UI
Creation date: 12/5/2024
Other sources of code: weeklyhow.com
Authors: Brinley Hull

Revisions
* None
Preconditions:
* Script must be attached to the healthbar slider object
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
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBar : MonoBehaviour
{
    public Slider healthBar; // the healthbar slider object

    private void Start()
    {
        healthBar = GetComponent<Slider>(); // get the slider for displaying UI
        healthBar.maxValue = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().maxHealth; // set the max value of the health bar
        healthBar.value = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().health; // set the currently value of the heatly bar
    }

    // Function that is called to set the current value of the health bar to an int provided
    public void SetHealth(int hp)
    {
        healthBar.value = hp;
    }
}
