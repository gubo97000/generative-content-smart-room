using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToDam : MonoBehaviour
{
    public int howManyBeavers = 3;
    public GameObject[] beavers;
    public GameObject[] newPaths;

    // Start is called before the first frame update
    void Start()
    {
        beavers = new GameObject[howManyBeavers];
        EventManager.StartListening("NewBeaver", setBeaver);
        EventManager.StartListening("BeaverTamed", checkAllBeavers);
    }

    void OnDestroy()
    {
        EventManager.StopListening("NewBeaver", setBeaver);
        EventManager.StopListening("BeaverTamed", checkAllBeavers);
    }

    void checkAllBeavers(EventDict dict)
    {
        if (System.Array.TrueForAll(beavers, m => m.GetComponent<BeaverAnimationManager>().isBeaverEating()))
        {
            // Change paths to follow
            foreach(GameObject b in beavers){
                b.GetComponent<FollowThePath>().ResetPath(newPaths);
            }
        }
    }

    void setBeaver(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        int order = (int)dict["order"];
        beavers[order] = sender;
    }

}
