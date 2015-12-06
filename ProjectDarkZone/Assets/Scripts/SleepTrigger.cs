using UnityEngine;
using System.Collections;

public class SleepTrigger : MonoBehaviour {
	
	private bool sleep;
	public Texture2D fadeOutTexture;
	
	// Use this for initialization
	void Start () {
		sleep = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (sleep && (Input.GetKeyDown("down") || Input.GetKeyDown("S"))) {
			StartCoroutine(goToBed());
		}
	}
	
	IEnumerator goToBed(){
		GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (1);
		yield return new WaitForSeconds (1.5f);
		GameObject.Find ("Fader").GetComponent<Fading> ().BeginFade (-1);
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.name == "Player") {
			sleep = true;
		}
	}
	void OnTriggerExit2D(Collider2D other){
		if (other.name == "Player") {
			sleep = false;
		}
	}
}