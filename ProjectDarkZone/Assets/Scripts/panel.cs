using UnityEngine;
using System.Collections;

public class panel : MonoBehaviour {

	private bool open;

	// Use this for initialization
	void Start () {
		open = false;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(open) {
			gameObject.SetActive(true);
		}
		if (!open) {
			gameObject.SetActive (false);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.name == "Player") {
			open = true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.name == "Player") {
			open = false;
		}
	}
}
