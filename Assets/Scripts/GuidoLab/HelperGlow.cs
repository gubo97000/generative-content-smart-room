using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HelperGlow : MonoBehaviour
{
    int _callCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Outline>().enabled = false;
        EventManager.StartListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }

    void OnDestroy()
    {
        GetComponent<Outline>().enabled = false;
        EventManager.StopListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }

    private void OnEnable() {
        EventManager.StartListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }
    private void OnDisable() {
        GetComponent<Outline>().enabled = false;
        EventManager.StopListening("OnHelperGlowStart", OnHandRaiseStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHandRaiseEnd);
    }

    void OnHandRaiseStart(EventDict dict)
    {
        GetComponent<Outline>().enabled = true;
        _callCounter+=1;
    }
    void OnHandRaiseEnd(EventDict dict)
    {
        _callCounter-=1;
        if (_callCounter == 0)
        {
            GetComponent<Outline>().enabled = false;
        }
    }

}
