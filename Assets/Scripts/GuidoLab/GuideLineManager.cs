using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

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
                    Debug.LogError("There needs to be one active GuideLineManager script on a GameObject in your scene.");
                    return null;
                }
                else
                {
                    guideLineManager.Init();
                }
            }

            return guideLineManager;
        }
    }
    void Init()
    {
        instance.transform.position = Vector3.zero;
    }

    public void CreateLine(Transform start, Transform end, float delay = 0f, float? duration = null)
    {
        Debug.Log("Creating line");
        var line = Instantiate(instance.guideLinePrefab, instance.transform);
        line.SetActive(false);
        DelayedActivation(line, delay);
        GuideLine gl = line.GetComponent<GuideLine>();
        gl.target = end;
        gl.toGuide = start;
        gl.killOnPointLost = true;
    }
    async void DelayedActivation(GameObject go, float delay)
    {
        await Task.Delay(((int)(delay*1000)));
        if (go)
        {
            go.SetActive(true);
        }
    }
    public static void DeleteLine(GameObject line)
    {
        Destroy(line);
    }

    public static void DeleteLine(Transform start = null, Transform end = null)
    {
        instance.BroadcastMessage("DeleteGuideLine", (start: start, end: end));
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
