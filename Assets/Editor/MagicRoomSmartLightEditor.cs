using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomLightManager))]
public class MagicRoomSmartLightEditor : Editor
{
    private bool showTips = true, showLiveTest = true, showLiveTestAll = true, showLiveTestLight = true, showLiveTestEff = true;
    private string TipsHelpBoxs = "Show Tips on the usage", LivetestBox = "Want to try out?";

    private Color message, message2;

    private int ind = 0, ind2 = 0;
    private bool loop = false;

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomLightManager m = (MagicRoomLightManager)target;

        EditorGUILayout.LabelField("Smart Light Module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        EditorGUILayout.LabelField("This is the list of avaialble Lights:");

        if (m.Lights.Count > 0)
        {
            foreach (string v in m.Lights)
            {
                if (v != "")
                {
                    currentStyle.normal.textColor = Color.black;
                    c = m.getColorofLight(v);
                    currentStyle.normal.background = MakeTex(2, 2, c);
                    GUILayout.Box(new GUIContent(v), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(20));
                }
            }
        }
        else
        {
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("No light has been found.\nPlease control the state of the module or the simulator."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(40));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        GUILayout.Label("This is the list of effects available in the system");
        if (m.effects.Count > 0)
        {
            foreach (string e in m.effects)
            {
                if (e != "")
                {
                    currentStyle.normal.textColor = Color.grey;
                    c = Color.white;
                    currentStyle.normal.background = MakeTex(2, 2, c);
                    GUILayout.Box(new GUIContent("   " + e), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(20));
                }
            }
        }
        else
        {
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("No effect has yet been registered"), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(20));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To color a specific light, please use the function SendColor(..., [...], [...], [..., ..., ...])", MessageType.Info);
            EditorGUILayout.HelpBox("The first paramter represent the color of the light and is mandatory.", MessageType.Info);
            EditorGUILayout.HelpBox("The second optional parameter represent brightness in an integer scale 1 to 100. The third optional parameter represent the name of the light (leave it null to color all lights). ", MessageType.Info);
            EditorGUILayout.HelpBox("The last three parameters make the command affect all the lights in a specific area of the room, described by the location segments, represented in the enums LocDepth, LocHorizontal and LocVertical", MessageType.Info);
            EditorGUILayout.HelpBox("Indication over location is overrided b the name of the light", MessageType.Warning);
            EditorGUILayout.HelpBox("Indication over location is not available for movable lights", MessageType.Warning);
        }

        if (m.Lights.Count > 0)
        {
            showLiveTest = EditorGUILayout.Foldout(showLiveTest, LivetestBox);
            if (showLiveTest)
            {
                ;

                showLiveTestAll = EditorGUILayout.Foldout(showLiveTestAll, "Act on all the room");
                if (showLiveTestAll)
                {
                    message = EditorGUILayout.ColorField(message);
                    if (GUILayout.Button("Try it out!"))
                    {
                        m.SendColor(message);
                    }
                }
                showLiveTestLight = EditorGUILayout.Foldout(showLiveTestLight, "Select one light");
                if (showLiveTestLight)
                {
                    message2 = EditorGUILayout.ColorField(message2);

                    ind = EditorGUILayout.Popup("Select a smart light", ind, m.Lights.ToArray());
                    if (GUILayout.Button("Try it out!"))
                    {
                        Debug.Log(m.Lights.ToArray()[ind]);
                        m.SendColor(message2, m.Lights.ToArray()[ind]);
                    }
                }
                showLiveTestEff = EditorGUILayout.Foldout(showLiveTestEff, "Select an effect");
                if (showLiveTestEff)
                {
                    ind2 = EditorGUILayout.Popup("Select an effect", ind2, m.effects.ToArray());

                    loop = EditorGUILayout.Toggle(new GUIContent("Il active in loop?"), loop);
                    if (GUILayout.Button("Try start the effect!"))
                    {
                        m.setEffect(m.effects.ToArray()[ind2], loop, true);
                    }
                    if (GUILayout.Button("stop the effect!"))
                    {
                        m.setEffect(m.effects.ToArray()[ind2], loop, false);
                    }
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