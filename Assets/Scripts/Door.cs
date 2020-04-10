using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
	public string next_scene;

    void OnTriggerEnter2D(Collider2D other)
    {
    	if (other.gameObject.tag == "Player")
    	{
    		SceneManager.LoadScene(next_scene);
    	}
    }
}
