using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchParticles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("OnCrouchStart", OnCrouchStartHandler);
        EventManager.StartListening("OnCrouchEnd", OnCrouchEndHandler);
    }
    void OnDestory()
    {
        EventManager.StopListening("OnCrouchStart", OnCrouchStartHandler);
        EventManager.StopListening("OnCrouchEnd", OnCrouchEndHandler);
    }
    void OnCrouchStartHandler(EventDict dict)
    {
        CancelInvoke("StopParticles");
        StopParticles();
        var sender = dict["sender"] as GameObject;
        if (sender == this.transform.parent.gameObject)
        {
            GetComponent<ParticleSystem>().Emit(30);
            GetComponent<ParticleSystemForceField>().gravity = 0;
        }

    }
    void OnCrouchEndHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == this.transform.parent.gameObject)
        {
            GetComponent<ParticleSystemForceField>().gravity = 2;
            //This should be an animation...
            // GetComponent<ParticleSystem>().Clear();
            Invoke("StopParticles", 0.5f);
        }

    }
    void StopParticles()
    {
        GetComponent<ParticleSystem>().Clear();
    }

}
