using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachButterfly : MonoBehaviour
{
    private List<GameObject> playerInsideTrigger = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnCrouchEnd", OnCrouchHandler);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouchEnd", OnCrouchHandler);
    }

    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender))
        {
            EventManager.TriggerEvent("ItemUncollected", gameObject, new EventDict() { ["player"] = sender });
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Add(other.gameObject);
        }
        Debug.Log("Jumped on lilypad");
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Remove(other.gameObject);
        }
        Debug.Log("Left the lilypad");
    }
}
