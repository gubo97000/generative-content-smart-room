using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// If there are too many carps, a bird will show up and take the extra fish away
public class CheckFishSurplus : MonoBehaviour
{
    private int instances = 0;
    public int limit = 3;
    public GameObject birdPrefab;    

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("SpawnFish", OnSpawnFish);
        EventManager.StartListening("EatFish", OnEatFish);
    }

    void OnDestroy()
    {
        EventManager.StopListening("SpawnFish", OnSpawnFish);
        EventManager.StopListening("EatFish", OnEatFish);
    }

    void OnSpawnFish(EventDict dict)
    {
        GameObject prey = (GameObject)dict["sender"];

        instances += 1;
        if (instances > limit)
        {
            GameObject bird = Instantiate(birdPrefab, transform.position, Quaternion.identity);
            bird.GetComponent<EatFish>().setPrey(prey);
        }
    }

    void OnEatFish(EventDict dict)
    {
        instances -= 1;
    }
}
