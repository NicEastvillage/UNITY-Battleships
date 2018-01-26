using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFireRandomizer : MonoBehaviour {

    public float lifeTime = 8f;

    public GameObject[] effects;
    public Transform[] positions;

	void Start () {

        Destroy(gameObject, lifeTime);

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject gm = Instantiate(effects[Random.Range(0, effects.Length)], positions[i]);
        }
	}
}
