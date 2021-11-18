using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomKinectV2Manager))]
public class MagicRoomKinectEditor : Editor
{
    private bool showTips;
    private string TipsHelpBoxs = "Show Tips on the usage";

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomKinectV2Manager m = (MagicRoomKinectV2Manager)target;

        EditorGUILayout.LabelField("Player motion detection Module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        if (m.StatusKinectSensor)
        {
            c = new Color(0f, 1f, 0f, 0.5f);
            currentStyle.normal.textColor = Color.black;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("Kinect Sensor is active and runnig."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }
        else
        {
            c = Color.red;
            currentStyle.normal.textColor = Color.black;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("Server not found.The simulator is turned on."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To register and receive the skeeltons use the action Skeletons", MessageType.Info);
            EditorGUILayout.HelpBox("To detect the server use the StatusKinectSensor property", MessageType.Info);
            EditorGUILayout.HelpBox("To start the automatic information flood (Highly suggested) from the server use the function StartStreamingSkeletons(...), where the parameter represent the time interval between two samplesof the skeleton. Suggested value is 250", MessageType.Info);
            EditorGUILayout.HelpBox("To stop the automatic information flood (Highly suggested) from the server use the function StopStreamingSkeletons()", MessageType.Info);
            EditorGUILayout.HelpBox("To read the players' position at command (Highly disouraged) use the function ReadLastSamplingKinect()", MessageType.Info);
            GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
            EditorGUILayout.HelpBox("Unless very peculiar needs, it is sugested to use the TrackerPlayerPosition class which provides easyer functions  and is already managing the transformation between the simulator and the Magic Room.", MessageType.Warning);
            GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
            EditorGUILayout.HelpBox("In case you see the server connected and the simulator active, the most common cause is that you registered for the stream of skeletons before the sevr rsponded with its state. Please consider to postpone the registration to the skeletons only when you need it(typically in the game scene after the menu scene", MessageType.Warning);
        }
    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}