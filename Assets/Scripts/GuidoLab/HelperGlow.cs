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
        EventManager.StartListening("OnHelperGlowStart", OnHelperGlowStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHelperGlowEnd);
    }

    void OnDestroy()
    {
        GetComponent<Outline>().enabled = false;
        EventManager.StopListening("OnHelperGlowStart", OnHelperGlowStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHelperGlowEnd);
    }

    private void OnEnable() {
        EventManager.StartListening("OnHelperGlowStart", OnHelperGlowStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHelperGlowEnd);
    }
    private void OnDisable() {
        GetComponent<Outline>().enabled = false;
        EventManager.StopListening("OnHelperGlowStart", OnHelperGlowStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHelperGlowEnd);
    }

    void OnHelperGlowStart(EventDict dict)
    {
        GetComponent<Outline>().enabled = true;
        _callCounter+=1;
    }
    void OnHelperGlowEnd(EventDict dict)
    {
        _callCounter-=1;
        if (_callCounter == 0)
        {
            GetComponent<Outline>().enabled = false;
        }
    }

}
