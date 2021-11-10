using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{
    public delegate void OnCollected(GameObject item);
    public static OnCollected onCollected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Player has entered the box");
            EventManager.TriggerEvent("ItemCollected");
            Destroy(gameObject);
            InventoryManager.onItemAdded(1);
        }
    }
}
