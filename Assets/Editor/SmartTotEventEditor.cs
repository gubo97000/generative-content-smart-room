using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SmartToyEventManager)), CanEditMultipleObjects]
public class SmartTotEventEditor : Editor
{
    private SerializedProperty smartObjectName;
    private SerializedProperty rfid;
    private SerializedProperty touch;
    private SerializedProperty button;
    private SerializedProperty gyroscope;
    private SerializedProperty acceleromenter;
    private SerializedProperty rfidEvent1, rfidEvent2;
    private SerializedProperty touchEvent1, touchEvent2;
    private SerializedProperty buttonEvent1, buttonEvent2;

    private bool showTips = true;
    private string TipsHelpBoxs = "Show Tips on the usage";

    private void OnEnable()
    {
        smartObjectName = serializedObject.FindProperty("smartObjectName");
        rfid = serializedObject.FindProperty("rfid");
        touch = serializedObject.FindProperty("touch");
        button = serializedObject.FindProperty("button");
        gyroscope = serializedObject.FindProperty("gyroscope");
        acceleromenter = serializedObject.FindProperty("accelerometer");
        rfidEvent1 = serializedObject.FindProperty("releaseRFID");
        rfidEvent2 = serializedObject.FindProperty("readRFID");
        touchEvent1 = serializedObject.FindProperty("detectTouch");
        touchEvent2 = serializedObject.FindProperty("releaseTouch");
        buttonEvent1 = serializedObject.FindProperty("detectButton");
        buttonEvent2 = serializedObject.FindProperty("releaseButton");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SmartToyEventManager m = (SmartToyEventManager)target;

        EditorGUILayout.LabelField("Helper for the management of Smart Toys", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorStyles.label.wordWrap = true;

        GUIStyle currentStyle = new GUIStyle();
        currentStyle.wordWrap = true;
        Color c = Color.white;

        EditorGUILayout.PropertyField(smartObjectName);

        if (m.toyobject == null)
        {
            c = Color.red;
            currentStyle.normal.textColor = Color.black;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("The smart toy you are searching is not available"), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        EditorGUILayout.LabelField("Select the channels of interest:");
        EditorGUILayout.PropertyField(rfid);
        if (m.rfid)
        {
            EditorGUILayout.PropertyField(rfidEvent2);
            EditorGUILayout.HelpBox("Remember to select the functions with DYNAMIC parameters (top half of the list of values)", MessageType.Warning);
            EditorGUILayout.PropertyField(rfidEvent1);
        }
        EditorGUILayout.PropertyField(touch);
        if (m.touch)
        {
            EditorGUILayout.PropertyField(touchEvent1);
            EditorGUILayout.HelpBox("Remember to select the functions with DYNAMIC parameters (top half of the list of values)", MessageType.Warning);
            EditorGUILayout.PropertyField(touchEvent2);
        }
        EditorGUILayout.PropertyField(button);
        if (m.button)
        {
            EditorGUILayout.PropertyField(buttonEvent1);
            EditorGUILayout.HelpBox("Remember to select the functions with DYNAMIC parameters (top half of the list of values)", MessageType.Warning);
            EditorGUILayout.PropertyField(buttonEvent2);
        }
        EditorGUILayout.PropertyField(acceleromenter);
        EditorGUILayout.PropertyField(gyroscope);

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        if (m.TCPopen)
        {
            c = Color.green;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("TCP channel is open.\n Message count = " + m.MessagecountTCP), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(40));
        }
        else
        {
            c = Color.red;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("TCP channel is close"), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }
        if (m.UDPopen)
        {
            c = Color.green;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("UDP channel is open.\n Message count = " + m.MessagecountUDP), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(40));
        }
        else
        {
            c = Color.red;
            currentStyle.normal.background = MakeTex(2, 2, c);
            GUILayout.Box(new GUIContent("UDP channel is close"), currentStyle, GUILayout.Width(Screen.width), GUILayout.Height(30));
        }

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("First insert the name of the smart object to search. Select the fields you desire to detect. Then use the editor pannels to set up functiont to be called", MessageType.Info);
            EditorGUILayout.HelpBox("For READ and DETECT events remember to select the functions with DYNAMIC parameters (top half of the list of values)", MessageType.Warning);
        }

        serializedObject.ApplyModifiedProperties();
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