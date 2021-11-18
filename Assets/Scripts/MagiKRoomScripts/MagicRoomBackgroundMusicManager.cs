using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MagicRoomBackgroundMusicManager : MonoBehaviour
{

    public string musicPlaying { get; private set; }

    public List<string> musicTracks
    {
        get
        {
            List<string> names = new List<string>();
            foreach (string s in tracks)
            {
                names.Add(s);
            }
            return names;
        }
    }

    private List<string> tracks;

    private readonly string address = "http://localhost:7074";

    // Start is called before the first frame update
    void Awake()
    {
        tracks = new List<string>();
    }

    private void Start()
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("Servermusic", "Searching Background Music Server");
        SendConfigurationRequest();
    }

    public void requestBackGroundMusicChange(string trackname) {
        MusicCommand cmd = new MusicCommand
        {
            action = "changeMusic",
            musicName = trackname
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            Debug.Log(body);
        }));
    }

    public void stopBackGroundMusic()
    {
        MusicCommand cmd = new MusicCommand
        {
            action = "changeMusic",
            musicName = "silence"
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            Debug.Log(body);
        }));
    }

    private void SendConfigurationRequest()
    {
        MusicCommand cmd = new MusicCommand
        {
            action = "getConfiguration"
        };
        StartCoroutine(SendCommand(cmd, (body) =>
        {
            Debug.Log(body);
            MusicBackGroundConf conf = JsonUtility.FromJson<MusicBackGroundConf>(body);
            tracks.Clear();
            tracks.AddRange(conf.configuration);
            musicPlaying = "";
        }));
    }

    private IEnumerator SendCommand(MusicCommand command, MagicRoomManager.WebCallback callback = null)
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
            musicPlaying = command.musicName;
        }
    }

}

[Serializable]
public class MusicBackGroundConf
{
    public string[] configuration;
}


[Serializable]
public class MusicCommand
{
    public string action;
    public string musicName;

    public override string ToString()
    {
        return string.Format("Command: {0} - {1}", action, musicName);
    }

    public string ToJson()
    {
        dynamic command = new JObject();
        command.action = action;
        command.musicName = musicName;
        return command.ToString();
    }
}
