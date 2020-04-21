using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RunePlacement : MonoBehaviour
{
	public GameObject selectHighlight; // The sprite with that adds a box around a selected tile

	private TilemapCollider2D env;	// The environment (aka Tilemap collision area) that can place runes in
	private bool canPlace = false;	// Determines if in placement mode
	private Vector2 cursorPos = new Vector2(0f, 0f); // The world position of the cursor
	private bool hovering = false;	// Determines if cursor is hovering over a placeable tile
	private Vector2 tilePos = new Vector2(0f ,0f);	// The position of the active tile
	private bool selected = false;	// Determines if a tile is currently selected
	private Vector2 lastClickTile = new Vector2(0f, 0f); // The last tile that was clicked, used to avoid repeating the PlaceRune func

	// Storage of placed runes
	private ArrayList placedRuneList;

	public struct PlacedRune
	{
		public Rune rune;
		public Vector2 tile;

		public PlacedRune(Rune r, Vector2 t)
		{
			this.rune = r;
			this.tile = t;
		}
	}


    // Start is called before the first frame update
    void Start()
    {
        env = GameObject.FindWithTag("Env").GetComponent<TilemapCollider2D>();
        placedRuneList = new ArrayList();
    }

    // Update is called once per frame
    void Update()
    {
    	// Evaluate updates to the mouse position
        if (canPlace)
        {
        	cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        	CheckTile(cursorPos);
        }
        // Evaluate a click when hovering on a placeable tile
        if (Input.GetButton("Fire1"))
        {
        	// Only call place if hovering on placeable tile and haven't already clicked this tile
        	if (hovering && (lastClickTile != tilePos))
        	{
        		lastClickTile = tilePos;
        		PlaceRune(tilePos);
        	}
        }
    }

    // Begins hovering on the tile at pos if it is placeable 
    void CheckTile(Vector2 pos)
    {
    	bool updateHover = hovering;

       	// If the cursor position is over the Env collider, check for placeable tile
    	if (env.OverlapPoint(pos))
    	{
    		// Snap tile position to grid
    		tilePos.x = Mathf.Floor(pos.x) + 0.5f;
    		tilePos.y = Mathf.Floor(pos.y) + 0.5f;

    		// If placeable tile, indicate that hovering and show the highlight
    		if (IsPlaceable(tilePos))
    		{
	    		selectHighlight.transform.position = tilePos;
	    		updateHover = true;
    		}
    		// Indicate else if otherwise
    		else
    		{
    			updateHover = false;
    		}
    	}
    	else
    	{
    		updateHover = false;
    	}

    	// Turn off sprite renderer if just stopped hovering
    	if (!updateHover && (updateHover != hovering))
    	{
    		selectHighlight.GetComponent<SpriteRenderer>().enabled = false;
    	}
    	// Turn on if just started hovering
    	else if (updateHover && (updateHover != hovering))
    	{
    		selectHighlight.GetComponent<SpriteRenderer>().enabled = true;
    	}

    	hovering = updateHover;
    }

    // Returns true if tile is placeable (has at least one empty adjacent tile) and false otherwise
    bool IsPlaceable(Vector2 tile)
    {
    	Vector2 topTile = new Vector2(tile.x, tile.y + 1f);
    	Vector2 bottomTile = new Vector2(tile.x, tile.y - 1f);
    	Vector2 leftTile = new Vector2(tile.x - 1f, tile.y);
    	Vector2 rightTile = new Vector2(tile.x + 1f, tile.y);

    	// Will return true if any of the adjacent tiles are empty (aka no collider present)
    	return (!env.OverlapPoint(topTile)) || (!env.OverlapPoint(bottomTile))
    			|| (!env.OverlapPoint(leftTile)) || (!env.OverlapPoint(rightTile));
    }

    // Places a rune at the clicked tile
    void PlaceRune(Vector2 tile)
    {
    	// First check that there's not already a rune here
		foreach (PlacedRune p in placedRuneList)
		{
			if (p.tile == tile)
				return;
		}

    	Vector2 runeBase = tile;
    	GameObject prefab = null;
    	float rot = 0f;

    	// Find which side to place the base of the rune
    	Vector2 topTile = new Vector2(tile.x, tile.y + 1f);
    	Vector2 bottomTile = new Vector2(tile.x, tile.y - 1f);
    	Vector2 leftTile = new Vector2(tile.x - 1f, tile.y);
    	Vector2 rightTile = new Vector2(tile.x + 1f, tile.y);

    	if (!env.OverlapPoint(topTile))
    	{
    		runeBase = topTile;
    		rot = 0f;
    	}
    	else if (!env.OverlapPoint(bottomTile))
    	{
    		runeBase = bottomTile;
    		rot = 180f;
    	}
    	else if (!env.OverlapPoint(leftTile))
    	{
    		runeBase = leftTile;
    		rot = 90f;
    	}
    	else if (!env.OverlapPoint(rightTile))
    	{
    		runeBase = rightTile;
    		rot = -90f;
    	}


    	// Pick the type of rune and get its prefab
    	prefab = (GameObject)Resources.Load("Prefabs/Runes/AirRune", typeof(GameObject));

	    // Instantiate a new (inactive) rune from prefab
	    GameObject newRune = Instantiate(prefab, runeBase, Quaternion.Euler(0f, 0f, rot), GameObject.FindWithTag("AllRunes").transform);
	    // Configure its properties
	    newRune.GetComponent<Rune>().rot = rot;
	    // Activate to call its Start() function
	    newRune.SetActive(true);

	    // Put the rune and its position in the array
	    placedRuneList.Add(new PlacedRune(newRune.GetComponent<Rune>(), tile));
    }

    // Turns rune placement mode on or off (true or false)
    public void InPlacementMode(bool update)
    {
		canPlace = update;

		if (update)
		{
			// Make the cursor visible and stay in the window
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.Confined;
		}
		else
		{
			// Hide the cursor
			Cursor.visible = false;

			// Turn off the highlight
			selectHighlight.GetComponent<SpriteRenderer>().enabled = false;
		}
    }
}
