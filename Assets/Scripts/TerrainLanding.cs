using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLanding : MonoBehaviour
{
    
    //void Start(){
    //    FindObjectOfType<AudioManager>().Add(gameObject,"Main");
    //} 

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jump")
            EventManager.TriggerEvent("OnJumpEnd", gameObject);
    }
}
