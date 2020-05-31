using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (PlayerPrefs.GetInt("sound") == 1)
		{
			GetComponent<AudioSource>().mute = false;
		}
		else
		{
			GetComponent<AudioSource>().mute = true;
		}
	}
}
