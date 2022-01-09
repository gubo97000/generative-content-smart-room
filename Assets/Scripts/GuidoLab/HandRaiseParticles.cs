using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Creates a particle countdown and call the helper glow, then creates a particle countdown to recover
[RequireComponent(typeof(ParticleSystem))]
public class HandRaiseParticles : MonoBehaviour
{

    public int numberOfParticles = 20;

    public float maxTime = 5f;
    public float recoverTime = 20f;

    public bool isRecovering = false;

    //Active particles
    public ParticleSystem ps;
    //Recovery particles
    public ParticleSystem recover_ps;
    public ParticleSystemForceField recoverPsForceField;
    private Color _color;

    //Array of single particles for the countdown effect
    private ParticleSystem.Particle[] _particles;
    private void Start()
    {
        _particles = new ParticleSystem.Particle[numberOfParticles];
        //Inizializing Color
        _color = GameStateManager.playersColor[this.GetComponentInParent<Player>().playerNumber];
        // var maxTime = ps.main.startLifetime.constant;
        var sCol = ps.main;
        sCol.startColor = _color;
        var grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] {
                new GradientColorKey(_color, 0.0f), new GradientColorKey(_color, maxTime) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, maxTime) });
        var col = ps.colorOverLifetime;
        col.color = grad;

        //Setting color, emission rate and speed (To make a full circle) of recovery particles
        var psMain = recover_ps.main;
        psMain.startColor = _color;
        var psEmission = recover_ps.emission;
        psEmission.rateOverTime = (float)numberOfParticles / recoverTime;
        var psShape = recover_ps.shape;
        psShape.arcSpeed = (-0.02f * recoverTime) + 0.3f;

        EventManager.StartListening("OnHandRaiseStart", OnHandRaiseStartHandler);
        // EventManager.StartListening("OnHandRaiseEnd", OnHandRaiseEndHandler);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnHandRaiseStart", OnHandRaiseStartHandler);
        // EventManager.StopListening("OnHandRaiseEnd", OnHandRaiseEndHandler);
    }

    // Start particles
    void OnHandRaiseStartHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == this.transform.parent.gameObject && !isRecovering)
        {
            isRecovering = true;
            EventManager.TriggerEvent("OnHelperGlowStart", sender);
            // CancelInvoke("StopParticles");
            // StopParticles();
            ps.Emit(numberOfParticles);
            GetComponent<ParticleSystemForceField>().gravity = 0;


            ps.GetParticles(_particles, numberOfParticles);
            for (int i = 0; i < numberOfParticles; i++)
            {
                _particles[i].startLifetime = (((float)i / (float)numberOfParticles) * maxTime);
            }

            ps.SetParticles(_particles);
            Invoke("StartRecoverParticles", maxTime);
        }

    }

    void StartRecoverParticles()
    {
        ps.Clear();
        ps.Stop();
        EventManager.TriggerEvent("OnHelperGlowEnd", this.transform.parent.gameObject); //I don't like this
        recover_ps.Play();
        Invoke("StopRecoverParticles", recoverTime);
    }
    void StopRecoverParticles()
    {
        recoverPsForceField.gravity = 1;
        Invoke("StopParticles", 0.4f);
    }

    void StopParticles()
    {
        recover_ps.Clear();
        recover_ps.Stop();
        isRecovering = false;
        recoverPsForceField.gravity = 0;
    }
}
