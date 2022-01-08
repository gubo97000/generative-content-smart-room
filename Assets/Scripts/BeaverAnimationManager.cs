using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverAnimationManager : MonoBehaviour
{
    // The animator to manage when eating the apple
    private Animator anim;

    private bool isEating = false;

    void Start()
    {
        // Get the animator for managing the state later on
        anim = GetComponent<Animator>();

        EventManager.StartListening("EndOfPath", OnEndOfPath);
        EventManager.StartListening("ResetPath", OnResetPath);
    }

    void OnDestroy()
    {
        EventManager.StopListening("EndOfPath", OnEndOfPath);
        EventManager.StopListening("ResetPath", OnResetPath);
    }

    // Feed the apple
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Apple" && !isEating)
        {
            EatApple(other);
        }
    }

    // Stop running
    void OnEndOfPath(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];

        if(sender == gameObject)
            anim.SetBool("hasReachedSpot", true);
    }

    void EatApple(Collider other)
    {
        Destroy(other.gameObject);

        // Start eating animation
        isEating = true;
        anim.SetBool("isEating", true);
        EventManager.TriggerEvent("BeaverTamed", gameObject);

        Debug.Log("Eat the apple!");
    }

    public bool isBeaverEating()
    {
        return isEating;
    }

    void OnResetPath(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];

        if (sender == gameObject)
        {
            anim.SetBool("hasReachedSpot", false);
            anim.SetBool("isEating", false);
        }
    }
}
