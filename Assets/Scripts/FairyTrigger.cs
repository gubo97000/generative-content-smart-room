using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO this class is temporary, as it does not implement the 2P interaction we want
public class FairyTrigger : MonoBehaviour
{
    private SwitchTree spawner;

    void Start()
    {
        spawner = GameObject.FindObjectsOfType<SwitchTree>()[0];
    }

    void OnMouseDown()
    {
        spawner.trigger();
    }
}
