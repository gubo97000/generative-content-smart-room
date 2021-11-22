using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider))]
public class Collector : MonoBehaviour
{

    public int collected = 0;
    public int toCollect = 3;
    public string[] TagsToCollect;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        Debug.Log(other.gameObject.name);
        if (TagsToCollect.Contains(other.gameObject.tag))
        {
            collected += 1;
            GameObject.Destroy(other.gameObject);
            if (collected >= toCollect)
            {
                this.SendMessage("OnCollectorFull");
            }
        }
    }
    private void OnDisable()
    {
        GetComponent<Collider>().enabled = false;
    }
    private void OnEnable()
    {
        GetComponent<Collider>().enabled = true;
    }

}

