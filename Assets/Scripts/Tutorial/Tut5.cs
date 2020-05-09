using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut5 : MonoBehaviour
{
	// The text box of the AI
	public Text talkAI;
	// The info text
	public Text infoText;
	// The AI Image
	public Image image;

	// The fire rune
	public Rune fireRune;

	// Did the cross happen?
	private bool madeSteam = false;

    // Start is called before the first frame update
    void Start()
    {
    	GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Event for when steam is created
        if ((fireRune.hybridRune != null) && !madeSteam)
        {
        	madeSteam = true;
    		float currSeconds = 1f;
    		StartCoroutine(ChangeAIText("You made steam! That could be useful for something.", currSeconds));
    		currSeconds += 7f;
    		StartCoroutine(HideUI(currSeconds));
        }
    }

    IEnumerator HideUI(float seconds)
    {
    	yield return new WaitForSeconds(seconds);
    	GetComponent<Canvas>().enabled = false;
    }

    IEnumerator HideImage(float seconds)
    {
    	yield return new WaitForSeconds(seconds);
    	image.enabled = false;
    }

    IEnumerator ShowImage(float seconds)
    {
    	yield return new WaitForSeconds(seconds);
    	image.enabled = true;
    }

    IEnumerator ChangeAIText(string text, float seconds)
    {
    	yield return new WaitForSeconds(seconds);
    	GetComponent<Canvas>().enabled = true;
    	talkAI.text = text;
    }

    IEnumerator ChangeInfoText(string text, float seconds)
    {
    	yield return new WaitForSeconds(seconds);
    	GetComponent<Canvas>().enabled = true;
    	infoText.text = text;
    }
}
