using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HelperGlow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Outline>().enabled = false;
        EventManager.StartListening("OnHandRaiseStart", OnHandRaiseStart);
        EventManager.StartListening("OnHandRaiseEnd", OnHandRaiseEnd);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        EventManager.StopListening("OnHandRaiseStart", OnHandRaiseStart);
        EventManager.StopListening("OnHandRaiseEnd", OnHandRaiseEnd);
    }

    private void OnEnable() {
        EventManager.StartListening("OnHandRaiseStart", OnHandRaiseStart);
        EventManager.StartListening("OnHandRaiseEnd", OnHandRaiseEnd);
    }
    private void OnDisable() {
        EventManager.StopListening("OnHandRaiseStart", OnHandRaiseStart);
        EventManager.StopListening("OnHandRaiseEnd", OnHandRaiseEnd);
    }

    void OnHandRaiseStart(EventDict dict)
    {
        GetComponent<Outline>().enabled = true;
    }
    void OnHandRaiseEnd(EventDict dict)
    {
        GetComponent<Outline>().enabled = false;
    }

}
