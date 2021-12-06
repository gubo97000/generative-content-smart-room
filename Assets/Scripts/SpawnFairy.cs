using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFairy : MonoBehaviour
{
    public GameObject[] mushrooms;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("MushroomHasFirefly", checkAllLilypads);
    }

    void OnDestroy()
    {
        EventManager.StopListening("MushroomHasFirefly", checkAllLilypads);
    }

    void checkAllLilypads(EventDict dict)
    {
        //if (System.Array.TrueForAll(mushrooms, m => !m.GetComponent<TransferLight>().isEmpty))
        {
            spawnFairy();
            EventManager.TriggerEvent("MushroomCleanUp");
        }

    }

    void spawnFairy()
    {
        Vector3 position = transform.position;
        //position.y = 1;

        Instantiate(prefab, position, Quaternion.identity);
    }
}
