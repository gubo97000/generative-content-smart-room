using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MagicRoomManager))]
[CanEditMultipleObjects]
public class MagicRommManagerEditor : Editor
{
    private SerializedProperty portHTTP;

    private SerializedProperty activityidentifier;
    private SerializedProperty activityName;

    private SerializedProperty Lights;
    private SerializedProperty Appliances;
    private SerializedProperty TextToSpeech;
    private SerializedProperty Kinect;
    private SerializedProperty SmartToy;
    private SerializedProperty Lidar;
    private SerializedProperty Music;

    private SerializedProperty indexScene;

    private void OnEnable()
    {
        portHTTP = serializedObject.FindProperty("portHTTP");
        Lights = serializedObject.FindProperty("Lights");
        Appliances = serializedObject.FindProperty("Appliances");
        TextToSpeech = serializedObject.FindProperty("TextToSpeech");
        Kinect = serializedObject.FindProperty("Kinect");
        Lidar = serializedObject.FindProperty("Lidar");
        Music = serializedObject.FindProperty("MusicbackGround");
        SmartToy = serializedObject.FindProperty("SmartToy");
        indexScene = serializedObject.FindProperty("indexScene");
        activityidentifier = serializedObject.FindProperty("activityidentifier");
        activityName = serializedObject.FindProperty("activityName");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        MagicRoomManager m = (MagicRoomManager)target;

        EditorGUILayout.LabelField("Configure the basic Element of your\n apllication in the Magic Room", EditorStyles.boldLabel, GUILayout.Height(40));

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Activity id:", EditorStyles.boldLabel, GUILayout.Height(40));
        EditorGUILayout.HelpBox("Obtian the id form the registering system in Magika's developer website", MessageType.Info);
        int i = 0;
        foreach (int number in m.activityidentifier)
        {
            string newval = "";
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.ToString() + ": ");
            newval = EditorGUILayout.TextField(m.activityidentifier[i].ToString());
            if (GUILayout.Button("X"))
            {
                List<int> temp = new List<int>();
                temp.AddRange(m.activityidentifier);
                temp.RemoveAt(i);
                m.activityidentifier = temp.ToArray();
            }
            bool error = false;
            int nvint = 0;
            error = !int.TryParse(newval, out nvint);

            foreach (int n in m.activityidentifier)
            {
                if (n == nvint)
                {
                    error = true;
                }
            }

            if (!error)
            {
                m.activityidentifier[i] = nvint;
            }

            i++;
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add a new ID"))
        {
            if (m.activityidentifier[m.activityidentifier.Length - 1] != 0)
            {
                List<int> temp = new List<int>();
                temp.AddRange(m.activityidentifier);
                temp.Add(0);
                m.activityidentifier = temp.ToArray();
            }
        }

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
        EditorGUILayout.PropertyField(activityName, new GUIContent("Activity name: ", "The common name associated to your game in Magika's developer website "));
        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        EditorGUILayout.LabelField("Configuration Parameter For HTTP", EditorStyles.boldLabel);
        EditorGUILayout.IntSlider(portHTTP, 7000, 7099, "Port: ");
        //EditorGUILayout.PropertyField(addressHTTP, new GUIContent("Accepting address"));
        //EditorGUILayout.PropertyField(portHTTP);

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        EditorGUILayout.LabelField("Select the component to activate:", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(Lights);
        EditorGUILayout.PropertyField(Appliances);
        EditorGUILayout.PropertyField(TextToSpeech);
        EditorGUILayout.PropertyField(Kinect);
        EditorGUILayout.PropertyField(Music);
        EditorGUILayout.PropertyField(Lidar);
        EditorGUILayout.PropertyField(SmartToy);

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));

        EditorGUILayout.LabelField("First scene index", EditorStyles.boldLabel);
        //EditorGUILayout.PropertyField(indexScene);

        EditorGUILayout.IntSlider(indexScene, 1, UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings, "Menu Scene Index");

        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Activity Manifest"))
        {
            WriteManifestFile();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void WriteManifestFile()
    {
        MagicRoomManager m = (MagicRoomManager)target;
        dynamic manifest = new JObject();
        if (m.activityidentifier.Length == 1)
        {
            manifest.id = m.activityidentifier[0];
        }
        else
        {
            manifest.id = JArray.FromObject(m.activityidentifier);
        }
        manifest.friendlyName = m.activityName;
        manifest.executableName = Application.productName + ".exe";
        manifest.port = m.portHTTP;
        File.WriteAllText(Application.dataPath + "/Resources/manifest.dat", manifest.ToString());

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}