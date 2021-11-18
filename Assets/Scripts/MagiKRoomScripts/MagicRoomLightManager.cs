using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomLightManager : MonoBehaviour
{
    public List<string> Lights
    {
        get
        {
            List<string> names = new List<string>();
            foreach (SmartLightConf s in lights)
            {
                names.Add(s.id);
            }
            return names;
        }
    }

    private List<SmartLightConf> lights;
    public List<string> effects;

    private readonly string address = "http://localhost:7070";

    private void Awake()
    {
        lights = new List<SmartLightConf>();
        effects = new List<string>();
    }

    private void Start()
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerHue", "Searching Light Server");
        SendConfigurationRequest();
    }

    private void SendConfigurationRequest()
    {
        LightCommand cmd = new LightCommand
        {
            action = "getConfiguration"
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            Debug.Log(body);
            SmartLightConfigurations conf = JsonUtility.FromJson<SmartLightConfigurations>(body);
            lights.Clear();
            effects.Clear();
            lights.AddRange(conf.configuration);
            effects.AddRange(conf.effects);
        }));
    }

    public void SendColor(string color, int brightness = 100, string name = null, LocDepth depth = LocDepth.all, LocHorizontal horizontal = LocHorizontal.all, LocVertical vertical = LocVertical.all)
    {
        color = color.ToLower();
        if (brightness < 0 || brightness > 255)
            return;
        if (!CheckStringColour(color))
            return;
        LightCommand command = new LightCommand
        {
            action = "lightCommand",
            color = color,
            brightness = brightness,
            id = name,
            location = new Location()
            {
                vertical = vertical,
                horizontal = horizontal,
                depth = depth
            }
        };

        MagicRoomManager.instance.Logger.AddToLogNewLine("Hue_allRoom", command.ToString());
        StartCoroutine(SendCommand(command, (body) =>
        {
            Debug.Log(body);
        }));
    }

    public void SendColor(Color col, string name = null, LocDepth depth = LocDepth.all, LocHorizontal horizontal = LocHorizontal.all, LocVertical vertical = LocVertical.all)
    {
        string color = "#" + ColorUtility.ToHtmlStringRGB(col).ToLower();
        int brightness = Mathf.RoundToInt(col.a * 255);
        SendColor(color, brightness, name, depth, horizontal, vertical);
    }

    private bool CheckStringColour(string c)
    {
        Regex rgx = new Regex(@"^#[0-9a-f]{6}$");
        return rgx.IsMatch(c);
    }

    private IEnumerator SendCommand(LightCommand command, MagicRoomManager.WebCallback callback = null)
    {
        string json = command.ToJson();
        byte[] body = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(address, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (!request.isNetworkError)
        {
            callback?.Invoke(request.downloadHandler.text);
        }
    }

    private IEnumerator SendCommand(LightEffectCommand command, MagicRoomManager.WebCallback callback = null)
    {
        string json = command.ToJson();
        byte[] body = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(address, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (!request.isNetworkError)
        {
            callback?.Invoke(request.downloadHandler.text);
        }
    }

    public Color getColorofLight(string n)
    {
        return lights.FirstOrDefault(x => x.id == n).color;
    }

    public void setEffect(string effectname, bool isloop, bool activate)
    {
        if (!effects.Contains(effectname))
        {
            return;
        }
        LightEffectCommand comm = new LightEffectCommand()
        {
            action = "effectCommand",
            effectName = effectname,
            state = activate ? "start" : "stop",
            isLoop = isloop
        };
        MagicRoomManager.instance.Logger.AddToLogNewLine("Hue_allRoom", comm.ToString());
        StartCoroutine(SendCommand(comm, (body) =>
        {
            Debug.Log(body);
        }));
    }
}

[Serializable]
public class SmartLightConfigurations
{
    public SmartLightConf[] configuration;
    public string[] effects;
}

[Serializable]
public class SmartLightConf
{
    public string id;
    public Location location;
    public Color color;
}

[Serializable]
public class LightCommand
{
    public string action;
    public string color;
    public int brightness;
    public Location location = null;
    public string id = null;

    public override string ToString()
    {
        return string.Format("Command: {0} - {1} - {2} - {3} - {4}", action, color, brightness, location, id);
    }

    public string ToJson()
    {
        dynamic command = new JObject();
        command.action = action;
        command.color = color;
        command.brightness = brightness;
        if (!string.IsNullOrEmpty(id))
        {
            command.id = id;
        }
        if (location != null)
        {
            command.location = location.ToJson();
        }
        return command.ToString();
    }
}

[Serializable]
public class LightEffectCommand
{
    public string action;
    public string effectName;
    public string state;
    public bool isLoop;

    public override string ToString()
    {
        return string.Format("Command: {0} - {1} - {2} - {3}", action, effectName, state, isLoop);
    }

    public string ToJson()
    {
        dynamic command = new JObject();
        command.action = action;
        command.effectName = effectName;
        command.state = state;
        command.isLoop = isLoop;
        return command.ToString();
    }
}

[Serializable]
public class Location
{
    public LocVertical vertical;
    public LocHorizontal horizontal;
    public LocDepth depth;

    public JObject ToJson()
    {
        dynamic location = new JObject();
        location.vertical = vertical.ToString();
        location.horizontal = horizontal.ToString();
        location.depth = depth.ToString();
        return location;
    }
}

public enum LocVertical { top, middle, bottom, all }

public enum LocHorizontal { right, middle, left, all }

public enum LocDepth { front, middle, back, all }