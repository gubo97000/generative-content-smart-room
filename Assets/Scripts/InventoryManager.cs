using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

// This dict has the same behavior as python's defaultdict 
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
    //Create a dictionary 
    public static DefaultDictionary<GameObject, HashSet<GameObject>> inventory = new DefaultDictionary<GameObject, HashSet<GameObject>>(() => new HashSet<GameObject>());
    public int maxSize = 3;
    
    private static InventoryManager inventoryManager;

    public static InventoryManager instance
    {
        get
        {
            if (!inventoryManager)
            {
                inventoryManager = FindObjectOfType(typeof(InventoryManager)) as InventoryManager;

                if (!inventoryManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    inventoryManager.Init();
                }
            }

            return inventoryManager;
        }
    }

    void Init()
    {
        if (inventory == null)
        {
            inventory = new DefaultDictionary<GameObject, HashSet<GameObject>>(() => new HashSet<GameObject>());
        }
    }
    void Start()
    {
        EventManager.StartListening("ItemCollected", AddItem);
        EventManager.StartListening("ItemUncollected", RemoveItem);
        EventManager.StartListening("ItemReceived", PassItem);
    }

    void OnDestroy()
    {
        EventManager.StopListening("ItemCollected", AddItem);
        EventManager.StopListening("ItemUncollected", RemoveItem);
        EventManager.StopListening("ItemReceived", PassItem);
    }

    //All the functions that subscribe to an event have this mandatory argument
    public void AddItem(EventDict dict)
    {
        //You need to cast the value of the dictionary to the corresponding type
        GameObject sender = (GameObject)dict["sender"];
        GameObject player = (GameObject)dict["player"];

        if (!inventory[player].Contains(sender))
        {
            if (inventory[player].Count < maxSize)
            {
                inventory[player].Add(sender);

                print("Item " + sender.name + " added to " + player.name + " inventory");
                Debug.Log(prettyPrintToString(inventory));
                EventManager.TriggerEvent("InventoryAddEvent", gameObject, new EventDict() { { "item", sender }, { "owner", player } });
            }
            else
            {
                EventManager.TriggerEvent("FlyAway", gameObject, new EventDict() { { "receiver", sender } });
            }
        }
    }

    public void RemoveItem(EventDict dict)
    {
        GameObject player = (GameObject)dict["player"];

        if (inventory[player].Count > 0)
        {
            //Very dangerous to remove the first item without knowing what it is
            GameObject sender = new List<GameObject>(inventory[player])[0];
            inventory[player].Remove(sender);

            print("Item " + sender.name + " removed from " + player.name + " inventory");
            Debug.Log(prettyPrintToString(inventory));
            //EventManager.TriggerEvent("InventoryRemoveEvent", gameObject, new EventDict() { { "item", sender }, { "owner", player }, { "newTarget", dict["newTarget"] } });

            EventManager.TriggerEvent("FlyAway", gameObject, new EventDict() { { "receiver", sender } });

            //Decide if recollectable
            //sender.GetComponent<SphereCollider>().isTrigger = ((bool)dict["isRecollectable"] != null ? (bool)dict["isRecollectable"] : false);
        }
    }

    //
    public void PassItem(EventDict dict)
    {
        // GameObject sender = (GameObject)dict["sender"];

        GameObject giver = (GameObject)dict["giver"];
        GameObject receiver = (GameObject)dict["receiver"];
        GameObject item = (GameObject)dict["item"];
        inventory[giver].Remove(item);
        inventory[receiver].Add(item);
Debug.Log(prettyPrintToString(inventory));
        print("Item " + item.name + " passed from " + giver.name + " to " + receiver.name);
        // EventManager.TriggerEvent("InventoryPassEvent", gameObject, new EventDict() { { "item", item }, { "giver", giver }, { "receiver", receiver } });
        // EventManager.TriggerEvent("InventoryRemoveEvent", gameObject, new EventDict() { { "item", sender }, { "receiver", player }, { "newTarget", dict["newTarget"] } });
        EventManager.TriggerEvent("InventoryRemoveEvent", gameObject, new EventDict() { { "item", item }, { "owner", giver } });
        EventManager.TriggerEvent("InventoryAddEvent", gameObject, new EventDict() { { "item", item }, { "owner", receiver } });
    }

    public static List<GameObject> GetItemsByTagName(GameObject owner, string tagName)
    {
        List<GameObject> res = new List<GameObject>();
        foreach (var item in inventory[owner])
        {
            if (item.tag == tagName)
            {
                res.Add(item);
            }
        }
        return res;
    }
    public static GameObject GetItemByTagName(GameObject owner, string tagName)
    {
        foreach (var item in inventory[owner])
        {
            if (item.tag == tagName)
            {
                return item;
            }
        }
        return null;
    }
    public static bool HasItemsByTagName(GameObject owner, string tagName)
    {
        foreach (var item in inventory[owner])
        {
            if (item.tag == tagName)
            {
                return true;
            }
        }
        return false;
    }
    public static string prettyPrintToString(Dictionary<GameObject, HashSet<GameObject>> inventory)
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
