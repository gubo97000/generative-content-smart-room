using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferLight : MonoBehaviour
{
    public bool onJump = true;
    public bool onCrouch = false;
    public bool isEmpty = true;
    public GameObject _slot = null;
    [SerializeField]
    private HashSet<GameObject> playerInsideTrigger = new HashSet<GameObject>();


    void Start()
    {
        EventManager.StartListening("ActivateMushroom", ActivateMushroomHandler);
        EventManager.StartListening("InventoryAddEvent", OnInventoryAddEvent);
        EventManager.StartListening("MushroomCleanUp", OnMushroomCleanUp);
        if (onCrouch) EventManager.StartListening("OnCrouchStart", ActionHandler);
        if (onJump) EventManager.StartListening("OnJumpStart", ActionHandler);
    }

    void OnDestroy()
    {
        EventManager.StopListening("ActivateMushroom", ActivateMushroomHandler);
        EventManager.StopListening("InventoryAddEvent", OnInventoryAddEvent);
        EventManager.StopListening("MushroomCleanUp", OnMushroomCleanUp);
        if (onCrouch) EventManager.StopListening("OnCrouchStart", ActionHandler);
        if (onJump) EventManager.StopListening("OnJumpStart", ActionHandler);
    }

    void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.tag == "Jump" && other.gameObject.GetComponent<JumpCollider>().isInAir())
        //     EventManager.TriggerEvent("ActivateMushroom", gameObject, new EventDict() {
        //         { "activator", other.gameObject.transform.parent.gameObject /* i.e. the player object */ }
        //     });

        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Add(other.gameObject);
            Debug.Log("Player entered trigger" + playerInsideTrigger);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Remove(other.gameObject);
            Debug.Log("Player exited trigger" + playerInsideTrigger);
        }
    }

    void ActionHandler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        if (playerInsideTrigger.Contains(sender))
        {
            Debug.Log("Player activated trigger");
            EventManager.TriggerEvent("ActivateMushroom", gameObject, new EventDict() {
                { "activator", sender /* i.e. the player object */ }
            });
        }

    }

    void ActivateMushroomHandler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        GameObject activator = (GameObject)dict["activator"];
        if (sender != gameObject) return;
        if (isEmpty && InventoryManager.HasItemsByTagName(activator, "Firefly"))
        {
            EventManager.TriggerEvent("ItemReceived", gameObject, new EventDict() { { "receiver", gameObject }, { "giver", activator }, { "item", InventoryManager.GetItemByTagName(activator, "Firefly") } });
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

            // Enable glow
            //transform.parent.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            transform.parent.transform.Find("Light").Find("Directional Light (2)").gameObject.SetActive(true);

            GetComponent<AudioSource>().Play();
            Debug.Log("FollowMe");
        }
    }

    void OnMushroomCleanUp(EventDict dict)
    {
        // EventManager.TriggerEvent("FlyAway", gameObject, new EventDict() { { "receiver", _slot } });
        EventManager.TriggerEvent("ItemUncollected", gameObject, new EventDict() { { "player", gameObject } });
        isEmpty = true;
        _slot = null;

        // Disable glow
        //transform.parent.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        transform.parent.transform.Find("Light").Find("Directional Light (2)").gameObject.SetActive(false);
    }
}
