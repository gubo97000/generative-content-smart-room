using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDam : MonoBehaviour
{
    void OnMouseDown()
    {
        StartCoroutine(DoDestroyDam());
    }

    IEnumerator DoDestroyDam()
    {
        EventManager.TriggerEvent("TriggerDust", gameObject);
        yield return new WaitForSeconds(1f);
        EventManager.TriggerEvent("SwitchPondState", gameObject);
    }
}
