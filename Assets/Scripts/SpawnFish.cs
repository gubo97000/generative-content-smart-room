using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFish : MonoBehaviour
{
    public GameObject[] lilypads;
    public GameObject[] spawnPoints;
    public float[] radiuses;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        lilypads = GameObject.FindGameObjectsWithTag("Lilypad");
        EventManager.StartListening("LilypadHasButterfly", checkAllLilypads);
    }

    void OnDestroy()
    {
        EventManager.StopListening("LilypadHasButterfly", checkAllLilypads);
    }

    void checkAllLilypads(EventDict dict)
    {
        if (System.Array.TrueForAll(lilypads, l => !l.GetComponent<LilyPad>().isEmpty))
        {
            spawnFish();
            EventManager.TriggerEvent("LilypadCleanUp");

        }

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
