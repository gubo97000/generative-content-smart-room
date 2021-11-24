using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachButterfly : MonoBehaviour
{
    private List<GameObject> playerInsideTrigger = new List<GameObject>();

    public bool isEmpty = true;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnCrouchStart", OnCrouchHandler);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouchStart", OnCrouchHandler);
    }

    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender) && isEmpty)
        {
            EventManager.TriggerEvent("ItemUncollected", gameObject, new EventDict() { { "player", sender }, { "isRecollectable", false }, { "newTarget", gameObject } });
            isEmpty = false;

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
