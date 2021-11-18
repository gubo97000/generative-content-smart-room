using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomLidarManager : MonoBehaviour
{
    
    private const int port = 12000;
    private string listeningAddress = "";

    private const string address = "http://localhost:7079";

    private GameObject LidarPoints;

    private LidarConfiguration configuration;
    public bool isConfigured;

    private Dictionary<LidarTouchPoints, GameObject> touchpoints = new Dictionary<LidarTouchPoints, GameObject>();

    public string[] connectedlidars;

    public Vector2 origin;
    public Vector2 size;

    // Start is called before the first frame update
    void Start()
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerLidar", "Searched Magic Room Lidar Server");
        MagicRoomManager.instance.UDPListenerForMagikRoom.RegisterUDPChannel(port, ManageUDPMessage);

        listeningAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address;
        
        LidarPoints = GameObject.Instantiate(new GameObject());
        LidarPoints.name = "LidarPoints";
        LidarPoints.transform.parent = transform;
        configuration = new LidarConfiguration();
        getConfiguration();
    }

    private void OnDestroy()
    {
        MagicRoomManager.instance.UDPListenerForMagikRoom.UnregisterUDPChannel(port);
    }
    private void OnApplicationQuit()
    {
        MagicRoomManager.instance.UDPListenerForMagikRoom.UnregisterUDPChannel(port);
    }

    List<LidarTouchPoints> tp = new List<LidarTouchPoints>();
    private bool trafficlight = false;

    private void ManageUDPMessage(string content)
    {
        tp =  GetLastPoint(content);
        trafficlight = true;
        /*foreach (LidarTouchPoints t in tp) {
            LidarTouchPoints found = touchpoints.Keys.Where(x => x.id == t.id).FirstOrDefault();
            if (found != null)
            {
                found.x = t.x;
                found.y = t.y;
                touchpoints[t].SetActive(true);
            }
            else {
                GameObject g = GameObject.Instantiate(new GameObject());
                g.name = "touchpoints_" + t.id;
                g.AddComponent<LidarTouch>();
                g.transform.parent = LidarPoints.transform;
                touchpoints.Add(t, g);
                g.GetComponent<LidarTouch>().point = t;
                touchpoints[t].SetActive(true);
            }
        }
        List<LidarTouchPoints> todeactivate = touchpoints.Keys.Where(x1 => !tp.Any(x2 => x2.id == x1.id)).ToList();
        foreach (LidarTouchPoints t in todeactivate) {
            touchpoints[t].SetActive(false);
        }*/
    }




    // Update is called once per frame
    void Update()
    {
        if (trafficlight)
        {
            foreach (LidarTouchPoints t in tp)
            {
                LidarTouchPoints found = touchpoints.Keys.Where(x => x.id == t.id).FirstOrDefault();
                if (found != null)
                {
                    found.x = t.x;
                    found.y = t.y;
                    Debug.Log(touchpoints[touchpoints.Keys.Where(x => x.id == t.id).FirstOrDefault()]);
                    touchpoints[touchpoints.Keys.Where(x => x.id == t.id).FirstOrDefault()].SetActive(true);
                }
                else
                {
                    GameObject g = GameObject.Instantiate(new GameObject());
                    g.name = "touchpoints_" + t.id;
                    g.AddComponent<LidarTouch>();
                    g.transform.parent = LidarPoints.transform;
                    touchpoints.Add(t, g);
                    g.GetComponent<LidarTouch>().point = t;
                    touchpoints[t].SetActive(true);
                }
            }
            List<LidarTouchPoints> todeactivate = touchpoints.Keys.Where(x1 => !tp.Any(x2 => x2.id == x1.id)).ToList();
            foreach (LidarTouchPoints t in todeactivate)
            {
                touchpoints[t].SetActive(false);
            }
            trafficlight = false;
        }
    }

    public List<LidarTouchPoints> GetLastPoint(string message)
    {
        LidarMultiTouch json;
        List<LidarTouchPoints> lastPoint = new List<LidarTouchPoints>();
        if (message != null)
        {
            //Debug.Log(_receivedMessage);
            json = JsonUtility.FromJson<LidarMultiTouch>("{\"points\":" + message + "}");
            if (json != null)
            {
                for (int i = 0; i < json.points.Count; i++)
                {
                    lastPoint.Add(new LidarTouchPoints(json.points[i].id, json.points[i].x, json.points[i].y));
                }
            }
        }
        
        return lastPoint;
    }

    //TODO sarà da sistemare anche nel middleware
    public void getConfiguration()
    {
        //string info = "{\"type\": \"configuration\",\"message\": {\"ip\": \"" + listeningAddress + "\",\"port\": \"15000\"}}";
        string info = "{\"type\": \"configuration\",\"message\": {\"ip\": \"" + listeningAddress + "\",\"port\": \"15000\"}}";
        StartCoroutine(SendCommand(info, null));

    }

    public void ChangeSettings(string sampleDistance, string minCentroidDistance)
    {
        string info = "{\"type\": \"settings\", \"message\": {\"sampleDistance\": \"" + sampleDistance + "\", \"minCentroidDistance\": \"" + minCentroidDistance + "\"}}";
        StartCoroutine(SendCommand(info, true));
    }

    public void StartStream(long lidarID)
    {
        string info = "{\"type\": \"getStream\", \"lidarID\": \"" + lidarID + "\"}";
        StartCoroutine(SendCommand(info, false));
    }

    public void StartStream()
    {
        string info = "{\"type\": \"getStream\", \"lidarID\": \"" + configuration.lidars.FirstOrDefault().ID + "\"}";
        StartCoroutine(SendCommand(info, false));
    }

    public void StopStream(long lidarID)
    {
        string info = "{\"type\": \"stopLidar\", \"lidarID\": \"" + lidarID + "\"}";
        StartCoroutine(SendCommand(info, false));
    }

    /// <summary>
    /// Send command to the specified uri
    /// </summary>
    /// <param name="uri">URI of the destination of the command</param>
    /// <param name="info">The command itself</param>
    /// <returns></returns>
    private IEnumerator SendCommand(string info, bool? settings)
    {
        UnityWebRequest _www = UnityWebRequest.Post(address, info);

        yield return _www.SendWebRequest();

        if (_www.isNetworkError || _www.isHttpError)
        {
            Debug.Log(_www.error);
        }
        else
        {
            string response = _www.downloadHandler.text;
            // Show results as text
            Debug.Log(response);

            if (settings == true)
            {
                JObject request = JObject.Parse(response);
                float.TryParse(request.GetValue("sampleDistance").ToString(), out configuration.sampleDistance);
                float.TryParse(request.GetValue("minCentroidDistance").ToString(), out configuration.minCentroidDistance);
                float.TryParse(request.GetValue("refreshRate").ToString(), out configuration.refreshRate);
            } else if (settings == null)
            {
                configuration.lidars = new List<LidarData>();
                JObject request = JObject.Parse(response);
                connectedlidars = new string[((JArray)(request["lidars"])["lidars"]).Count];
                int i = 0;
                foreach (JObject o in (JArray)(request["lidars"])["lidars"]) {
                    configuration.lidars.Add(o.ToObject<LidarData>());
                    connectedlidars[i] = o.GetValue("ID").ToString();
                    i++;
                }
                
                isConfigured = true;
            }

            // Or retrieve results as binary data
            byte[] results = _www.downloadHandler.data;
        }
    }


    public bool IsConfigured()
    {
        return isConfigured;
    }

    public List<LidarData> GetLidars()
    {
        return configuration.lidars;
    }

    public float GetSampleDistance()
    {
        return configuration.sampleDistance;
    }

    public float GetMinCentroidDistance()
    {
        return configuration.minCentroidDistance;
    }

    public float GetRefreshRate()
    {
        return configuration.refreshRate / 1000f;
    }
}


public class LidarConfiguration{
    public float sampleDistance;
    public float minCentroidDistance;
    public float refreshRate;

    public List<LidarData> lidars;
}

public class LidarData
{
    public long ID;
    public string Name;
    public string Model;
    public string HealthStatus;
    public int CurrentStatus;
    public string IpAddress;
    public int Port;
    public bool IsConfigured;
    public int OriginX;
    public int OriginY;
    public int LimitX;
    public int LimitY;
    public int WallLimitWidth;
    public int WallLimitHeight;
}

[System.Serializable]
public class LidarTouchPoints
{
    public int id;
    public float x;
    public float y;

    public LidarTouchPoints(int id, float x, float y)
    {
        this.id = id;
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public class LidarMultiTouch
{
    public List<LidarTouchPoints> points;
}