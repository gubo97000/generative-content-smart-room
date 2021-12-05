using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferLight : MonoBehaviour
{
    void Start()
    {
        EventManager.StartListening("ActivateMushroom", ActivateMushroomHandler);
    }

    void OnDestroy()
    {
        EventManager.StopListening("ActivateMushroom", ActivateMushroomHandler);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Jump" && other.gameObject.GetComponent<JumpCollider>().isInAir())
            EventManager.TriggerEvent("ActivateMushroom", gameObject);
    }

    void ActivateMushroomHandler(EventDict dict)
    {

    }
}
