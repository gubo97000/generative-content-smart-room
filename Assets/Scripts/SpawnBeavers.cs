using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBeavers : MonoBehaviour
{
    public int amount = 3;
    public GameObject prefab;
    public int spawnCooldown = 1;
    public List<GameObject> paths;

    private List<Transform[]> waypoints = new List<Transform[]>();

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("SpawnBeavers", OnSpawnBeavers);

        // Map each path to the array of its waypoints
        foreach(GameObject path in paths)
        {
            Transform[] ts = path.GetComponentsInChildren<Transform>();
            waypoints.Add(ts);
        }
    }

    void OnDestroy()
    {
        EventManager.StopListening("SpawnBeavers", OnSpawnBeavers);
    }

    void OnSpawnBeavers(EventDict dict)
    {
        StartCoroutine(spawn());
    }

    IEnumerator spawn()
    {
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForSeconds(spawnCooldown);

            GameObject beaver = Instantiate(prefab, transform.position, Quaternion.identity);
            beaver.GetComponent<FollowThePath>().waypoints = waypoints[i];
            beaver.GetComponent<FollowThePath>().setProgressiveNumber(i);

            EventManager.TriggerEvent("NewBeaver", beaver, new EventDict() { { "order", i } });
        }
    }
}
