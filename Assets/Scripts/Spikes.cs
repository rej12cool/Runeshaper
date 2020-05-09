using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
        	StartCoroutine(ImpalePlayer(other.gameObject));
        }
    }

    IEnumerator ImpalePlayer(GameObject player)
    {
    	// Play sound
    	GetComponent<AudioSource>().Play();

    	yield return new WaitForSeconds(0.4f);

    	player.GetComponent<Death>().Die();
    }
}
