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
    private List<GameObject> playerAllowed = new List<GameObject>();

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

    void OnCrouchHandler(object data)
    {
        var dict = (Dictionary<string, object>)data;
        GameObject sender = (GameObject)dict["sender"];

        if (playerAllowed.Contains(sender) && onCrouch)
        {
            Debug.Log(gameObject + " collected");
            EventManager.TriggerEvent("ItemCollected", gameObject, new Dictionary<string, object>() { ["player"] = sender });
            isCollectable = false;
            playerAllowed.RemoveAll(item => item);

        }
    }
    void OnJumpHandler(object data)
    {
        var dict = (Dictionary<string, object>)data;
        GameObject sender = (GameObject)dict["sender"];

        if (playerAllowed.Contains(sender) && onJump)
        {
            Debug.Log(gameObject + " collected");
            EventManager.TriggerEvent("ItemCollected", gameObject, new Dictionary<string, object>() { ["player"] = sender });
            isCollectable = false;
            playerAllowed.RemoveAll(item => item);
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
                playerAllowed.Add(other.gameObject);

                if (onStep)
                {
                    Debug.Log(gameObject + " collected");
                    EventManager.TriggerEvent("ItemCollected", gameObject, new Dictionary<string, object>() { ["player"] = other.gameObject });
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
                playerAllowed.Remove(other.gameObject);
            }
        }
    }
}
