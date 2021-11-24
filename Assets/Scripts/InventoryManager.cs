using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// Same behavior as python's defaultdict 
[System.Serializable]
public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    Func<TValue> _init;
    public DefaultDictionary(Func<TValue> init)
    {
        _init = init;
    }
    public new TValue this[TKey k]
    {
        get
        {
            if (!ContainsKey(k))
                Add(k, _init());
            return base[k];
        }
        set => base[k] = value;
    }
}

public class InventoryManager : MonoBehaviour
{
    //Create an event
    public DefaultDictionary<GameObject, HashSet<GameObject>> inventory = new DefaultDictionary<GameObject, HashSet<GameObject>>(() => new HashSet<GameObject>());

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("ItemCollected", AddItem);
        EventManager.StartListening("ItemUncollected", RemoveItem);
    }

    // Update is called once per frame
    void Update()
    {
        // print(onItemAdded(5));
    }

    void OnDestroy()
    {
        EventManager.StopListening("ItemCollected", AddItem);
        EventManager.StopListening("ItemUncollected", RemoveItem);
    }

    //All the functions that subscribe to an event have this mandatory argument
    public void AddItem(EventDict dict)
    {
        //You need to cast the value of the dictionary to the corresponding type
        GameObject sender = (GameObject)dict["sender"];
        GameObject player = (GameObject)dict["player"];

        if (!inventory[player].Contains(sender))
        {
            inventory[player].Add(sender);

            print("Item " + sender.name + " added to " + player.name + " inventory");
            Debug.Log(prettyPrintToSring(inventory));
            EventManager.TriggerEvent("InventoryAddEvent", gameObject, new EventDict() { { "item", sender }, { "receiver", player } });
        }
    }

    public void RemoveItem(EventDict dict)
    {
        GameObject player = (GameObject)dict["player"];
        
        if (inventory[player].Count > 0)
        {
            GameObject sender = new List<GameObject>(inventory[player])[0];
            inventory[player].Remove(sender);

            print("Item " + sender.name + " removed from " + player.name + " inventory");
            Debug.Log(prettyPrintToSring(inventory));
            EventManager.TriggerEvent("InventoryRemoveEvent", gameObject, new EventDict() { { "item", sender }, { "receiver", player }, { "newTarget", dict["newTarget"] } });
            Debug.Log("Removed a butterfly");

            //Decide if recollectable
            sender.GetComponent<SphereCollider>().isTrigger = (bool)dict["isRecollectable"];
        }
    }

    //Deprecated
    public void PassItem(EventDict dict)
    {

        GameObject sender = (GameObject)dict["sender"];
        GameObject receiver = (GameObject)dict["receiver"];
        GameObject item = (GameObject)dict["item"];
        inventory[sender].Remove(item);
        inventory[receiver].Add(item);

        print("Item " + item.name + " passed from " + sender.name + " to " + receiver.name);
        EventManager.TriggerEvent("InventoryPassEvent", gameObject, new EventDict() { { "item", item }, { "from", sender }, { "to", receiver } });
    }

    public static string prettyPrintToSring(Dictionary<GameObject, HashSet<GameObject>> inventory)
    {
        string result = "";
        foreach (KeyValuePair<GameObject, HashSet<GameObject>> entry in inventory)
        {
            result += entry.Key.name + ": ";
            foreach (GameObject item in entry.Value)
            {
                result += item.name + " ";
            }
            result += "\n";
        }
        return result;
    }

}
