using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutOfBounds : MonoBehaviour
{
    public GameObject respawn;

    private Vector2 respawnPosition;

    void Start()
    {
        respawnPosition = respawn.transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    	if (other.gameObject.tag == "Player")
    	{
    		other.gameObject.transform.position = respawnPosition;
            other.GetComponent<PlayerController>().velocity = Vector3.zero;
    	}
    }
}
