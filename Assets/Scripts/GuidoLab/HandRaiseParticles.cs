using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HandRaiseParticles : MonoBehaviour
{

    public int numberOfParticles = 20;
    ParticleSystem ps;
    private void Start()
    {
        Debug.Log("Called");
        ps = GetComponent<ParticleSystem>();
        var maxTime = ps.main.startLifetime;
        var sCol = ps.main;
        sCol.startColor = GameStateManager.P1Color;
        var grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] {
                new GradientColorKey(GameStateManager.P1Color, 0.0f), new GradientColorKey(GameStateManager.P1Color, 10.0f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 10.0f) });
        var col = ps.colorOverLifetime;
        col.color = grad;

        EventManager.StartListening("OnHandRaiseStart", OnHandRaiseStartHandler);
        EventManager.StartListening("OnHandRaiseEnd", OnHandRaiseEndHandler);
    }

    void OnDestory()
    {
        EventManager.StopListening("OnHandRaiseStart", OnHandRaiseStartHandler);
        EventManager.StopListening("OnHandRaiseEnd", OnHandRaiseEndHandler);
    }
    void OnHandRaiseStartHandler(EventDict dict)
    {
        CancelInvoke("StopParticles");
        StopParticles();
        var sender = dict["sender"] as GameObject;
        if (sender == this.transform.parent.gameObject)
        {
            GetComponent<ParticleSystem>().Emit(numberOfParticles);
            GetComponent<ParticleSystemForceField>().gravity = 0;
        }

    }
    void OnHandRaiseEndHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == this.transform.parent.gameObject)
        {
            GetComponent<ParticleSystemForceField>().gravity = 1;
            //This should be an animation...
            // GetComponent<ParticleSystem>().Clear();
            // Debug.Log($"{ps.time} {ps.main.duration}");
            Invoke("StopParticles", 0.4f);
        }

    }
    void StopParticles()
    {
        GetComponent<ParticleSystem>().Clear();
    }
}
