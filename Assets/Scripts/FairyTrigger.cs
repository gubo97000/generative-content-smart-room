using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO this class is temporary, as it does not implement the 2P interaction we want
public class FairyTrigger : MonoBehaviour
{
    void OnMouseDown()
    {
        EventManager.TriggerEvent("SwitchTreesForward");
    }
}
