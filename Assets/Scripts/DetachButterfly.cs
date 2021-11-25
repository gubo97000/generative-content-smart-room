using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachButterfly : MonoBehaviour
{
    private HashSet<GameObject> playerInsideTrigger = new HashSet<GameObject>();

    public bool isEmpty = true;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnCrouchStart", OnCrouchHandler);
        EventManager.StartListening("InventoryAddEvent", OnInventoryAddEvent);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouchStart", OnCrouchHandler);
        EventManager.StopListening("InventoryAddEvent", OnInventoryAddEvent);
    }

    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender) && isEmpty && InventoryManager.HasItemsByTagName(sender, "Butterfly"))
        {
            EventManager.TriggerEvent("ItemReceived", gameObject, new EventDict() { { "receiver", gameObject }, { "giver", sender }, { "item", InventoryManager.GetItemByTagName(sender, "Butterfly") } });
            isEmpty = false;
            // EventManager.TriggerEvent("ItemUncollected", gameObject, new EventDict() { { "player", sender }, { "isRecollectable", false }, { "newTarget", gameObject } });
            EventManager.TriggerEvent("LilypadHasButterfly", gameObject);
        }
    }

    //When the butterfly is confirmed to be collected, will ask to be followed
    void OnInventoryAddEvent(EventDict dict)
    {
        GameObject owner = (GameObject)dict["owner"];
        GameObject item = (GameObject)dict["item"];
        if (owner == gameObject)
        {
            EventManager.TriggerEvent("FollowMe", gameObject, new EventDict() { { "receiver", item } });
            Debug.Log("FollowMe");
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
