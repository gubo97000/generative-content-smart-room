using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedStateManager : ObjectStateHandler
{
    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Collectable", new MonoBehaviour[]{gameObject.GetComponentInChildren<HelperGlow>()}),
    new State("Grabbed"),
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
    }


    void OnGrabbed()
    {
        CurrentState = "Grabbed";
    }

}
