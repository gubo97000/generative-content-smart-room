using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collector))]
public class Campfire : MonoBehaviour
{
    public string _currentState = "";
    public State[] states = new State[]
    {
    new State("Collecting"),
    new State("Triggerable"),
    new State("Lit")
     };

    public string CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            if (_currentState != value)
            {
                Array.Find(states, el => el.name == _currentState).Deactivate();
                _currentState = value;
                Array.Find(states, el => el.name == value).Activate();
                Debug.Log(gameObject.name + " has state " + _currentState);
            }
        }
    }

    void initState()
    {
        foreach (var state in states)
        {
            state.Deactivate();
        }
        _currentState = states[0].name;
        states[0].Activate();
    }
    void Start()
    {
        initState();
    }

    void OnCollectorFull()
    {
        CurrentState = "Triggerable";
    }

}

