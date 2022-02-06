using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		/*
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;

			s.source.outputAudioMixerGroup = mixerGroup;
		}
		*/
	}

	public void Add(GameObject target, string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		var audioSource = target.AddComponent<AudioSource>();
		audioSource.clip = s.clip;
		audioSource.loop = s.loop;
		audioSource.spatialBlend = 1;
		audioSource.outputAudioMixerGroup = mixerGroup;
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		audioSource.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		audioSource.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		audioSource.Play();
	}
	
	public void Pause(GameObject target)
	{
		
		var audioSource = target.GetComponent<AudioSource>();
		
		audioSource.Pause();
	}
	
	public void Play(GameObject target)
	{
		
		var audioSource = target.GetComponent<AudioSource>();
		
		audioSource.Play();
	}
	
	public void Remove(GameObject target)
	{
		
		var audioSource = target.GetComponent<AudioSource>();
		
		audioSource.Pause();
		
		Destroy(audioSource);
	}

}
