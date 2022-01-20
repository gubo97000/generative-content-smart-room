using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using UnityToolbarExtender;

static class ToolbarStyles
{
    public static readonly GUIStyle commandButtonStyle;
    public static readonly GUIStyle activeButtonStyle;
    public static readonly GUIStyle inactiveButtonStyle;

    static ToolbarStyles()
    {
        commandButtonStyle = new GUIStyle("Command")
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter,
            imagePosition = ImagePosition.ImageAbove,
            // fontStyle = FontStyle.Bold
        };
        activeButtonStyle = new GUIStyle("ToolbarButton")
        {
            // fontSize = 16,
            // alignment = TextAnchor.MiddleCenter,
            // imagePosition = ImagePosition.ImageAbove,
            fontStyle = FontStyle.Bold
        };
        inactiveButtonStyle = new GUIStyle("ToolbarButton")
        {
            // fontSize = 16,
            // alignment = TextAnchor.MiddleCenter,
            // imagePosition = ImagePosition.ImageAbove,
            // fontStyle = FontStyle.Bold
        };
        {
            // fontSize = 16,
            // alignment = TextAnchor.MiddleCenter,
            // imagePosition = ImagePosition.ImageAbove,
            // // fontStyle = FontStyle.Bold
        };
    }
}
static class ToRestore{
    public static string? sceneName;
}

[InitializeOnLoad]
public class SceneSwitchLeftButton
{
    static SceneSwitchLeftButton()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }
    static private bool _startFromInit;
    static private string? _toRestore;
    static void RestoreScene(PlayModeStateChange state)
    {
        Debug.Log("Called!!" + state+_toRestore);
        if (_toRestore != null && state == PlayModeStateChange.EnteredEditMode)
        {
            Debug.Log("Restoring scene: " + _toRestore);
            EditorSceneManager.OpenScene(_toRestore);
            _toRestore = null;
            EditorApplication.playModeStateChanged -= RestoreScene;

        }
    }
    static void OnToolbarGUI()
    {
        EditorApplication.playModeStateChanged += RestoreScene;

        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorSceneManager.playModeStartScene = null;
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(new GUIContent("Init", "Start to play from Init"), _startFromInit ? ToolbarStyles.activeButtonStyle : ToolbarStyles.inactiveButtonStyle))
        {
            SetPlayModeStartScene("Assets/Scenes/Init.unity");
            EditorApplication.EnterPlaymode();
            // EditorSceneManager.
        }
        if (GUILayout.Button(new GUIContent("Init2", "Start to play from Init"), _startFromInit ? ToolbarStyles.activeButtonStyle : ToolbarStyles.inactiveButtonStyle))
        {
            // SetPlayModeStartScene("Assets/Scenes/Init.unity");
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            // var _toRestore=EditorApplication.currentScene;
            _toRestore = EditorSceneManager.GetActiveScene().path;
            EditorSceneManager.OpenScene("Assets/Scenes/Init.unity");
            EditorApplication.EnterPlaymode();
            EditorApplication.playModeStateChanged += delegate (PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.EnteredEditMode)
                {
                    EditorSceneManager.OpenScene(_toRestore);
                    _toRestore = null;
                    EditorApplication.playModeStateChanged -= RestoreScene;
                }
            };

        }

        // if (GUILayout.Button(new GUIContent("2", "Start Scene 2"), ToolbarStyles.commandButtonStyle))
        // {
        //     // SceneHelper.StartScene("Assets/ToolbarExtender/Example/Scenes/Scene2.unity");
        // }
    }
    static void SetPlayModeStartScene(string scenePath)
    {
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
        if (myWantedStartScene != null)
            EditorSceneManager.playModeStartScene = myWantedStartScene;
        else
            Debug.Log("Could not find Scene " + scenePath);
    }
}
