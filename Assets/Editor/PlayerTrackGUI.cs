using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[CustomEditor(typeof(TrackerPlayerPosition)), CanEditMultipleObjects]
public class PlayerTrackGUI : Editor
{
    private SerializedProperty activeTrackAxis;
    private SerializedProperty playerIdentifier;
    private SerializedProperty trackingSchema;
    private SerializedProperty activateHandEvents;
    private SerializedProperty activateGestureEvents;
    private SerializedProperty interactiveArea;
    private SerializedProperty handAction;
    private SerializedProperty handAction2;
    private SerializedProperty gestureAction;

    private void OnEnable()
    {
        activateHandEvents = serializedObject.FindProperty("activateHandCloseEvents");
        activateGestureEvents = serializedObject.FindProperty("activateGestureEvents");
        activeTrackAxis = serializedObject.FindProperty("activeTrackAxis");
        playerIdentifier = serializedObject.FindProperty("playerIdentifier");
        trackingSchema = serializedObject.FindProperty("trackingSchema");
        interactiveArea = serializedObject.FindProperty("interactiveArea");

        handAction = serializedObject.FindProperty("HandStateRight");
        handAction2 = serializedObject.FindProperty("HandStateLeft");
        gestureAction = serializedObject.FindProperty("GestureState");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
        EditorGUILayout.LabelField("Activate Hand Events");
        EditorGUILayout.PropertyField(activateHandEvents, new GUIContent(""), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();
        if (((TrackerPlayerPosition)target).activateHandCloseEvents)
        {
            EditorGUILayout.PropertyField(handAction);
            EditorGUILayout.PropertyField(handAction2);
            EditorGUILayout.HelpBox("Remember to select the functions with DYNAMIC parameters (top half of the list of values)", MessageType.Warning);
        }

        EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
        EditorGUILayout.LabelField("Activate Gesture Events");
        EditorGUILayout.PropertyField(activateGestureEvents, new GUIContent(""), GUILayout.Height(20));
        EditorGUILayout.EndHorizontal();
        if (((TrackerPlayerPosition)target).activateGestureEvents)
        {
            EditorGUILayout.PropertyField(gestureAction);

            EditorGUILayout.HelpBox("Remember to select the functions with DYNAMIC parameters (top half of the list of values)", MessageType.Warning);
        }
        EditorGUILayout.PropertyField(activeTrackAxis);
        EditorGUILayout.PropertyField(playerIdentifier);
        EditorGUILayout.PropertyField(interactiveArea);
        GUILayout.Box(GUIContent.none, GUILayout.Width(Screen.width), GUILayout.Height(2));
        EditorGUILayout.PropertyField(trackingSchema);
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(Vector3Bool))]
public class IngredientDrawerUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var X = new Rect(position.x, position.y, 30, position.height);
        var XLabel = new Rect(position.x + 20, position.y, 10, position.height);
        var Y = new Rect(position.x + 35, position.y, 30, position.height);
        var YLabel = new Rect(position.x + 55, position.y, 10, position.height);
        var Z = new Rect(position.x + 70, position.y, 30, position.height);
        var ZLabel = new Rect(position.x + 90, position.y, 10, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(X, property.FindPropertyRelative("X"), GUIContent.none);
        EditorGUI.LabelField(XLabel, "X");
        EditorGUI.PropertyField(Y, property.FindPropertyRelative("Y"), GUIContent.none);
        EditorGUI.LabelField(YLabel, "Y");
        EditorGUI.PropertyField(Z, property.FindPropertyRelative("Z"), GUIContent.none);
        EditorGUI.LabelField(ZLabel, "Z");

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}

public abstract class DictionaryDrawer<TK, TV> : PropertyDrawer
{
    protected SerializableDictionary<TK, TV> _Dictionary;
    private bool _Foldout;
    private const float kButtonWidth = 18f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);
        if (_Foldout)
            return (_Dictionary.Count + 1) * 17f;
        return 17f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        CheckInitialize(property, label);

        position.height = 17f;

        var foldoutRect = position;
        foldoutRect.width -= 2 * kButtonWidth;
        EditorGUI.BeginChangeCheck();
        _Foldout = EditorGUI.Foldout(foldoutRect, _Foldout, label, true);
        if (EditorGUI.EndChangeCheck())
            EditorPrefs.SetBool(label.text, _Foldout);

        var buttonRect = position;
        buttonRect.x = position.width - kButtonWidth + position.x;
        buttonRect.width = kButtonWidth + 2;

