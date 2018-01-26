using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShip : MonoBehaviour {

    public Ship ship { get; protected set; }

    [Header("UI")]
    public Text nameText;
    public Image icon;
    public Slider healthSlider;
    public Text healthText;
    public string healthFormat = "{0} / {1}";

    public bool isShowing { get { return gameObject.activeSelf; } }

    private static UIShip _instance;
    public static UIShip instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<UIShip>();
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Hide();
    }

	public void SetShip(Ship ship)
    {
        if (ship != null)
        {
            // unsubscribe events
            ship.OnTakeDamage -= Ship_OnTakeDamage;
        }

        this.ship = ship;

        nameText.text = ship.type.name;
        //icon.sprite = ship.type.icon;

        healthSlider.value = ship.healthPercentage;
        healthText.text = string.Format(healthFormat, ship.health, ship.type.health);

        ship.OnTakeDamage += Ship_OnTakeDamage;
    }

    private void Ship_OnTakeDamage(int amount)
    {
        healthSlider.value = ship.healthPercentage;
        healthText.text = string.Format(healthFormat, ship.health, ship.type.health);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
