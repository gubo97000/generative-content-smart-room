using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SkyManager : MonoBehaviour
{
    public Material _materialOne;
    public Material _materialTwo;

    public GameObject dayLight;
    public GameObject nightLight;

    public Color dayColor;
    public Color nightColor;

    void Start()
    {
        EventManager.StartListening("OnState-Night", NightSwitch);
        EventManager.StartListening("OnState-Day", DaySwitch);

        RenderSettings.skybox.SetFloat("_CubemapTransition", 0f);
    }

    void OnDestroy()
    {
        EventManager.StopListening("OnState-Night", NightSwitch);
        EventManager.StopListening("OnState-Day", DaySwitch);

        // Resets scene to day
        RenderSettings.skybox.SetFloat("_CubemapTransition", 0f);
        RenderSettings.skybox.SetColor("_TintColor", dayColor);

        DynamicGI.UpdateEnvironment();
    }

    public void Update()
    {
        if (!Application.isPlaying)
        {
            EventManager.StopListening("OnState-Night", NightSwitch);
            EventManager.StopListening("OnState-Day", DaySwitch);
            EventManager.StartListening("OnState-Night", NightSwitch);
            EventManager.StartListening("OnState-Day", DaySwitch);
        }
    }

    void DaySwitch(EventDict dict = null)
    {
        SwitchToDay(Application.isPlaying ? 2f : 0f);
    }

    void NightSwitch(EventDict dict = null)
    {
        SwitchToNight(Application.isPlaying ? 2f : 0f);
    }

    //To work the first time it needs to be controlled by the DayTimeStateManager
    public void OnFirstTimeInit(string state)
    {
        // Debug.Log(state);
        if (state == "Day")
        {
            SwitchToDay(0f);
        }
        else if (state == "Night")
        {
            SwitchToNight(0f);
        }
    }

    void SwitchToNight(float duration = 2f)
    {
        StartCoroutine(SkyTransition(0f, 0.8f, dayColor, nightColor, 
            dayLight.GetComponent<Light>(), nightLight.GetComponent<Light>(), 1.5f, 1.2f, duration));
    }

    void SwitchToDay(float duration = 2f)
    {
        StartCoroutine(SkyTransition(0.8f, 0f, nightColor, dayColor,
            nightLight.GetComponent<Light>(), dayLight.GetComponent<Light>(), 1.2f, 1.5f, duration));
    }

    IEnumerator SkyTransition(float v_start, float v_end, Color color1, Color color2,
        Light oldLight, Light newLight, float oldIntensity, float newIntensity, float duration)
    {
        newLight.enabled = true;

        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            RenderSettings.skybox.SetFloat("_CubemapTransition", Mathf.Lerp(v_start, v_end, elapsed / duration));
            RenderSettings.skybox.SetColor("_TintColor", Color.Lerp(color1, color2, elapsed / duration));

            oldLight.intensity = Mathf.Lerp(oldIntensity, 0f, elapsed / duration);
            newLight.intensity = Mathf.Lerp(0f, newIntensity, elapsed / duration);

            elapsed += Time.deltaTime;

            DynamicGI.UpdateEnvironment();

            yield return null;
        }
        RenderSettings.skybox.SetFloat("_CubemapTransition", v_end);
        RenderSettings.skybox.SetColor("_TintColor", color2);

        oldLight.enabled = false;

        DynamicGI.UpdateEnvironment();

    }
}
