using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ships/Attack Type")]
public class AttackType : ScriptableObject {

	public new string name = "Canons";
    public Sprite icon;
    public Color color; // temp, until icons are made
    public int attackDiceCount = 3; // amount of canonballs. If some miss they migth hit the ship behind the first
    public int range = 3;
    public int rangeDiagonal = 2;
    public int cooldown = 2;
    public GameObject effectPrefab;
}
