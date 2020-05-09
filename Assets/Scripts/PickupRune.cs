using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRune : MonoBehaviour
{
	// The type of rune this gem gives
	public Rune.RuneType type;
	// The rate of oscillation
	public float oscSpeed = 0.1f;
	// The scale of oscillation
	public float oscScale = 0.5f;

	// The RuneCount script in the rune placement prefab
	private RuneCount runeCount;
	// The initial starting position
	private Vector2 initPos;
	// The current radians of oscillation
	private float currRadians = 0f;


	// Start is called when the script begins
	void Start()
	{
		initPos = transform.position;
		runeCount = GameObject.FindWithTag("RuneCount").GetComponent<RuneCount>();
	}

    // Update is called once per frame
    void Update()
    {
    	// Incremement oscillation and move the gem
        currRadians += oscSpeed * Time.deltaTime;
        transform.position = initPos + new Vector2(0f, oscScale * Mathf.Sin(currRadians));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "AstralForm")
        {
            StartCoroutine(PickupThis());
        }
    }

    // Pickup gem, hide it, player sound, then delete
    IEnumerator PickupThis()
    {
    	// Increment count
    	runeCount.PickedUpRune(type);

    	// Deactivate this gem
    	GetComponent<SpriteRenderer>().enabled = false;
    	GetComponent<BoxCollider2D>().enabled = false;

		// Play sound
    	GetComponent<AudioSource>().Play();

    	yield return new WaitForSeconds(2f);

        Destroy(this.gameObject);
    }
}
