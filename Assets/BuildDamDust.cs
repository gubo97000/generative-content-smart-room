using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildDamDust : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("TriggerDust", PlayDust);
        EventManager.StartListening("SwitchPondState", StopDust);
        GetComponent<ParticleSystem>().Stop();
    }

    void PlayDust(EventDict dict)
    {
        GetComponent<ParticleSystem>().Play();
    }

    void StopDust(EventDict dict)
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
