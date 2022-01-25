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

    // Start is called before the first frame update
    void Start()
    {    
        EventManager.StartListening("OnCrouch", OnCrouchHandler);
        EventManager.StartListening("OnJumpLanding", OnJumpHandler);

        if (requiresDraggingBall)
            onCrouch = false;
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouch", OnCrouchHandler);
        EventManager.StopListening("OnJumpLanding", OnJumpHandler);
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("check1") == true && animator.GetBool("check2") == true)
        {
            animator.SetTrigger("close");
            StartCoroutine(GetNextTutorial());
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jump" && other.gameObject.GetComponent<JumpCollider>().isInAir())
            EventManager.TriggerEvent("OnJumpLanding", gameObject, new EventDict() { { "isPlayer1", other.transform.parent.gameObject.tag == "Player1" } });
    
        if(other.gameObject.tag == "Seed" && requiresDraggingBall)
        {
            // change color of circle for feedback
            onCrouch = true;
        }
    }

    void OnTriggerLeave(Collider other)
    {
        if (other.gameObject.tag == "Seed" && requiresDraggingBall)
        {
            // change color of circle for feedback
            onCrouch = false;
        }
    }

    IEnumerator GetNextTutorial()
    {
        yield return new WaitForSeconds(2f);
        EventManager.TriggerEvent("NextTutorial", gameObject);
    }
}
