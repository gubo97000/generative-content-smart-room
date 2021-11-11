using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    //Create an event
    public UnityEvent<int> onItemAdded;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("ItemCollected", AddItem);
    }

    // Update is called once per frame
    void Update()
    {
        // print(onItemAdded(5));
    }

    void OnDestroy()
    {
        EventManager.StopListening("ItemCollected", AddItem);
    }

    //All the functions that subscribe to an event have this mandatory argument
    private void AddItem(object data)
    {
        var dict = (Dictionary<string, object>)data; //You need to cast the object to a dictionary
        //You need to cast the value of the dictionary to the corresponding type
        GameObject sender = (GameObject)dict["sender"];
        print("Item " + sender.GetInstanceID() + " added to inventory");
    }

}
