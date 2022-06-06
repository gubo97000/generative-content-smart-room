using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverApple : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "AppleUndragged")
        {
            EatApple(other.gameObject);
        }
    }

    void EatApple(GameObject go)
    {
        Destroy(go);

        EventManager.TriggerEvent("RecoverApple", gameObject);
    }
}
