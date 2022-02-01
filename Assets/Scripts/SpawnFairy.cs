using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFairy : MonoBehaviour
{
    public GameObject[] mushrooms;
    public GameObject prefab;
    public GameObject spawnParticles;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("MushroomHasFirefly", checkAllMushrooms);
    }

    void OnDestroy()
    {
        EventManager.StopListening("MushroomHasFirefly", checkAllMushrooms);
    }

    void checkAllMushrooms(EventDict dict)
    {
        if (System.Array.TrueForAll(mushrooms, m => !m.GetComponent<TransferLight>().isEmpty))
        {
            StartCoroutine(DoSpawn());
            EventManager.TriggerEvent("MushroomCleanUp");
        }

    }

    IEnumerator DoSpawn()
    {
        Vector3 position = transform.position;
        //position.y = 1;

        GameObject sp = Instantiate(spawnParticles, position + new Vector3(0,0.5f,0), Quaternion.identity); 
        sp.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2.5f);
        Instantiate(prefab, position, Quaternion.identity);

        sp.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(5f);
        Destroy(sp);
    }
}
