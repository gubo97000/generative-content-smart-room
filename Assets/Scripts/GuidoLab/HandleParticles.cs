using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HandleParticles : MonoBehaviour
{
    // public ParticleSystem ps;
    void Start()
    {

        EventManager.StartListening("HandleGrabbed", onHandleGrabbed);
        // EventManager.StartListening("HandleUngrabbed", onHandleUngrabbed);
    }
    void OnDestroy()
    {

        EventManager.StopListening("HandleGrabbed", onHandleGrabbed);
        // EventManager.StopListening("HandleUngrabbed", onHandleUngrabbed);
    }
    void onHandleGrabbed(EventDict dict)
    {
        if ((GameObject)dict["sender"] == gameObject)
        {
            var ps = GetComponent<ParticleSystem>();
            var sCol = ps.main;
            sCol.startColor = GameStateManager.P1Color;
            var grad = new Gradient();
            grad.SetKeys(new GradientColorKey[] {
                new GradientColorKey(GameStateManager.P1Color, 0.0f), new GradientColorKey(GameStateManager.P1Color, 10.0f) },
                new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 10.0f) });
            var col = ps.colorOverLifetime;
            col.color = grad;

            ps.Play();
        }
    }
}
