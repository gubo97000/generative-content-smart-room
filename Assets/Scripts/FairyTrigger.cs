using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: it does not implement a 2P interaction yet (in case we want it)
public class FairyTrigger : MonoBehaviour
{
    void Start()
    {
        EventManager.StartListening("SwitchNight", OnSwitchNight);
    }

    void OnDestroy()
    {
        EventManager.StopListening("SwitchNight", OnSwitchNight);
    }

    void OnMouseDown()
    {
        EventManager.TriggerEvent("SwitchTreesForward");
    }

    // When the night starts all over again, the fairy disappears, so you can replay the mushroom part
    void OnSwitchNight(EventDict dict)
    {
        Destroy(gameObject);
    }
}
