using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuneCount : MonoBehaviour
{
	// The rune placement script
	public RunePlacement rp;

	// The buttons for each rune (null if none)
	public Button airBtn;
	public Button waterBtn;
	public Button fireBtn;

	// The text element for the count of each rune available to place
	public Text airCountTxt;
	public Text waterCountTxt;
	public Text fireCountTxt;

	// The number of each type that can be placed
	public int airNum;
	public int waterNum;
	public int fireNum;

	// The tile highlight sprite for each rune
	public Sprite airS;
	public Sprite waterS;
	public Sprite fireS;
	public Sprite noneS;

	// The currently selected type of rune
	private Rune.RuneType currSelected = Rune.RuneType.AIR;
	// Boolean that tells if out of runes or not
	private bool noneToPlace = false;


    // Start is called before the first frame update
    void Start()
    {
        // Set up all the buttons
        UpdateButton(airBtn, airCountTxt, airNum);
        UpdateButton(waterBtn, waterCountTxt, waterNum);
        UpdateButton(fireBtn, fireCountTxt, fireNum);

        // Set up the listeners
        airBtn.onClick.AddListener (() => {SelectAir ();});
        waterBtn.onClick.AddListener (() => {SelectWater ();});
        fireBtn.onClick.AddListener (() => {SelectFire ();});

        // Set up default selected rune in RunePlacement
        if (airNum > 0)
        {
        	SelectAir();
        }
        else if (waterNum > 0)
        {
        	SelectWater();
        }
        else if (fireNum > 0)
        {
        	SelectFire();
        }
        else
        {
        	rp.UpdateRuneAvailability(true);
        	rp.UpdateRuneType("", "", noneS);
        	noneToPlace = true;
        }
    }

    // Called by RunePlacement when placing a rune (subtract from count)
    public void PlacedARune()
    {
    	bool downToZero = false;

    	if (currSelected == Rune.RuneType.AIR)
    	{
    		airNum--;
    		UpdateButton(airBtn, airCountTxt, airNum);
    		// If out of runes, select a different type or NONE
    		if (airNum <= 0)
    			downToZero = true;
    	}
    	if (currSelected == Rune.RuneType.WATER)
    	{
    		waterNum--;
    		UpdateButton(waterBtn, waterCountTxt, waterNum);
    		// If out of runes, select a different type or NONE
    		if (waterNum <= 0)
    			downToZero = true;
    	}
    	if (currSelected == Rune.RuneType.FIRE)
    	{
    		fireNum--;
    		UpdateButton(fireBtn, fireCountTxt, fireNum);
    		// If out of runes, select a different type or NONE
    		if (fireNum <= 0)
    			downToZero = true;
    	}

    	// Either select one of the remaining types or disable placing
    	if (downToZero)
    	{
			if (airNum > 0)
			{
				SelectAir();
			}
			else if (waterNum > 0)
			{
				SelectWater();
			}
			else if (fireNum > 0)
			{
				SelectFire();
			}
			else
			{
				rp.UpdateRuneAvailability(true);
				rp.UpdateRuneType("", "", noneS);
				noneToPlace = true;
			}
    	}
    }

    // Called by RunePlacement with deleting a rune (add 1 to count)
    public void RemovedARune(Rune.RuneType type)
    {
    	if (type == Rune.RuneType.AIR)
    	{
    		airNum++;
    		UpdateButton(airBtn, airCountTxt, airNum);
    		// If was out of runes, select this type
    		if (noneToPlace)
    		{
    			noneToPlace = false;
    			rp.UpdateRuneAvailability(false);
    			SelectAir();
    		}
    	}
    	if (type == Rune.RuneType.WATER)
    	{
    		waterNum++;
    		UpdateButton(waterBtn, waterCountTxt, waterNum);
    		// If was out of runes, select this type
    		if (noneToPlace)
    		{
    			noneToPlace = false;
    			rp.UpdateRuneAvailability(false);
    			SelectWater();
    		}
    	}
    	if (type == Rune.RuneType.FIRE)
    	{
    		fireNum++;
    		UpdateButton(fireBtn, fireCountTxt, fireNum);
    		// If was out of runes, select this type
    		if (noneToPlace)
    		{
    			noneToPlace = false;
    			rp.UpdateRuneAvailability(false);
    			SelectFire();
    		}
    	}
    }

    // Called when the air button is clicked
    public void SelectAir()
    {
    	rp.UpdateRuneType("Prefabs/Runes/AirRune", "Prefabs/PlacedRuneTiles/AirRuneTile", airS);
    	currSelected = Rune.RuneType.AIR;
    }

    // Called when the water button is clicked
    public void SelectWater()
    {
    	rp.UpdateRuneType("Prefabs/Runes/WaterRune", "Prefabs/PlacedRuneTiles/WaterRuneTile", waterS);
    	currSelected = Rune.RuneType.WATER;
    }

    // Called when the fire button is clicked
    public void SelectFire()
    {
    	rp.UpdateRuneType("Prefabs/Runes/FireRune", "Prefabs/PlacedRuneTiles/FireRuneTile", fireS);
    	currSelected = Rune.RuneType.FIRE;
    }


    // Updates a button and its text based on the number of runes to place given
    void UpdateButton(Button btn, Text txt, int num)
    {
    	// Stop if the button doesn't exist
    	if (btn == null)
    	{
    		return;
    	}

    	// If no runes left, disable the button
    	if (num <= 0)
    	{
    		btn.interactable = false;
    	}
    	// Otherwise, allow to be clicked
    	else
    	{
    		btn.interactable = true;
    	}

    	// Display the number remaining in the 'Count' text
    	txt.text = num.ToString();
    }
}
