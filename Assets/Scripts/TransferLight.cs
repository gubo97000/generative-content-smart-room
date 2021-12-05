using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferLight : MonoBehaviour
{
    public bool isEmpty = true;
    public GameObject _slot = null;

    void Start()
    {
        EventManager.StartListening("ActivateMushroom", ActivateMushroomHandler);
        EventManager.StartListening("InventoryAddEvent", OnInventoryAddEvent);
    }

    void OnDestroy()
    {
        EventManager.StopListening("ActivateMushroom", ActivateMushroomHandler);
        EventManager.StopListening("InventoryAddEvent", OnInventoryAddEvent);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Jump" && other.gameObject.GetComponent<JumpCollider>().isInAir())
            EventManager.TriggerEvent("ActivateMushroom", gameObject, new EventDict() { 
                { "sender", other.gameObject.transform.parent.gameObject /* i.e. the player object */ }
            });
    }

    void ActivateMushroomHandler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];

        if (isEmpty && InventoryManager.HasItemsByTagName(sender, "Firefly"))
        {
            EventManager.TriggerEvent("ItemReceived", gameObject, new EventDict() { { "receiver", gameObject }, { "giver", sender }, { "item", InventoryManager.GetItemByTagName(sender, "Firefly") } });
            isEmpty = false;
            EventManager.TriggerEvent("MushroomHasFirefly", gameObject);
        }
    }

    void OnInventoryAddEvent(EventDict dict)
    {
        GameObject owner = (GameObject)dict["owner"];
        GameObject item = (GameObject)dict["item"];
        if (owner == gameObject)
        {
            _slot = item;
            EventManager.TriggerEvent("FollowMe", gameObject, new EventDict() { { "receiver", item } });
            Debug.Log("FollowMe");
        }
    }
}
