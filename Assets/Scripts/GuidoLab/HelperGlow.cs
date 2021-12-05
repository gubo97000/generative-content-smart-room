using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HelperGlow : MonoBehaviour
{
    int callCounter = 0;
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
        callCounter+=1;
    }
    void OnHandRaiseEnd(EventDict dict)
    {
        callCounter-=1;
        if (callCounter == 0)
        {
            GetComponent<Outline>().enabled = false;
        }
    }

}
