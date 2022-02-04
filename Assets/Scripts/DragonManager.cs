using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assign path to newly spawned dragons, and count how many required to Win
public class DragonManager : MonoBehaviour
{
    public List<GameObject> dragonSpots;
    private int index = 0;

    public int requiredToWin = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("SpawnDragon", OnSpawnDragon);
        EventManager.StartListening("EndOfPath", CheckWin);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        EventManager.StopListening("SpawnDragon", OnSpawnDragon);
        EventManager.StopListening("EndOfPath", CheckWin);
    }

    void OnSpawnDragon(EventDict dict)
    {
        GameObject d = (GameObject)dict["spawned"];

        Transform[] path = new Transform[2];
        path[1] = dragonSpots[index].transform;
        d.GetComponent<FollowThePath>().waypoints = path;

        if (index <= 2)
            index++;
        else
            d.GetComponent<FollowThePath>().rotate = true;
    }

    void CheckWin(EventDict dict)
    {
        if (index == requiredToWin)
        {
            EventManager.TriggerEvent("NextTutorial", gameObject);
            Debug.Log("WIN!!");
        }
    }
}
