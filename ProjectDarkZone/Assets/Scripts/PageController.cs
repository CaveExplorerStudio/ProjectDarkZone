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
    private GameObject prefab;


	// Use this for initialization
	void Start () {
        pages = new List<journalPage>();
        file = new System.IO.StreamReader("I:\\Github\\ProjectDarkZone\\ProjectDarkZone\\Assets\\Resources\\entries.txt");

        prefab = Resources.Load("PageUI", typeof(GameObject)) as GameObject;

        pageGUI = Instantiate(prefab) as GameObject;



        while ((h = file.ReadLine()) != null)
        {
            Debug.Log(h);
            pages.Add(new journalPage(h, file.ReadLine(), file.ReadLine(), numberOfPages));
            numberOfPages++;
        }

	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pagesCollected++;
            timeToSet = true;
        }

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


            timeToSet = !timeToSet;
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
}
