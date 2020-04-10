using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutOfBounds : MonoBehaviour
{
    public Vector2 respawn_position;

    void OnTriggerEnter2D(Collider2D other)
    {
    	if (other.gameObject.tag == "Player")
    	{
    		other.gameObject.transform.position = respawn_position;
    	}
    }
}
