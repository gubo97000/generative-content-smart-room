using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class HelperGlow : MonoBehaviour
{
    HashSet<GameObject> _callerArray = new HashSet<GameObject>() { };
    public int _callerCount = 0;
    Outline _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }
    private void Reset()
    {
        _outline = GetComponent<Outline>();
    }
    // Start is called before the first frame update
    void Start()
    {
        OnHelperGlowEnable();
    }
    private void Update()
    {
        _callerCount = _callerArray.Count;
    }

    void OnDestroy()
    {
        OnHelperGlowDisable();
    }
    private void OnEnable()
    {
        OnHelperGlowEnable();
    }
    private void OnDisable()
    {
        OnHelperGlowDisable();
    }

    void OnHelperGlowEnable()
    {
        _outline.enabled = false;
        _callerArray.Clear();
        EventManager.StartListening("OnHelperGlowStart", OnHelperGlowStart);
        EventManager.StartListening("OnHelperGlowEnd", OnHelperGlowEnd);

    }
    void OnHelperGlowDisable()
    {
        _outline.enabled = false;
        _callerArray.Clear();
        EventManager.StopListening("OnHelperGlowStart", OnHelperGlowStart);
        EventManager.StopListening("OnHelperGlowEnd", OnHelperGlowEnd);

    }

    void OnHelperGlowStart(EventDict dict)
    {
        _outline.enabled = true;
        _callerArray.Add(dict["sender"] as GameObject);
    }
    void OnHelperGlowEnd(EventDict dict)
    {
        _callerArray.Remove(dict["sender"] as GameObject);
        if (_callerArray.Count == 0)
        {
            _outline.enabled = false;
        }
    }

}
