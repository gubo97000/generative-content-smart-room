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

        if (Input.GetKeyUp(KeyCode.X))
        {
            EventManager.TriggerEvent("DayNightSwitch", gameObject);
        }
    }

    void DaySwitch(EventDict dict = null)
    {
        SwitchToDay();
    }

    void NightSwitch(EventDict dict = null)
    {
        SwitchToNight();
    }

    //To work the first time it needs to be controlled by the DayTimeStateManager
    public void OnFirstTimeInit(string state)
    {
        Debug.Log(state);
        if (state == "Day")
        {
            SwitchToDay();
        }
        else if (state == "Night")
        {
            SwitchToNight();
        }
    }

    void SwitchToNight()
    {
        StartCoroutine(SkyTransition(0f, 0.8f, dayColor, nightColor, 2f));

        dayLight.GetComponent<Light>().enabled = false;
        nightLight.GetComponent<Light>().enabled = true;
    }

    void SwitchToDay()
    {
        StartCoroutine(SkyTransition(0.8f, 0f, nightColor, dayColor, 2f));

        dayLight.GetComponent<Light>().enabled = true;
        nightLight.GetComponent<Light>().enabled = false;
    }

    IEnumerator SkyTransition(float v_start, float v_end, Color color1, Color color2, float duration)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            RenderSettings.skybox.SetFloat("_CubemapTransition", Mathf.Lerp(v_start, v_end, elapsed / duration));
            RenderSettings.skybox.SetColor("_TintColor", Color.Lerp(color1, color2, elapsed / duration));

            elapsed += Time.deltaTime;

            DynamicGI.UpdateEnvironment();

            yield return null;
        }
        RenderSettings.skybox.SetFloat("_CubemapTransition", v_end);
        RenderSettings.skybox.SetColor("_TintColor", color2);

        DynamicGI.UpdateEnvironment();

    }
}
