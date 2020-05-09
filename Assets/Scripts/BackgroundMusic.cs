using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
	// Use a "singleton" to make sure there's only one instance of the music gameobject playing
	private static BackgroundMusic instance = null;
	public static BackgroundMusic Instance {
		get { return instance; }
	}

	void Awake() {
	 	// If already playing, don't use this
	    if (instance != null && instance != this)
	    {
	        Destroy(this.gameObject);
	    }
	    // Otherwise, make this the music player and start the music
	    else
	    {
	        instance = this;
	        GetComponent<AudioSource>().Play();
	        DontDestroyOnLoad(this.gameObject);
	    }
	}
}