using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class JournalController : MonoBehaviour {
    private List<journalPage> collectedPages;
    private Canvas journalCanvas;
    private CanvasGroup journalCanvasGroup;
    private GameObject prefab;
    private GameObject journalGUI;

    private bool journalActive;
    private bool isFading;
    private int fadeState;
    private int fadeCounter = 10;

    void Start () {
        collectedPages = new List<journalPage>();
        prefab = Resources.Load("Journal", typeof(GameObject)) as GameObject;
        journalGUI = Instantiate(prefab) as GameObject;
        journalActive = false;

        journalCanvas = journalGUI.GetComponent<Canvas>();
        journalCanvasGroup = journalGUI.GetComponent<CanvasGroup>();
    }
	
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.J) && !journalActive)
        {
            journalActive = !journalActive;
            journalCanvas.enabled = true;
            isFading = true;
            fadeState = 0;
        }
        else if (Input.GetKeyDown(KeyCode.J) && journalActive)
        {
            journalActive = !journalActive;
            isFading = true;
            fadeState = 1;
        }


        if (isFading)
            FadePage(fadeState);

    }

    private void FadePage(int i)
    {
        if (fadeCounter != 0 && i == 1)
        {
            fadeCounter--;
            journalCanvasGroup.alpha -= .1f;
        }
        else if (i == 1)
        {
            isFading = false;
            journalCanvas.enabled = false;
            fadeCounter = 10;
            journalCanvasGroup.alpha = 0;
        }

        if (fadeCounter != 0 && i == 0)
        {
            fadeCounter--;
            journalCanvasGroup.alpha += .1f;
        }
        else if (i == 0)
        {
            isFading = false;
            fadeCounter = 10;
            journalCanvasGroup.alpha = 1;
        }
    }
}
