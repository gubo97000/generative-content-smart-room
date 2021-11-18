using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerMovementSimultor))]
public class PlayerMovementSimulatorEditor : Editor
{
    private SerializedProperty animationkeys;
    private SerializedProperty rotationSpeed;
    private SerializedProperty movementSpeed;

    private void OnEnable()
    {
        animationkeys = serializedObject.FindProperty("animationkeys");
        rotationSpeed = serializedObject.FindProperty("rotationSpeed");
        movementSpeed = serializedObject.FindProperty("movementSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("The simulated player id is " + (((PlayerMovementSimultor)target).id));

        EditorGUILayout.Space();
        EditorStyles.label.wordWrap = true;

        EditorGUILayout.LabelField("To move the player keep pressed the corresponding id number. To move forward and backward use the up and down arrow. To turn use Left and Right arrow.");
        EditorGUILayout.LabelField("To move the hands freely, keep pressed the corresponding id number and press the Capsloc key. Then move the mouse on the screen to guide the player's right hand. to sto following the mouse repress capsloc key. ");
        EditorGUILayout.LabelField("To open and close the player's hands keep pressed the corresponding id number and press the shift key (left shift for left hand, right shift for right hand. One press to add, one to remove. . ");
        EditorGUILayout.LabelField("To Add a new animation: pen the animator, import the animation (or create it), then connect it to the idle state through a TRIGGER EVENT. The add the trigger's name in the following dictionary together with the key used to trigger the action");

        EditorGUILayout.PropertyField(movementSpeed, new GUIContent("Movement Speed"));
        EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed"));

        EditorGUILayout.PropertyField(animationkeys);

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(SimulationAnimatorDict))]
public class SimulationAnimatorDictDrawer1 : DictionaryDrawer<KeyCode, string> { }