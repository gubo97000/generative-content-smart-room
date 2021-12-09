using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayTimeManager : ObjectStateHandler
{
    [Tooltip("If the script/object is not inside a state, you can know the starting state by subscribing here, on start OnFirstTimeInit function will be called in the script")]
    public Object[] scriptsToInit;

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
        EventManager.StartListening("SwitchDay", OnSwitchDay);
        EventManager.StartListening("SwitchNight", OnSwitchNight);
        base.Start();
        foreach (var script in scriptsToInit)
        {
            // type = script.GetType();
            if (script is GameObject)
            {
                (script as GameObject).BroadcastMessage("OnFirstTimeInit", CurrentState);
            }
            else if (script is MonoBehaviour)
            {
                (script as MonoBehaviour).BroadcastMessage("OnFirstTimeInit", CurrentState);
            }
            else
            {
                Debug.LogError("The script is not a GameObject or a MonoBehaviour");
            }

        }
    }
    private void OnDestroy()
    {
        EventManager.StopListening("SwitchDay", OnSwitchDay);
        EventManager.StopListening("SwitchNight", OnSwitchNight);
    }

    // protected override void Update()
    // {
    //     base.Update();
    // }
    void OnSwitchDay(EventDict dict = null)
    {
        CurrentState = "Day";
    }
    void OnSwitchNight(EventDict dict = null)
    {
        CurrentState = "Night";
    }
}
