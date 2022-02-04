using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatInAir : MonoBehaviour
{
    private float timingOffset;
    private int speed = 2;

    void Start()
    {
        timingOffset = Random.value * (Mathf.PI / 2);
    }

    void Update()
    {
        transform.position += new Vector3(0.0f, Mathf.Sin((Time.time + timingOffset) * speed) / 250, 0.0f);
    }
}
