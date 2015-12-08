using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PageController : MonoBehaviour {
    //Static members
    private static int numberOfPages = 0;
    private static int pagesCollected = 0;

    //Object references
    private List<journalPage> pages;
    System.IO.StreamReader file;
    public GameObject pageGUI;
    private bool timeToSet = true;
    public GameObject prefab;
    private MapGenerator mapGenScript;
    private GameObject pagePrefab;
    private GameObject pageParent;
    private LayerMask page_layer;
    private Canvas pageCanvas;
    private CanvasGroup pageCanvasGroup;

    //Primatives
    private bool pageActive;
    private bool isFading;
    private int fadeCounter = 10; //Change to adjust fade speed 
    private int fadeState = -1;
    private string h;

    // Use this for initialization
    void Start () {

        //Standard game object loading
        this.mapGenScript = GameObject.Find("Map Generator").GetComponent<MapGenerator>();
        pages = new List<journalPage>();
        file = new System.IO.StreamReader("Assets/Resources/entries.txt");

        prefab = Resources.Load("PageUI", typeof(GameObject)) as GameObject;
        pagePrefab = Resources.Load("Page", typeof(GameObject)) as GameObject;

        pageGUI = Instantiate(prefab) as GameObject;
        pageParent = Instantiate(new GameObject()) as GameObject;
        pageParent.name = "PageParent";
        page_layer = 1 << LayerMask.NameToLayer("Page");

        pageActive = false;

        while ((h = file.ReadLine()) != null) //Read each line of the entries.txt file, with each title, body, and footer being divided by enter. May want to change to \t or something
        {
            Debug.Log(h);
            pages.Add(new journalPage(h, file.ReadLine(), file.ReadLine(), numberOfPages));
            numberOfPages++;
        }

        placePages();
        pageCanvas = pageGUI.GetComponent<Canvas>();

        if (pageCanvas == null)
            Debug.Log("The reference is null");
        else
            Debug.Log("I have a reference!!"); //Verifying reference to the page Canvas because changes to the scene have caused problems.

        pageCanvasGroup = pageGUI.GetComponent<CanvasGroup>();

    }
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    pagesCollected++;
        //    timeToSet = true;
        //}

        if (pagesCollected < numberOfPages)
        {


            if (timeToSet)
            {
                //GameObject h = pageGUI.transform.GetChild(1).gameObject;
                //GameObject b = pageGUI.transform.GetChild(2).gameObject;
                //GameObject f = pageGUI.transform.GetChild(3).gameObject;

                //h.GetComponent<GUIText>().text = pages[0].header;
                //b.GetComponent<GUIText>().text = pages[0].footer;
                //f.GetComponent<GUIText>().text = pages[0].header;

                Text[] fields = pageGUI.GetComponentsInChildren<Text>();
                fields[0].text = pages[pagesCollected].header;
                fields[1].text = pages[pagesCollected].body;
                fields[2].text = pages[pagesCollected].footer;


                pageCanvas.enabled = false;

                timeToSet = !timeToSet;
            }

            if (!(Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), 1f, page_layer) == null) && Input.GetKeyDown(KeyCode.E))
            {
                pageCanvas.enabled = true;
                pageActive = true;
                isFading = true;
                fadeState = 0; //Fade-in
            }

            if (Input.GetKeyDown(KeyCode.X) && pageActive) //Also have to store data.
            {
                isFading = true;
                fadeState = 1; //Fade-out
            }

            if (isFading)
                FadePage(fadeState);

        }

    }

    private void FadePage(int i) //Different states depending if it's starting/stoping fading-in/fading-out
    {
        if (fadeCounter != 0 && i == 1)
        {
            fadeCounter--;
            pageCanvasGroup.alpha -= .1f;
        }
        else if (i == 1)
        {
            isFading = false;
            CollectPage();
            fadeCounter = 10;
            pageCanvasGroup.alpha = 0;
        }

        if (fadeCounter != 0 && i == 0)
        {
            fadeCounter--;
            pageCanvasGroup.alpha += .1f;
        }
        else if (i == 0)
        {
            isFading = false;
            fadeCounter = 10;
            pageCanvasGroup.alpha = 1;
        }
    }

    private void CollectPage()
    {
        pagesCollected++;
        pageCanvas.enabled = false;
        pageActive = false;
        timeToSet = true;
    }

    

    public void placePages() //Evenly distributed pages throughout the map, from the map generator script
    {
        this.mapGenScript.PlaceEvenlyDistributed(pagePrefab, pageParent, 18);
    }

    public int getAllCurrentPages()
    {
        return pagesCollected;
    }

    public List<journalPage> getCollectedPages()
    {
        return pages;
    }

}

public struct journalPage //Struct used to hold page data. May be replaced by class in later development
{
    public string header;
    public string footer;
    public string body;
    public int pageNumber;

    public journalPage(string head, string bod, string foot, int num)
    {
        header = head;
        footer = foot;
        body = bod;
        pageNumber = num;
    }
}
