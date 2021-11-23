using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static HashSet<GameObject> playersCrouched = new HashSet<GameObject>();

    private static GameStateManager gameStateManager;

    public static GameStateManager instance
    {
        get
        {
            if (!gameStateManager)
            {
                gameStateManager = FindObjectOfType(typeof(GameStateManager)) as GameStateManager;

                if (!gameStateManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    gameStateManager.Init();
                }
            }

            return gameStateManager;
        }
    }

    void Init()
    {
        if (playersCrouched == null)
        {
            playersCrouched = new HashSet<GameObject>();
        }
    }

    private void Start()
    {
        EventManager.StartListening("OnCrouchStart", OnCrouchStartHandler);
        EventManager.StartListening("OnCrouchEnd", OnCrouchEndHandler);
    }
    private void OnDestroy()
    {
        EventManager.StopListening("OnCrouchStart", OnCrouchStartHandler);
        EventManager.StopListening("OnCrouchEnd", OnCrouchEndHandler);
    }
    void OnCrouchStartHandler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        playersCrouched.Add(sender);
    }
    void OnCrouchEndHandler(EventDict dict)
    {
        GameObject sender = (GameObject)dict["sender"];
        playersCrouched.Remove(sender);

    }
}
