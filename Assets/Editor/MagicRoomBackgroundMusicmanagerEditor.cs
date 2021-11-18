using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MagicRoomBackgroundMusicManager))]

public class MagicRoomBackgroundMusicmanagerEditor : Editor
{

    private bool showTips = true, showLiveTestSetMusic = true;
    private string TipsHelpBoxs = "Show Tips on the usage", LivetestBox = "Want to try out?";


    private List<string> names;
    private int selected;
    private string message;

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomBackgroundMusicManager m = (MagicRoomBackgroundMusicManager)target;

        EditorGUILayout.LabelField("Background Music Module for the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        EditorGUILayout.LabelField("This is the list of smart appliances:");
        names = new List<string>();
        if (m.musicTracks.Count > 0)
        {
            foreach (string n in m.musicTracks)
            {
                if (n != "silence")
                {
                    names.Add(n);
                    c = m.musicPlaying == n ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
                    currentStyle.normal.textColor = Color.black;
                    currentStyle.normal.background = MakeTex(2, 2, c);
                    GUILayout.Box(new GUIContent(n), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
                }
            }
        }
        else
        {
            c = Color.black;
            currentStyle.normal.textColor = Color.white;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("No music has been found.\nPlease control the state of the module or the simulator."), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To set the music, please use the function requestBackGroundMusicChange(...)", MessageType.Warning);
            EditorGUILayout.HelpBox("The first parameter is the name of the music, obtained by the list musicTracks", MessageType.Info);
            EditorGUILayout.HelpBox("To stop the music, please use the function stopBackGroundMusic()", MessageType.Warning);
        }

        

        if (m.musicTracks.Count > 0)
        {
            showLiveTestSetMusic = EditorGUILayout.Foldout(showLiveTestSetMusic, LivetestBox);
            if (showLiveTestSetMusic)
            {
                //message = EditorGUILayout.TextField(message);
                selected = EditorGUILayout.Popup("Select a music track", selected, names.ToArray());
                message = names.ElementAt(selected);
                if (GUILayout.Button("Start the selected track!"))
                {
                    m.requestBackGroundMusicChange(message);
                }
                if (GUILayout.Button("Stop music!"))
                {
                    m.stopBackGroundMusic();
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
