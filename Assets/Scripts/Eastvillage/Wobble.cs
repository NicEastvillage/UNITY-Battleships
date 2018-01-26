using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour {

    public float amountX = 1;
    public float amountZ = 1;
    public float speed = 1;

    private float randomOffsetX, randomOffsetZ;

    void Awake ()
    {
        randomOffsetX = Random.value * 5;
        randomOffsetZ = Random.value * 5;
    }

	void Update () {

        float yaw = Mathf.Sin(Time.time * 0.11f * speed + randomOffsetX) * amountX;
        float pitch = Mathf.Sin(Time.time * 0.07f * speed + randomOffsetZ) * amountZ;

        transform.eulerAngles = new Vector3(yaw, transform.rotation.eulerAngles.y, pitch);
    }
}
