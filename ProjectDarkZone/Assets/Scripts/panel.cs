using UnityEngine;
using System.Collections;

public class panel : MonoBehaviour {

	private bool open;
	public Renderer rend;

	// Use this for initialization
	void Start () {
		open = false;
		rend = GetComponent<Renderer>();
		rend.enabled = false;
	}

	
	// Update is called once per frame
	void Update () {
		if(open) {
			rend.enabled = true;
		}
		if (!open) {
			rend.enabled = false;
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
