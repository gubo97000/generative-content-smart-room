using UnityEngine;
using System.Collections.Generic;

public class ChaseWithEvent : MonoBehaviour
{
    public float speed = 1.0f;
    public bool lookAtTarget = true;
    private GameObject target;

    void Start()
    {
        EventManager.StartListening("FollowMe", onFollowMe);
        EventManager.StartListening("FlyAway", OnFlyAway);
    }
    void OnDestroy()
    {
        EventManager.StopListening("FollowMe", onFollowMe);
        EventManager.StopListening("FlyAway", OnFlyAway);
        // EventManager.StartListening("UnfollowMe", onUnfollowMe);
    }

    void onFollowMe(EventDict dict)
    {
        if ((dict["receiver"] as GameObject) == gameObject)
        {
            target = (GameObject)dict["sender"];
        }
    }
    void OnFlyAway(EventDict dict)
    {
        if ((dict["receiver"] as GameObject) == gameObject)
        {
            var go = new GameObject();
            go.transform.position = new Vector3(3,100,-100);
            target = go;
        }
    }

    // void onUnfollowMe(EventDict dict)
    // {
    //     if (((GameObject)dict["receiver"]) == gameObject)
    //     {
    //         target = (GameObject)dict["newTarget"];
    //     }
    // }

    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
            if (lookAtTarget)
            {
                transform.position = Vector3.RotateTowards(transform.position, target.transform.position, speed * Time.deltaTime, speed * Time.deltaTime);
            }
                // Vector3 localPosition = target.transform.position - transform.position;
                // localPosition = localPosition.normalized; // The normalized direction in LOCAL space
                //                                           //I think there's the need to unpack it:
                //                                           // localPosition * Time.deltaTime * speed; //Should do the work
                // transform.Translate(localPosition.x * Time.deltaTime * speed, localPosition.y * Time.deltaTime * speed, localPosition.z * Time.deltaTime * speed);
        }

    }
}

