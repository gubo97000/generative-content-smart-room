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
        var color = GameStateManager.playersColor[myPlayer.GetComponent<PlayerInfo>().playerNumber];
        GetComponent<Renderer>().material.color = color;
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
            StartCoroutine(PreparingInteraction(0.4f));
        }
    }
    void OnHandsForwardEndHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == myPlayer)
        {
            StopAllCoroutines();
            foreach (var obj in objectsInsideTrigger)
            {
                if (obj == null) continue;
                obj.BroadcastMessage("OnMouseUp", SendMessageOptions.DontRequireReceiver);
                obj.BroadcastMessage("OnFairyUp", SendMessageOptions.DontRequireReceiver);
            }

            GetComponent<Collider>().enabled = false;
            objectsInsideTrigger.Clear();
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!objectsInsideTrigger.Contains(other.gameObject))
        {
            objectsInsideTrigger.Add(other.gameObject);
            other.gameObject.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
            other.gameObject.SendMessage("OnFairyDown", gameObject, SendMessageOptions.DontRequireReceiver);
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
