using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPad : MonoBehaviour
{
    private HashSet<GameObject> playerInsideTrigger = new HashSet<GameObject>();

    public bool isEmpty = true;

    public GameObject _slot = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnCrouchStart", OnCrouchHandler);
        EventManager.StartListening("InventoryAddEvent", OnInventoryAddEvent);
        EventManager.StartListening("LilypadCleanUp", OnLilypadCleanUp);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouchStart", OnCrouchHandler);
        EventManager.StopListening("InventoryAddEvent", OnInventoryAddEvent);
        EventManager.StopListening("LilypadCleanUp", OnLilypadCleanUp);
    }

    //Player Crouch in Lily Pad, we ask inventory to give us one butterfly
    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender) && isEmpty && InventoryManager.HasItemsByTagName(sender, "Butterfly"))
        {
            BroadcastMessage("OnHelperGlowDisable");
            EventManager.TriggerEvent("ItemReceived", gameObject, new EventDict() { { "receiver", gameObject }, { "giver", sender }, { "item", InventoryManager.GetItemByTagName(sender, "Butterfly") } });
            isEmpty = false;
            // EventManager.TriggerEvent("ItemUncollected", gameObject, new EventDict() { { "player", sender }, { "isRecollectable", false }, { "newTarget", gameObject } });
            EventManager.TriggerEvent("LilypadHasButterfly", gameObject);
            
        }
    }

    //When the butterfly is confirmed to be collected, lilyPad will ask to be followed
    void OnInventoryAddEvent(EventDict dict)
    {
        GameObject owner = (GameObject)dict["owner"];
        GameObject item = (GameObject)dict["item"];
        if (owner == gameObject)
        {
            _slot = item;
            EventManager.TriggerEvent("FollowMe", gameObject, new EventDict() { { "receiver", item } });
        }
    }

    void OnLilypadCleanUp(EventDict dict)
    {
        EventManager.TriggerEvent("FlyAway", gameObject, new EventDict() { { "receiver", _slot } });
        isEmpty = true;
        _slot = null;
        BroadcastMessage("OnHelperGlowEnable");
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
