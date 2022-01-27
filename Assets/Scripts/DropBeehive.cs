using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBeehive : MonoBehaviour
{
    public GameObject prefab;
    public int requiredBeesToTrigger = 3;
    public GameObject[] beehivePathPoints;

    private int beeCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("BeeEnteredHoneyTree", OnBeeEntered);
        EventManager.StartListening("EndOfPath", OnFollowThePathEnded);
    }

    void OnDestroy()
    {
        EventManager.StopListening("BeeEnteredHoneyTree", OnBeeEntered);
        EventManager.StopListening("EndOfPath", OnFollowThePathEnded);
    }

    void OnBeeEntered(EventDict dict)
    {
        beeCounter += 1;

        if(beeCounter >= requiredBeesToTrigger)
        {
            Vector3 position = transform.position;
            position.x += Random.Range(-4, 4) / 5;
            position.y += 2.5f;
            position.z += Random.Range(6, 8) / 5;

            GameObject instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));

            instance.GetComponent<RollAlongPath>().SetWaypoints(beehivePathPoints);

            beeCounter = 0;

            Debug.Log("A beehive fell from the honey tree");
        }
    }

    void OnFollowThePathEnded(EventDict dict)
    {
        if(((GameObject)dict["activator"]).tag == "Bee")
        {
            EventManager.TriggerEvent("BeeEnteredHoneyTree", gameObject);
        }
    }
}
