using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutOne : MonoBehaviour
{
	// The text box of the AI
	public Text talkAI;
	// The info text
	public Text infoText;

    // Start is called before the first frame update
    void Start()
    {
    	GetComponent<Canvas>().enabled = false;

        float currSeconds = 3f;
        StartCoroutine(ChangeAIText("< powering on >", currSeconds));

        currSeconds += 2f;
        StartCoroutine(ChangeAIText("< powering on >\n< initializing... >", currSeconds));

        currSeconds += 2f;
        StartCoroutine(ChangeAIText("< powering on >\n< initializing... >\n< inform: directive >", currSeconds));

        currSeconds += 4f;
        StartCoroutine(ChangeAIText("Hello, SEEKR-09! I have been assigned to be your assistant AI on this expedition.", currSeconds));
        currSeconds += 5f;
        StartCoroutine(ChangeAIText("A reminder: your directive is to find the unindentified power source on this planet. Based on the data, it should be in these ruins.", currSeconds));
        currSeconds += 5f;
        StartCoroutine(ChangeAIText("When you find it, you are to immediately transmit the information to Lab 8.", currSeconds));
		currSeconds += 6f;
        StartCoroutine(ChangeAIText("Lead the way!", currSeconds));
		currSeconds += 4f;
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
