using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State
{
    public string name;
    public MonoBehaviour[] scripts;
    public State(string name, MonoBehaviour[] scripts = null)
    {

        this.name = name;
        this.scripts = scripts;

    }

    public void Activate()
    {
        foreach (var script in scripts)
        {
            script.enabled = true;
        }
    }
    public void Deactivate()
    {
        foreach (var script in scripts)
        {
            script.enabled = false;
        }
    }

}
