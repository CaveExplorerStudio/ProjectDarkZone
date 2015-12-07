using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class JournalController : MonoBehaviour {
    //Private object references
    private List<journalPage> collectedPages;
    private Canvas journalCanvas;
    private CanvasGroup journalCanvasGroup;
    private GameObject prefab;
    private GameObject journalGUI;
    private PageController pageControl;
    private Button[] buttons;

    //Private primative variables
    private bool journalActive;
    private bool isFading;
    private int fadeState;
    private int fadeCounter = 10;
    private int currentPage = 0;

    void Start () {
        collectedPages = new List<journalPage>();
        prefab = Resources.Load("Journal", typeof(GameObject)) as GameObject;
        journalGUI = Instantiate(prefab) as GameObject;
        journalActive = false;
        journalGUI.name = "ActiveJournal";

        //Get's components from scene in one place to aid in run-time efficiency. 
        journalCanvas = journalGUI.GetComponent<Canvas>();
        journalCanvasGroup = journalGUI.GetComponent<CanvasGroup>();
        pageControl = GameObject.Find("Player").GetComponent<PageController>();
        buttons = GameObject.Find("ActiveJournal").GetComponentsInChildren<Button>();

        buttons[0].onClick.AddListener(delegate { clickForward(); });
        buttons[1].onClick.AddListener(delegate { clickBack(); });
    }
	
	
	void Update () {
	    if (Input.GetKeyDown(KeyCode.J) && !journalActive) //Journal bounded to "J" here
        {
            UpdateText();
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

    private void FadePage(int i) //Used to slowly fade the page. i == 1 is a fade out, i == 0 is a fade in.
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

    private void UpdateText()
    {
        Text[] fields = journalGUI.GetComponentsInChildren<Text>(); //Gets components of the text in GUI

        collectedPages = pageControl.getCollectedPages();

        if (pageControl.getAllCurrentPages() == 0) //Controls for if the pages are blank or not even.
        {
            fields[0].text = "You have no collected entries";
            fields[1].text = "";
            fields[2].text = "";
            fields[3].text = "";
            fields[4].text = "";
            fields[5].text = "";
        }
        else if (currentPage <= pageControl.getAllCurrentPages() - 2)
        {
            fields[0].text = collectedPages[currentPage].header;
            fields[1].text = collectedPages[currentPage].body;
            fields[2].text = collectedPages[currentPage].footer;
            fields[3].text = collectedPages[currentPage + 1].header;
            fields[4].text = collectedPages[currentPage + 1].body;
            fields[5].text = collectedPages[currentPage + 1].footer;
        }
        else if (currentPage <= pageControl.getAllCurrentPages() - 1)
        {
            fields[0].text = collectedPages[currentPage].header;
            fields[1].text = collectedPages[currentPage].body;
            fields[2].text = collectedPages[currentPage].footer;
            fields[3].text = "";
            fields[4].text = "";
            fields[5].text = "";
        }
    }

    public void clickForward() //Connected to the "onclick" of the right button
    {
        if (currentPage < pageControl.getAllCurrentPages() - 2)
            currentPage += 2;
        UpdateText();
    }

    public void clickBack() //Connected to the "onclick" of the left button
    {
        if (currentPage >= 2)
            currentPage -= 2;
        UpdateText();
    }
}
