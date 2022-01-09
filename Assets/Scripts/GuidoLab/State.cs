using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State
{
    public string name;
    public Object[] scripts;
    public State(string name, Object[] scripts = null)
    {

        this.name = name;
        this.scripts = scripts;

    }

    public void Activate()
    {
        if (scripts == null) return;
        foreach (var script in scripts)
        {
            if (script == null) continue;
            if (script is MonoBehaviour)
            {
                (script as MonoBehaviour).enabled = true;
                // Debug.Log("Mono"+script.name);
            }
            else if (script.GetType() == typeof(GameObject))
            {
                (script as GameObject).SetActive(true);
                // Debug.Log("GO"+script.name);
            }
            else
            {
                Debug.LogError("Unknown type: " + script.GetType());
                break;
            }
        }
    }
    public void Deactivate()
    {
        if (scripts == null) return;
        foreach (var script in scripts)
        {
            if (script == null) continue;
            if (script is MonoBehaviour)
            {
                (script as MonoBehaviour).enabled = false;
                // Debug.Log("Mono"+script.name);
            }
            else if (script.GetType() == typeof(GameObject))
            {
                (script as GameObject).SetActive(false);
                // Debug.Log("GO"+script.name);
            }
            else
            {
                Debug.LogError("Unknown type: " + script.GetType());
                break;
            }
            
        }
    }

}
