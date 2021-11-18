using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MagicRoomManager : MonoBehaviour
{
    public static MagicRoomManager instance;

    public delegate void WebCallback(string body);

    public MagicRoomLightManager MagicRoomLightManager
    {
        get;
        private set;
    }

    public MagicRoomAppliancesManager MagicRoomAppliancesManager
    {
        get;
        private set;
    }

    public ExperienceManagerComunication ExperienceManagerComunication
    {
        get;
        private set;
    }

    public MagicRoomTextToSpeachManager MagicRoomTextToSpeachManager
    {
        get;
        private set;
    }

    public MagicRoomKinectV2Manager MagicRoomKinectV2Manager
    {
        get;
        private set;
    }

    public MagicRoomSmartToyManager MagicRoomSmartToyManager
    {
        get;
        private set;
    }

    public MagicRoomLidarManager MagicRoomLidarManager
    {
        get;
        private set;
    }

    public MagicRoomBackgroundMusicManager MagicRoomBackgroundMusicManager
    {
        get;
        private set;
    }

    public HttpListenerForMagiKRoom HttpListenerForMagiKRoom
    {
        get;
        private set;
    }

    public UDPListener UDPListenerForMagikRoom {
        get;
        private set;
    }

    public Logger Logger
    {
        get;
        private set;
    }

    public int portHTTP;

    public int[] activityidentifier;
    public string activityName;

    public bool Lights;
    public bool Appliances;
    public bool TextToSpeech;
    public bool Kinect;
    public bool SmartToy;
    public bool Lidar = true;
    public bool MusicbackGround;

    public int indexScene = 1;

    public SystemConfiguration systemConfiguration;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (Display dis in Display.displays)
            {
                dis.Activate();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dynamic message = new JObject();
        message.request = new JObject();
        message.request.type = "getSystemConfiguration";
        string payload = message.ToString();
        StartCoroutine(SendRequest("http://localhost:7100", payload, (body) =>
        {
            try
            {
                JObject response = JObject.Parse(body);
                systemConfiguration = response.Value<JObject>("response").Value<JObject>("payload").ToObject<SystemConfiguration>();
            }
            catch (Exception)
            {
                systemConfiguration = new SystemConfiguration();
            }
            finally
            {
                InitComponent();
            }
        }, () =>
        {
            systemConfiguration = new SystemConfiguration();
            InitComponent();
        }));
    }

    private void InitComponent()
    {
        Logger = gameObject.AddComponent<Logger>();
        HttpListenerForMagiKRoom = gameObject.AddComponent<HttpListenerForMagiKRoom>();
        UDPListenerForMagikRoom = gameObject.AddComponent<UDPListener>();
        ExperienceManagerComunication = gameObject.AddComponent<ExperienceManagerComunication>();

        if (Lights)
            MagicRoomLightManager = gameObject.AddComponent<MagicRoomLightManager>();

        if (Appliances)
            MagicRoomAppliancesManager = gameObject.AddComponent<MagicRoomAppliancesManager>();

        if (TextToSpeech)
            MagicRoomTextToSpeachManager = gameObject.AddComponent<MagicRoomTextToSpeachManager>();

        if (Kinect)
            MagicRoomKinectV2Manager = gameObject.AddComponent<MagicRoomKinectV2Manager>();

        if (SmartToy)
            MagicRoomSmartToyManager = gameObject.AddComponent<MagicRoomSmartToyManager>();

        if (Lidar)
            MagicRoomLidarManager = gameObject.AddComponent<MagicRoomLidarManager>();

        if (MusicbackGround) {
            MagicRoomBackgroundMusicManager = gameObject.AddComponent<MagicRoomBackgroundMusicManager>();
        }

        if (StreamingAssetManager.instance)
            StreamingAssetManager.instance.PathResources = systemConfiguration.resourcesPath;

        SceneManager.LoadScene(indexScene);
    }

    private IEnumerator SendRequest(string url,
                                    string payload,
                                    WebCallback callback = null,
                                    UnityAction error = null)
    {
        byte[] body = System.Text.Encoding.UTF8.GetBytes(payload);
        UnityWebRequest request = new UnityWebRequest(url, "POST")
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
            error?.Invoke();
        }
    }
}

public class SystemConfiguration
{
    public int frontalScreen = 0;
    public int floorScreen = 1;
    public string resourcesPath = "C:\\Users\\Utente\\Desktop\\Magika\\Resources";
    public string backendUrl = null;
    public string roomId = null;
    public float floorSizeX = 2.74f;
    public float floorSizeY = 2.88f;
    public float floorOffsetX = 0;
    public float floorOffsetY = 0;
    public string logPath;
    public string sessionActivityID;
}