using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDam : MonoBehaviour
{
    void OnMouseDown()
    {
        EventManager.TriggerEvent("SwitchPondState", gameObject);
    }
}
