using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<AudioSource>().Play(0);
        Debug.Log("Diiing");
    }
}
