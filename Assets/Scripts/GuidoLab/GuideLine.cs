using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GuideLine : MonoBehaviour
{
    public Transform target;

    public Transform toGuide; //Object to be guided (Line Start)
    public bool killOnPointLost = false; //If true, the line will be destroyed when the start or toGuide is lost


    void Start()
    {
        EventManager.StartListening("DeleteGuideLine", DeleteGuideLine);

    }

    private void OnDestroy()
    {
        EventManager.StopListening("DeleteGuideLine", DeleteGuideLine);
    }

    void Update()
    {
        if (target != null && toGuide != null)
        {
            LineRenderer line = GetComponent<LineRenderer>();
            line.SetPosition(0, target.position);
            line.SetPosition(1, toGuide.position);
        }
        else if (killOnPointLost)
        {
            Destroy(gameObject);
        }

    }
    void DeleteGuideLine(EventDict dict)
    {
        GameObject sender = dict["sender"] as GameObject;
        if (sender.transform == target)
        {
            Destroy(gameObject);
        }
    }
    void DeleteGuideLine((Transform start, Transform end) data) //Doesn't work
    {
        if (data.start == target && data.end == toGuide)
        {
            Destroy(gameObject);
            return;
        }
        else if (data.start == target && data.end == null)
        {
            Destroy(gameObject);
            return;
        }
        else if (data.start == null && data.end == toGuide)
        {
            Destroy(gameObject);
            return;
        }
    }


    public void SetTarget(Transform t)
    {
        target = t;
    }
    public void SetToGuide(Transform t)
    {
        toGuide = t;
    }
}
