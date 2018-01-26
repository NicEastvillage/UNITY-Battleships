using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tile {

    public Coord pos { get; private set; }

    public Action OnMouseEnter;
    public Action OnMouseLeave;
    public Action OnMouseClick;
    
    public Ship ship { get; protected set; }

    public Tile (Coord position)
    {
        pos = position;
    }

    public void MoveShipHere(Ship ship, bool isPlacing = false)
    {
        if (isPlacing)
        {
            this.ship = ship;
            ship.MoveInstant(pos);
        }
        else
        {
            MapController.instance.GetTile(ship.pos).ship = null;
            this.ship = ship;
        }
    }

    public void RemoveShip()
    {
        ship = null;
    }
}
