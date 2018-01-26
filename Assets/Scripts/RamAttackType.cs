using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ships/Ram Type")]
public class RamAttackType : ScriptableObject {

    public new string name = "Simple Ram";
    public int attackDice = 3;
    public int sideRamBonusDice = 1;
    public GameObject effectPrefab;

    public int GetDamage(Ship attacker, Ship target)
    {
        int faceDiff = target.facing - attacker.facing;

        int dice = attackDice + ((Mathf.Abs(faceDiff % 4) == 2) ? sideRamBonusDice : 0);
        int dmg = 0;

        // standard damage
        for (int d = 0; d < dice; d++)
        {
            int die = Random.Range(1, 7); // 1d6
            if (die > 2)
            {
                dmg++;
            }
        }

        return dmg;
    }
}
