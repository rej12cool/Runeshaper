using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirRune : MonoBehaviour
{
    private const float Force = 40f;

    private Rune rune;
    private ArrayList inside;

    void Start()
    {
        rune = transform.parent.GetComponent<Rune>();
        inside = new ArrayList();
    }

    void FixedUpdate()
    {
        if (inside.Count > 0)
        {
            // Loop through all entities that have entered the effect area
            foreach (PlayerController player in inside)
            {
                // Apply the upward "force" to the velocity to have the appearance of it applying the force
                float distance = Vector2.Distance(player.transform.position, rune.GetPosition());

                // Multiply by the direction of the rune and add to player's velocity
                Vector2 dir = rune.GetVector() * (Force / (Mathf.Pow(distance, .6f)));
            	player.velocity.x = dir.x;
                player.velocity.y = dir.y;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            inside.Add(other.gameObject.GetComponent<PlayerController>());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        inside.Remove(other.gameObject.GetComponent<PlayerController>());
    }
}