using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SmartToy : MonoBehaviour
{
    public SmartToyDescription state;

    public event UnityAction<JArray> EventTcp;

    public event UnityAction EventUdp;

    public void UpdateEvent(JArray eventMessage)
    {
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
                            RfidDescription rfid = state.sensors.rfids.Find(x => x.Id == id);
                            if (rfid != null)
                            {
                                rfid.Code = eventObject.GetValue("value").Value<string>();
                            }
                            break;
                        }
                    case "button":
                        {
                            List<ButtonDescription> allButton = state.sensors.buttons.Concat(state.sensors.capacitives).ToList();
                            ButtonDescription button = allButton.Find(x => x.Id == id);
                            if (button != null)
                            {
                                button.Press = eventObject.GetValue("value").Value<int>() == 1;
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
        EventTcp?.Invoke(eventMessage);
    }

    public void UpdateStreaming(JArray streamingMessage)
    {
        foreach (var singleStreaming in streamingMessage)
        {
            try
            {
                JObject streamingObject = singleStreaming.Value<JObject>();
                string id = streamingObject.GetValue("id").Value<string>();
                CinematicState component = state.sensors.kinematics.Find(x => x.Id == id);
                if (component != null)
                {
                    JToken scan;
                    if (streamingObject.TryGetValue("acc", out scan))
                    {
                        dynamic acc = scan.Value<JObject>();
                        component.accelerometer.X = acc.x;
                        component.accelerometer.Y = acc.y;
                        component.accelerometer.Z = acc.z;
                    }
                    if (streamingObject.TryGetValue("gyr", out scan))
                    {
                        dynamic gyr = scan.Value<JObject>();
                        component.gyroscope.X = gyr.x;
                        component.gyroscope.Y = gyr.y;
                        component.gyroscope.Z = gyr.z;
                    }
                    if (streamingObject.TryGetValue("pos", out scan))
                    {
                        dynamic pos = scan.Value<JObject>();
                        component.position.X = pos.x;
                        component.position.Y = pos.y;
                        component.position.Z = pos.z;
                    }
                }
            }
            catch (Exception)
            {
                Debug.Log("Error parse event " + singleStreaming.ToString());
            }
        }
        EventUdp?.Invoke();
    }
}