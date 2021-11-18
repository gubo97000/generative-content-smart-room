using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Handle : MonoBehaviour
{
    public bool onCrouch;
    public bool onStep;
    public bool onJump;
    public bool isGrabbable = true;
    public bool isGrabbed = false;
    private List<GameObject> playerInsideTrigger = new List<GameObject>();
    private List<GameObject> playerNotAllowed = new List<GameObject>();

    void Start()
    {
        EventManager.StartListening("OnCrouch", OnCrouchHandler);
        EventManager.StartListening("OnJump", OnJumpHandler);
        EventManager.StartListening("HandleGrabbed", onHandleGrabbed);
    }
    void OnDestroy()
    {
        EventManager.StopListening("OnCrouch", OnCrouchHandler);
        EventManager.StopListening("OnJump", OnJumpHandler);
        EventManager.StopListening("HandleGrabbed", onHandleGrabbed);
    }
    void Grab(GameObject sender)
    {
        Debug.Log(gameObject + " grabbed");
        EventManager.TriggerEvent("HandleGrabbed", gameObject, new EventDict() { ["player"] = sender, ["parent"] = transform.parent });

        gameObject.GetComponent<ChaseWithRigidBody>().target = sender.transform;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        isGrabbable = false;
        isGrabbed = true;
        playerInsideTrigger.RemoveAll(item => item);
    }
    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender)&& !playerNotAllowed.Contains(sender) && onCrouch)
        {
            Grab(sender);
        }
    }
    void OnJumpHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];

        if (playerInsideTrigger.Contains(sender) && !playerNotAllowed.Contains(sender) && onJump)
        {
            Grab(sender);
        }
    }
    void onHandleGrabbed(EventDict dict)
    {
        if (isGrabbed) return; //If is grabbed, don't want to stuck
        
        if ((GameObject)dict["sender"] == gameObject) return;//Check if message is from myself

        if ((Transform)dict["parent"] != transform.parent) return; //Check if message is from my brother

        //If I'm here. I'm the other handle
        //I don't want to be grabbed by the same player
        playerNotAllowed.Add((GameObject)dict["player"]);
        //Stuck myself for 2P interaction
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (isGrabbable)
        {
            if (other.gameObject.tag == "Player")
            {
                playerInsideTrigger.Add(other.gameObject);

                if (onStep && !playerNotAllowed.Contains(other.gameObject))
                {
                    Grab(other.gameObject);
                }

            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isGrabbable)
        {
            if (other.gameObject.tag == "Player")
            {
                playerInsideTrigger.Remove(other.gameObject);
            }
        }
    }
}
