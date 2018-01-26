using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack {

    public readonly Ship ship;
    public readonly AttackType type;
    public readonly int index;
    public readonly AttackHighlight highlight;
    public int cooldownRemaining { get; protected set; }
    public int direction {
        get {
            return (int)Mathf.Repeat(ship.facing + index, 8);
        }
    }

	public Attack (AttackType type, int index, Ship ship)
    {
        this.type = type;
        this.index = index;
        this.ship = ship;

        cooldownRemaining = 0;

        highlight = new AttackHighlight(ship, this);
    }

    public bool IsUsable ()
    {
        return cooldownRemaining == 0;
    }

    public void Activate ()
    {
        cooldownRemaining = type.cooldown;

        CreateEffects();

        // damage!
        int range = ship.IsDiagonal() ? type.rangeDiagonal : type.range;
        int diceLeft = type.attackDiceCount;
        Coord pos = ship.pos;

        for (int r = 0; r < range; r++)
        {
            pos += Coord.neighbours[direction];

            if (MapController.instance.IsWithinMap(pos))
            {
                Ship target = MapController.instance.GetTile(pos).ship;

                if (target != null)
                {
                    int dmg = 0;
                    int dice = diceLeft;

                    for (int d = 0; d < dice; d++)
                    {
                        int die = Random.Range(1, 7); // 1d6
                        if (die > 2)
                        {
                            dmg++;
                            diceLeft--;
                        }
                    }

                    target.TakeDamage(dmg);
                }
            }
        }

        GameController.instance.ResolveDeaths();
    }

    public void ReduceCooldown ()
    {
        if (cooldownRemaining > 0) cooldownRemaining--;
    }

    private void CreateEffects()
    {
        Transform eft = ((GameObject)GameObject.Instantiate(type.effectPrefab, ship.transform)).transform;

        eft.position = ship.transform.position;
        eft.eulerAngles = new Vector3(0, (direction - 2) * 45, 0);
    }
}
