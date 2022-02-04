using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveToDragon : MonoBehaviour
{
    public GameObject prefab;
    public GameObject spawnParticles;

    private GameObject sp = null;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Honey")
        {
            StartCoroutine(Evolve());
        }
    }

    void Update()
    {
        if(sp != null)
        {
            sp.transform.position = transform.position;
        }
    }

    IEnumerator Evolve()
    {
        Vector3 position = transform.position;

        sp = Instantiate(spawnParticles, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        sp.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(3f);

        GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity);

        // Decrease number of fish in the pond (since all carps will evolve at once, counter eventually becomes 0)
        EventManager.TriggerEvent("EatFish", gameObject);

        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) { r.enabled = false; }
        gameObject.GetComponent<FollowThePath>().enabled = false;

        sp.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(3f);
        Destroy(sp);

        //gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
