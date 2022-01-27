using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractionFairy : MonoBehaviour
{
    public GameObject myPlayer;
    private HashSet<GameObject> objectsInsideTrigger = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnHandsForwardStart", OnHandsForwardStartHandler);
        EventManager.StartListening("OnHandsForwardEnd", OnHandsForwardEndHandler);
    }
    private void OnDestroy()
    {
        EventManager.StopListening("OnHandsForwardStart", OnHandsForwardStartHandler);
        EventManager.StopListening("OnHandsForwardEnd", OnHandsForwardEndHandler);
    }

    void OnHandsForwardStartHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == myPlayer)
        {
            StopAllCoroutines();
            StartCoroutine(PreparingInteraction(2));
        }
    }
    void OnHandsForwardEndHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == myPlayer)
        {
            StopAllCoroutines();
            GetComponent<Collider>().enabled = false;
            objectsInsideTrigger.Clear();
        }
    }

    IEnumerator PreparingInteraction(float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
        //Activate the collider
        GetComponent<Collider>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!objectsInsideTrigger.Contains(other.gameObject))
        {
            objectsInsideTrigger.Add(other.gameObject);
            other.gameObject.SendMessage("OnMouseDown");
        }
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     if (!objectsInsideTrigger.Contains(other.gameObject))
    //     {
    //         other.gameObject.SendMessage("OnMouseUp");
    //         objectsInsideTrigger.Add(other.gameObject);
    //     }
    // }

}
