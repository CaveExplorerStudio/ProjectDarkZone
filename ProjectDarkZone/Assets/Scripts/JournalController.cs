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

    void Start () {
        collectedPages = new List<journalPage>();
        prefab = Resources.Load("Journal", typeof(GameObject)) as GameObject;
        journalGUI = Instantiate(prefab) as GameObject;
        journalActive = false;

        journalCanvas = journalGUI.GetComponent<Canvas>();
        journalCanvasGroup = journalGUI.GetComponent<CanvasGroup>();
    }
	
	
	void Update () {
	
	}
}
