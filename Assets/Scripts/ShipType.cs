using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ships/Ship Type")]
public class ShipType : ScriptableObject {

    [System.Serializable]
    public class SetOfAttacks
    {
        public RamAttackType ram;
        public AttackType front;
        public AttackType frontRight;
        public AttackType right;
        public AttackType backRight;
        public AttackType back;
        public AttackType backLeft;
        public AttackType left;
        public AttackType frontLeft;

        public AttackType Get(int direction)
        {
            switch (direction)
            {
                case 0: return front;
                case 1: return frontRight;
                case 2: return right;
                case 3: return backRight;
                case 4: return back;
                case 5: return backLeft;
                case 6: return left;
                case 7: return frontLeft;
                default: return null;
            }
        }
    }

    public new string name = "New Ship";
    public Sprite icon;
    public string description;
    public GameObject model;
    public int health = 10;
    public SetOfAttacks attacks;
}
