using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PageController : MonoBehaviour {
    private static int numberOfPages = 0;
    private static int pagesCollected = 0;
    private List<journalPage> pages;
    private string h;
    System.IO.StreamReader file;


	// Use this for initialization
	void Start () {
        pages = new List<journalPage>();
        file = new System.IO.StreamReader("I:\\Github\\ProjectDarkZone\\ProjectDarkZone\\Assets\\Resources\\entries.txt");

        while ((h = file.ReadLine()) != null)
        {
            pages.Add(new journalPage(h, file.ReadLine(), file.ReadLine(), numberOfPages));
            numberOfPages++;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
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
