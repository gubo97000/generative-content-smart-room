using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomTextToSpeachManager))]
public class MagicRoomTTSEditor : Editor
{

    bool showTips = true, showLiveTest = true;
    string TipsHelpBoxs = "Show Tips on the usage", LivetestBox= "Want to try out?";

    string message;

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomTextToSpeachManager m = (MagicRoomTextToSpeachManager)target;

        EditorGUILayout.LabelField("Text to Speach Module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        int height = 0;

        string listofvoices = "This is the list of avaialble voices:\n";
        if (m.ListOfVoice.Count > 0) {
            foreach (Voices v in m.ListOfVoice) {
                if (v.alias != "")
                {
                    listofvoices += v.alias + "\n";
                    height++;
                }
            }
            c = m.IsPlaying ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
            currentStyle.normal.textColor = Color.black;
        }
        else{
            listofvoices = "No voice has been found.\nPlease control the state of the module or the simulator.";
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
        }
        currentStyle.normal.background = MakeTex(2, 2, c);
        GUILayout.Box(new GUIContent(listofvoices), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30 + 10*height));

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To invoque the text to speech use function GenerateAudioFromText(...)", MessageType.Info);
            EditorGUILayout.HelpBox("To Detect if the Text To Speech finisched reading a sentence subscribe the action EndSpeak", MessageType.Info);
            EditorGUILayout.HelpBox("Text To Speech module ignore audios required while another audio is playing!", MessageType.Warning);
        }

        if (m.ListOfVoice.Count > 0)
        {
            showLiveTest = EditorGUILayout.Foldout(showLiveTest, LivetestBox);
            if (showLiveTest)
            {
                message = EditorGUILayout.TextField(message);
                if (GUILayout.Button("Try it out!"))
                {
                    m.GenerateAudioFromText(message);
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