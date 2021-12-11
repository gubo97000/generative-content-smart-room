using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideLineManager : MonoBehaviour
{
    public GameObject guideLinePrefab;
    private static GuideLineManager guideLineManager;
    public static GuideLineManager instance
    {
        get
        {
            if (!guideLineManager)
            {
                guideLineManager = FindObjectOfType(typeof(GuideLineManager)) as GuideLineManager;

                if (!guideLineManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    // guideLineManager.Init();
                }
            }

            return guideLineManager;
        }
    }
    // void Init()
    // {
    //     if (eventDictionary == null)
    //     {
    //         eventDictionary = new Dictionary<string, Event>();
    //     }
    // }

    public static void CreateLine(Transform start, Transform end)
    {
        var line = Instantiate(instance.guideLinePrefab, instance.transform);
        GuideLine gl = line.GetComponent<GuideLine>();
        gl.target = end;
        gl.toGuide = start;
        gl.killOnPointLost=true;
    }

    public static void DeleteLine(GameObject line)
    {
        Destroy(line);
    }

    public static void DeleteLine(Transform start = null, Transform end = null)
    {
        instance.BroadcastMessage("DeleteGuideLine", (start:start, end:end));
        // if (start != null && end != null)
        // {
        //     // var async = (start: start, end: end);
        //     instance.BroadcastMessage("DeleteGuideLine", (start, end));
        // }
        // else if (start != null)
        // {
        //     instance.BroadcastMessage("DeleteGuideLine", (start, end));
        // }
        // else if (end != null)
        // {
        //     instance.BroadcastMessage("DeleteGuideLine", new EventDict { { "end", end } });
        // }

    }


}
