using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TreesSwitcher : ObjectStateHandler
{
    private int _index = 0;
    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Oak"),
    new State("Apple"),
    new State("Honey")
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
        EventManager.StartListening("SwitchTreesForward", OnSwitchTreeForward);
    }
    void OnDestroy()
    {
        EventManager.StopListening("SwitchTreesForward", OnSwitchTreeForward);
    }

    //Messages from other scripts activate functions, 
    //to change the state change the value of CurrentState
    void OnSwitchTreeForward(EventDict dict)
    {
        _index++;
        _index %= states.Length;
        CurrentState = states[_index].name;
    }

}
