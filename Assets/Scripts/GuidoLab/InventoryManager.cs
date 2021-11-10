using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //Create an event
    public delegate int OnItemAdded(int num);
    public static OnItemAdded onItemAdded;

    // Start is called before the first frame update
    void Start()
    {
        // Collectable.onItemCollected += AddItem;
        EventManager.StartListening("ItemCollected", AddItem);
        onItemAdded += (num) => { return num; };
        onItemAdded += (num) => { return num + 1; };
    }

    // Update is called once per frame
    void Update()
    {
        // print(onItemAdded(5));
    }

    void AddItem(int num)
    {
        print("Item "  + " added to inventory");
    }
}
