using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;

public class HttpListenerForMagiKRoom : MonoBehaviour
{
    public string Address
    {
        get;
        private set;
    }

    public int Port
    {
        get;
        private set;
    }

    public string lastreadmessage
    {
        get;
        private set;
    }

    public string lastreceivedaddress
    {
        get;
        private set;
    }

    public delegate void RequestHandler(string content, NameValueCollection query);

    public Dictionary<Regex, RequestHandler> RequestHandlers = new Dictionary<Regex, RequestHandler>();
    private HttpListener _listener;

    private void Awake()
    {
        Address = "http://localhost";
        Port = MagicRoomManager.instance.portHTTP;
    }

    private void Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/");
        _listener.Prefixes.Add("http://" + GetLocalIPAddress() + ":" + Port.ToString() + "/");
        _listener.Start();
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);

        MagicRoomManager.instance.ExperienceManagerComunication.SendEvent("ready");
    }

    private void OnDestroy()
    {
        if (_listener != null)
        {
            _listener.Close();
        }
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void ListenerCallback(IAsyncResult result)
    {
        HttpListener listener = (HttpListener)result.AsyncState;
        HttpListenerContext context = listener.EndGetContext(result);
        HttpListenerRequest request = context.Request;
        string contentread = new StreamReader(request.InputStream).ReadToEnd();
        lastreadmessage = contentread;
        lastreceivedaddress = request.Url.AbsolutePath;
        //Debug.LogError(contentread);
        NameValueCollection query = request.QueryString;
        HttpListenerResponse response = context.Response;
        foreach (KeyValuePair<Regex, RequestHandler> entry in RequestHandlers)
        {
            if (entry.Key.IsMatch(request.Url.AbsolutePath))
            {
                entry.Value(contentread, query);
                SetResponse(response, "ok", 200);
                response.Close();
                _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
                return;
            }
        }
        SetResponse(response, "not found", 404);
        response.Close();
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }

    private void SetResponse(HttpListenerResponse response, string body, int status)
    {
        response.StatusCode = status;
        string responseString = body;
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }
}