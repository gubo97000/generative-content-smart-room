using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: it does not implement a 2P interaction yet (in case we want it)
public class FairyTrigger : MonoBehaviour
{
    void Start()
    {
        EventManager.StartListening("SwitchNight", DestroyFairy);
        EventManager.StartListening("ActivateMushroom", DestroyFairy);
    }

    void OnDestroy()
    {
        EventManager.StopListening("SwitchNight", DestroyFairy);
        EventManager.StopListening("ActivateMushroom", DestroyFairy);
    }

    void OnMouseDown()
    {
        EventManager.TriggerEvent("SwitchTreesForward");
    }

    // When the night starts all over again, the fairy disappears, so you can replay the mushroom part
    void DestroyFairy(EventDict dict)
    {
        Destroy(gameObject);
    }
}
