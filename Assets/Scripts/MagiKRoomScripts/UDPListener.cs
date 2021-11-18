using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public class UDPListener : MonoBehaviour
{
    private static GameObject instance;
    private UdpClient _listener;
    private IPEndPoint _groupEP;
    Thread receiveThread;
    private string _receivedMessage;
    private bool abort = false;
    public bool started = false;
    static readonly object lockObject = new object();

    //public event Action<string> MessageReceived;

    public delegate void RequestHandler(string content);

    public Dictionary<Regex, RequestHandler> RequestHandlers = new Dictionary<Regex, RequestHandler>();

    public Dictionary<int, RequestHandler> requestPortHandlers = new Dictionary<int, RequestHandler>();
    public Dictionary<int, Thread> requestPortThread = new Dictionary<int, Thread>();
    public Dictionary<int, UdpClient> requestPortListener = new Dictionary<int, UdpClient>();

    public void RegisterUDPChannel(int port, RequestHandler handler) {
        if (requestPortHandlers.ContainsKey(port))
        {
            requestPortHandlers[port] = handler;

        }
        else {
            requestPortHandlers.Add(port, handler);
            UdpClient listner = new UdpClient(port);
            requestPortListener.Add(port, listner);
            Thread TempreceiveThread = new Thread(() => ReceiveData(port));
            TempreceiveThread.IsBackground = true;
            TempreceiveThread.Start();
            requestPortThread.Add(port, TempreceiveThread);
        }
    }
    public void UnregisterUDPChannel(int port) {
        if (requestPortListener.ContainsKey(port))
        {
            requestPortListener[port].Close();
            requestPortListener.Remove(port);
            requestPortThread[port].Abort();
            requestPortThread.Remove(port);
            requestPortHandlers.Remove(port);
        }
    }

    private void OnApplicationQuit()
    {
        abort = true;
        foreach (int p in requestPortHandlers.Keys) {
            UnregisterUDPChannel(p);
        }
        _listener.Close();
    }

    private void OnDestroy()
    {
        abort = true;
        foreach (int p in requestPortHandlers.Keys)
        {
            UnregisterUDPChannel(p);
        }
    }

    private void ReceiveData(int port)
    {
        started = true;
        string _receivedMessage = "";
        try
        {
            string address = "";
            while (!abort)
            {
                byte[] bytes = requestPortListener[port].Receive(ref _groupEP);
                address = _groupEP.Address.ToString();
                string[] msgRcv = { Encoding.ASCII.GetString(bytes, 0, bytes.Length) };
                lock (lockObject)
                {
                    _receivedMessage = msgRcv[0];
                }

                Debug.Log(_receivedMessage);

                requestPortHandlers[port](_receivedMessage);
            }

        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
        finally
        {
            requestPortListener[port].Close();
        }
    }


    //[SerializeField]
    //private int port = 15000;

    //public int Port { get => port; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //_listener = new UdpClient(port);
        _groupEP = new IPEndPoint(IPAddress.Any, 0);

        //receiveThread = new Thread(
        //   new ThreadStart(ReceiveData));
        //receiveThread.IsBackground = true;
        //receiveThread.Start();
    }


    /// <summary>
    /// Receiver Threads
    /// </summary>
    /*private void ReceiveData()
    {
        started = true;
        try
        {
            string address = "";
            while (!abort)
            {

                byte[] bytes = _listener.Receive(ref _groupEP);
                address = _groupEP.Address.ToString();
                string[] msgRcv = { Encoding.ASCII.GetString(bytes, 0, bytes.Length) };
                lock (lockObject)
                {
                    _receivedMessage = msgRcv[0];
                }

                Debug.Log(_receivedMessage);

                foreach (KeyValuePair<Regex, RequestHandler> entry in RequestHandlers)
                {
                    if (entry.Key.IsMatch(address))
                    {
                        entry.Value(_receivedMessage);
                        return;
                    }
                }

                //MessageReceived?.Invoke(_receivedMessage);
            }

        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
        finally
        {
            _listener.Close();
        }
    }

    public void StopStream()
    {
        abort = true;
        _listener.Close();
    }*/
}
