using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstralProject : MonoBehaviour
{
	private float moveSpeed = 10f;
	private Controller2D controller;

	// Determines whether the astral form is being used or not
	private bool isActive = false;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        // Hide the astral form
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    void FixedUpdate()
    {
    	if (isActive)
    	{
        	// Get axis input 
        	Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        	Move(directionalInput);
    	}
    }

    // Move the astral form and inform the camera
	void Move(Vector2 dir)
	{
        if (dir.magnitude > .01f)
        {
        	transform.Translate(dir * moveSpeed * Time.deltaTime);

        	// Specify which direction the player with move with the input
        	// (Only used for compatability with CameraFollow)
        	controller.playerInput = dir;
        }
	}

	// Start astral projection
	public void Begin()
	{
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
		isActive = true;

		// Activate rune placement script
		GetComponent<RunePlacement>().InPlacementMode(true);
	}

	// Stop astral projection
	public void End()
	{
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
		isActive = false;

		// Deactivate rune placement script
		GetComponent<RunePlacement>().InPlacementMode(false);
	}
}
