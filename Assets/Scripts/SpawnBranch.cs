using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBranch : MonoBehaviour
{
    public GameObject prefab;
    public GameObject instance = null;

    void OnMouseDown()
    {
        Vector3 position = transform.position;
        position.x += Random.Range(-10, 10) / 5;
        position.y += 3;
        position.z += Random.Range(5, 10) / 5;

        if (instance == null)
            instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
        Debug.Log("A branch fell from the oak tree");
    }
}
