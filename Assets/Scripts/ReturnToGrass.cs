using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGrass : MonoBehaviour
{
    private float startTime;
    public float timer = 10;

    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.time;
    }
    void OnDisable()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= timer)
            ReplaceBack();
    }

    void ReplaceBack()
    {
        gameObject.SetActive(false);
    }
}
