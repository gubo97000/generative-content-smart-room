using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnApples : Tree
{
    public GameObject prefab;
    public int amount;
    private List<GameObject> instances = new List<GameObject>();
    private int recoveredApples = 0;

    void Start()
    {
        EventManager.StartListening("ClearAppleTree", OnClearTree);
        EventManager.StartListening("RecoverApple", RecoverApple);
    }

    void OnDestroy()
    {
        EventManager.StopListening("ClearAppleTree", OnClearTree);
        EventManager.StopListening("RecoverApple", RecoverApple);
    }
    // void OnObjectDestroyed(EventDict dict)
    // {
    //     Debug.Log(instances.Count);
    //     if (instances.Count == 0)
    //     {
    //         BroadcastMessage("OnHelperGlowEnable", gameObject, SendMessageOptions.DontRequireReceiver);
    //     }
    // }

    void OnMouseDown()
    {
        StartCoroutine(Shake());
        
        if (instances.Count == 0)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 position = transform.position;
                position.x += Random.Range(-8, 8) / 5;
                position.y += 2.5f;
                position.z += Random.Range(5, 10) / 5;

                GameObject instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
                instances.Add(instance);
            }
            BroadcastMessage("OnHelperGlowDisable", gameObject);
            EventManager.TriggerEvent("SpawnBeavers", gameObject);
            Debug.Log("Apples fell from the apple tree");
        }
        else if (recoveredApples > 0)
        {
            for (int i = 0; i < recoveredApples; i++)
            {
                Vector3 position = transform.position;
                position.x += Random.Range(-8, 8) / 5;
                position.y += 2.5f;
                position.z += Random.Range(5, 10) / 5;

                GameObject instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
                
                instances.Add(instance);
            }
            recoveredApples = 0;

            BroadcastMessage("OnHelperGlowDisable", gameObject);
            Debug.Log("Recovered apple fell from the apple tree");
        }
    }

    // Reset tree: now more apples can spawn
    // TODO: do not show apples on tree until reset
    void OnClearTree(EventDict dict)
    {
        BroadcastMessage("OnHelperGlowEnable", gameObject, SendMessageOptions.DontRequireReceiver);
        instances.Clear();
    }

    void RecoverApple(EventDict dict)
    {
        recoveredApples++;
    }
}
