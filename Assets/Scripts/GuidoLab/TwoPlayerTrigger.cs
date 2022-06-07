using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TwoPlayerTrigger : MonoBehaviour
{
    private HashSet<GameObject> playerInsideTrigger = new HashSet<GameObject>();
    private bool _GuideLineCalled = false;
    public float GuideLineDelay = 1f;

    private void OnEnable()
    {
        GetComponent<SphereCollider>().enabled = true;
        playerInsideTrigger.Clear();
    }

    private void OnDisable()
    {
        GetComponent<SphereCollider>().enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (enabled && other.gameObject.tag == "Player")
        {
            playerInsideTrigger.Add(other.gameObject);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (!enabled || other.gameObject.tag != "Player") return;


        if (GameStateManager.playersCrouched.Count >= 2 && GameStateManager.playersCrouched.SetEquals(playerInsideTrigger))
        {
            gameObject.SendMessage("OnTwoPlayerTrigger");
            EventManager.TriggerEvent("DeleteGuideLine", gameObject);
            _GuideLineCalled = false;
        }
        //Case only one player is crouched
        else if (GameStateManager.playersCrouched.Count >= 1 && GameStateManager.playersCrouched.SetEquals(playerInsideTrigger) && !_GuideLineCalled)
        {
            _GuideLineCalled = true;
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (playerInsideTrigger.Contains(player)) continue;
                GuideLineManager.instance?.CreateLine(player.transform, transform, GuideLineDelay);
                break;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (!enabled || other.gameObject.tag != "Player") return;

        playerInsideTrigger.Remove(other.gameObject);
        EventManager.TriggerEvent("DeleteGuideLine", gameObject);
        _GuideLineCalled = false;

    }
}
