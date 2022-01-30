using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class TutorialManager : ObjectStateHandler
{
    [Tooltip("If the script/object is not inside a state, you can know the starting state by subscribing here, on start OnFirstTimeInit function will be called in the script")]
    public Object[] scriptsToInit;

    private static TutorialManager dayTimeManager;

    public static TutorialManager instance
    {
        get
        {
            if (!dayTimeManager)
            {
                dayTimeManager = FindObjectOfType(typeof(TutorialManager)) as TutorialManager;

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

    private int _index = 0;
    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Jump"),
    new State("Crouch"),
    new State("Crouch Drag"),
    new State("Touch"),
    new State("Touch Drag"),
    new State("Raise Hand"),
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        EventManager.StartListening("NextTutorial", NextTutorial);
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
        EventManager.StopListening("NextTutorial", NextTutorial);
    }

    // protected override void Update()
    // {
    //     base.Update();
    // }
    void NextTutorial(EventDict dict = null)
    {
        _index++;

        if (_index % states.Length == 0)
        {
            SceneManager.LoadScene(sceneBuildIndex: 3);
        }
        else
        {
            _index %= states.Length;
            CurrentState = states[_index].name;
        }
    }
}
