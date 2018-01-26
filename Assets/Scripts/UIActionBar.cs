using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionBar : MonoBehaviour {

    public Ship ship { get; protected set; }

    public static UIActionBar instance;

    [SerializeField]
    private UIAttack[] UIAttacks;

    void Awake()
    {
        instance = this;

        GameController.instance.OnTurnBegin += Game_OnTurnBegin;
        GameController.instance.OnTurnEnd += Game_OnTurnEnd;
    }

    void Start()
    {
        
    }

    private void Game_OnTurnBegin()
    {
        SetShip(GameController.instance.currentShip);
    }

    private void Game_OnTurnEnd()
    {
        
    }

    public void SetShip(Ship ship)
    {
        this.ship = ship;
        for (int i = 0; i < UIAttacks.Length; i++)
        {
            UIAttacks[i].SetAttack(ship.GetAttack(i));
        }
    }
}
