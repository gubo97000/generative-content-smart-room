using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFloorGesture : MonoBehaviour
{
    public Animator animator;

    public bool isPlayer1;

    public bool onCrouch;
    public bool onJump;

    // Start is called before the first frame update
    void Start()
    {    
        EventManager.StartListening("OnCrouch", OnCrouchHandler);
        EventManager.StartListening("OnJumpLanding", OnJumpHandler);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        EventManager.StopListening("OnCrouch", OnCrouchHandler);
        EventManager.StopListening("OnJumpLanding", OnJumpHandler);
    }

    void OnCrouchHandler(EventDict dict)
    {
        if(onCrouch)
            animator.SetTrigger(isPlayer1 ? "check1" : "check2");
    }

    void OnJumpHandler(EventDict dict)
    {
        if (onJump)
            animator.SetTrigger(isPlayer1 ? "check1" : "check2");
    }
}
