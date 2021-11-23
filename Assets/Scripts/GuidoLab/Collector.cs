using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//This is one component for the ObjectStateMachine
//IMPORTANT: Scripts are dis/enabled from the ObjectStateHandler 
//but disable state only stops the Update function,
//So you need to check with the variable "enabled" the state of the script
[RequireComponent(typeof(BoxCollider))]
public class Collector : MonoBehaviour
{

    public int collected = 0;
    public int toCollect = 3;
    public string[] TagsToCollect;

    void OnTriggerEnter(Collider other)
    {
        if (enabled && TagsToCollect.Contains(other.gameObject.tag))
        {
            collected += 1;
            GameObject.Destroy(other.gameObject);
            if (collected >= toCollect)
            {
                this.SendMessage("OnCollectorFull", options: SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                this.SendMessage("OnCollectorChange", collected, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    
    private void OnDisable()
    {
        GetComponent<BoxCollider>().enabled = false;
    }
    private void OnEnable()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

}

