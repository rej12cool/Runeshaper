using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tut4 : MonoBehaviour
{
	// The text box of the AI
	public Text talkAI;
	// The info text
	public Text infoText;
	// The AI Image
	public Image image;

    // Start is called before the first frame update
    void Start()
    {
    	GetComponent<Canvas>().enabled = false;
    	float currSeconds = 3f;
    	StartCoroutine(ChangeAIText("That looks dangerous. Maybe there's a way to move diagonally instead...", currSeconds));
    	currSeconds += 6f;
    	StartCoroutine(HideUI(currSeconds));
    }

    // Update is called once per frame
    void Update()
    {
        
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
