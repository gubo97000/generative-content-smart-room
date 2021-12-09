using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayTimeManager : ObjectStateHandler
{
    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Day"),
    new State("Night"),
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
        Debug.Log("DayTimeManager started");
    }

    protected override void Update()
    {
        base.Update();
    }

}
