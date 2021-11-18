using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomSmartToyManager))]
public class MagicRoomSmartToyEditor : Editor
{
    private bool showTips = true;
    private string TipsHelpBoxs = "Show Tips on the usage";

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomSmartToyManager m = (MagicRoomSmartToyManager)target;

        EditorGUILayout.LabelField("Text to Speach Module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        EditorGUILayout.LabelField("This is the list of smart toys found:");
        if (m.GetAllToy().Count > 0)
        {
            foreach (string v in m.GetAllToy())
            {
                c = Color.white;
                currentStyle.normal.textColor = Color.black;
                currentStyle.normal.background = MakeTex(2, 2, c);
                GUILayout.Box(new GUIContent(v), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
            }
        }
        else
        {
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("No Smart Toy has been found.\nPlease control the state of the module or the simulator."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        EditorGUILayout.LabelField("This is the list of registered tags:");
        if (m.Rfids.Count > 0)
        {
            foreach (string v in m.Rfids.Keys)
            {
                c = Color.white;
                currentStyle.normal.textColor = Color.black;
                currentStyle.normal.background = MakeTex(2, 2, c);
                GUILayout.Box(new GUIContent(v + " -> " + m.Rfids[v]), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
            }
        }
        else
        {
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("No Tag has been found.\nPlease control the state of the module or the simulator."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To start listening to the messages SubscribeEvent(... , ...). the first parameter is the type of stream (TCP or UDP), the second is the ID of the object.", MessageType.Info);
            EditorGUILayout.HelpBox("To stop listening to the messages UnsubscribeEvent(... , ...). the first parameter is the type of stream (TCP or UDP), the second is the ID of the object. MUST match the prevously started stream", MessageType.Info);
            EditorGUILayout.HelpBox("To retrieve the ID of a smart object use the function GetSmartToyIDByName(....) with the parameter the name of the smart object", MessageType.Warning);
            EditorGUILayout.HelpBox("To retrieve the gameObject of a smart object use the function GetSmartToyByName(....) with the parameter the name of the smart object", MessageType.Warning);
            EditorGUILayout.HelpBox("To retrieve the intellegible value for a tag use the function GetRfidAssosiation(....) with the parameter the received tag", MessageType.Warning);
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