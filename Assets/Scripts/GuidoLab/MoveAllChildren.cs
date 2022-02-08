using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAllChildren : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            var newpos = new Vector3(transform.position.x, child.position.y, transform.position.z);
            child.position = newpos;
        }
    }
}
