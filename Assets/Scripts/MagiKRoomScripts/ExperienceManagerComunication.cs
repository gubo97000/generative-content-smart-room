using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class ExperienceManagerComunication : MonoBehaviour
{
    public delegate void MessageHandler(string content);

    public event MessageHandler MessageHandlerEvent;

    private const string endpoint = "ExperienceManager";
    private const string address = "http://localhost:7100";

    private void Start()
    {
        MagicRoomManager.instance.HttpListenerForMagiKRoom.RequestHandlers.Add(new Regex(@"^/" + endpoint + @"$"), MenageMessage);
    }

    private void MenageMessage(string message, NameValueCollection query)
    {
        MessageHandlerEvent?.Invoke(message);
    }

    public void SendEvent(string eventName, JObject payload = null)
    {
        dynamic message = new JObject();
        message.@event = new JObject();
        message.@event.type = eventName;
        if (payload != null)
        {
            message.@event.payload = payload;
        }
        StartCoroutine(SendCommand(message));
    }

    public void SendMessage(JObject payload)
    {
        dynamic message = new JObject();
        message.message = payload;
        StartCoroutine(SendCommand(message));
    }

    private IEnumerator SendCommand(JObject command, MagicRoomManager.WebCallback callback = null)
    {
        string json = command.ToString(Newtonsoft.Json.Formatting.None);
        Debug.LogError("Message Sent " + json);
        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);
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

    public void SendResponse(string eventName, JObject payload = null)
    {
        dynamic response = new JObject();
        response.@response = new JObject();
        response.@response.type = eventName;
        if (payload != null)
        {
            response.response.payload = payload;
        }
        StartCoroutine(SendCommand(response));
    }
}