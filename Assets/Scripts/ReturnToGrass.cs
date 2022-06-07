using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToGrass : MonoBehaviour
{
    private float startTime;
    public float timer = 10;
    public GameObject spawnParticles;
    private bool executedOnce;

    // Start is called before the first frame update
    void OnEnable()
    {
        startTime = Time.time;
        executedOnce = false;

    }
    void OnDisable()
    {
        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) { r.enabled = true; }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime >= timer)
            StartCoroutine(ReplaceBack());
    }

    IEnumerator ReplaceBack()
    {
        if (!executedOnce)
        {
            executedOnce = true;
            GameObject sp = Instantiate(spawnParticles, new Vector3(5.52f, -1.47f, 1.89f), Quaternion.identity);
            sp.GetComponent<ParticleSystem>().Play();
            sp.GetComponent<AudioSource>()?.Play();
            
            yield return new WaitForSeconds(3.5f);

            foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) { r.enabled = false; }

            sp.GetComponent<ParticleSystem>().Stop();
            yield return new WaitForSeconds(3f);
            Destroy(sp);

            gameObject.SetActive(false);
            GameObject g = GameObject.Find("--GRASS--");
            foreach (Transform child in g.transform)
            {
                child.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
            
        }

    }
}
