using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collector))]
public class Campfire : ObjectStateHandler
{
    private void Reset()
    {
        states = new State[]
        {
    new State("Collecting", new MonoBehaviour[]{GetComponent<Collector>()}),
    new State("Triggerable",  new MonoBehaviour[]{GetComponent<TwoPlayerTrigger>()}),
    new State("Lit")
        };
    }

    /// <summary>
    /// Calling the Start function of ObjectStateHandler
    /// </summary>
    protected override void Start()
    {
        base.Start();
    }

    //Called from Collector
    void OnCollectorFull()
    {
        CurrentState = "Triggerable";
    }
    void OnTwoPlayerTrigger()
    {
        CurrentState = "Lit";
    }
}

