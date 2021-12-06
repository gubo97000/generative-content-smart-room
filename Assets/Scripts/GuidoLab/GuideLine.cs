using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GuideLine : MonoBehaviour
{
    public Transform target;

    public Transform toGuide; //Object to be guided (Line Start)

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("DeleteGuideLine", DeleteGuideLine);

    }
    // Start is called before the first frame update
    private void OnDestroy()
    {


        EventManager.StopListening("DeleteGuideLine", DeleteGuideLine);

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && toGuide != null)
        {
            LineRenderer line = GetComponent<LineRenderer>();
            line.SetPosition(0, target.position);
            line.SetPosition(1, toGuide.position);
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

    public void SetTarget(Transform t)
    {
        target = t;
    }
    public void SetToGuide(Transform t)
    {
        toGuide = t;
    }
}
