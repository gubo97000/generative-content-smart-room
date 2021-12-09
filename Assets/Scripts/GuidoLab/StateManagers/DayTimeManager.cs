using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayTimeManager : ObjectStateHandler
{
    [Tooltip("Scripts that work with events may not listen to the first one, subscribe to this to avoid this issue")]
    public MonoBehaviour[] scriptsToInit;

    private static DayTimeManager dayTimeManager;

    public static DayTimeManager instance
    {
        get
        {
            if (!dayTimeManager)
            {
                dayTimeManager = FindObjectOfType(typeof(DayTimeManager)) as DayTimeManager;

                if (!dayTimeManager)
                {
                    // Debug.LogError("There needs to be one active DayTimeManager script on a GameObject in your scene.");
                }
                else
                {
                    // dayTimeManager.Init();
                }
            }

            return dayTimeManager;
        }
    }

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
        foreach (MonoBehaviour script in scriptsToInit)
        {
            script.BroadcastMessage("OnFirstTimeInit", CurrentState);
        }
    }

    // protected override void Update()
    // {
    //     base.Update();
    // }
    void OnSwitchDay()
    {
        CurrentState = "Day";
    }
    void OnSwitchNight()
    {
        CurrentState = "Night";
    }
}
