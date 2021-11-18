using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class adds the collectable behaviour to any object, a trigger collider is required
/// to activate the collectable zone (should be bigger than the object)
/// </summary>
public class Collectable : MonoBehaviour
{
    public bool onCrouch;
    public bool onStep = true;
    public bool onJump;
    public bool isCollectable = true;
    private List<GameObject> playerInsideTrigger = new List<GameObject>();

    void Start()
    {
        EventManager.StartListening("OnCrouch", OnCrouchHandler);
        EventManager.StartListening("OnJump", OnJumpHandler);
    }
    void OnDestroy()
    {
        EventManager.StopListening("OnCrouch", OnCrouchHandler);
        EventManager.StopListening("OnJump", OnJumpHandler);
    }

    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender) && onCrouch)
        {
            Debug.Log(gameObject + " collected");
            EventManager.TriggerEvent("ItemCollected", gameObject, new EventDict() { ["player"] = sender });
            isCollectable = false;
            playerInsideTrigger.RemoveAll(item => item);

        }
    }
    void OnJumpHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender) && onJump)
        {
            Debug.Log(gameObject + " collected");
            EventManager.TriggerEvent("ItemCollected", gameObject, new EventDict() { ["player"] = sender });
            isCollectable = false;
            playerInsideTrigger.RemoveAll(item => item);
        }
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollectable)
        {
            if (other.gameObject.tag == "Player")
            {
                playerInsideTrigger.Add(other.gameObject);

                if (onStep)
                {
                    Debug.Log(gameObject + " collected");
                    EventManager.TriggerEvent("ItemCollected", gameObject, new EventDict() { ["player"] = other.gameObject });
                    isCollectable = false;
                }

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isCollectable)
        {
            if (other.gameObject.tag == "Player")
            {
                playerInsideTrigger.Remove(other.gameObject);
            }
        }
    }
}
