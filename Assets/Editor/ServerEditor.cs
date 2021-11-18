using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HttpListenerForMagiKRoom))]
public class ServerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("HTTP Listener is Running", EditorStyles.boldLabel, GUILayout.Height(40));
        EditorGUILayout.LabelField("Last message received from ");
        EditorGUILayout.LabelField(((HttpListenerForMagiKRoom)target).lastreadmessage);
    }
}