using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceGrass : MonoBehaviour
{
    public GameObject flowers;
    public GameObject spawnParticles;

    IEnumerator GrowFlowers()
    {
        GameObject sp = Instantiate(spawnParticles, new Vector3(5.52f, -1f, 1.89f), Quaternion.identity);
        sp.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2f);

        flowers.SetActive(true);

        sp.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(3f);
        Destroy(sp);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Seed")
        {
            Debug.Log("Seed has touched the grass");
            StartCoroutine(GrowFlowers());
            Destroy(other.transform.parent.gameObject);
        }
    }
}