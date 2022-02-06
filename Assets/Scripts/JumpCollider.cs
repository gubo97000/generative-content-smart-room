using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpCollider : MonoBehaviour
{
    private bool inAir = false;

    void Start()
    {
        EventManager.StartListening("OnJumpStart", OnJumpStartHandler);
        EventManager.StartListening("OnJumpEnd", OnJumpEndHandler);
    }

    void onDestroy()
    {
        EventManager.StopListening("OnJumpStart", OnJumpStartHandler);
        EventManager.StopListening("OnJumpEnd", OnJumpEndHandler);
    }

    void OnJumpStartHandler(EventDict dict)
    {
        inAir = true;
        GetComponent<AudioSource>()?.Play();
    }

    void OnJumpEndHandler(EventDict dict)
    {
        inAir = false;
    }

    public bool isInAir()
    {
        return inAir;
    }
}
