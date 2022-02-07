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

    public bool isWalk;
    public bool isJump = true;

    public List<KeyValuePair> prefabsList;
    public bool cleanupInsects = true;

    private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    private GameObject currentPrefab;

    private List<GameObject> insects = new List<GameObject>();

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

        // Reset insects
        if (cleanupInsects)
        {
            foreach(GameObject i in insects)
            {
                StartCoroutine(GoAwayAndThenDestroy(i));
            }
            insects.Clear();
        }
    }

    void NightSwitch(EventDict dict = null)
    {
        currentPrefab = prefabs["Night"];

        // Reset insects
        if (cleanupInsects)
        {
            foreach (GameObject i in insects)
            {
                StartCoroutine(GoAwayAndThenDestroy(i));
            }
            insects.Clear();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jump")
        {
            AudioSource a = GetComponent<AudioSource>();
            if (a != null) a.Play();
        }

        if (isWalk && other.gameObject.tag == "Player")
        {
            Debug.Log("Player has touched the grass");
            SpawnInsect();
        }
        else if (isJump && other.gameObject.tag == "Jump" && other.gameObject.GetComponent<JumpCollider>().isInAir())
        {
            EventManager.TriggerEvent("OnJumpEnd", gameObject);
            SpawnInsect();
        }
    }

    void SpawnInsect()
    {
        Vector3 position = gameObject.transform.position;
        position.y += 1;

        int isSpawned = isWalk ? Random.Range(1, 50) : 1;
        if (isSpawned <= 1)
            insects.Add(Instantiate(currentPrefab, position, Quaternion.identity));
    }

    IEnumerator GoAwayAndThenDestroy(GameObject g)
    {
        EventManager.TriggerEvent("FlyAway", gameObject, new EventDict() { { "receiver", g } });
        yield return new WaitForSeconds(10);
        Destroy(g);
    }
}
