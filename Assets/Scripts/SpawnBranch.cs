using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBranch : Tree
{
    public GameObject prefab;

    public float xOffsetMin = -2f, xOffsetMax = 2f;
    public float yOffsetMin = 3f, yOffsetMax = 3f;
    public float zOffsetMin = 1f, zOffsetMax = 2f;

    //Branch Instance
    private GameObject instance = null;


    private void Start()
    {
        EventManager.StartListening("ObjectDestroyed", OnObjectDestroyed);
    }
    private void OnDestroy()
    {
        EventManager.StopListening("ObjectDestroyed", OnObjectDestroyed);
    }
    void OnObjectDestroyed(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        Debug.Log($"Destroyed + {instance} + {sender} + {instance == null} {instance == sender}");
        if (instance == sender)
        {
            Debug.Log("Destroyedd");
            BroadcastMessage("OnHelperGlowEnable", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }


    void OnMouseDown()
    {
        Vector3 position = transform.position;
        position.x += Random.Range(5 * xOffsetMin, 5 * xOffsetMax) / 5;
        position.y += Random.Range(5 * yOffsetMin, 5 * yOffsetMax) / 5;
        position.z += Random.Range(5 * zOffsetMin, 5 * zOffsetMax) / 5;

        StartCoroutine(Shake());

        if (instance == null)
        {
            instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
            BroadcastMessage("OnHelperGlowDisable", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
