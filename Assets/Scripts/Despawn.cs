using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used after state switch empty -> full pond. The beavers will go away, and then despawn
public class Despawn : MonoBehaviour
{
    public GameObject newPath;
    public string thisTag;

    private bool hasBuiltDam = false;

    void Start()
    {
        EventManager.StartListening("WaterPond", OnSwitchPondState);
        EventManager.StartListening("EmptyPond", OnSwitchPondState);
        EventManager.StartListening("EndOfPath", OnEndOfPath);
    }

    void OnDestroy()
    {
        EventManager.StopListening("WaterPond", OnSwitchPondState);
        EventManager.StopListening("EmptyPond", OnSwitchPondState);
        EventManager.StopListening("EndOfPath", OnEndOfPath);
    }

    void OnSwitchPondState(EventDict dict)
    {
        // Second time will trigger the "else". The beavers will run away (new path set)
        if (!hasBuiltDam) hasBuiltDam = true;
    }

    // Despawn beaver when behind the rock (i.e. no longer visible)
    void OnEndOfPath(EventDict dict)
    {
        if (hasBuiltDam && ((GameObject)dict["sender"]).tag == thisTag)
        {
            Destroy(gameObject);
        }
    }
}
