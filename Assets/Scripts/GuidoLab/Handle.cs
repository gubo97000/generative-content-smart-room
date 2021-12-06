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
    public float? guidePlayerAfter = 10f;
    private List<GameObject> playerInsideTrigger = new List<GameObject>();
    private List<GameObject> playerNotAllowed = new List<GameObject>(); //List of players that have grabbed another brother handle

    void Start()
    {
        if (transform.parent == null) Debug.LogWarning("Handle MUST have a parent to work correctly");
        EventManager.StartListening("OnCrouch", OnCrouchHandler);
        EventManager.StartListening("OnJump", OnJumpHandler);
        EventManager.StartListening("HandleGrabbed", onHandleGrabbed);
        // EventManager.StartListening("HandleUngrabbed", onHandleUngrabbed);
    }
    void OnDestroy()
    {
        EventManager.StopListening("OnCrouch", OnCrouchHandler);
        EventManager.StopListening("OnJump", OnJumpHandler);
        EventManager.StopListening("HandleGrabbed", onHandleGrabbed);
        // EventManager.StopListening("HandleUngrabbed", onHandleUngrabbed);
    }
    void Grab(GameObject sender)
    {
        Debug.Log(gameObject + " grabbed");
        EventManager.TriggerEvent("HandleGrabbed", gameObject, new EventDict() { ["player"] = sender, ["parent"] = transform.parent });

        gameObject.GetComponent<ChaseWithRigidBody>().target = sender.tag == "Player" ? sender.GetComponent<Player>().target : sender.transform;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        isGrabbable = false;
        isGrabbed = true;
        playerInsideTrigger.RemoveAll(item => item);

        CancelGuidePlayer();

        if (GetComponent<HelperGlow>() != null)
        {
            GetComponent<HelperGlow>().enabled = false;
        }
        else if (GetComponentInParent<ObjectStateHandler>())
        {
            GetComponentInParent<ObjectStateHandler>().SendMessage("OnGrabbed", gameObject);
        }

    }
    void Ungrab(GameObject sender)
    {
        Debug.Log(gameObject + " ungrabbed");
        EventManager.TriggerEvent("HandleUngrabbed", gameObject, new EventDict() { ["player"] = sender, ["parent"] = transform.parent });

        gameObject.GetComponent<ChaseWithRigidBody>().target = null;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        isGrabbable = true;
        isGrabbed = false;
    }
    void OnCrouchHandler(EventDict data)
    {
        GameObject sender = (GameObject)data["sender"];
        //Two cases:
        //is going to be grabbed
        if (playerInsideTrigger.Contains(sender) && !playerNotAllowed.Contains(sender) && onCrouch && !isGrabbed)
        {
            Grab(sender);
        }
        // //is going to be ungrabbed
        // else if (onCrouch && isGrabbed)
        // {
        //     Ungrab(sender);
        // }
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
        if (guidePlayerAfter != null)
        {
            Invoke("GuidePlayer", guidePlayerAfter ?? 0);
        }
    }

    void GuidePlayer()
    {
        var lineObj = new GameObject("HelpLine");

        // GuideLine line = new GuideLine();
        GuideLine line = lineObj.AddComponent<GuideLine>();
        // line.SendMessage("SetTarget", gameObject.transform.position);
        line.target = gameObject.transform;
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (playerNotAllowed.Contains(player)) continue;
            // line.SendMessage("SetToGuide",player.transform);
            line.toGuide = player.transform;
            break;
        }
    }

    void CancelGuidePlayer()
    {
        CancelInvoke("GuidePlayer");
        EventManager.TriggerEvent("DeleteGuideLine", gameObject);
    }

    // void onHandleUngrabbed(EventDict dict)
    // {
    //     if (!isGrabbed) return; //If is grabbed, don't want to stuck

    //     if ((GameObject)dict["sender"] == gameObject) return; //Return if message is from myself

    //     if ((Transform)dict["parent"] != transform.parent) return; //Return if message is not from my brother

    //     //If I'm here. I'm the other handle
    //     //I ungrab too
    //     Debug.Log(gameObject + " ungrabbed");

    //     gameObject.GetComponent<ChaseWithRigidBody>().target = null;
    //     gameObject.GetComponent<Rigidbody>().isKinematic = false;
    //     isGrabbable = true;
    //     isGrabbed = false;
    //     playerNotAllowed.RemoveAll(item => item);
    // }
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
