using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    private void OnDestroy()
    {
        EventManager.TriggerEvent("ObjectDestroyed", transform.parent.gameObject);
        Destroy(transform.parent.gameObject);
    }
}
