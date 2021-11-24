using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint
{
    public GameObject Spawner;
    public int radius;
}

public class SpawnFish : MonoBehaviour
{
    public GameObject[] lilypads = GameObject.FindGameObjectsWithTag("Lilypad");
    public GameObject[] spawnPoints;
    public float[] radiuses;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("LilypadHasButterfly", checkAllLilypads);
    }

    void OnDestroy()
    {
        EventManager.StopListening("LilypadHasButterfly", checkAllLilypads);
    }

    void checkAllLilypads(EventDict dict)
    {
        if (System.Array.TrueForAll(lilypads, l => !l.GetComponent<DetachButterfly>().isEmpty))
            spawnFish();
    }

    void spawnFish()
    {
        int rand = Random.Range(1, spawnPoints.Length);
        GameObject point = spawnPoints[rand];
        Vector3 position = Random.insideUnitCircle * radiuses[rand];
        position.x += point.transform.position.x;
        position.y = 1;
        position.z += point.transform.position.z;

        Instantiate(prefab, position, Quaternion.identity); 
    }
}
