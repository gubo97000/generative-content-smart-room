using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collector))]
public class Campfire : MonoBehaviour
{
    public string[] states = new string[] { "Collecting", "Triggerable", "Lit" };

    // public Dictionary<string, Mono> ScriptsInStates = new Dictionary<string, MonoBehaviour[]> { ["Collecting"] = Collector };
    public string _state = "Collecting";
    public string State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            switch (_state)
            {
                case "Collecting":
                    Debug.Log("Campfire is now collecting");
                    break;
                case "Triggerable":
                    Debug.Log("Campfire is now triggerable");
                    break;
                case "Lit":
                    Debug.Log("Campfire is now lit");
                    break;
            }
        }
    }
    // public MonoBehaviour[] componentsToDisable;


    void OnCollectorFull()
    {
        State = "Triggerable";
    }

}

