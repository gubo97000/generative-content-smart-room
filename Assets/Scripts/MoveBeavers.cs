using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBeavers : MonoBehaviour
{
    public int howManyBeavers = 3;
    public GameObject[] beavers;
    public GameObject[] damPaths;
    public GameObject[] goAwayPaths;
    public float cooldown = 0.5f;

    private bool hasBuiltDam = false;

    // Start is called before the first frame update
    void Start()
    {
        beavers = new GameObject[howManyBeavers];
        EventManager.StartListening("NewBeaver", setBeaver);
        EventManager.StartListening("BeaverTamed", checkAllBeavers);
        EventManager.StartListening("ReadyToBuild", checkAllReadyToBuild);
        EventManager.StartListening("WaterPond", OnSwitchPondState);
        EventManager.StartListening("EmptyPond", OnSwitchPondState);
    }

    void OnDestroy()
    {
        EventManager.StopListening("NewBeaver", setBeaver);
        EventManager.StopListening("BeaverTamed", checkAllBeavers);
        EventManager.StopListening("ReadyToBuild", checkAllReadyToBuild);
        EventManager.StopListening("WaterPond", OnSwitchPondState);
        EventManager.StopListening("EmptyPond", OnSwitchPondState);
    }

    void checkAllBeavers(EventDict dict)
    {
        if (System.Array.TrueForAll(beavers, m => m != null && m.GetComponent<BeaverAnimationManager>().isBeaverEating()))
        {
            StartCoroutine(changePath(damPaths));
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

    IEnumerator changePath(GameObject[] newPaths)
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
        yield return new WaitForSeconds(1.5f); 
        EventManager.TriggerEvent("TriggerDust", gameObject); 
        yield return new WaitForSeconds(2f);
        EventManager.TriggerEvent("EmptyPond", gameObject);
    }


    void OnSwitchPondState(EventDict dict)
    {
        if (!hasBuiltDam) hasBuiltDam = true;
        else
        {
            StartCoroutine(changePath(goAwayPaths));
            hasBuiltDam = false;

            // Enable more beavers by resetting apple tree: now there can spawn others
            EventManager.TriggerEvent("ClearAppleTree", gameObject);
        }
    }
}
