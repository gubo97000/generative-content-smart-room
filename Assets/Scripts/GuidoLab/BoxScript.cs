using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("FollowMe", setTargetToFollow);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has entered the box");
            EventManager.TriggerEvent("ItemCollected", gameObject, new Dictionary<string, object>() { ["player"] = other.gameObject });
            this.GetComponent<Chase>().player = other.gameObject;
            // InventoryManager.onItemAdded(1);
        }
    }

    public void setTargetToFollow(object data)
    {
        if ((int)((Dictionary<string, object>)data)["to"] == gameObject.GetInstanceID())
        {
            GameObject sender = (GameObject)((Dictionary<string, object>)data)["sender"];
            // GameObject targe = (GameObject)((Dictionary<string, object>)data)["sender"];
            // targetToFollow = sender;
            
        }
    }
}


