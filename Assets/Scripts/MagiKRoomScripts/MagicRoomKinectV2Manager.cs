using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomKinectV2Manager : MonoBehaviour
{
    public event Action<Dictionary<ulong, Skeleton>> Skeletons;

    public bool StatusKinectSensor
    {
        get;
        private set;
    }

    private const string endpoint = "KinectPosition";
    private const string address = "http://localhost:7080";
    private Dictionary<ulong, Skeleton> skeletons;

    private PlayerMovementSimultor[] playerSimulator = null;
    private float interval;
    private float timer = 0;

    private void Start()
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerKinect", "Searched Magic Room Kinect Server");
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + endpoint + @"$"), ManageHttpRequest);
        GetStatusKinect();
    }

    private void Update()
    {
        if (!StatusKinectSensor && playerSimulator != null)
        {
            if (timer == 0)
            {
                for (int i = 0; i < playerSimulator.Length; i++)
                {
                    skeletons[(ulong)i] = playerSimulator[i].skeleton;
                }
                Skeletons?.Invoke(skeletons);
                timer = interval;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }

    public void StartStreamingSkeletons(int interval)
    {
        if (StatusKinectSensor)
        {
            string listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint;
            KinectCommand command = new KinectCommand
            {
                action = "startStreaming",
                interval = interval,
                address = listeningAddress
            };
            MagicRoomManager.instance.Logger.AddToLogNewLine("ServerKinect", "Start Streaming from Kinect V2 interval frequency " + interval);
            StartCoroutine(SendCommand(command));
        }
        else
        {
            if (playerSimulator == null)
            {
                skeletons = new Dictionary<ulong, Skeleton>();
                playerSimulator = new PlayerMovementSimultor[6];
                for (int i = 0; i < 6; i++)
                {
                    GameObject g = GameObject.Instantiate(Resources.Load("Simulation/PlayerSimulator") as GameObject);
                    playerSimulator[i] = g.GetComponent<PlayerMovementSimultor>();
                    playerSimulator[i].Setup(i + 1);
                    skeletons.Add((ulong)i, playerSimulator[i].skeleton);
                }
            }
            this.interval = interval / 1000;
            Debug.Log(this.interval);
            timer = interval / 1000;
        }
    }

    public void StopStreamingSkeletons()
    {
        if (StatusKinectSensor)
        {
            string listeningaddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint;
            KinectCommand command = new KinectCommand
            {
                action = "stopStreaming",
                address = listeningaddress
            };
            MagicRoomManager.instance.Logger.AddToLogNewLine("ServerKinect", "Stop Kinect V2");
            StartCoroutine(SendCommand(command));
        }
    }

    public void ReadLastSamplingKinect()
    {
        if (StatusKinectSensor)
        {
            KinectCommand command = new KinectCommand
            {
                action = "readBody"
            };
            StartCoroutine(SendCommand(command, (body) =>
            {
                ParseSkeleton(body);
            }));
        }
        else
        {
            if (playerSimulator == null)
            {
                skeletons = new Dictionary<ulong, Skeleton>();
                playerSimulator = new PlayerMovementSimultor[6];
                for (int i = 0; i < 6; i++)
                {
                    GameObject g = GameObject.Instantiate(Resources.Load("Simulation/PlayerSimulator") as GameObject);
                    playerSimulator[i] = g.GetComponent<PlayerMovementSimultor>();
                    playerSimulator[i].Setup(i);
                    skeletons.Add((ulong)i, playerSimulator[i].skeleton);
                }
            }
        }
    }

    public void ResetGestureRecognized()
    {
        KinectCommand command = new KinectCommand
        {
            action = "resetGesture"
        };
        StartCoroutine(SendCommand(command));
    }

    public void SetGestureRecognitionKinect(Dictionary<string, float> gesture)
    {
        KinectCommand command = new KinectCommand
        {
            action = "addGesture",
            gesture = gesture
        };
        StartCoroutine(SendCommand(command));
    }

    public void GetStatusKinect()
    {
        KinectCommand command = new KinectCommand
        {
            action = "getStatus"
        };
        StartCoroutine(SendCommand(command, (body) =>
        {
            dynamic payload = JObject.Parse(body);
            try
            {
                StatusKinectSensor = (bool)payload.status;
            }
            catch (Exception)
            {
                StatusKinectSensor = false;
            }
        }));
    }

    private void ParseSkeleton(string message)
    {
        try
        {
            dynamic payload = JObject.Parse(message);
            JObject body = payload.body;
            if (body.HasValues)
            {
                skeletons = body.ToObject<Dictionary<ulong, Skeleton>>();
                Skeletons?.Invoke(skeletons);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void ManageHttpRequest(string message, NameValueCollection query)
    {
        ParseSkeleton(message);
    }

    private IEnumerator SendCommand(KinectCommand command, MagicRoomManager.WebCallback callback = null)
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
public class KinectCommand
{
    public string action;
    public string address;
    public int interval;
    public Dictionary<string, float> gesture;
}

[Serializable]
public class Skeleton
{
    public Vector3 SpineBase;
    public Vector3 SpineMid;
    public Vector3 Neck;
    public Vector3 Head;
    public Vector3 ShoulderLeft;
    public Vector3 ElbowLeft;
    public Vector3 WristLeft;
    public Vector3 HandLeft;
    public Vector3 ShoulderRight;
    public Vector3 ElbowRight;
    public Vector3 WristRight;
    public Vector3 HandRight;
    public Vector3 HipLeft;
    public Vector3 KneeLeft;
    public Vector3 AnkleLeft;
    public Vector3 FootLeft;
    public Vector3 HipRight;
    public Vector3 KneeRight;
    public Vector3 AnkleRight;
    public Vector3 FootRight;
    public Vector3 SpineShoulder;
    public Vector3 HandTipLeft;
    public Vector3 ThumbLeft;
    public Vector3 HandTipRight;
    public Vector3 ThumbRight;
    public string[] Gestures;

    public void SetPropertyValue(string name, Vector3 position)
    {
        Type t = this.GetType();
        FieldInfo p = t.GetField(name);
        if (p != null)
        {
            p.SetValue(this, position);
        }
    }

    public bool IsRightHandClosed()
    {
        return Array.Exists(Gestures, el => string.Equals(el, "CLOSEHANDRIGHT", StringComparison.OrdinalIgnoreCase));
    }

    public bool IsLeftHandClosed()
    {
        return Array.Exists(Gestures, el => string.Equals(el, "CLOSEHANDLEFT", StringComparison.OrdinalIgnoreCase));
    }
}