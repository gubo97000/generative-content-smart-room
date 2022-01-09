using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDam : MonoBehaviour
{
    public int howManyBeavers = 3;
    public GameObject[] beavers;
    public GameObject[] newPaths;
    public float cooldown = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        beavers = new GameObject[howManyBeavers];
        EventManager.StartListening("NewBeaver", setBeaver);
        EventManager.StartListening("BeaverTamed", checkAllBeavers);
        EventManager.StartListening("ReadyToBuild", checkAllReadyToBuild);
    }

    void OnDestroy()
    {
        EventManager.StopListening("NewBeaver", setBeaver);
        EventManager.StopListening("BeaverTamed", checkAllBeavers);
        EventManager.StopListening("ReadyToBuild", checkAllReadyToBuild);
    }

    void checkAllBeavers(EventDict dict)
    {
        if (System.Array.TrueForAll(beavers, m => m != null && m.GetComponent<BeaverAnimationManager>().isBeaverEating()))
        {
            StartCoroutine(changePath());
        }
    }

    void checkAllReadyToBuild(EventDict dict)
    {
        if (System.Array.TrueForAll(beavers, m => m.GetComponent<BeaverAnimationManager>().isBeaverReadyToBuild()))
        {
            StartCoroutine(buildDam());
        }
    }

    void setBeaver(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        int order = (int)dict["order"];
        beavers[order] = sender;
    }

    IEnumerator changePath()
    {
        // Change paths to follow
        foreach (GameObject b in beavers)
        {
            yield return new WaitForSeconds(cooldown);
            b.GetComponent<FollowThePath>().ResetPath(newPaths);
        }
    }

    IEnumerator buildDam()
    {
        yield return new WaitForSeconds(3.5f);
        EventManager.TriggerEvent("SwitchPondState", gameObject);
    }

}
