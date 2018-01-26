using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShipInterface : MonoBehaviour {

    public Ship ship;

    [Header("UI")]
    public Slider healthSlider;
    //public Gradient healthGradient;

	void Start()
    {
        ship.OnTakeDamage += UpdateHealthBar;
	}

    public void UpdateHealthBar(int amount)
    {
        healthSlider.value = ship.healthPercentage;
    }

    void OnDestroy()
    {
        ship.OnTakeDamage -= UpdateHealthBar;
    }
}