        if (GUI.Button(buttonRect, new GUIContent("+", "Add item"), EditorStyles.miniButton))
        {
            AddNewItem();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        buttonRect.x -= kButtonWidth;

        if (GUI.Button(buttonRect, new GUIContent("X", "Clear dictionary"), EditorStyles.miniButtonRight))
        {
            ClearDictionary();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        if (!_Foldout)
            return;

        foreach (var item in _Dictionary)
        {
            var key = item.Key;
            var value = item.Value;

            position.y += 17f;

            var keyRect = position;
            keyRect.width /= 2;
            keyRect.width -= 4;
            EditorGUI.BeginChangeCheck();
            var newKey = DoField(keyRect, typeof(TK), key);
            if (EditorGUI.EndChangeCheck())
            {
                try
                {
                    _Dictionary.Remove(key);
                    _Dictionary.Add(newKey, value);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                break;
            }

            var valueRect = position;
            valueRect.x = position.width / 2 + 15;
            valueRect.width = keyRect.width - kButtonWidth;
            EditorGUI.BeginChangeCheck();
            value = DoField(valueRect, typeof(TV), value);
            if (EditorGUI.EndChangeCheck())
            {
                //_Dictionary[key] = value;
                _Dictionary.Remove(key);
                _Dictionary.Add(newKey, value);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                break;
            }

            var removeRect = valueRect;
            removeRect.x = valueRect.xMax + 2;
            removeRect.width = kButtonWidth;
            if (GUI.Button(removeRect, new GUIContent("x", "Remove item"), EditorStyles.miniButtonRight))
            {
                RemoveItem(key);
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                break;
            }
        }
    }

    private void RemoveItem(TK key)
    {
        _Dictionary.Remove(key);
    }

    private void CheckInitialize(SerializedProperty property, GUIContent label)
    {
        if (_Dictionary == null)
        {
            var target = property.serializedObject.targetObject;
            _Dictionary = fieldInfo.GetValue(target) as SerializableDictionary<TK, TV>;
            if (_Dictionary == null)
            {
                _Dictionary = new SerializableDictionary<TK, TV>();
                fieldInfo.SetValue(target, _Dictionary);
            }

            _Foldout = EditorPrefs.GetBool(label.text);
        }
    }

    private static readonly Dictionary<Type, Func<Rect, object, object>> _Fields =
        new Dictionary<Type, Func<Rect, object, object>>()
        {
            //{ typeof(PartToTrack), (rect, value) => EditorGUI.Popup(rect, (int)value, Enum.GetNames(typeof(PartToTrack)))},
            { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
            { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
            { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
            { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
            { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
            { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
            { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
            { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
        };

    private static T DoField<T>(Rect rect, Type type, T value)
    {
        Func<Rect, object, object> field;
        if (_Fields.TryGetValue(type, out field))
            return (T)field(rect, value);

        if (type.IsEnum)
            return (T)(object)EditorGUI.EnumPopup(rect, (Enum)(object)value);

        if (typeof(UnityObject).IsAssignableFrom(type))
            return (T)(object)EditorGUI.ObjectField(rect, (UnityObject)(object)value, type, true);

        Debug.Log("Type is not supported: " + type);
        return value;
    }

    private void ClearDictionary()
    {
        _Dictionary.Clear();
    }

    private void AddNewItem()
    {
        TK key;
        if (typeof(TK) == typeof(string))
            key = (TK)(object)"";
        else key = default(TK);

        var value = default(TV);
        try
        {
            _Dictionary.Add(key, value);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}

[CustomPropertyDrawer(typeof(PlayerPositiondDict))]
public class PlayerPositiondDictDrawer1 : DictionaryDrawer<PartToTrack, Transform> { }

[CustomPropertyDrawer(typeof(BoxArea))]
public class BoxAreaDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Define the Interactive Area"));

        Rect boxRect = EditorGUILayout.BeginVertical();
        //This draws a Line to separate the Controls
        EditorGUILayout.LabelField(new GUIContent("Origin: "));
        EditorGUI.PropertyField(new Rect(position.x, position.y + 20, EditorGUIUtility.currentViewWidth / 2, position.height), property.FindPropertyRelative("center"), GUIContent.none);

        EditorGUILayout.EndVertical();

        boxRect = EditorGUILayout.BeginVertical();
        //This draws a Line to separate the Controls
        EditorGUILayout.LabelField(new GUIContent("Dimension: "));

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var XLabel = new Rect(position.x, position.y + 40, EditorGUIUtility.currentViewWidth / 12, position.height);
        var X = new Rect(position.x + EditorGUIUtility.currentViewWidth / 12, position.y + 40, EditorGUIUtility.currentViewWidth / 12, position.height);
        var YLabel = new Rect(position.x + (EditorGUIUtility.currentViewWidth / 12) * 2, position.y + 40, EditorGUIUtility.currentViewWidth / 12, position.height);
        var Y = new Rect(position.x + (EditorGUIUtility.currentViewWidth / 12) * 3, position.y + 40, EditorGUIUtility.currentViewWidth / 12, position.height);
        var ZLabel = new Rect(position.x + (EditorGUIUtility.currentViewWidth / 12) * 4, position.y + 40, EditorGUIUtility.currentViewWidth / 12, position.height);
        var Z = new Rect(position.x + (EditorGUIUtility.currentViewWidth / 12) * 5, position.y + 40, EditorGUIUtility.currentViewWidth / 12, position.height);

        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(X, property.FindPropertyRelative("width"), GUIContent.none);
        EditorGUI.LabelField(XLabel, "width");
        EditorGUI.PropertyField(Y, property.FindPropertyRelative("depth"), GUIContent.none);
        EditorGUI.LabelField(YLabel, "depth");
        EditorGUI.PropertyField(Z, property.FindPropertyRelative("height"), GUIContent.none);
        EditorGUI.LabelField(ZLabel, "height");

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUILayout.EndVertical();

        EditorGUI.EndProperty();
    }
}