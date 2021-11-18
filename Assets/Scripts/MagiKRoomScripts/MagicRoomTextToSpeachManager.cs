using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class MagicRoomTextToSpeachManager : MonoBehaviour
{
    public List<Voices> ListOfVoice
    {
        get;
        private set;
    }

    private bool isPlaying = false;

    public bool IsPlaying { get; private set; }
    private readonly string endpoint = "SpeachToText";
    private readonly string address = "http://localhost:7073";

    public event Action StartSpeak;

    public event Action EndSpeak;

    private void Awake()
    {
        ListOfVoice = new List<Voices>();
    }

    private void Start()
    {
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + endpoint + @"$"), IsCompleted);
        GetConfiguration();
    }

    private void IsCompleted(string message, NameValueCollection query)
    {
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerTTSO", "endplay");
        isPlaying = false;
        EndSpeak?.Invoke();
    }

    private void GetConfiguration()
    {
        SpeachToTextCommand command = new SpeachToTextCommand
        {
            action = "getVoicesList"
        };
        StartCoroutine(SendCommand(command, (body) =>
        {
            SpeachToTextOfflineConfiguration conf = JsonUtility.FromJson<SpeachToTextOfflineConfiguration>(body);
            ListOfVoice.Clear();
            ListOfVoice.AddRange(conf.voices);
            string log = "";
            foreach (Voices s in ListOfVoice)
            {
                log += "Available " + s.name + " as a voice, ";
            }
            MagicRoomManager.instance.Logger.AddToLogNewLine("ServerTTSO", log);
        }));
    }

    public bool GenerateAudioFromText(string text)
    {
        Voices voice = ListOfVoice.FirstOrDefault(x => x.alias == "Magika");
        if (voice != null)
        {
            return GenerateAudioFromText(text, voice);
        }
        return false;
    }

    public bool GenerateAudioFromText(string text, Voices voice)
    {
        if (isPlaying || voice == null)
        {
            return false;
        }
        SpeachToTextCommand command = new SpeachToTextCommand
        {
            action = "speechSynthesis",
            activityAddress = MagicRoomManager.instance.HttpListenerForMagiKRoom.Address + ":" + MagicRoomManager.instance.HttpListenerForMagiKRoom.Port + "/" + endpoint,
            text = text,
            voice = voice.name
        };
        MagicRoomManager.instance.Logger.AddToLogNewLine("ServerTTSO", text + "," + voice.name + " started");
        StartCoroutine(SendCommand(command, (body) =>
        {
            isPlaying = true;
            StartSpeak?.Invoke();
        }, () =>
        {
            StartSpeak?.Invoke();
            EndSpeak?.Invoke();
            isPlaying = false;
        }));
        return true;
    }

    private IEnumerator SendCommand(SpeachToTextCommand command, MagicRoomManager.WebCallback callback = null, UnityAction errorCallback = null)
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
        else
        {
            errorCallback?.Invoke();
        }
    }
}

[Serializable]
public class SpeachToTextCommand
{
    public string action;
    public string activityAddress;
    public string text;
    public string voice;
}

[Serializable]
public class Voices
{
    public string name;
    public string gender;
    public string language;
    public string alias;
}

[Serializable]
public class SpeachToTextOfflineConfiguration
{
    public Voices[] voices;
}