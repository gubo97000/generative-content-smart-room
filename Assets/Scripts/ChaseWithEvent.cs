using UnityEngine;
using System.Collections.Generic;

public class ChaseWithEvent : MonoBehaviour
{
    public int speed = 2;
    private GameObject target;

    void Start()
    {
        EventManager.StartListening("FollowMe", onFollowMe);
        EventManager.StartListening("UnfollowMe", onUnfollowMe);
    }

    void onFollowMe(EventDict dict)
    {
        if (((GameObject)dict["receiver"]) == gameObject)
        {
            target = (GameObject)dict["sender"];
        }
    }

    void onUnfollowMe(EventDict dict)
    {
        if (((GameObject)dict["receiver"]) == gameObject)
        {
            target = (GameObject)dict["newTarget"];
        } 
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 localPosition = target.transform.position - transform.position;
            localPosition = localPosition.normalized; // The normalized direction in LOCAL space
                                                      //I think there's the need to unpack it:
                                                      // localPosition * Time.deltaTime * speed; //Should do the work
            transform.Translate(localPosition.x * Time.deltaTime * speed, localPosition.y * Time.deltaTime * speed, localPosition.z * Time.deltaTime * speed);
        }

    }
}

