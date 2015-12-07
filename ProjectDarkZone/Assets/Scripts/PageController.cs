using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PageController : MonoBehaviour {
    private static int numberOfPages = 0;
    private static int pagesCollected = 0;
    private List<journalPage> pages;
    private string h;
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
    private bool pageActive;
    private bool isFading;
    private int fadeCounter = 10;
    private int fadeState = -1;

    // Use this for initialization
    void Start () {
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

        while ((h = file.ReadLine()) != null)
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
            Debug.Log("I have a reference!!");

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

            if (!(Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y), .8f, page_layer) == null) && Input.GetKeyDown(KeyCode.E))
            {
                pageCanvas.enabled = true;
                pageActive = true;
                isFading = true;
                fadeState = 0;
            }

            if (Input.GetKeyDown(KeyCode.X) && pageActive) //Also have to store data.
            {
                isFading = true;
                fadeState = 1;
            }

            if (isFading)
                FadePage(fadeState);

        }

    }

    private void FadePage(int i)
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

    

    public void placePages()
    {
        this.mapGenScript.PlaceEvenlyDistributed(pagePrefab, pageParent, 10);
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

public struct journalPage
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
