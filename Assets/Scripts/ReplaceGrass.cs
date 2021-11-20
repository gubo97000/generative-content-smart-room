using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceGrass : MonoBehaviour
{
    public GameObject flowerPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReplaceAll()
    {
        ReplaceGrass[] grassPatch = FindObjectsOfType(typeof(ReplaceGrass)) as ReplaceGrass[];
        foreach (var grass in grassPatch)
        {
            Instantiate(flowerPrefab, grass.transform.position, grass.transform.rotation);
            Destroy(grass.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Seed")
        {
            Debug.Log("Seed has touched the grass");
            ReplaceAll();
            Destroy(other.gameObject);
        }
    }
}