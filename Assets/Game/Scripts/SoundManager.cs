﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour {
	public AudioSource efxSource;
	public AudioSource musicSource;

	public static SoundManager singleton = null;

	public float lowPitchRange = .95f;
	public float highPitchRange = 1.05f;

	// Use this for initialization
	void Awake() { 
		if (singleton == null) { 
			singleton = this;
		} else if (singleton != this) {
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);		
	}

	public void PlaySingle( AudioClip clip)  {
				
		efxSource.clip = clip;
		efxSource.Play ();
	}

	public void RandomizeSFX(params AudioClip [] clips) {
		
		float randomPitch = Random.Range (lowPitchRange, highPitchRange);
		efxSource.pitch = randomPitch;
		
		int randomIndex = Random.Range (0, clips.Length);			
		efxSource.clip = clips [randomIndex];
		efxSource.Play();
	}
}
