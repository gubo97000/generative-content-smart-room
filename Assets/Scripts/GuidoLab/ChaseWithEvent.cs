using UnityEngine;
using System.Collections.Generic;

public class ChaseWithEvent : MonoBehaviour
{
    public int speed = 2;
    public GameObject target;

void Start(){
    EventManager.StartListening("FollowMe", onFollowMe);
}
void onFollowMe(object data)
    {
        var dict = (Dictionary<string, object>)data;
        if (((GameObject)dict["receiver"]) == gameObject)
        {
            target = (GameObject)dict["sender"];

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

