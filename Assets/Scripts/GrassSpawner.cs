using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GrassSpawner : MonoBehaviour
{
    [System.Serializable]
    public class KeyValuePair
    {
        public string state;
        public GameObject prefab;
    }

    public List<KeyValuePair> prefabsList;

    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    private GameObject currentPrefab;

    void Awake()
    {
        foreach (var kvp in prefabsList)
        {
            prefabs[kvp.state] = kvp.prefab;
        }

        if (DayTimeManager.instance.CurrentState == "Day")
        {
            DaySwitch();
        }
        else if (DayTimeManager.instance.CurrentState == "Night")
        {
            NightSwitch();
        }
    }
    
    void Start()
    {
        EventManager.StartListening("OnState-Night", NightSwitch);
        EventManager.StartListening("OnState-Day", DaySwitch);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnState-Night", NightSwitch);
        EventManager.StopListening("OnState-Day", DaySwitch);
    }

    public void OnFirstTimeInit(string state)
    {
       if (state == "Day")
       {
           DaySwitch();
       }
       else if (state == "Night")
       {
           NightSwitch();
       }
    }

    void DaySwitch(EventDict dict = null)
    {
        currentPrefab = prefabs["Day"];
    }

    void NightSwitch(EventDict dict = null)
    {
        currentPrefab = prefabs["Night"];
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has touched the grass");
            SpawnInsect();
        }
    }

    void SpawnInsect()
    {
        Vector3 position = gameObject.transform.position;
        position.y += 1;

        int isSpawned = Random.Range(1, 50);
        if (isSpawned <= 1)
            Instantiate(currentPrefab, position, Quaternion.identity);
    }
}
