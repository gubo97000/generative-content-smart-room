using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaverAnimationManager : MonoBehaviour
{
    // The animator to manage when eating the apple
    private Animator anim;

    private bool isEating = false;
    private bool hasEaten = false;
    private bool isReadyToBuild = false;

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
            EatApple(other.gameObject);
        }
    }
    void AppleCollision(GameObject apple)
    {
        if(apple.tag == "Apple" && !isEating)
        {
            EatApple(apple);
        }
    }

    // Stop running
    void OnEndOfPath(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];

        if(sender == gameObject)
            anim.SetBool("hasReachedSpot", true);

        if (hasEaten && !isReadyToBuild)
        {
            isReadyToBuild = true;
            EventManager.TriggerEvent("ReadyToBuild", gameObject);
        }
    }

    void EatApple(GameObject go)
    {
        Destroy(go);

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

    public bool isBeaverReadyToBuild()
    {
        return isReadyToBuild;
    }

    void OnResetPath(EventDict dict)
    {
        isEating = false;
        hasEaten = true;
        GameObject sender = (GameObject)dict["sender"];

        if (sender == gameObject)
        {
            anim.SetBool("hasReachedSpot", false);
            anim.SetBool("isEating", false);
        }
    }
}
