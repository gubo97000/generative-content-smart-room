using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    private bool isDay = true;

    public Material _materialOne;
    public Material _materialTwo;

    public GameObject dayLight;
    public GameObject nightLight;

    public Color dayColor;
    public Color nightColor;

    void Start()
    {
        EventManager.StartListening("DayNightSwitch", DayNightSwitch);

        RenderSettings.skybox.SetFloat("_CubemapTransition", 0f);
    }

    void OnDestroy()
    {
        EventManager.StopListening("DayNightSwitch", DayNightSwitch);

        // Resets scene to day
        RenderSettings.skybox.SetFloat("_CubemapTransition", 0f);
        RenderSettings.skybox.SetColor("_TintColor", dayColor);

        DynamicGI.UpdateEnvironment();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            EventManager.TriggerEvent("DayNightSwitch", gameObject);
        }
    }

    void DayNightSwitch(EventDict dict)
    {
        if (isDay) SwitchToNight();
        else SwitchToDay();

        isDay = !isDay;
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
