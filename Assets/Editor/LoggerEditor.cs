using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Logger))]
public class LoggerEditor : Editor
{
    private bool showTips;
    private string TipsHelpBoxs = "How to use the logger";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Logger l = (Logger)target;
        EditorGUILayout.LabelField("The log session is " + l.SessionID, EditorStyles.boldLabel, GUILayout.Height(40));
        EditorGUILayout.LabelField("You have logged " + l.logcount + " lines.");

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("To add a new line to the persistent log: AddToLogNewLine(\".......\")", MessageType.Info);
        }
    }
}