using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomSmartToyManager : MonoBehaviour
{
    private readonly string address = "http://localhost:7081";
    private GameObject SmartObjectSpawn;
    private Dictionary<string, string> rfids;
    private Dictionary<string, GameObject> toys;
    private const string endpointUpdate = "UpdateSmartToy";
    private const string receiveTcpEvent = "ReceiveEvent";
    private const string receiveUdpStreaming = "ReceiveStreaming";

    public Dictionary<string, string> Rfids { get => rfids; }

    private event Action Consumer;

    private void Start()
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerToy", "Discoverying server smart toy");
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + endpointUpdate + @"$"), ManageUpdate);
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + receiveTcpEvent + @"$"), ManageEvent);
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + receiveUdpStreaming + @"$"), ManageStreaming);
        SmartObjectSpawn = new GameObject("SmartObjects");
        SmartObjectSpawn.transform.parent = transform;
        rfids = new Dictionary<string, string>();
        toys = new Dictionary<string, GameObject>();
        RegisterApp();
    }

    private void Update()
    {
        Consumer?.Invoke();
        Consumer = null;
    }

    private void RegisterApp()
    {
        string listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpointUpdate;
        JObject body = new JObject
        {
            { "action", "registerApp" },
            { "address", listeningAddress }
        };
        StartCoroutine(SendCommand(body, (response) =>
        {
            Consumer += () => ParseUpdate(response);
        }));
    }

    public Coroutine UnregisterApp()
    {
        string listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpointUpdate;
        JObject body = new JObject
        {
            { "action", "unregisterApp" },
            { "address", listeningAddress }
        };
        return StartCoroutine(SendCommand(body));
    }

    private void ManageUpdate(string message, NameValueCollection query)
    {
        Consumer += () => ParseUpdate(message);
    }

    private void ManageEvent(string message, NameValueCollection query)
    {
        Consumer += () =>
        {
            try
            {
                JObject request = JObject.Parse(message);
                string id = request.GetValue("id").Value<string>();
                if (toys.TryGetValue(id, out GameObject value))
                {
                    value.GetComponent<SmartToy>().UpdateEvent(request.GetValue("events").Value<JArray>());
                }
            }
            catch (Exception e)
            {
                Debug.Log("Parse error " + message);
                Debug.LogError(e);
            }
        };
    }

    private void ManageStreaming(string message, NameValueCollection query)
    {
        Consumer += () =>
        {
            try
            {
                JObject request = JObject.Parse(message);
                string id = request.GetValue("id").Value<string>();
                if (toys.TryGetValue(id, out GameObject value))
                {
                    value.GetComponent<SmartToy>().UpdateEvent(request.GetValue("kinematics").Value<JArray>());
                }
            }
            catch (Exception)
            {
                Debug.Log("Parse error " + message);
            }
        };
    }

    private void ParseUpdate(string message)
    {
        MessageUpdate update;
        try
        {
            update = JsonConvert.DeserializeObject<MessageUpdate>(message);
        }
        catch (Exception)
        {
            Debug.Log("Parse error");
            return;
        }
        if (update.rfids != null)
        {
            rfids = update.rfids;
        }
        if (update.toys != null)
        {
            List<string> toyConnected = update.toys.Select(x => x.Id).ToList();
            List<string> toyToDelete = toys.Keys.Where(x => !toyConnected.Contains(x)).ToList();
            foreach (string toy in toyToDelete)
            {
                Destroy(toys[toy]);
                toys.Remove(toy);
            }
            foreach (SmartToyDescription toy in update.toys)
            {
                if (toys.TryGetValue(toy.Id, out GameObject value))
                {
                    value.GetComponent<SmartToy>().state = toy;
                }
                else
                {
                    GameObject toyAdded = new GameObject(toy.Name);
                    toyAdded.transform.parent = SmartObjectSpawn.transform;
                    SmartToy description = toyAdded.AddComponent<SmartToy>();
                    description.state = toy;
                    toys.Add(toy.Id, toyAdded);
                }
            }
        }
    }

    public void SubscribeEvent(EventType type, string id)
    {
        string endpoint = type == EventType.TCP ? receiveTcpEvent : receiveUdpStreaming;
        string listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint;
        JObject body = new JObject
        {
            { "action", "subscribeEvent" },
            { "id", id },
            { "address", listeningAddress },
            { "type", type.ToString() }
        };
        StartCoroutine(SendCommand(body));
    }

    public void UnsubscribeEvent(EventType type, string id)
    {
        string endpoint = type == EventType.TCP ? receiveTcpEvent : receiveUdpStreaming;
        string listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint;
        JObject body = new JObject
        {
            { "action", "unsubscribeEvent" },
            { "id", id },
            { "address", listeningAddress },
            { "type", type.ToString() }
        };
        StartCoroutine(SendCommand(body));
    }

    public void UpdateStatusToy(string id)
    {
        JObject body = new JObject
        {
            { "action", "updateStatus" },
            { "id", id }
        };
        StartCoroutine(SendCommand(body));
    }

    public void SendCommandToToy(string id, JArray effects)
    {
        JObject body = new JObject
        {
            { "action", "sendCommand" },
            { "id", id },
            { "effects", effects }
        };
        StartCoroutine(SendCommand(body));
    }

    public string GetRfidAssosiation(string code)
    {
        if (rfids.TryGetValue(code, out string value))
        {
            return value;
        }
        return null;
    }

    public List<string> GetAllToy()
    {
        return toys.Select(x => x.Value.name).ToList();
    }

    public List<string> GetAllToyId()
    {
        return new List<string>(toys.Keys);
    }

    public GameObject GetSmartToyByName(string name)
    {
        return toys.Values.FirstOrDefault(x => x.name == name);
    }

    public string GetSmartToyIDByName(string name)
    {
        return toys.Values.FirstOrDefault(x => x.name == name).GetComponent<SmartToy>().state.Id;
    }

    private IEnumerator SendCommand(JObject command, MagicRoomManager.WebCallback callback = null)
    {
        string json = command.ToString();
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

    public JToken elaborateSmartToyCommand(string typology, Dictionary<string, JToken> parameters)
    {
        JToken item = new JObject();
        item["name"] = typology;
        JObject para = new JObject();
        foreach (var v in parameters)
        {
            para[v.Key] = v.Value;
        }
        item["parameters"] = para;
        return item;
    }
}

public enum EventType
{
    TCP, UDP
}

public class MessageUpdate
{
    public string status;
    public List<SmartToyDescription> toys;
    public Dictionary<string, string> rfids;
}