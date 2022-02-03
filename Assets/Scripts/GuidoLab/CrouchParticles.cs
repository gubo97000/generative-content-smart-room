using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(ParticleSystemForceField))]
public class CrouchParticles : MonoBehaviour
{
    public int numberOfParticles = 20;
    ParticleSystem ps;
    private Color _color;
    private MagicRoomLightManager? _lM = null;
    private void Start()
    {
        _color = GameStateManager.playersColor[this.GetComponentInParent<PlayerInfo>().playerNumber];
        Debug.Log("Called");
        ps = GetComponent<ParticleSystem>();
        var maxTime = ps.main.startLifetime;
        var sCol = ps.main;
        sCol.startColor = _color;
        var grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] {
                new GradientColorKey(_color, 0.0f), new GradientColorKey(_color, 10.0f) },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 10.0f) });
        var col = ps.colorOverLifetime;
        col.color = grad;

        //MagicRoomLight
        if (MagicRoomManager.instance.MagicRoomLightManager != null)
        {
            _lM = MagicRoomManager.instance.MagicRoomLightManager;
            Debug.Log("MagicRoomLightManager Found");
        }
        _lM?.SendColor(_color.ToString(""), 100, $"Hue go {(GetComponentInParent<PlayerInfo>().playerNumber + 1)}");

        EventManager.StartListening("OnCrouchStart", OnCrouchStartHandler);
        EventManager.StartListening("OnCrouchEnd", OnCrouchEndHandler);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnCrouchStart", OnCrouchStartHandler);
        EventManager.StopListening("OnCrouchEnd", OnCrouchEndHandler);
    }
    void OnCrouchStartHandler(EventDict dict)
    {
        var sender = dict["sender"] as GameObject;
        if (sender == this.transform.parent.gameObject) //Crouch from this player
        {
            CancelInvoke("StopParticles");
            StopParticles();
            GetComponent<ParticleSystem>().Emit(numberOfParticles);
            GetComponent<ParticleSystemForceField>().gravity = 0;

            _lM?.SendColor(_color.ToString(""), 20, $"Hue go {(GetComponentInParent<PlayerInfo>().playerNumber + 1)}");
        }

    }
    void OnCrouchEndHandler(EventDict dict)
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
        this.GetComponent<ParticleSystem>().Clear();

        _lM?.SendColor(_color.ToString(""), 100, $"Hue go {(GetComponentInParent<PlayerInfo>().playerNumber + 1)}");
    }

}
