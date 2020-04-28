using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class RunePlacement : MonoBehaviour
{
	public GameObject selectHighlight; // The sprite with that adds a box around a selected tile
    public Slider rotationSlider; // The slider that is shown when a rune is selected

	private TilemapCollider2D env;	// The environment (aka Tilemap collision area) that can place runes in
    private BoxCollider2D sliderCollider; // The rectangle of the slider, used to ignore clicking a tile while adjusting the slider
	private bool canPlace = false;	// Determines if in placement mode
	private Vector2 cursorPos = new Vector2(0f, 0f); // The world position of the cursor
	private bool hovering = false;	// Determines if cursor is hovering over a placeable tile
	private Vector2 tilePos = new Vector2(0f ,0f);	// The position of the active tile
	private bool selected = false;	// Determines if a tile is currently selected
    private PlacedRune selRune = new PlacedRune(null, new Vector2(0f, 0f), 0f, 0f, 0f); // The info for the currently selected rune
	private Vector2 lastClickTile = new Vector2(0f, 0f); // The last tile that was clicked, used to avoid repeating the PlaceRune func
    private bool resetingSlider = false; // True if SliderChangeCheck should ignore changes to slider at the current moment

	// Storage of placed runes
	private ArrayList placedRuneList;

	public struct PlacedRune
	{
		public Rune rune;
		public Vector2 tile;
		public float curr_rot;
		public float min_rot;
		public float max_rot;

		public PlacedRune(Rune r, Vector2 t, float rot, float mn, float mx)
		{
			this.rune = r;
			this.tile = t;
			this.curr_rot = rot;
			this.min_rot = mn;
			this.max_rot = mx;
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        env = GameObject.FindWithTag("Env").GetComponent<TilemapCollider2D>();
        sliderCollider = rotationSlider.GetComponent<BoxCollider2D>();
        placedRuneList = new ArrayList();

        //Adds a listener to the rotation slider and calls the rotate function when changed
        rotationSlider.onValueChanged.AddListener (delegate {SliderChangeCheck ();});
    }

    // Update is called once per frame
    void Update()
    {
    	// Evaluate updates to the mouse position
        if (canPlace)
        {
        	cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Only try to hover if not currently using the slider
            if (!selected || !sliderCollider.OverlapPoint(cursorPos))
            {
        	   CheckTile(cursorPos);
            }
        }
        // Evaluate a click when hovering on a placeable tile
        if (Input.GetButton("Fire1"))
        {
        	// Only call select if hovering on placeable tile, and haven't already clicked this tile, and
            // not currently moving the slider in with a tile selected
        	if (hovering && (lastClickTile != tilePos) && (!selected || !sliderCollider.OverlapPoint(cursorPos)))
        	{
        		lastClickTile = tilePos;
        		SelectTile(tilePos);
        	}
        }
    }

    // Selects a tile by:
    //  1) Placing a rune, if none exists
    //  2) Showing the slider and the 'remove rune' button
    void SelectTile(Vector2 tile)
    {
        selected = true;

        // First check if a rune exists at this tile
        bool runeExists = false;
        foreach (PlacedRune p in placedRuneList)
        {
            if (p.tile == tile)
            {
                selRune = p;
                runeExists = true;
                break;
            }
        }
        // If not, place one first
        if (!runeExists)
        {
            selRune = PlaceRune(tile);
        }

        // Tell SliderChangeCheck() to ignore these changes
        resetingSlider = true;

        // Adjust the slider's config for this rune
        rotationSlider.minValue = selRune.min_rot;
        rotationSlider.maxValue = selRune.max_rot;
        rotationSlider.value = selRune.curr_rot;

        // Put the rotation slider either above or below the rune
        float upOrDown = selRune.curr_rot % 360;
        upOrDown = (upOrDown > 180f) ? (360 - upOrDown) : upOrDown;
        upOrDown = (upOrDown < -180f) ? (360 + upOrDown) : upOrDown;

        Vector2 sliderPos = tile + (new Vector2(0f, -1f));
        if (Mathf.Abs(upOrDown) > 90)
            sliderPos = tile + (new Vector2(0f, 1f));

        // Move the slider and show to the player
        rotationSlider.gameObject.transform.position = sliderPos;
        rotationSlider.gameObject.SetActive(true);

        // Tell SliderChangeCheck() to start listening for changes again
        resetingSlider = false;
    }

    // Invoked when the value of the slider changes.
    public void SliderChangeCheck()
    {
        if (resetingSlider)
        {
            return;
        }
        
        // Determine the change of degrees
        float deltaRot = rotationSlider.value - selRune.curr_rot;
        
        // Move the position of the rune's base
        selRune.rune.SetPosition(selRune.tile + RotToVector(rotationSlider.value));

        // Rotate this rune by that change
        selRune.rune.QueueRotation(deltaRot);

        // Update the selected rune's struct
        selRune.curr_rot = rotationSlider.value;
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
    PlacedRune PlaceRune(Vector2 tile)
    {
    	Vector2 runeBase = tile;
    	GameObject prefab = null;
    	float rot = 0f;
    	// The ray from the clicked tile to runeBase
    	Vector2 point = new Vector2(0f, 0f);

    	// Find which side to place the base of the rune
    	Vector2 topTile = new Vector2(tile.x, tile.y + 1f);
    	Vector2 bottomTile = new Vector2(tile.x, tile.y - 1f);
    	Vector2 leftTile = new Vector2(tile.x - 1f, tile.y);
    	Vector2 rightTile = new Vector2(tile.x + 1f, tile.y);

    	if (!env.OverlapPoint(topTile))
    	{
    		runeBase = topTile;
    		rot = 0f;
    		point = new Vector2(0f, 1f);
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
    		rot = 270f;
    	}


    	// Pick the type of rune and get its prefab
    	prefab = (GameObject)Resources.Load("Prefabs/Runes/AirRune", typeof(GameObject));

	    // Instantiate a new (inactive) rune from prefab
	    GameObject newRune = Instantiate(prefab, runeBase, Quaternion.Euler(0f, 0f, rot), GameObject.FindWithTag("AllRunes").transform);
	    // Configure its properties
	    newRune.GetComponent<Rune>().rot = rot;
	    // Activate to call its Start() function
	    newRune.SetActive(true);

	    // Determine the range of rotation to be stored in the struct, starting at the rune's curr rot

	    // Check clockwise until encounters wall or comes full circle
        float min = rot;
	    while (!env.OverlapPoint(tile + RotToVector(min - 1f)) && ((rot - min) < 360))
	    {
	    	min -= 1;
	    }
	    // Check counter-clockwise until encounters wall or comes full circle
        float max = rot;
	    while (!env.OverlapPoint(tile + RotToVector(max + 1f)) && ((max - rot) < 360))
	    {
	    	max += 1;
	    }

	    // Put the rune, its position, the min-max rotations, and curr rot into the array
        PlacedRune rinfo = new PlacedRune(newRune.GetComponent<Rune>(), tile, rot, min, max);
	    placedRuneList.Add(rinfo);

	    return rinfo;
    }

    // Helper function, converts a rune rotation amount to a normalized Vector2
    private Vector2 RotToVector(float rot)
    {
    	return new Vector2((-1 * Mathf.Sin(Mathf.PI * rot / 180)), Mathf.Cos(Mathf.PI * rot / 180));
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

            // Reset vars
            selected = false;
            hovering = false;
            lastClickTile = new Vector2(0f, 0f);
		}
		else
		{
			// Hide the cursor
			Cursor.visible = false;

			// Turn off the highlight and slider
			selectHighlight.GetComponent<SpriteRenderer>().enabled = false;
            rotationSlider.gameObject.SetActive(false);
		}
    }
}
