using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbBeehive : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Beehive")
        {
            EventManager.TriggerEvent("ResetBeehive", gameObject);
            Destroy(other.gameObject);
        }
    }
}
