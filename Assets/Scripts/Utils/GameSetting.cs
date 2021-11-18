using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetting : MonoBehaviour
{
    public event Action<List<Player>> ReceivedPlayers;

    public GameConfiguration configuration;
    public List<Player> players;

    [HideInInspector]
    public Camera Frontcamera, FloorCamera;

    public static GameSetting instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //set up a base configuration

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        FloorCamera = null;
        Frontcamera = null;
    }

    public void SetConfiguration(GameConfiguration conf, List<Player> players)
    {
        this.players = players;
        ReceivedPlayers?.Invoke(players);
        configuration = conf;
        if (configuration.IsValidConfiguration(configuration))
        {
            Debug.Log("Parsing error in the configuration");
            JObject result = new JObject();
            result["result"] = true;
            MagicRoomManager.instance.ExperienceManagerComunication.SendResponse("setConfiguration", result);
            StartGame();
        }
        else
        {
            Debug.Log("Parsing error in the configuration");
            JObject result = new JObject();
            result["result"] = false;
            MagicRoomManager.instance.ExperienceManagerComunication.SendResponse("setConfiguration", result);
        }
    }

    public void StartGame()
    {
        //set up here the proepr aprameter for the story.

        SceneManager.LoadScene(2);
    }

    public void LoadMenu()
    {
        //destroy eventaul controllers and close streams for
        SceneManager.LoadScene(1);
    }

    private void OnApplicationQuit()
    {
        if (MagicRoomManager.instance != null)
        {
            if (MagicRoomManager.instance.MagicRoomLightManager != null)
            {
                MagicRoomManager.instance.MagicRoomLightManager.SendColor(Color.black);
            }
            if (MagicRoomManager.instance.MagicRoomAppliancesManager != null)
            {
                foreach (string s in MagicRoomManager.instance.MagicRoomAppliancesManager.Appliances)
                {
                    MagicRoomManager.instance.MagicRoomAppliancesManager.SendChangeCommand(s, "OFF");
                }
            }
        }
    }
}