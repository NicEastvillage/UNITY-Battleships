using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHighlight {

    private Ship ship;
    private Attack attack;
    private List<Highlight> lights;

    private bool isShowing = false;

    public AttackHighlight (Ship ship, Attack atk)
    {
        this.ship = ship;
        attack = atk;

        lights = new List<Highlight>();
    }

    public void Show()
    {
        if (!isShowing) {
            isShowing = true;

            Coord pos = ship.pos;
            int range = ship.IsDiagonal() ? attack.type.rangeDiagonal : attack.type.range;

            for (int i = 0; i < range; i++)
            {
                pos += Coord.neighbours[attack.direction];

                Highlight light = TileHighlighter.instance.CreateHighlight(pos, 1);

                lights.Add(light);
            }
        }
    }

    public void Hide()
    {
        if (isShowing)
        {
            isShowing = false;

            foreach (Highlight light in lights)
            {
                TileHighlighter.DestroyHighlight(light);
            }

            lights.Clear();
        }
    }
}
