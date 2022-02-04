using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonManager : MonoBehaviour
{
    public List<GameObject> dragonSpots;
    private int index = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("SpawnDragon", OnSpawnDragon);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        EventManager.StopListening("SpawnDragon", OnSpawnDragon);
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
}
