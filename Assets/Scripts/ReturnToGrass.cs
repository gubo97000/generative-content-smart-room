using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGrass : MonoBehaviour
{
    public GameObject grassPrefab; 
    
    private float startTime;
    public float timer = 10;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= timer)
            ReplaceBack();
    }

    void ReplaceBack()
    {
        ReturnToGrass[] flowerPatch = FindObjectsOfType(typeof(ReturnToGrass)) as ReturnToGrass[];
        foreach (var flower in flowerPatch)
        {
            Instantiate(grassPrefab, flower.transform.position, flower.transform.rotation);
            Destroy(flower.gameObject);
        }
    }
}
