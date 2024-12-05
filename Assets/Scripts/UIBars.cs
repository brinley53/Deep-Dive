/*
Creation date: 12/5/2024
Other sources of code: weeklyhow.com
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIBar : MonoBehaviour
{
    public Slider healthBar;

    private void Start()
    {
        healthBar = GetComponent<Slider>();
        healthBar.maxValue = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().maxHealth;
        healthBar.value = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().health;
    }

    public void SetHealth(int hp)
    {
        healthBar.value = hp;
    }
}
