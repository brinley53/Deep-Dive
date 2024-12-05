/*
Creation date: 12/5/2024
Other sources of code: weeklyhow.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OxyBar : MonoBehaviour
{

    public Slider oxygenBar;

    private void Start()
    {
        oxygenBar = GetComponent<Slider>();
        oxygenBar.maxValue = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().maxOxygen;
        oxygenBar.value = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().oxygen;
    }

    public void SetOxygen(int oxy) {
        oxygenBar.value = oxy;
    }
}
