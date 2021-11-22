using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnMouseDown(){
        Debug.Log("Wind starts to play");
        FindObjectOfType<AudioManager>().Play("Wind");
    }  
}
