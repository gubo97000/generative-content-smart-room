using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TwoPlayerTrigger : MonoBehaviour
{
    private HashSet<GameObject> playerInsideTrigger = new HashSet<GameObject>();

    private void OnEnable()
    {
        GetComponent<SphereCollider>().enabled = true;
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
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (!enabled || other.gameObject.tag != "Player") return;

        playerInsideTrigger.Remove(other.gameObject);

    }
}
