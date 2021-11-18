using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SmartToyEventManager : MonoBehaviour
{
    public string smartObjectName;
    private bool _TCPopen = false, _UDPopen = false;
    public bool rfid, button, touch, accelerometer, gyroscope;

    public StringEvent readRFID, detectButton, detectTouch;
    public UnityEvent releaseRFID, releaseButton, releaseTouch;

    private string lastreadrfid, lastbutton, lasttouch;
    private string id;

    private int messagecountTCP = 0, messagecountUDP = 0;

    public GameObject toyobject;
    private List<CinematicVectors> cinematic;

    public string Id { get => id; private set => id = value; }
    public bool TCPopen { get => _TCPopen; private set => _TCPopen = value; }
    public bool UDPopen { get => _UDPopen; private set => _UDPopen = value; }
    public int MessagecountTCP { get => messagecountTCP; private set => messagecountTCP = value; }
    public int MessagecountUDP { get => messagecountUDP; private set => messagecountUDP = value; }

    // Start is called before the first frame update
    private void Start()
    {
        toyobject = MagicRoomManager.instance.MagicRoomSmartToyManager.GetSmartToyByName(smartObjectName);
        id = MagicRoomManager.instance.MagicRoomSmartToyManager.GetSmartToyIDByName(smartObjectName);

        if (rfid || button || touch)
        {
            MagicRoomManager.instance.MagicRoomSmartToyManager.SubscribeEvent(EventType.TCP, id);
            TCPopen = true;
        }
        if (accelerometer || gyroscope)
        {
            MagicRoomManager.instance.MagicRoomSmartToyManager.SubscribeEvent(EventType.UDP, id);
            UDPopen = false;
        }
        if (rfid)
        {
            readRFID = new StringEvent();
            releaseRFID = new UnityEvent();
        }
        if (touch)
        {
            detectTouch = new StringEvent();
            releaseTouch = new UnityEvent();
        }
        if (button)
        {
            detectButton = new StringEvent();
            releaseButton = new UnityEvent();
        }
        toyobject.GetComponent<SmartToy>().EventTcp += ManageTCP;
        toyobject.GetComponent<SmartToy>().EventUdp += ManageUDP;
        cinematic = new List<CinematicVectors>();
    }

    private void ManageUDP()
    {
        MessagecountUDP++;
        cinematic = new List<CinematicVectors>();
        foreach (CinematicState s in toyobject.GetComponent<SmartToy>().state.sensors.kinematics)
        {
            cinematic.Add(new CinematicVectors(s));
        }
    }

    private void ManageTCP(JArray eventMessage)
    {
        MessagecountTCP++;
        foreach (var singleEvent in eventMessage)
        {
            try
            {
                JObject eventObject = singleEvent.Value<JObject>();
                string type = eventObject.GetValue("type").Value<string>();
                string id = eventObject.GetValue("id").Value<string>();
                switch (type)
                {
                    case "rfid":
                        {
                            if (rfid)
                            {
                                string readtag = eventObject.GetValue("value").Value<string>();
                                if (lastreadrfid != readtag && (readtag != "" && readtag != null))
                                {
                                    readRFID?.Invoke(MagicRoomManager.instance.MagicRoomSmartToyManager.GetRfidAssosiation(readtag));
                                }
                                if ((readtag == "" || readtag == null))
                                {
                                    releaseRFID?.Invoke();
                                }
                            }
                            break;
                        }
                    case "button":
                        {
                            if (button)
                            {
                                if (toyobject.GetComponent<SmartToy>().state.sensors.buttons.ToList().Find(x => x.Id == id) != null)
                                {
                                    if (eventObject.GetValue("value").Value<int>() == 1)
                                    {
                                        detectButton?.Invoke(id);
                                    }
                                    else
                                    {
                                        releaseButton?.Invoke();
                                    }
                                }
                            }
                            if (touch)
                            {
                                if (toyobject.GetComponent<SmartToy>().state.sensors.capacitives.ToList().Find(x => x.Id == id) != null)
                                {
                                    if (eventObject.GetValue("value").Value<int>() == 1)
                                    {
                                        detectTouch?.Invoke(id);
                                    }
                                    else
                                    {
                                        releaseTouch?.Invoke();
                                    }
                                }
                            }
                            break;
                        }
                    default: break;
                }
            }
            catch (Exception)
            {
                Debug.Log("Error parse event " + singleEvent.ToString());
            }
        }
    }

    public void actuateLightChange(string areatolit, Color color, int brightness)
    {
        MagicRoomManager.instance.MagicRoomSmartToyManager.SendCommandToToy(id, new JArray() { MagicRoomManager.instance.MagicRoomSmartToyManager.elaborateSmartToyCommand("light", new Dictionary<string, JToken>() { { "id", areatolit }, { "color", "#" + ColorUtility.ToHtmlStringRGB(color) }, { "brightness", brightness } }) });
    }

    public void actuateSoundChange(string idsound, string track = null)
    {
        if (track != null)
        {
            MagicRoomManager.instance.MagicRoomSmartToyManager.SendCommandToToy(id, new JArray() { MagicRoomManager.instance.MagicRoomSmartToyManager.elaborateSmartToyCommand("speakers", new Dictionary<string, JToken>() { { "id", idsound }, { "sound", track } }) });
        }
        else
        {
            MagicRoomManager.instance.MagicRoomSmartToyManager.SendCommandToToy(id, new JArray() { MagicRoomManager.instance.MagicRoomSmartToyManager.elaborateSmartToyCommand("buzzer", new Dictionary<string, JToken>() { { "id", idsound }, { "sound", "" } }) });
        }
    }

    public void actuateEffect(string effectname, int numofiteration = -1, Dictionary<string, JToken> otherparameters = null)
    {
        dynamic command = new JObject();
        command.name = effectname;
        JObject parameters = new JObject();
        foreach (string s in otherparameters.Keys)
        {
            parameters.Add(s, otherparameters[s]);
        }
        parameters["period"] = numofiteration;
        command.parameters = parameters;
        MagicRoomManager.instance.MagicRoomSmartToyManager.SendCommandToToy(id, new JArray() { command });
    }

    private void OnDestroy()
    {
        if (rfid || button || touch)
        {
            MagicRoomManager.instance.MagicRoomSmartToyManager.UnsubscribeEvent(EventType.TCP, id);
        }
        if (accelerometer || gyroscope)
        {
            MagicRoomManager.instance.MagicRoomSmartToyManager.UnsubscribeEvent(EventType.UDP, id);
        }
    }
}

[Serializable]
public class CinematicVectors
{
    public string name;
    public Vector3 acceleration;
    public Vector3 gyroscope;
    public Vector3 position;

    public CinematicVectors(CinematicState s)
    {
        name = s.Id;
        acceleration = new Vector3(s.accelerometer.X, s.accelerometer.Y, s.accelerometer.Z);
        gyroscope = new Vector3(s.gyroscope.X, s.gyroscope.Y, s.gyroscope.Z);
        position = new Vector3(s.position.X, s.position.Y, s.position.Z);
    }
}

[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}