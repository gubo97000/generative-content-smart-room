using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLanding : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jump")
            EventManager.TriggerEvent("OnJumpEnd", gameObject);
    }
}
