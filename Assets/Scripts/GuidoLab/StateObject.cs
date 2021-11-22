using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StateObject : ScriptableObject
{
    [SerializeField]
    public Dictionary<string, MonoBehaviour[]> States;
}
