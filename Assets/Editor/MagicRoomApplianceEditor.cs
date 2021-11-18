using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomAppliancesManager))]
public class MagicRoomApplianceEditor : Editor
{
    private bool showTips = true, showLiveTest = true;
    private string TipsHelpBoxs = "Show Tips on the usage", LivetestBox = "Want to try out?";

    private string message;
    private bool state;
    private int duration;
    private List<string> names;
    private int selected;

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomAppliancesManager m = (MagicRoomAppliancesManager)target;

        EditorGUILayout.LabelField("Text to Speach Module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        EditorGUILayout.LabelField("This is the list of smart appliances:");
        names = new List<string>();
        if (m.Appliances.Count > 0)
        {
            foreach (SmartPlugState v in m.appliances)
            {
                names.Add(v.associatedname);
                c = v.isActive ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
                currentStyle.normal.textColor = Color.black;
                currentStyle.normal.background = MakeTex(2, 2, c);
                GUILayout.Box(new GUIContent(v.associatedname), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
            }
        }
        else
        {
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("No Applaince has been found.\nPlease control the state of the module or the simulator."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To set the state of the smart appliance use SendChangeCommand(... , ... , [...])", MessageType.Info);
            EditorGUILayout.HelpBox("The first parameter is the name of the smart plug, the second is the state you want to set: true or \"ON\" to set it on, false or \"OFF\" to turn off.", MessageType.Warning);
            EditorGUILayout.HelpBox("The third indicates for how long the command (to turn on) has to be kept before automatically stopping. Default value is 0 (immediate action with no delay). Parameter is optional", MessageType.Warning);
        }

        if (m.appliances.Count > 0)
        {
            showLiveTest = EditorGUILayout.Foldout(showLiveTest, LivetestBox);
            if (showLiveTest)
            {
                //message = EditorGUILayout.TextField(message);
                selected = EditorGUILayout.Popup("Select a smart plug", selected, names.ToArray());
                message = names.ElementAt(selected);
                state = EditorGUILayout.Toggle("set it on or off?", state);
                duration = EditorGUILayout.IntField("Duration of the activation", duration);
                string st = state ? "ON" : "OFF";
                if (GUILayout.Button("Try it out!"))
                {
                    m.SendChangeCommand(message, st, duration);
                }
            }
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