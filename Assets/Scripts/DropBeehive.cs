using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBeehive : Tree
{
    public GameObject beehive;
    public GameObject prefab;
    public int requiredBeesToTrigger = 3;
    public GameObject[] beehivePathPoints;

    private int beeCounter = 0;
    private Coroutine lastRoutine = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("BeeEnteredHoneyTree", OnBeeEntered);
        EventManager.StartListening("EndOfPath", OnFollowThePathEnded);
        EventManager.StartListening("ResetBeehive", ResetBeehive);
    }

    void OnDestroy()
    {
        EventManager.StopListening("BeeEnteredHoneyTree", OnBeeEntered);
        EventManager.StopListening("EndOfPath", OnFollowThePathEnded);
        EventManager.StopListening("ResetBeehive", ResetBeehive);
    }

    void OnBeeEntered(EventDict dict)
    {
        beeCounter += 1;

        if(beeCounter >= requiredBeesToTrigger)
        {
            // Stop shaking every second
            if (lastRoutine != null)
            {
                StopCoroutine(lastRoutine);
                lastRoutine = null;
            }

            beehive.SetActive(false);

            Vector3 position = transform.position;
            position.x += Random.Range(-4, 4) / 5;
            position.y += 2.5f;
            position.z += Random.Range(6, 8) / 5;

            GameObject instance = Instantiate(prefab, position, Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));

            instance.GetComponent<RollAlongPath>().SetWaypoints(beehivePathPoints);

            beeCounter = 0;

            Debug.Log("A beehive fell from the honey tree");
        } else if(beeCounter == requiredBeesToTrigger - 1)
        {
            lastRoutine = StartCoroutine(KeepShaking());
        }

        StartCoroutine(Shake());

    }

    void OnFollowThePathEnded(EventDict dict)
    {
        if(((GameObject)dict["activator"]).tag == "Bee")
        {
            if (gameObject.active)
                EventManager.TriggerEvent("BeeEnteredHoneyTree", gameObject);
        }
    }

    void ResetBeehive(EventDict dict)
    {
        beehive.SetActive(true);
    }

    IEnumerator KeepShaking()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            StartCoroutine(Shake());
        }
    }
}
