using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawn : MonoBehaviour
{
    [SerializeField] GameObject prefab = null;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SpawnObject();
    }

    private void SpawnObject()
    {
        GameObject instance = Instantiate(prefab, transform.position, transform.rotation);
    }
}
