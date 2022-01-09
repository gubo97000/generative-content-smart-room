using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisions : MonoBehaviour
{
    void Start()
    {
        Physics.IgnoreLayerCollision(6, 7);
    }
}
