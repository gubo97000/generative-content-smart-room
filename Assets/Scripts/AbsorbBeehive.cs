using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbBeehive : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Beehive")
        {
            EventManager.TriggerEvent("HoneyPond", gameObject);
            EventManager.TriggerEvent("ResetBeehive", gameObject);
            GetComponent<AudioSource>().Play(0);
            Destroy(other.gameObject);
        }
    }
}
