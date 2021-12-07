using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatApple : MonoBehaviour
{
    // The animator to manage when eating the apple
    private Animator anim;

    private bool isEating = false;

    void Start()
    {
        // Get the animator for managing the state later on
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Apple" && !isEating)
        {
            Destroy(other.gameObject);

            // Start eating animation
            isEating = true;
            anim.SetBool("isEating", true);

            Debug.Log("Eat the apple!");
        }
    }
}
