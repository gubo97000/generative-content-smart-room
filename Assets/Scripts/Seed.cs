using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private void OnDestroy()
    {
        EventManager.TriggerEvent("ObjectDestroyed", gameObject);
    }
}
