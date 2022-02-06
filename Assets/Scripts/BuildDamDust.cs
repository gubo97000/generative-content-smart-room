using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDamDust : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("TriggerDust", PlayDust);
        EventManager.StartListening("EmptyPond", StopDust);
        EventManager.StartListening("WaterPond", StopDust);
        GetComponent<ParticleSystem>().Stop();
    }

    void OnDestroy()
    {
        EventManager.StopListening("TriggerDust", PlayDust);
        EventManager.StopListening("EmptyPond", StopDust);
        EventManager.StopListening("WaterPond", StopDust);
    }

    void PlayDust(EventDict dict)
    {
        GetComponent<ParticleSystem>().Play();
        GetComponent<AudioSource>()?.Play();
    }

    void StopDust(EventDict dict)
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
