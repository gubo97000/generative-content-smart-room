using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    // Start is called before the first frame update
    // void Start()
    // {
    //     EventManager.StartListening("FollowMe", setTargetToFollow);
    // }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has entered the box");
            EventManager.TriggerEvent("ItemCollected", gameObject, new EventDict() { ["player"] = other.gameObject });
        }
    }

    // public void setTargetToFollow(object data)
    // {
    //     if ((int)((EventDict)data)["to"] == gameObject.GetInstanceID())
    //     {
    //         GameObject sender = (GameObject)((EventDict)data)["sender"];
    //         // GameObject targe = (GameObject)((EventDict)data)["sender"];
    //         // targetToFollow = sender;
            
    //     }
    // }
}


