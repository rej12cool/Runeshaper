using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut3 : MonoBehaviour
{
	// The text box of the AI
	public Text talkAI;
	// The info text
	public Text infoText;
	// The AI Image
	public Image image;

	// The RuneCount script
	private RuneCount runeCount;

	// Has the player pressed R yet?
	private bool pressedR = false;
	// Has the player picked up a rune yet?
	private bool gotRune = false;
	// Has the player placed a rune?
	private bool placedRune = false;

	// The current talking event, used to let later popups interrupt earlier popups
	private int currEvent = 0;

    // Start is called before the first frame update
    void Start()
    {
    	runeCount = GameObject.FindWithTag("RuneCount").GetComponent<RuneCount>();

    	GetComponent<Canvas>().enabled = false;

    	float currSeconds = 4f;
    	StartCoroutine(ChangeAIText("It looks like there isn't an easy path here.", currSeconds, 0));
    	currSeconds += 3f;
    	StartCoroutine(ChangeAIText("You can try scouting around in your Energy Form.", currSeconds, 0));
    	currSeconds += 2f;
    	StartCoroutine(ChangeInfoText("Press 'R' to go into Energy Form", currSeconds, 0));
    	currSeconds += 3f;
    	StartCoroutine(HideImage(currSeconds, 0));
    	StartCoroutine(ChangeAIText("", currSeconds, 0));
    }

    // Update is called once per frame
    void Update()
    {
    	// Event for when player goes astral
        if (Input.GetKey("r") && !pressedR)
        {
        	pressedR = true;
        	currEvent = 1;
        	float currSeconds = 0f;
        	StartCoroutine(ChangeInfoText("Use WASD (or a controller) to float around the room", currSeconds, 1));
        	currSeconds += 4f;
        	StartCoroutine(ShowImage(currSeconds, 1));
        	StartCoroutine(ChangeAIText("In Energy Form, you're like a ghost. You can float and move through walls to explore.", currSeconds, 1));
        	currSeconds += 6f;
        	StartCoroutine(HideImage(currSeconds, 1));
    		StartCoroutine(ChangeAIText("", currSeconds, 1));

        }
        // Event for when player picks up rune
        if ((runeCount.airNum > 0) && !gotRune)
        {
        	gotRune = true;
        	currEvent = 2;
        	float currSeconds = 0f;
        	StartCoroutine(ChangeInfoText("", currSeconds, 2));
        	StartCoroutine(ShowImage(currSeconds, 2));
        	StartCoroutine(ChangeAIText("It looks like you picked up a rune! Based on our research, this civilization used them to control the elements.", currSeconds, 2));
        	currSeconds += 6f;
        	StartCoroutine(ChangeAIText("It could be useful here. Try placing it somewhere!", currSeconds, 2));
        	currSeconds += 1f;
        	StartCoroutine(ChangeInfoText("Use the mouse to hover over tiles. Click to place on anywhere the cursor can highlight.", currSeconds, 2));
        	currSeconds += 4f;
    		StartCoroutine(HideImage(currSeconds, 2));
    		StartCoroutine(ChangeAIText("", currSeconds, 2));
        }
        // Event for when player places rune
        if ((runeCount.airNum == 0) && !placedRune && gotRune)
        {
        	placedRune = true;
        	currEvent = 3;
        	float currSeconds = 2f;
        	StartCoroutine(ChangeInfoText("", currSeconds, 3));
        	StartCoroutine(ShowImage(currSeconds, 3));
        	StartCoroutine(ChangeAIText("Now that looks like something that can get you up to the door!", currSeconds, 3));
        	currSeconds += 1f;
        	StartCoroutine(ChangeInfoText("Drag the slider to rotate. Click the X to pick the rune back up.", currSeconds, 3));
        	currSeconds += 4f;
    		StartCoroutine(HideImage(currSeconds, 3));
    		StartCoroutine(ChangeAIText("", currSeconds, 3));
        }
    }

    IEnumerator HideUI(float seconds, int thisEvent)
    {
    	yield return new WaitForSeconds(seconds);
    	if (thisEvent < currEvent)
    		yield break;
    	GetComponent<Canvas>().enabled = false;
    }

    IEnumerator HideImage(float seconds, int thisEvent)
    {
    	yield return new WaitForSeconds(seconds);
    	if (thisEvent < currEvent)
    		yield break;
    	image.enabled = false;
    }

    IEnumerator ShowImage(float seconds, int thisEvent)
    {
    	yield return new WaitForSeconds(seconds);
    	if (thisEvent < currEvent)
    		yield break;
    	image.enabled = true;
    }

    IEnumerator ChangeAIText(string text, float seconds, int thisEvent)
    {
    	yield return new WaitForSeconds(seconds);
    	if (thisEvent < currEvent)
    		yield break;
    	GetComponent<Canvas>().enabled = true;
    	talkAI.text = text;
    }

    IEnumerator ChangeInfoText(string text, float seconds, int thisEvent)
    {
    	yield return new WaitForSeconds(seconds);
    	if (thisEvent < currEvent)
    		yield break;
    	GetComponent<Canvas>().enabled = true;
    	infoText.text = text;
    }
}
