using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
[ExecuteAlways]
//This is an ObjectStateHandler
[RequireComponent(typeof(Collector))]
public class Campfire : ObjectStateHandler
{
    private bool _afterDelay = true;
    public float delay = 5f;
    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Collecting", new MonoBehaviour[]{GetComponent<Collector>()}),
    new State("Triggerable",  new MonoBehaviour[]{GetComponent<TwoPlayerTrigger>()}),
    new State("Lit")
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
    }
    // protected override void Update()
    // {
    //     base.Update();
    // }

    //Messages from other scripts activate functions, 
    //to change the state change the value of CurrentState
    void OnCollectorFull() //Called from Collector
    {
        CurrentState = "Triggerable";
    }
    void OnTwoPlayerTrigger() //Called from TwoPlayerTrigger
    {
        if (CurrentState == "Triggerable")
        {
            CurrentState = "Lit";
            DelayedActivation(delay);
            _afterDelay = false;
            EventManager.TriggerEvent("SwitchNight");
        }
        else if (CurrentState == "Lit" && _afterDelay)
        {
            CurrentState = "Collecting";
            EventManager.TriggerEvent("SwitchDay");
            _afterDelay = false;
        }
    }
    async void DelayedActivation(float delay)
    {
        await Task.Delay(((int)(delay * 1000)));
        _afterDelay = true;
    }

    void OnFirstTimeInit(string state)
    {
        if (state == "Night")
        {
            initState("Lit");
        }

    }
}

