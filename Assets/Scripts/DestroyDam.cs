using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDam : MonoBehaviour
{
    void OnMouseDown()
    {
        StartCoroutine(DoDestroyDam());
    }

    IEnumerator DoDestroyDam()
    {
        EventManager.TriggerEvent("TriggerDust", gameObject);
        transform.parent.GetComponent<AudioSource>().Play(0); 
        
        yield return new WaitForSeconds(1f);
        EventManager.TriggerEvent("WaterPond", gameObject);
    }
}
