using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFloorGesture : MonoBehaviour
{
    public Animator animator;

    public bool isPlayer1;

    public bool onCrouch;
    public bool onJump;

    public bool requiresDraggingBall;

    private bool isHandRaiseEnabled = false;

    private GameObject ball = null;
    private HashSet<GameObject> playerInsideTrigger = new HashSet<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnCrouch", OnCrouchHandler);
        EventManager.StartListening("OnJumpLanding", OnJumpHandler);
        EventManager.StartListening("OnJumpStart", OnJump1Handler);
        EventManager.StartListening("OnHandRaise", OnHandRaise);

        if (requiresDraggingBall)
            onCrouch = false;
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouch", OnCrouchHandler);
        EventManager.StopListening("OnJumpLanding", OnJumpHandler);

        EventManager.StopListening("OnJumpStart", OnJump1Handler);
        EventManager.StopListening("OnHandRaise", OnHandRaise);
    }

    // Update is called once per frame
    void Update()
    {
        // Check if tutorial step completed
        if (animator.GetBool("check1") == true && animator.GetBool("check2") == true)
        {
            animator.SetTrigger("close");
            StartCoroutine(GetNextTutorial());
        }

        // Used in Raise Hand tutorial, to check if player is inside circle
        if (!onCrouch && !onJump)
        {
            Collider[] thingsInBounds = Physics.OverlapSphere(transform.position, transform.localScale.x / 2);

            // If player1 (2) is inside, then enable raise hand
            foreach (Collider thing in thingsInBounds)
            {
                if (thing.tag.StartsWith("Player") && isPlayer1 == ((thing.GetComponent<PlayerInfo>().playerNumber + 1) == 1))
                {
                    isHandRaiseEnabled = true;
                    break;
                }
                isHandRaiseEnabled = false;
            }
        }
    }

    void OnCrouchHandler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        // if triggerable on crouch
        // and the player who crouched is the one required by this
        if (onCrouch &&
            (isPlayer1 && sender.tag == "Player1" || !isPlayer1 && sender.tag == "Player2"))
        {
            animator.SetBool(isPlayer1 ? "check1" : "check2", true);
            Debug.Log(isPlayer1 ? "check1" : "check2");

            if (ball != null)
            {
                Destroy(ball);
            }
        }
    }

    void OnJumpHandler(EventDict dict)
    {
        if (onJump && (bool)dict["isPlayer1"] == isPlayer1)
        {
            animator.SetBool(isPlayer1 ? "check1" : "check2", true);
            Debug.Log(isPlayer1 ? "check1" : "check2");
        }
    }
    void OnJump1Handler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        if (playerInsideTrigger.Contains(sender))
        {
             animator.SetBool(isPlayer1 ? "check1" : "check2", true);
            Debug.Log(isPlayer1 ? "check1" : "check2");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Jump tutorial

        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Add(other.gameObject);
            Debug.Log("Player entered trigger" + playerInsideTrigger);
        }


        if (other.gameObject.tag == "Jump" && other.gameObject.GetComponent<JumpCollider>().isInAir())
            EventManager.TriggerEvent("OnJumpLanding", gameObject, new EventDict() { { "isPlayer1", other.transform.parent.gameObject.tag == "Player1" } });

        // Crouch-Drag tutorial
        if (other.gameObject.tag == "Seed" && requiresDraggingBall)
        {
            // change color of circle for feedback
            onCrouch = true;
            ball = other.gameObject;
        }

        // Raise hand tutorial
        //if (!onCrouch && !onJump && other.transform.parent.gameObject.tag == (isPlayer1 ? "Player1" : "Player2"))
        //{
        //    isHandRaiseEnabled = true;
        //}
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Remove(other.gameObject);
            Debug.Log("Player exited trigger" + playerInsideTrigger);
        }
    }


    void OnTriggerLeave(Collider other)
    {
        // Crouch-Drag tutorial
        if (other.gameObject.tag == "Seed" && requiresDraggingBall)
        {
            // change color of circle for feedback
            onCrouch = false;
            ball = null;
        }

        // Raise hand tutorial
        //if (!onCrouch && !onJump && other.transform.parent.gameObject.tag == (isPlayer1 ? "Player1" : "Player2"))
        //{
        //    isHandRaiseEnabled = false;
        //}
    }

    IEnumerator GetNextTutorial()
    {
        yield return new WaitForSeconds(2f);
        EventManager.TriggerEvent("NextTutorial", gameObject);
    }

    void OnHandRaise(EventDict dict)
    {
        if (isHandRaiseEnabled && !onCrouch && !onJump)
            animator.SetBool((((GameObject)dict["sender"]).GetComponent<PlayerInfo>().playerNumber + 1) == 1 ? "check1" : "check2", true);
    }

}
