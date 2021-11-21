using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawner : MonoBehaviour
{
    public GameObject prefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has touched the grass");
            SpawnInsect();
        }
    }

    void SpawnInsect()
    {
        Vector3 position = gameObject.transform.position;
        position.y += 1;

        int isSpawned = Random.Range(1, 50);
        if (isSpawned <= 1)
            Instantiate(prefab, position, Quaternion.identity);
    }
}
