/*
OxyBar.cs
Description: script for handling the oxygen bar
Creation date: 12/5/2024
Authors: Brinley Hull
Other sources of code: weeklyhow.com

Revisions
* None
Preconditions:
* Script must be attached to the oxygen slider object
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
public class OxyBar : MonoBehaviour
{

    public Slider oxygenBar; // the slider for dispaying the oxygen

    private void Start()
    {
        oxygenBar = GetComponent<Slider>(); // get the slider for the oxygen bar
        oxygenBar.maxValue = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().maxOxygen; // set the max value for the slider
        oxygenBar.value = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().oxygen; // set starting alue for the slider
    }

    // Function to set the oxygen level of the slider to a provided int
    public void SetOxygen(int oxy) {
        oxygenBar.value = oxy;
    }
}
