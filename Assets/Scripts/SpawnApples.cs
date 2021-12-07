using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnApples : MonoBehaviour
{
    public GameObject prefab;
    public int amount;
    private List<GameObject> instances = new List<GameObject>();

    void OnMouseDown()
    {
        if (instances.Count == 0)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector3 position = transform.position;
                position.x += Random.Range(-10, 10) / 5;
                position.y += 3;
                position.z += Random.Range(5, 10) / 5;

                GameObject instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
                instances.Add(instance);
            }
            EventManager.TriggerEvent("SpawnBeavers", gameObject);
            Debug.Log("Apples fell from the apple tree");
        }
    }
}
