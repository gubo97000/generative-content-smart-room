using Newtonsoft.Json.Linq;
using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExperienceManagerComunication))]
public class ExperienceManagerComunicationEditor: Editor {
    private bool showTips, showTips2;

    private string TipsHelpBoxs = "Mandatory messages for syncronization with Magika";
    private string TipsHelpBoxs2 = "Messages to the tablet";

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        showTips = EditorGUILayout.Foldout(showTips, TipsHelpBoxs);
        if (showTips)
        {
            EditorGUILayout.HelpBox("Without these messages your applicaiton will not be able to syncronize with Magika", MessageType.Warning);
            EditorGUILayout.HelpBox("As soon as the aplication is ready to read from the http listener: SendMessage(\"ready\")", MessageType.Info);
            EditorGUILayout.HelpBox("As soon as the game scene is ready to be played: SendMessage(\"started\")", MessageType.Info);
            EditorGUILayout.HelpBox("To call for the next player to play : SendMessage(\"newTurn\")", MessageType.Info);
            EditorGUILayout.HelpBox("When the game is finished: SendMessage(\"endGame\", JObject.Parse(@\"'result': 'win'\");)", MessageType.Info);
            EditorGUILayout.HelpBox("When the game is finished and the child failed the game: SendMessage(\"endGame\", JObject.Parse(@\"'result': 'failure'\");)", MessageType.Info);
        }

        showTips2 = EditorGUILayout.Foldout(showTips2, TipsHelpBoxs2);
        if (showTips2)
        {
            EditorGUILayout.HelpBox("To send a message to the tablet application: SendMessage(JObject.Parse(@\"........\");)", MessageType.Info);
            EditorGUILayout.HelpBox("The message init, to set up the tablet are in the form (where payload is a JObject) : JObject.Parse(@\"'type': 'live_message', 'action': 'init' 'payload':.........\");)", MessageType.Info);
            EditorGUILayout.HelpBox("The message to inform the tablet during the game (where payload is a JObject) : JObject.Parse(@\"'type': 'live_message', 'action': 'command' 'payload':.........\");)", MessageType.Info);

        }

    }

    public void OnInspectorUpdate()
    {
        this.Repaint();
    }

}
