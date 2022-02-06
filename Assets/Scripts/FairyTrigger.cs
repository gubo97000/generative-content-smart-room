using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note: it does not implement a 2P interaction yet (in case we want it)
public class FairyTrigger : MonoBehaviour
{
    public GameObject spawnParticles;
    
    void Start()
    {
        EventManager.StartListening("SwitchNight", DestroyFairy);
        EventManager.StartListening("MushroomCleanUp", DestroyFairy);
    }

    void OnDestroy()
    {
        EventManager.StopListening("SwitchNight", DestroyFairy);
        EventManager.StopListening("MushroomCleanUp", DestroyFairy);
    }

    void OnMouseDown()
    {
        EventManager.TriggerEvent("SwitchTreesForward");
    }

    // When the night starts all over again, the fairy disappears, so you can replay the mushroom part
    void DestroyFairy(EventDict dict)
    {
        StartCoroutine(DoDestroy());
    }

    IEnumerator DoDestroy()
    {
        Vector3 position = transform.position;
        
        GameObject sp = Instantiate(spawnParticles, position + new Vector3(0, 0.5f, 0), Quaternion.identity);
        sp.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);

        sp.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(5f);
        Destroy(sp);
    }
}
