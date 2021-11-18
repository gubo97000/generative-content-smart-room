using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomAppliancesManager : MonoBehaviour
{
    public List<string> Appliances
    {
        get
        {
            List<string> l = new List<string>();
            foreach (SmartPlugState a in appliances)
            {
                l.Add(a.associatedname);
            }
            return l;
        }
    }

    public List<SmartPlugState> appliances;

    private readonly string address = "http://localhost:7071";

    private void Start()
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerSP", "Searched Magic Room Appliances");
        appliances = new List<SmartPlugState>();
        SendConfigurationRequest();
    }

    private void SendConfigurationRequest()
    {
        SmartPlugCommand cmd = new SmartPlugCommand
        {
            type = "SmartPlugDiscovery"
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            ServerSmartPlugConfiguration conf = JsonUtility.FromJson<ServerSmartPlugConfiguration>(body);
            appliances.Clear();
            foreach (string s in conf.configuration)
            {
                appliances.Add(new SmartPlugState(s, false));
            }
            MagicRoomManager.instance.Logger.AddToLogNewLine("ServerSP", Appliances.ToString());
        }));
    }

    public void SendChangeCommand(string appliance, string cmd, int duration = 0)
    {
        cmd = cmd.ToUpper();
        if (cmd != "ON" && cmd != "OFF")
            return;
        if (appliances.Where(x => x.associatedname == appliance).Count() == 0)
        {
            return;
        }
        appliances.FirstOrDefault(x => x.associatedname == appliance).isActive = cmd == "ON" ? true : false;
        SmartPlugCommand command = new SmartPlugCommand()
        {
            type = "SmartPlugCommand",
            command = cmd,
            id = appliance,
            delay = duration
        };
        if (duration > 0)
        {
            StartCoroutine(resetstate(appliance, duration));
        }
        MagicRoomManager.instance.Logger.AddToLogNewLine(appliance, cmd.ToUpper());
        StartCoroutine(SendCommand(command));
    }

    private IEnumerator resetstate(string associatedname, int time)
    {
        yield return new WaitForSeconds(time);
        appliances.FirstOrDefault(x => x.associatedname == associatedname).isActive = false;
    }

    public void SendChangeCommand(string appliance, bool cmd)
    {
        string command = cmd ? "ON" : "OFF";
        SendChangeCommand(appliance, command);
    }

    private IEnumerator SendCommand(SmartPlugCommand command, MagicRoomManager.WebCallback callback = null)
    {
        string json = JsonUtility.ToJson(command);
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
}

[Serializable]
public class SmartPlugCommand
{
    public string type;
    public string command;
    public string id;
    public int delay;
}

[Serializable]
public class SmartPlugState
{
    public string associatedname;
    public bool isActive;

    public SmartPlugState(string s, bool v)
    {
        associatedname = s;
        isActive = v;
    }
}

[Serializable]
public class ServerSmartPlugConfiguration
{
    public string[] configuration;
}