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
        EventManager.StartListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        EventManager.StopListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }

    private void OnEnable() {
        EventManager.StartListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }
    private void OnDisable() {
        EventManager.StopListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHandRaiseEnd);
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
