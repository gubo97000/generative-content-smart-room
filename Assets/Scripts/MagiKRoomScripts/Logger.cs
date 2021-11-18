using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Logger : MonoBehaviour
{
    [HideInInspector]
    public int SessionID = -1;

    public string LogDir = "";

    private string DBAddress = null;
    private string RoomId = null;
    private string filePath;
    private readonly List<string> log = new List<string>();
    private List<string> pendingList = new List<string>();
    private int autoincrement = 0;

    private int activityIndex = 0;

    public int logcount { get { return log.Count; } }

    public void SetActivityIndex(int index)
    {
        activityIndex = index;
    }

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/" + Application.productName + "_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".log";
        Application.wantsToQuit += WaitForQuit;
    }

    private void Start()
    {
        LogDir = MagicRoomManager.instance.systemConfiguration.logPath;
        DBAddress = MagicRoomManager.instance.systemConfiguration.backendUrl;
        RoomId = MagicRoomManager.instance.systemConfiguration.roomId;
    }

    /// <summary>
    /// Use only for modules
    /// </summary>
    /// <param name="source">Modules who add a log line</param>
    /// <param name="payload">Contnet of the log</param>
    public void AddToLogNewLine(string source, string payload)
    {
        dynamic s = new JObject();
        s.source = source;
        s.payload = payload;
        s.creation = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        s.autoincrement = autoincrement;
        File.AppendAllText(filePath, s.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);
        if (LogDir != null)
        {
            if (pendingList.Count > 0) {
                foreach (string ps in pendingList) {
                    File.AppendAllText(LogDir, ps);
                }
                pendingList.Clear();
            }
            File.AppendAllText(LogDir, s.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);
        }
        else {
            pendingList.Add(s.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);
        }
        log.Add(s.ToString());
        autoincrement++;
    }

    /// <summary>
    /// Log al message on the activity behaviour or a relevant moment for the application
    /// </summary>
    /// <param name="payload"></param>
    public void AddToLogNewLine(string payload)
    {
        dynamic s = new JObject();
        s.source = Application.productName;
        s.payload = payload;
        s.creation = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        s.autoincrement = autoincrement;
        File.AppendAllText(filePath, s.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);
        File.AppendAllText(LogDir, s.ToString(Newtonsoft.Json.Formatting.None) + Environment.NewLine);
        log.Add(s.ToString());
        autoincrement++;
    }

    private bool WaitForQuit()
    {
        //if(string.IsNullOrEmpty(DBAddress) || string.IsNullOrEmpty(SessionID) || string.IsNullOrEmpty(RoomId))
        if (string.IsNullOrEmpty(DBAddress) || SessionID == -1 || string.IsNullOrEmpty(RoomId))
        {
            //Debug.LogError(DBAddress + "_" + SessionID + "_" + RoomId);
            return true;
        }
        JObject message = new JObject
        {
            { "sessionId", SessionID },
            { "activityTypeId", MagicRoomManager.instance.activityidentifier[activityIndex] },
            { "payload", new JArray(log) }
        };
        Debug.LogError(message.ToString());
        StartCoroutine(SendCommand(message, (body) =>
        {
            Debug.LogError(body.ToString());
            Application.Quit();
        }));
        Debug.Log("Player prevent quitting in Logger");
        Application.wantsToQuit -= WaitForQuit;
        return false;
    }

    private IEnumerator SendCommand(JObject command, MagicRoomManager.WebCallback callback = null)
    {
        string json = command.ToString(Newtonsoft.Json.Formatting.None);
        byte[] body = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest request = new UnityWebRequest(DBAddress, "POST")
        {
            uploadHandler = new UploadHandlerRaw(body),
            downloadHandler = new DownloadHandlerBuffer()
        };
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", RoomId);
        yield return request.SendWebRequest();
        if (!request.isNetworkError)
        {
            callback?.Invoke(request.downloadHandler.text);
        }
    }
}