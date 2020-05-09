using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
	// The head of the turret
	public GameObject head;
	// The dotted laser of the turret
	public GameObject dotted;
	// The blast laser of the turret
	public GameObject blast;

	// The starting direction of the turret
	public float initAngle = 90f;
	// The scale of the turret, used to keep the math consistent for different sizings
	public float scale = 1f;

	// The ratio of the length of laser sprite to length of tile
	private float laserScalar = 0.202839f;
	// The positive x offset of the lasers from the head
	private float laserOffsetX = 3f;

	// The player
	private GameObject player;
	// The number of frames since start
	private int numFrames = 0;


    // Start is called before the first frame update
    void Start()
    {
    	// Init variables
    	player = GameObject.FindWithTag("Player");

    	// Initialize the turret
    	dotted.GetComponent<SpriteRenderer>().enabled = true;
    	blast.GetComponent<SpriteRenderer>().enabled = false;
        Pivot(initAngle, 0.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    	if (numFrames > 40)
    	{
        	SeekPlayer();
    	}
    	else
    	{
    		numFrames++;
    	}
    }

    // Raycast to player and see if can hit
    public void SeekPlayer()
    {
    	// Player's position
    	Vector2 playerPos = player.transform.position;

    	// Make the raycast from the head, stopping at platforms or obscuration (steam)
    	RaycastHit2D hitInfo;
    	float max_reach = 40f;
    	int layerMask = 1 << 10;
    	layerMask += 1 << 11;
    	Vector2 vectorToPlayer = playerPos - ((Vector2)head.transform.position);
    	Vector2 direction = vectorToPlayer;
    	direction.Normalize();
    	hitInfo = Physics2D.Raycast(head.transform.position, direction, max_reach, layerMask);

    	// First pivot the turret visually by the angle of the vector to the player and the reach of the laser
    	// If hit something before the player, use ray length for reach, otherwise use distance to player
    	if (hitInfo.collider != null && (hitInfo.distance < vectorToPlayer.magnitude))
    	{
    		Pivot(Vector2.SignedAngle(new Vector2(0f, 1f), vectorToPlayer), hitInfo.distance);
    	}
    	else
    	{
    		Pivot(Vector2.SignedAngle(new Vector2(0f, 1f), vectorToPlayer), vectorToPlayer.magnitude);
    	}

    	// If didn't hit any walls or hit the player before a wall, call coroutine to make the laser hit the player
    	if (hitInfo.collider == null || (hitInfo.distance > vectorToPlayer.magnitude))
    	{
    		StartCoroutine(LaserHit());
    	}
    }

    // Coroutine to shoot the laser and kill the player
    IEnumerator LaserHit()
    {
    	// Show the laser
    	blast.GetComponent<SpriteRenderer>().enabled = true;
    	// Play sound
    	GetComponent<AudioSource>().Play();

    	yield return new WaitForSeconds(0.3f);

    	player.GetComponent<Death>().Die();
    }

    // Rotate and adjust the turret based on an angle and reach
    public void Pivot(float degrees, float reach)
    {
    	// Shorten reach by the laser x offset, and add a little
    	reach = reach - (laserOffsetX * scale) + 1f;

    	// Make in range of 0-359
    	degrees = degrees % 360;
    	if (degrees < 0)
    	{
    		degrees += 360;
    	}

    	// The x direction of the angle (-1 for left, +1 for right)
    	float xDir = -1f;

    	// Rotate the head from either the left or the right, depending on the angle
    	Vector2 temp = new Vector2(0f, 0f);
    	if (degrees > 180f)
    	{
    		head.transform.rotation = Quaternion.Euler(0f, 0f, (degrees - 360f + 90f));
    		xDir = 1f;
    	}
    	else
    	{
    		head.transform.rotation = Quaternion.Euler(0f, 0f, (degrees - 90f));
    		xDir = -1f;
    	}

    	// Stretch and position the lasers
    	dotted.transform.localScale = new Vector3(laserScalar * reach / scale, 1f, 1f);
        blast.transform.localScale = new Vector3(laserScalar * reach / scale, 2f, 1f);
        float x = ((reach - (1f * scale)) / 2f) / scale;
        dotted.transform.localPosition = new Vector3((x + (laserOffsetX * scale)) * xDir, dotted.transform.localPosition.y, 0f);
        blast.transform.localPosition = new Vector3((x + (laserOffsetX * scale)) * xDir, blast.transform.localPosition.y, 0f);
    }
}
