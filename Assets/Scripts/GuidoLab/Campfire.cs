using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//This is an ObjectStateHandler
[RequireComponent(typeof(Collector))]
public class Campfire : ObjectStateHandler
{
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

    //Messages from other scripts activate functions, 
    //to change the state change the value of CurrentState
    void OnCollectorFull() //Called from Collector
    {
        CurrentState = "Triggerable";
    }
    void OnTwoPlayerTrigger() //Called from TwoPlayerTrigger
    {
        CurrentState = "Lit";
        GameStateManager.night = true;
    }
}

